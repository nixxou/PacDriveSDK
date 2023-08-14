// ****************************************************************
// PacDrive.cpp : Defines the entry point for the DLL application.
// Author: Ben Baker [headsoft.com.au]
// ****************************************************************
#include "stdafx.h"
#include "PacDrive.h"
extern "C" {
#include "Include\hidsdi.h"
#include "Include\hid.h"
}
#include <uuids.h>
#include <tchar.h>
#include <stdio.h>
#include <wtypes.h>

UINT32 m_id = 0;
HINSTANCE m_hInstance = NULL;
HANDLE m_hThread = NULL;
DWORD m_dwThreadId = 0;
HANDLE m_hThreadExitEvent = NULL;
HWND m_hWnd = NULL;
HANDLE m_hStopEvent = NULL;
CRITICAL_SECTION m_crSection;
HID_DEVICE_DATA m_hidDeviceData[32] = { NULL };
INT m_deviceCount = 0;
USBDEVICE_ATTACHED_CALLBACK m_usbDeviceAttachedCallback = NULL;
USBDEVICE_REMOVED_CALLBACK m_usbDeviceRemovedCallback = NULL;

#pragma data_seg(".shared")
USHORT m_LEDState[32] = { 0 };
BYTE m_LED64State[32][16] = { { 0 } };
#pragma data_seg()

BOOL APIENTRY DllMain( HANDLE hModule, 
						DWORD  fdwReason, 
						LPVOID lpReserved
					 )
{
	switch (fdwReason)
	{
		case DLL_PROCESS_ATTACH:
			m_hInstance = (HINSTANCE) hModule;
			DisableThreadLibraryCalls(m_hInstance);

			InitializeCriticalSection(&m_crSection);
			break;
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
		case DLL_PROCESS_DETACH:
			break;
	}

	return TRUE;
}

PACDRIVE_API VOID __stdcall PacSetCallbacks(USBDEVICE_ATTACHED_CALLBACK usbDeviceAttachedCallback, USBDEVICE_REMOVED_CALLBACK usbDeviceRemovedCallback)
{
	m_usbDeviceAttachedCallback = usbDeviceAttachedCallback;
	m_usbDeviceRemovedCallback = usbDeviceRemovedCallback;
}

VOID SendMsgTimeout(HWND hWnd, UINT msg, UINT uTimeout)
{
	DWORD_PTR dwResult = 0;
	SendMessageTimeout(hWnd, msg, 0, 0, SMTO_ABORTIFHUNG | SMTO_NOTIMEOUTIFNOTHUNG, uTimeout, &dwResult);
}

VOID Format(PWCHAR szBuf, LPWSTR fmt, ...)
{
	va_list marker;
	va_start(marker, fmt);
	vswprintf(szBuf, fmt, marker);
	va_end(marker);
}

VOID RemoveNewDriver()
{
	for(INT i = 0; i < m_deviceCount; i++)
	{
		if(m_hidDeviceData[i].Type != DEVICETYPE_IPACULTIMATEIO)
			continue;

		if(!IS_NEWDRIVER(m_hidDeviceData[i].UsagePage, m_hidDeviceData[i].Usage, m_hidDeviceData[i].OutputReportLen))
			continue;

		for(INT j = 0; j < m_deviceCount; j++)
		{
			if(m_hidDeviceData[i].ProductID != m_hidDeviceData[j].ProductID)
				continue;

			if(!IS_OLDDRIVER(m_hidDeviceData[j].UsagePage, m_hidDeviceData[j].Usage, m_hidDeviceData[j].OutputReportLen))
				continue;

			m_hidDeviceData[j].pHidDeviceData = (PHID_DEVICE_DATA)malloc(sizeof(HID_DEVICE_DATA));
			memcpy(m_hidDeviceData[j].pHidDeviceData, &m_hidDeviceData[i], sizeof(HID_DEVICE_DATA));

			for(INT k = i; k < m_deviceCount - 1; k++)
				m_hidDeviceData[k] = m_hidDeviceData[k + 1];
			
			m_deviceCount--;

			break;
		}
	}
}

// Sort the PacDrive and U-HID devices by Product Id and Version Number
// This should result in PacDrive's listed first by Version Number then U-HID's by Product Id

VOID SortDevices()
{
	for(INT i = 0; i < m_deviceCount; i++)
	{
		for(INT j = 0; j < i; j++)
		{
			if(m_hidDeviceData[i].ProductID < m_hidDeviceData[j].ProductID || m_hidDeviceData[i].VersionNumber < m_hidDeviceData[j].VersionNumber)
			{
				HID_DEVICE_DATA temp = m_hidDeviceData[i];
				m_hidDeviceData[i] = m_hidDeviceData[j];
				m_hidDeviceData[j] = temp;
			}
		}
	}
}

VOID OutputDevice(INT id, PHID_DEVICE_DATA pHidDeviceData)
{
	DEBUGLOG(L"Id: %d", id);
	DEBUGLOG(L"HID Handle: %08x", pHidDeviceData->hDevice);
	DEBUGLOG(L"VendorID: %04x", pHidDeviceData->VendorID);
	DEBUGLOG(L"ProductID: %04x", pHidDeviceData->ProductID);
	DEBUGLOG(L"VersionNumber: %04x", pHidDeviceData->VersionNumber);
	DEBUGLOG(L"VendorName: %s", pHidDeviceData->VendorName);
	DEBUGLOG(L"ProductName: %s", pHidDeviceData->ProductName);
	DEBUGLOG(L"SerialNumber: %s", pHidDeviceData->SerialNumber);
	DEBUGLOG(L"DevicePath: %s", pHidDeviceData->DevicePath);
	DEBUGLOG(L"InputReportByteLength: %d", pHidDeviceData->InputReportLen);
	DEBUGLOG(L"OutputReportByteLength: %d", pHidDeviceData->OutputReportLen);
	DEBUGLOG(L"UsagePage: %d", pHidDeviceData->UsagePage);
	DEBUGLOG(L"Usage: %d", pHidDeviceData->Usage);
}

VOID OutputDevices()
{
	DEBUGLOG(L"===============================");
	DEBUGLOG(L"DEVICE LIST");

	for(INT i = 0; i < m_deviceCount; i++)
	{
		DEBUGLOG(L"===============================");

		OutputDevice(i, &m_hidDeviceData[i]);
		
		if(m_hidDeviceData[i].pHidDeviceData != NULL)
		{
			DEBUGLOG(L"========= SUB DEVICE ==========");

			OutputDevice(i, m_hidDeviceData[i].pHidDeviceData);
		}
	}

	DEBUGLOG(L"===============================");
}

// int PacInitialize()
// Initialize all PacDrive Devices
// Returns the number of PacDrives on the PC

PACDRIVE_API INT __stdcall PacInitialize()
{
	struct _GUID hidGuid;
	SP_DEVICE_INTERFACE_DATA deviceInterfaceData;
	struct { DWORD cbSize; WCHAR DevicePath[256]; } FunctionClassDeviceData;
	INT success;
	DWORD hidDevice;
	HANDLE pnPHandle;
	ULONG bytesReturned;
	DWORD dwThrdParam = 1;

	PacShutdown();

	m_deviceCount = 0;
		
	m_hStopEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

	m_hThreadExitEvent = CreateEvent(NULL, FALSE, FALSE, NULL);;

	m_hThread = CreateThread(NULL, 0, EventWindowThread, &dwThrdParam, 0, &m_dwThreadId);

	HidD_GetHidGuid(&hidGuid);

	pnPHandle = SetupDiGetClassDevs(&hidGuid, 0, 0, 0x12);

	if ((INT) pnPHandle == -1)
		return 0;

	for (hidDevice = 0; hidDevice < 127; hidDevice++)
	{
		deviceInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);

		success = SetupDiEnumDeviceInterfaces(pnPHandle, 0, &hidGuid, hidDevice, &deviceInterfaceData);

		if (success == 1)
		{
			FunctionClassDeviceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);
			success = SetupDiGetDeviceInterfaceDetail(pnPHandle, &deviceInterfaceData, (PSP_DEVICE_INTERFACE_DETAIL_DATA) &FunctionClassDeviceData, sizeof(FunctionClassDeviceData), &bytesReturned, 0);

			if (success == 0)
				continue;

			if (UsbOpen(FunctionClassDeviceData.DevicePath, &m_hidDeviceData[m_deviceCount]))
				m_deviceCount++;
		}
	}

	SetupDiDestroyDeviceInfoList(pnPHandle);

	RemoveNewDriver();
	SortDevices();

#ifdef DEBUG_OUTPUT
	OutputDevices();
#endif

	DEBUGLOG(L"%d Ultimarc Devices Found\n", m_deviceCount);

	return m_deviceCount;
}

// void PacShutdown()
// Shutdown all PacDrive Devices
// No return value

PACDRIVE_API VOID __stdcall PacShutdown()
{
	for(INT i = 0; i < m_deviceCount; i++)
	{
		DEBUGLOG(L"Closing HID Handle: %08x\n", m_hidDeviceData[i].hDevice);

		CloseHandle(m_hidDeviceData[i].hDevice);

		if(m_hidDeviceData[i].pHidDeviceData != NULL)
		{
			DEBUGLOG(L"Sub Device...");
			DEBUGLOG(L"Closing HID Handle: %08x\n", m_hidDeviceData[i].pHidDeviceData->hDevice);

			CloseHandle(m_hidDeviceData[i].pHidDeviceData->hDevice);

			free(m_hidDeviceData[i].pHidDeviceData);
		}
	}
	
	if (m_hStopEvent != NULL)
	{
		CloseHandle(m_hStopEvent);
		m_hStopEvent = NULL;
	}

	if (m_hWnd != NULL)
	{
		SendMsgTimeout(m_hWnd, WM_CLOSE, 1000);

		DWORD dwEvent = WaitForSingleObject(m_hThreadExitEvent, 5000);

		CloseHandle(m_hThreadExitEvent);

		m_hWnd = NULL;
	}
	
	if (m_hThread != NULL)
	{
		CloseHandle(m_hThread);

		m_dwThreadId = 0;
		m_hThread = NULL;
	}
}

// bool PacSetLEDStates(int id, USHORT data)
// Sends data to the PacDrive specified by id
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall PacSetLEDStates(INT id, USHORT data)
{
	if (!IS_TYPE_PACDRIVE(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;
	
	m_LEDState[id] = data;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_BLUEHID ? 5 : 0);
	outputReport.ReportBuffer[0] = 0;
	outputReport.ReportBuffer[1] = (m_hidDeviceData[id].Type == DEVICETYPE_BLUEHID ? 0 : 0xdd);
	outputReport.ReportBuffer[2] = HIBYTE(m_LEDState[id]);
	outputReport.ReportBuffer[3] = LOBYTE(m_LEDState[id]);

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

// bool PacSetLEDState(int id, int port, USHORT dat)
// Sends data to the PacDrive specified by id
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall PacSetLEDState(INT id, INT port, BOOL state)
{
	if (!IS_TYPE_PACDRIVE(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;
	
	if (state)
		m_LEDState[id] |= (1 << port);
	else
		m_LEDState[id] &= ~(1 << port);

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_BLUEHID ? 5 : 0);
	outputReport.ReportBuffer[0] = 0;
	outputReport.ReportBuffer[1] = (m_hidDeviceData[id].Type == DEVICETYPE_BLUEHID ? 0 : 0xdd);
	outputReport.ReportBuffer[2] = HIBYTE(m_LEDState[id]);
	outputReport.ReportBuffer[3] = LOBYTE(m_LEDState[id]);

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

// bool Pac64SetLEDStates(int id, int group, BYTE data)
// Sends data to the PacDrive specified by id
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall Pac64SetLEDStates(INT id, INT group, BYTE data)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;
	
	m_LED64State[id][group] = (BYTE) data;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = (group > 8 ? group + 8 : group) | 0x80;
	outputReport.ReportBuffer[1] = m_LED64State[id][group];
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

// bool Pac64SetLEDStatesRandom(INT id)
// Sends data to the PacDrive specified by id
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall Pac64SetLEDStatesRandom(INT id)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;
	
	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0x89;
	outputReport.ReportBuffer[1] = 0;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

// bool PacSetLEDState(int id, int group, int port, bool state)
// Sends data to the PacDrive specified by id
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall Pac64SetLEDState(INT id, INT group, INT port, BOOL state)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;
	
	if (state)
		m_LED64State[id][group] |= (1 << port);
	else
		m_LED64State[id][group] &= ~(1 << port);

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = (group > 8 ? group + 8 : group) | 0x80;
	outputReport.ReportBuffer[1] = m_LED64State[id][group];
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

// bool Pac64SetLEDIntensities(int id, PBYTE data)
// Sends data to the PacDrive specified by id
// data is a pointer to 64 bytes of data
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall Pac64SetLEDIntensities(INT id, PBYTE data)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	if(m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO && m_hidDeviceData[id].pHidDeviceData != NULL)
	{
		int ledCount = 96;
		REPORT_BUF outputReport;
		outputReport.ReportID = 4;

		for(int i = 0; i < ledCount; i++)
		{
			outputReport.ReportBuffer[i] = *data;
			data++;
		}

		if(!UsbWrite(m_hidDeviceData[id].pHidDeviceData, &outputReport))
			return FALSE;
	}
	else
	{
		REPORT_BUF outputReport;
		int ledCount = (m_hidDeviceData[id].Type == DEVICETYPE_NANOLED ? 30 : m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 48 : 32);

		outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
		outputReport.ReportBuffer[0] = 0xfe;
		outputReport.ReportBuffer[1] = 0;
		outputReport.ReportBuffer[2] = 0;
		outputReport.ReportBuffer[3] = 0;

		if(!UsbWrite(&m_hidDeviceData[id], &outputReport))
			return FALSE;

		for(int i = 0; i < ledCount; i++)
		{
			outputReport.ReportBuffer[0] = *data;
			data++;
			outputReport.ReportBuffer[1] = *data;
			data++;
			outputReport.ReportBuffer[2] = 0;
			outputReport.ReportBuffer[3] = 0;

			if(!UsbWrite(&m_hidDeviceData[id], &outputReport))
				return FALSE;
		}
	}

	return TRUE;
}

// bool Pac64SetLEDIntensity(int id, int port, byte intensity)
// Sends intensity to the PacDrive specified by id
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall Pac64SetLEDIntensity(INT id, INT port, BYTE intensity)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;
	BYTE bitMask = (m_hidDeviceData[id].Type == DEVICETYPE_PACLED64 ? 0x3F : 0x7F);

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = (port == -1 ? 0x80 : port & bitMask);
	outputReport.ReportBuffer[1] = intensity;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64SetLEDFadeTime(INT id, BYTE fadeTime)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0xc0;
	outputReport.ReportBuffer[1] = fadeTime;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64SetLEDFlashSpeeds(INT id, BYTE flashSpeed)
{
	if(m_hidDeviceData[id].Type != DEVICETYPE_PACLED64)
		return FALSE;

	REPORT_BUF outputReport;
	
	outputReport.ReportID = 0;
	outputReport.ReportBuffer[0] = 0x40;
	outputReport.ReportBuffer[1] = (flashSpeed & 0xb) | 0x4;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64SetLEDFlashSpeed(INT id, INT port, BYTE flashSpeed)
{
	if(m_hidDeviceData[id].Type != DEVICETYPE_PACLED64)
		return FALSE;

	REPORT_BUF outputReport;
	
	outputReport.ReportID = 0;
	outputReport.ReportBuffer[0] = 0x40 | (port & 0x3f);
	outputReport.ReportBuffer[1] = (flashSpeed & 0xb);
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64StartScriptRecording(INT id)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0xff;
	outputReport.ReportBuffer[1] = 0x1;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64StopScriptRecording(INT id)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0xff;
	outputReport.ReportBuffer[1] = 0x3;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64SetScriptStepDelay(INT id, BYTE stepDelay)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0xc1;
	outputReport.ReportBuffer[1] = stepDelay;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64RunScript(INT id)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0xff;
	outputReport.ReportBuffer[1] = 0x0;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64ClearFlash(INT id)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0xff;
	outputReport.ReportBuffer[1] = 0x4;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

PACDRIVE_API BOOL __stdcall Pac64SetDeviceId(INT id, INT newId)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0xfe;
	outputReport.ReportBuffer[1] = newId | 0xf0;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}
\
PACDRIVE_API BOOL __stdcall Pac64UpdateFirmware(INT id)
{
	if (!IS_TYPE_PAC64(m_hidDeviceData[id].Type))
		return FALSE;

	REPORT_BUF outputReport;

	outputReport.ReportID = (m_hidDeviceData[id].Type == DEVICETYPE_IPACULTIMATEIO ? 3 : 0);
	outputReport.ReportBuffer[0] = 0xff;
	outputReport.ReportBuffer[1] = 0x5;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	return UsbWrite(&m_hidDeviceData[id], &outputReport);
}

// Returns the device type

PACDRIVE_API INT __stdcall PacGetDeviceType(INT id)
{
	return m_hidDeviceData[id].Type;
}

// Returns the Vendor Id of the device specified by id

PACDRIVE_API INT __stdcall PacGetVendorId(INT id)
{
	return m_hidDeviceData[id].VendorID;
}

// Returns the Product Id of the device specified by id

PACDRIVE_API INT __stdcall PacGetProductId(INT id)
{
	return m_hidDeviceData[id].ProductID;
}

// Returns the Version Number of the device specified by id

PACDRIVE_API INT __stdcall PacGetVersionNumber(INT id)
{
	return m_hidDeviceData[id].VersionNumber;
}

// Copies the Vendor Name string into sVendorName
// No Return Value

PACDRIVE_API VOID __stdcall PacGetVendorName(INT id, PWCHAR sVendorName)
{
	wcscpy(sVendorName, m_hidDeviceData[id].VendorName);
}

// Copies the Product Name string into sVendorName
// No Return Value

PACDRIVE_API VOID __stdcall PacGetProductName(INT id, PWCHAR sProductName)
{
	wcscpy(sProductName, m_hidDeviceData[id].ProductName);
}

// Copies the Serial Number string into sVendorName
// No Return Value

PACDRIVE_API VOID __stdcall PacGetSerialNumber(INT id, PWCHAR sSerialNumber)
{
	wcscpy(sSerialNumber, m_hidDeviceData[id].SerialNumber);
}

// Copies the Device Path string into sVendorName
// No Return Value

PACDRIVE_API VOID __stdcall PacGetDevicePath(INT id, PWCHAR sDevicePath)
{
	wcscpy(sDevicePath, m_hidDeviceData[id].DevicePath);
}

PACDRIVE_API BOOL __stdcall PacProgramUHid(INT id, PWCHAR sFilePath)
{
	if(m_hidDeviceData[id].Type != DEVICETYPE_UHID)
		return FALSE;

	REPORT_BUF inputReport;
	REPORT_BUF outputReport;
	INT i = 0, byteCount = 0;
	BOOL retVal = FALSE;
	CHAR lineArray[256];
	UCHAR readBuffer[256];
	UCHAR writeBuffer[256];
	FILE *file = NULL;

	memset(&outputReport, 0, sizeof(REPORT_BUF));
	memset(&readBuffer, 0, sizeof(readBuffer));
	memset(&writeBuffer, 0, sizeof(writeBuffer));

	if(_wfopen_s(&file, sFilePath, L"r") != 0)
		return FALSE;

	while (fgets(lineArray, sizeof lineArray, file) != NULL && byteCount < (id + 1) * 256)
	{
		PCHAR newline = strchr(lineArray, '\n');

		if (newline != NULL)
			*newline = '\0';

		if (strncmp(lineArray, "#", 1) == 0)
			continue;

		PCHAR pChar = strtok(lineArray, ",");

		while(pChar != NULL)
		{
			if(byteCount >= id * 256)
				writeBuffer[i++] = (UCHAR) strtol(pChar, NULL, 16);

			byteCount++;

			pChar = strtok(NULL, ",");
		}
	}

	fclose (file);

	for(i = 0; i < 256; i++)
	{
		outputReport.ReportBuffer[i & 3] = writeBuffer[i];

		if((i + 1) % 4 == 0)
		{
			if(!UsbWrite(&m_hidDeviceData[id], &outputReport))
				return false;
		}
	}

	memset(&inputReport, 0, sizeof(REPORT_BUF));
	memset(&outputReport, 0, sizeof(REPORT_BUF));

	outputReport.ReportBuffer[0] = 0x59;
	outputReport.ReportBuffer[1] = 0xdd;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	if(!UsbWrite(&m_hidDeviceData[id], &outputReport))
		return FALSE;

	byteCount = 0;

	for (i = 0; i < 64; i++)
	{
		if(!UsbRead(&m_hidDeviceData[id], &inputReport))
			return FALSE;

		readBuffer[byteCount++] = inputReport.ReportBuffer[0];
		readBuffer[byteCount++] = inputReport.ReportBuffer[1];
		readBuffer[byteCount++] = inputReport.ReportBuffer[2];
		readBuffer[byteCount++] = inputReport.ReportBuffer[3];
	}

    for (i = 4; i < 192; i++)
    {
        retVal = (readBuffer[i] == writeBuffer[i]);

        if (!retVal)
            break;
    }

	return retVal;
}

// bool PacSetServoStik4Way()
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall PacSetServoStik4Way()
{
	REPORT_BUF outputReport;
	BOOL retVal = FALSE;
	
	outputReport.ReportID = 0;
	outputReport.ReportBuffer[0] = 0;
	outputReport.ReportBuffer[1] = 0xdd;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 0;

	for(int i=0; i<m_deviceCount; i++)
	{
		if(m_hidDeviceData[i].Type != DEVICETYPE_SERVOSTIK)
			continue;

		retVal = UsbWrite(&m_hidDeviceData[i], &outputReport);
	}

	return retVal;
}

// bool PacSetServoStik8Way()
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall PacSetServoStik8Way()
{
	REPORT_BUF outputReport;
	BOOL retVal = FALSE;
	
	outputReport.ReportID = 0;
	outputReport.ReportBuffer[0] = 0;
	outputReport.ReportBuffer[1] = 0xdd;
	outputReport.ReportBuffer[2] = 0;
	outputReport.ReportBuffer[3] = 1;

	for(int i=0; i<m_deviceCount; i++)
	{
		if(m_hidDeviceData[i].Type != DEVICETYPE_SERVOSTIK)
			continue;

		retVal = UsbWrite(&m_hidDeviceData[i], &outputReport);
	}

	return retVal;
}

// bool USBButtonConfigurePermanent(char* data)
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall USBButtonConfigurePermanent(INT id, PBYTE data)
{
	if(m_hidDeviceData[id].Type != DEVICETYPE_USBBUTTON)
		return FALSE;

	REPORT_BUF outputReport;
	BOOL retVal = FALSE;

	outputReport.ReportID = 0;
	outputReport.ReportBuffer[0] = 0x50;
	outputReport.ReportBuffer[1] = 0xdd;
	outputReport.ReportBuffer[2] = data[0];
	outputReport.ReportBuffer[3] = data[1];

	retVal = UsbWrite(&m_hidDeviceData[id], &outputReport);

	if(!retVal)
		return retVal;

	for(int j = 0; j < 60; j++)
	{
		outputReport.ReportBuffer[j & 3] = data[j + 2];

		if((j + 1) % 4 == 0)
		{
			retVal = UsbWrite(&m_hidDeviceData[id], &outputReport);

			if(!retVal)
				return retVal;
		}
	}

	return retVal;
}

// bool USBButtonConfigureTemporary(char* data)
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall USBButtonConfigureTemporary(INT id, PBYTE data)
{
	if(m_hidDeviceData[id].Type != DEVICETYPE_USBBUTTON)
		return FALSE;

	REPORT_BUF outputReport;
	BOOL retVal = FALSE;
	
	outputReport.ReportID = 0;
	outputReport.ReportBuffer[0] = 0x51;
	outputReport.ReportBuffer[1] = 0xdd;
	outputReport.ReportBuffer[2] = data[0];
	outputReport.ReportBuffer[3] = data[1];

	retVal = UsbWrite(&m_hidDeviceData[id], &outputReport);

	if(!retVal)
		return retVal;

	for(int j = 0; j < 60; j++)
	{
		outputReport.ReportBuffer[j & 3] = data[j + 2];

		if((j + 1) % 4 == 0)
		{
			retVal = UsbWrite(&m_hidDeviceData[id], &outputReport);

			if(!retVal)
				return retVal;
		}
	}

	return retVal;
}

// bool USBButtonConfigureColor(char red, char green, char blue)
// Returns true for success and false for failure

PACDRIVE_API BOOL __stdcall USBButtonConfigureColor(INT id, BYTE red, BYTE green, BYTE blue)
{
	if(m_hidDeviceData[id].Type != DEVICETYPE_USBBUTTON)
		return FALSE;

	REPORT_BUF outputReport;
	BOOL retVal = FALSE;
	
	outputReport.ReportID = 0;
	outputReport.ReportBuffer[0] = 0x01;
	outputReport.ReportBuffer[1] = red;
	outputReport.ReportBuffer[2] = green;
	outputReport.ReportBuffer[3] = blue;

	retVal = UsbWrite(&m_hidDeviceData[id], &outputReport);
	
	return retVal;
}

PACDRIVE_API BOOL __stdcall USBButtonGetState(INT id, PBOOL state)
{
	if(m_hidDeviceData[id].Type != DEVICETYPE_USBBUTTON)
		FALSE;

	REPORT_BUF reportBuffer;
	BOOL retVal = FALSE;

	reportBuffer.ReportID = 0;
	reportBuffer.ReportBuffer[0] = 0x02;
	reportBuffer.ReportBuffer[1] = 0x00;
	reportBuffer.ReportBuffer[2] = 0x00;
	reportBuffer.ReportBuffer[3] = 0x00;

	retVal = UsbWrite(&m_hidDeviceData[id], &reportBuffer);

	if(!retVal)
		return retVal;

	memset(&reportBuffer, 0, sizeof(REPORT_BUF));

	retVal = UsbRead(&m_hidDeviceData[id], &reportBuffer);

	if(!retVal)
		return retVal;

	*state = reportBuffer.ReportBuffer[0];

	return retVal;
}

BOOL UsbOpen(LPCWSTR devicePath, PHID_DEVICE_DATA pDeviceData)
{
	HANDLE hidHandle;
	USHORT vendorID, productID, versionNumber;
	USHORT inputReportLen, outputReportLen;
	USHORT usagePage, usage;
	WCHAR tempPath[256];
	INT success;

	wcscpy_s(tempPath, devicePath);

	strlow(tempPath);

	hidHandle = CreateFile(tempPath, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_WRITE | FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);

	// Open device as non-overlapped so we can get data
	//hidHandle = CreateFile(tempPath, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_WRITE | FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);

	if (hidHandle == INVALID_HANDLE_VALUE)
		return FALSE;

	if(GetDeviceInfo(hidHandle, vendorID, productID, versionNumber, usage, usagePage, inputReportLen, outputReportLen))
	{
		if(IS_BLUEHID(usage, usagePage, inputReportLen, outputReportLen) ||
			(IS_ULTIMARC(vendorID) &&
			(IS_PACDRIVE(productID) ||
			IS_UHID(productID) ||
			IS_PACLED64(productID) ||
			IS_SERVOSTIK(productID) ||
			IS_USBBUTTON(productID) ||
			IS_NANOLED(productID) ||
			IS_IPACULTIMATEIO(productID))))
		{
			if(IS_UHID(productID) && !IS_OLDDRIVER(usagePage, usage, outputReportLen))
			{
				CloseHandle(hidHandle);
				return FALSE;
			}

			if(IS_USBBUTTON(productID) && !IS_OLDDRIVER(usagePage, usage, outputReportLen))
			{
				CloseHandle(hidHandle);
				return FALSE;
			}

			if(IS_NANOLED(productID) && !IS_OLDDRIVER(usagePage, usage, outputReportLen))
			{
				CloseHandle(hidHandle);
				return FALSE;
			}

			if(IS_IPACULTIMATEIO(productID) && !IS_OLDDRIVER(usagePage, usage, outputReportLen) && !IS_NEWDRIVER(usagePage, usage, outputReportLen))
			{
				CloseHandle(hidHandle);
				return FALSE;
			}

			if(IS_PACDRIVE(productID))
				pDeviceData->Type = DEVICETYPE_PACDRIVE;
			else if(IS_UHID(productID))
				pDeviceData->Type = DEVICETYPE_UHID;
			else if(IS_PACLED64(productID))
				pDeviceData->Type = DEVICETYPE_PACLED64;
			else if(IS_SERVOSTIK(productID))
				pDeviceData->Type = DEVICETYPE_SERVOSTIK;
			else if(IS_USBBUTTON(productID))
				pDeviceData->Type = DEVICETYPE_USBBUTTON;
			else if(IS_NANOLED(productID))
				pDeviceData->Type = DEVICETYPE_NANOLED;
			else if(IS_IPACULTIMATEIO(productID))
				pDeviceData->Type = DEVICETYPE_IPACULTIMATEIO;
			else if(IS_BLUEHID(usage, usagePage, inputReportLen, outputReportLen))
				pDeviceData->Type = DEVICETYPE_BLUEHID;
			else
				pDeviceData->Type = DEVICETYPE_UNKNOWN;

			wcscpy_s(pDeviceData->DevicePath, tempPath);

			pDeviceData->hDevice = hidHandle;
			pDeviceData->VendorID = vendorID;
			pDeviceData->ProductID = productID;
			pDeviceData->VersionNumber = versionNumber;
			pDeviceData->InputReportLen = inputReportLen;
			pDeviceData->OutputReportLen = outputReportLen;
			pDeviceData->UsagePage = usagePage;
			pDeviceData->Usage = usage;
			pDeviceData->pHidDeviceData = NULL;

			HidD_GetManufacturerString(hidHandle, pDeviceData->VendorName, sizeof(pDeviceData->VendorName));
			HidD_GetProductString(hidHandle, pDeviceData->ProductName, sizeof(pDeviceData->ProductName));
			HidD_GetSerialNumberString(hidHandle, pDeviceData->SerialNumber, sizeof(pDeviceData->SerialNumber));

			//DEBUGLOG(L"Error: %x\n", GetLastError());
			
			//OutputDevice(pDeviceData);

			return TRUE;
		}
	}

	CloseHandle(hidHandle);

	return FALSE;
}

BOOL UsbRead(PHID_DEVICE_DATA pHidDeviceData, PREPORT_BUF pInputReport)
{
	return UsbRead(pHidDeviceData, pInputReport, INFINITE);
}

BOOL UsbRead(PHID_DEVICE_DATA pHidDeviceData, PREPORT_BUF pInputReport, DWORD timeOut)
{
	OVERLAPPED ol;
	DWORD cbRet = 0;
	BOOL bRet = FALSE;

	memset(&ol, 0, sizeof(ol));
	ol.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

	EnterCriticalSection(&m_crSection);
	ResetEvent(ol.hEvent);

	bRet = ReadFile(pHidDeviceData->hDevice, pInputReport, pHidDeviceData->InputReportLen, &cbRet, &ol);
	LeaveCriticalSection(&m_crSection);

	if (!bRet)
	{
		if (GetLastError() == ERROR_IO_PENDING)
		{
			HANDLE handles[2] = { ol.hEvent, m_hStopEvent };
			DWORD waitRet = WaitForMultipleObjects(2, handles, FALSE, timeOut);

			if (waitRet == WAIT_OBJECT_0)
			{
				// Data came in
				bRet = GetOverlappedResult(pHidDeviceData->hDevice, &ol, &cbRet, TRUE);
			}
			else if (waitRet == (WAIT_OBJECT_0 + 1))
			{
				// Stop event was set
				ResetEvent(m_hStopEvent);

				bRet = FALSE;
			}
			else if (waitRet == WAIT_TIMEOUT)
			{
				bRet = FALSE;
			}
		}
	}

	CloseHandle(ol.hEvent);

	return bRet;
}

BOOL UsbWrite(PHID_DEVICE_DATA pHidDeviceData, PREPORT_BUF pOutputReport)
{
	return UsbWrite(pHidDeviceData, pOutputReport, INFINITE);
}

BOOL UsbWrite(PHID_DEVICE_DATA pHidDeviceData, PREPORT_BUF pOutputReport, DWORD timeOut)
{
	OVERLAPPED ol;
	DWORD cbRet = 0;
	BOOL bRet = FALSE;

	memset(&ol, 0, sizeof(ol));
	ol.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

	EnterCriticalSection(&m_crSection);
	ResetEvent(ol.hEvent);

	bRet = WriteFile(pHidDeviceData->hDevice, pOutputReport, pHidDeviceData->OutputReportLen, &cbRet, &ol);
	LeaveCriticalSection(&m_crSection);

	if (!bRet)
	{
		if (GetLastError() == ERROR_IO_PENDING)
		{
			HANDLE handles[2] = { ol.hEvent, m_hStopEvent };
			DWORD waitRet = WaitForMultipleObjects(2, handles, FALSE, timeOut);

			if (waitRet == WAIT_OBJECT_0)
			{
				// Data came in
				bRet = GetOverlappedResult(pHidDeviceData->hDevice, &ol, &cbRet, TRUE);
			}
			else if (waitRet == (WAIT_OBJECT_0 + 1))
			{
				// Stop event was set
				ResetEvent(m_hStopEvent);

				bRet = FALSE;
			}
			else if (waitRet == WAIT_TIMEOUT)
			{
				bRet = FALSE;
			}
		}
	}

	CloseHandle(ol.hEvent);

	return bRet;
}

DWORD WINAPI EventWindowThread(LPVOID lpParam)
{
	INT exitcode = 1;
	WNDCLASS wc = { 0 };
	m_id = GetCurrentThreadId();
	WCHAR szClass[512];

	Format(szClass, WINDOW_CLASS, m_id);

	HWND hWnd = FindWindow(szClass, WINDOW_NAME);

	if (hWnd != NULL)
	{
		UnregisterClass(szClass, m_hInstance);
	}

	wc.lpszClassName 	= WINDOW_CLASS;
	wc.hInstance 		= m_hInstance;
	wc.lpfnWndProc		= WndProc;

	if (!RegisterClass(&wc))
		return 0;

	m_hWnd = CreateWindowEx(0, szClass, WINDOW_NAME, WS_OVERLAPPEDWINDOW, 0, 0, 1, 1, NULL, NULL, m_hInstance, NULL);

	if (m_hWnd == NULL)
	{
		UnregisterClass(szClass, m_hInstance);

		return 0;
	}

	RegisterDeviceInterface(m_hWnd);

	MSG msg;

	while(GetMessage(&msg, NULL, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	UnregisterClass(szClass, m_hInstance);

	SetEvent(m_hThreadExitEvent);

	return msg.wParam;
}

BOOL RegisterDeviceInterface(HWND hWnd)
{
	DEV_BROADCAST_DEVICEINTERFACE NotificationFilter;

	ZeroMemory(&NotificationFilter, sizeof(NotificationFilter));
	NotificationFilter.dbcc_size =
	sizeof(DEV_BROADCAST_DEVICEINTERFACE);
	NotificationFilter.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
	NotificationFilter.dbcc_classguid = GUID_DEVINTERFACE_HID;

	HDEVNOTIFY hDevNotify = RegisterDeviceNotification(m_hWnd, &NotificationFilter, DEVICE_NOTIFY_WINDOW_HANDLE);

	if(!hDevNotify)
		return FALSE;

	return TRUE;
}

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	PDEV_BROADCAST_HDR pHdr;
	PDEV_BROADCAST_HANDLE pHandle;
	PDEV_BROADCAST_DEVICEINTERFACE pInterface;

	switch(message)
	{
	case WM_CLOSE:
		DestroyWindow(hWnd);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	case WM_DEVICECHANGE:
		switch(wParam)
		{
			case DBT_DEVICEARRIVAL:
				pHdr = (PDEV_BROADCAST_HDR) lParam;

				switch (pHdr->dbch_devicetype)
				{
					case DBT_DEVTYP_DEVICEINTERFACE:
						pInterface = (PDEV_BROADCAST_DEVICEINTERFACE) lParam;
						INT id = 0;

						if(UsbOpen(pInterface->dbcc_name, &m_hidDeviceData[m_deviceCount]))
						{
							m_deviceCount++;

							RemoveNewDriver();
							SortDevices();

#ifdef DEBUG_OUTPUT
							DEBUGLOG(L"DBT_DEVICEARRIVAL");
							OutputDevices();
#endif

							for(INT id = 0; id < m_deviceCount; id++)
							{
								if(_wcsicmp(m_hidDeviceData[id].DevicePath, pInterface->dbcc_name) != 0)
									continue;

								if(m_usbDeviceAttachedCallback != NULL)
									m_usbDeviceAttachedCallback(id);

								break;
							}
						}
						break;
				}
				break;
			case DBT_DEVICEREMOVECOMPLETE:
				pHdr = (PDEV_BROADCAST_HDR) lParam;

				switch (pHdr->dbch_devicetype)
				{
					case DBT_DEVTYP_HANDLE:
						pHandle = (PDEV_BROADCAST_HANDLE) pHdr;

						UnregisterDeviceNotification(pHandle->dbch_hdevnotify);
						break;
					case DBT_DEVTYP_DEVICEINTERFACE:
						pInterface = (PDEV_BROADCAST_DEVICEINTERFACE) lParam;

						for(INT id = 0; id < m_deviceCount; id++)
						{
							if(_wcsicmp(m_hidDeviceData[id].DevicePath, pInterface->dbcc_name) != 0)
								continue;

							if(m_hidDeviceData[id].pHidDeviceData != NULL)
								free(m_hidDeviceData[id].pHidDeviceData);

							for(INT i = id; i < m_deviceCount - 1; i++)
								m_hidDeviceData[i] = m_hidDeviceData[i + 1];

							m_deviceCount--;

#ifdef DEBUG_OUTPUT
							DEBUGLOG(L"DBT_DEVICEREMOVECOMPLETE");
							OutputDevices();
#endif

							if(m_usbDeviceRemovedCallback != NULL)
								m_usbDeviceRemovedCallback(id);

							break;
						}
						break;
				}
				break;
		}
		break;
	default:
		break;
	}

	return DefWindowProc(hWnd, message, wParam, lParam);
}

BOOL GetDeviceInfo(HANDLE hidHandle, USHORT& vendorID, USHORT& productID, USHORT& versionNumber, USHORT& usage, USHORT& usagePage, USHORT& inputReportLen, USHORT& outputReportLen)
{
	HIDD_ATTRIBUTES hidAttributes;

	if (!HidD_GetAttributes(hidHandle, &hidAttributes))
		return FALSE;

	vendorID = hidAttributes.VendorID;
	productID = hidAttributes.ProductID;
	versionNumber = hidAttributes.VersionNumber;

	//DEBUGLOG(L"VendorID: %04x\n", hidAttributes.VendorID);
	//DEBUGLOG(L"ProductID: %04x\n", hidAttributes.ProductID);
	//DEBUGLOG(L"VersionNumber: %04x\n", hidAttributes.VersionNumber);

	PHIDP_PREPARSED_DATA hidPreparsedData;

	if (!HidD_GetPreparsedData(hidHandle, &hidPreparsedData))
		return FALSE;

	HIDP_CAPS hidCaps;

	if(HidP_GetCaps(hidPreparsedData, &hidCaps) != HIDP_STATUS_SUCCESS)
		return FALSE;

	usage = hidCaps.Usage;
	usagePage = hidCaps.UsagePage;
	inputReportLen = hidCaps.InputReportByteLength;
	outputReportLen = hidCaps.OutputReportByteLength;

	/* DEBUGLOG(L"UsagePage: %d\n", hidCaps.UsagePage);
	DEBUGLOG(L"Usage: %d\n", hidCaps.Usage);
	DEBUGLOG(L"InputReportByteLength: %d\n", hidCaps.InputReportByteLength);
	DEBUGLOG(L"OutputReportByteLength: %d\n", hidCaps.OutputReportByteLength);
	DEBUGLOG(L"FeatureReportByteLength: %d\n", hidCaps.FeatureReportByteLength);
	DEBUGLOG(L"NumberLinkCollectionNodes: %d\n", hidCaps.NumberLinkCollectionNodes);
	DEBUGLOG(L"NumberInputButtonCaps: %d\n", hidCaps.NumberInputButtonCaps);
	DEBUGLOG(L"NumberInputValueCaps: %d\n", hidCaps.NumberInputValueCaps);
	DEBUGLOG(L"NumberOutputButtonCaps: %d\n", hidCaps.NumberOutputButtonCaps);
	DEBUGLOG(L"NumberOutputValueCaps: %d\n", hidCaps.NumberOutputValueCaps);
	DEBUGLOG(L"NumberFeatureButtonCaps: %d\n", hidCaps.NumberFeatureButtonCaps);
	DEBUGLOG(L"NumberFeatureValueCaps: %d\n", hidCaps.NumberFeatureValueCaps); */

	// For more info use FillDeviceInfo
	// https://xp-dev.com/sc/36636/44/%2Ftrunk%2FProjects%2Fusb-device-hid-transfer-project-at91sam7x-ek%2Fusb-device-hid-transfer-project%2FHIDTest%2Fpnp.c

	return TRUE;
}

void strlow(wchar_t *src)
{
	for (unsigned int i = 0; i < wcslen(src); i++)
		src[i] = towlower(src[i]);
}

void strlow(char *s)
{
	for (;*s != '\0';s++)
		*s = tolower(*s);
}
