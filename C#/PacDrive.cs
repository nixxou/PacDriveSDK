// * ----------------------------------------------------------------------------
// * Author: Ben Baker
// * Website: headsoft.com.au
// * E-Mail: benbaker@headsoft.com.au
// * Copyright (C) 2015 Headsoft. All Rights Reserved.
// * ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PacDriveDemo
{
	public class PacDrive : IDisposable
	{
		// ****************************************************************
		// PacDrive / U-HID / Blue-HID
		// ****************************************************************
		// - 16 LED channels

		// ****************************************************************
		// Nano-LED
		// ****************************************************************
		// - 60 LED channels
		// - 256 brightness levels
		// - 20 RGB LEDs

		// ****************************************************************
		// PacLED64
		// ****************************************************************
		// - 64 LED channels
		// - 256 brightness levels
		// - 21 RGB LEDs

		// ****************************************************************
		// I-Pac Ultimate I/O
		// ****************************************************************
		// - 96 LED channels
		// - 256 brightness levels
		// - 32 RGB LEDs

		public const int MAX_DEVICES = 16;
		public const int MAX_LEDCOUNT = 96;
		public const int MAX_INTENSITY = 255;

		public enum DeviceType
		{
			Unknown,
			PacDrive,
			UHID,
			BlueHID,
			NanoLED,
			PacLED64,
			IPacUltimateIO,
			ServoStik,
			USBButton
		};

		public enum FlashSpeed : byte
		{
			AlwaysOn = 0,
			Seconds_2 = 1,
			Seconds_1 = 2,
			Seconds_0_5 = 3
		};

		// ================== 32-bit ====================

		[DllImport("PacDrive32.dll", EntryPoint = "PacSetCallbacks", CallingConvention = CallingConvention.StdCall)]
		private static extern void PacSetCallbacks32(USBDEVICE_ATTACHED_CALLBACK usbDeviceAttachedCallback, USBDEVICE_REMOVED_CALLBACK usbDeviceRemovedCallback);

		[DllImport("PacDrive32.dll", EntryPoint = "PacInitialize", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacInitialize32();

		[DllImport("PacDrive32.dll", EntryPoint = "PacShutdown", CallingConvention = CallingConvention.StdCall)]
		private static extern void PacShutdown32();

		[DllImport("PacDrive32.dll", EntryPoint = "PacSetLEDStates", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacSetLEDStates32(int id, ushort data);

		[DllImport("PacDrive32.dll", EntryPoint = "PacSetLEDState", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacSetLEDState32(int id, int port, [MarshalAs(UnmanagedType.Bool)] bool state);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetLEDStates", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDStates32(int id, int group, byte data);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetLEDState", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDState32(int id, int group, int port, [MarshalAs(UnmanagedType.Bool)] bool state);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetLEDStatesRandom", CallingConvention = CallingConvention.StdCall)]
		private static extern bool Pac64SetLEDStatesRandom32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetLEDIntensities", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDIntensities32(int id, byte[] dataArray);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetLEDIntensity", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDIntensity32(int id, int port, byte intensity);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetLEDFadeTime", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDFadeTime32(int id, byte fadeTime);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetLEDFlashSpeeds", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDFlashSpeeds32(int id, byte flashSpeed);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetLEDFlashSpeed", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDFlashSpeed32(int id, int port, byte flashSpeed);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64StartScriptRecording", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64StartScriptRecording32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64StopScriptRecording", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64StopScriptRecording32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetScriptStepDelay", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetScriptStepDelay32(int id, byte stepDelay);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64RunScript", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64RunScript32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64ClearFlash", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64ClearFlash32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "Pac64SetDeviceId", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetDeviceId32(int id, int newId);

		[DllImport("PacDrive32.dll", EntryPoint = "PacGetDeviceType", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacGetDeviceType32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "PacGetVendorId", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacGetVendorId32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "PacGetProductId", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacGetProductId32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "PacGetVersionNumber", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacGetVersionNumber32(int id);

		[DllImport("PacDrive32.dll", EntryPoint = "PacGetVendorName", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		private static extern void PacGetVendorName32(int id, StringBuilder vendorName);

		[DllImport("PacDrive32.dll", EntryPoint = "PacGetProductName", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		private static extern void PacGetProductName32(int id, StringBuilder productName);

		[DllImport("PacDrive32.dll", EntryPoint = "PacGetSerialNumber", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		private static extern void PacGetSerialNumber32(int id, StringBuilder serialNumber);

		[DllImport("PacDrive32.dll", EntryPoint = "PacGetDevicePath", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		private static extern void PacGetDevicePath32(int id, StringBuilder devicePath);

		[DllImport("PacDrive32.dll", EntryPoint = "PacProgramUHid", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacProgramUHid32(int id, StringBuilder fileName);

		[DllImport("PacDrive32.dll", EntryPoint = "PacSetServoStik4Way", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacSetServoStik4Way32();

		[DllImport("PacDrive32.dll", EntryPoint = "PacSetServoStik8Way", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacSetServoStik8Way32();

		[DllImport("PacDrive32.dll", EntryPoint = "USBButtonConfigurePermanent", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool USBButtonConfigurePermanent32(int id, byte[] dataArray);

		[DllImport("PacDrive32.dll", EntryPoint = "USBButtonConfigureTemporary", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool USBButtonConfigureTemporary32(int id, byte[] dataArray);

		[DllImport("PacDrive32.dll", EntryPoint = "USBButtonConfigureColor", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool USBButtonConfigureColor32(int id, byte red, byte green, byte blue);

		[DllImport("PacDrive32.dll", EntryPoint = "USBButtonGetState", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool USBButtonGetState32(int id, [MarshalAs(UnmanagedType.Bool)] ref bool state);

		// ================== 64-bit ====================

		[DllImport("PacDrive64.dll", EntryPoint = "PacSetCallbacks", CallingConvention = CallingConvention.StdCall)]
		private static extern void PacSetCallbacks64(USBDEVICE_ATTACHED_CALLBACK usbDeviceAttachedCallback, USBDEVICE_REMOVED_CALLBACK usbDeviceRemovedCallback);

		[DllImport("PacDrive64.dll", EntryPoint = "PacInitialize", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacInitialize64();

		[DllImport("PacDrive64.dll", EntryPoint = "PacShutdown", CallingConvention = CallingConvention.StdCall)]
		private static extern void PacShutdown64();

		[DllImport("PacDrive64.dll", EntryPoint = "PacSetLEDStates", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacSetLEDStates64(int id, ushort data);

		[DllImport("PacDrive64.dll", EntryPoint = "PacSetLEDState", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacSetLEDState64(int id, int port, [MarshalAs(UnmanagedType.Bool)] bool state);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetLEDStates", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDStates64(int id, int group, byte data);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetLEDState", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDState64(int id, int group, int port, [MarshalAs(UnmanagedType.Bool)] bool state);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetLEDStatesRandom", CallingConvention = CallingConvention.StdCall)]
		private static extern bool Pac64SetLEDStatesRandom64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetLEDIntensities", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDIntensities64(int id, byte[] dataArray);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetLEDIntensity", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDIntensity64(int id, int port, byte intensity);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetLEDFadeTime", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDFadeTime64(int id, byte fadeTime);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetLEDFlashSpeeds", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDFlashSpeeds64(int id, byte flashSpeed);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetLEDFlashSpeed", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetLEDFlashSpeed64(int id, int port, byte flashSpeed);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64StartScriptRecording", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64StartScriptRecording64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64StopScriptRecording", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64StopScriptRecording64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetScriptStepDelay", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetScriptStepDelay64(int id, byte stepDelay);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64RunScript", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64RunScript64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64ClearFlash", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64ClearFlash64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "Pac64SetDeviceId", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool Pac64SetDeviceId64(int id, int newId);

		[DllImport("PacDrive64.dll", EntryPoint = "PacGetDeviceType", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacGetDeviceType64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "PacGetVendorId", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacGetVendorId64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "PacGetProductId", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacGetProductId64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "PacGetVersionNumber", CallingConvention = CallingConvention.StdCall)]
		private static extern int PacGetVersionNumber64(int id);

		[DllImport("PacDrive64.dll", EntryPoint = "PacGetVendorName", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		private static extern void PacGetVendorName64(int id, StringBuilder vendorName);

		[DllImport("PacDrive64.dll", EntryPoint = "PacGetProductName", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		private static extern void PacGetProductName64(int id, StringBuilder productName);

		[DllImport("PacDrive64.dll", EntryPoint = "PacGetSerialNumber", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		private static extern void PacGetSerialNumber64(int id, StringBuilder serialNumber);

		[DllImport("PacDrive64.dll", EntryPoint = "PacGetDevicePath", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		private static extern void PacGetDevicePath64(int id, StringBuilder devicePath);

		[DllImport("PacDrive64.dll", EntryPoint = "PacProgramUHid", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacProgramUHid64(int id, StringBuilder fileName);

		[DllImport("PacDrive64.dll", EntryPoint = "PacSetServoStik4Way", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacSetServoStik4Way64();

		[DllImport("PacDrive64.dll", EntryPoint = "PacSetServoStik8Way", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PacSetServoStik8Way64();

		[DllImport("PacDrive64.dll", EntryPoint = "USBButtonConfigurePermanent", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool USBButtonConfigurePermanent64(int id, byte[] dataArray);

		[DllImport("PacDrive64.dll", EntryPoint = "USBButtonConfigureTemporary", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool USBButtonConfigureTemporary64(int id, byte[] dataArray);

		[DllImport("PacDrive64.dll", EntryPoint = "USBButtonConfigureColor", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool USBButtonConfigureColor64(int id, byte red, byte green, byte blue);

		[DllImport("PacDrive64.dll", EntryPoint = "USBButtonGetState", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool USBButtonGetState64(int id, [MarshalAs(UnmanagedType.Bool)] ref bool state);

		private delegate void USBDEVICE_ATTACHED_CALLBACK(int id);
		private delegate void USBDEVICE_REMOVED_CALLBACK(int id);

		public delegate void UsbDeviceAttachedDelegate(int id);
		public delegate void UsbDeviceRemovedDelegate(int id);

		public event UsbDeviceAttachedDelegate OnUsbDeviceAttached = null;
		public event UsbDeviceRemovedDelegate OnUsbDeviceRemoved = null;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		USBDEVICE_ATTACHED_CALLBACK UsbDeviceAttachedCallbackPtr = null;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		USBDEVICE_REMOVED_CALLBACK UsbDeviceRemovedCallbackPtr = null;

		private Control m_ctrl;

		private bool m_is64Bit = false;

		private bool[][] m_LEDState;
		private byte[][] m_LEDIntensity;

		private int m_deviceCount = 0;

		private bool m_disposed = false;

		public PacDrive(Control ctrl)
		{
			m_ctrl = ctrl;
			m_is64Bit = Is64Bit();

			UsbDeviceAttachedCallbackPtr = new USBDEVICE_ATTACHED_CALLBACK(UsbDeviceAttachedCallback);
			UsbDeviceRemovedCallbackPtr = new USBDEVICE_REMOVED_CALLBACK(UsbDeviceRemovedCallback);

			if (m_is64Bit)
				PacSetCallbacks64(UsbDeviceAttachedCallbackPtr, UsbDeviceRemovedCallbackPtr);
			else
				PacSetCallbacks32(UsbDeviceAttachedCallbackPtr, UsbDeviceRemovedCallbackPtr);

			m_LEDState = new bool[MAX_DEVICES][];
			m_LEDIntensity = new byte[MAX_DEVICES][];

			for (int i = 0; i < MAX_DEVICES; i++)
			{
				m_LEDState[i] = new bool[MAX_LEDCOUNT];
				m_LEDIntensity[i] = new byte[MAX_LEDCOUNT];

				for (int j = 0; j < MAX_LEDCOUNT; j++)
				{
					m_LEDState[i][j] = false;
					m_LEDIntensity[i][j] = 0;
				}
			}
		}

		private void UsbDeviceAttachedCallback(int id)
		{
			m_deviceCount++;

			if (OnUsbDeviceAttached != null)
				m_ctrl.BeginInvoke(OnUsbDeviceAttached, id);
		}

		private void UsbDeviceRemovedCallback(int id)
		{
			m_deviceCount--;

			if (OnUsbDeviceRemoved != null)
				m_ctrl.BeginInvoke(OnUsbDeviceRemoved, id);
		}

		public int Initialize()
		{
			m_deviceCount = (m_is64Bit ? PacInitialize64() : PacInitialize32());

			return m_deviceCount;
		}

		public void Shutdown()
		{
			if (m_is64Bit)
				PacShutdown64();
			else
				PacShutdown32();
		}

		public bool SetLEDStates(int id, ushort data)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? PacSetLEDStates64(id, data) : PacSetLEDStates32(id, data));
		}

		public bool SetLEDState(int id, int port, bool state)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? PacSetLEDState64(id, port, state) : PacSetLEDState32(id, port, state));
		}

		public bool SetLEDStates(int id, bool[] stateArray)
		{
			Array.Copy(stateArray, m_LEDState[id], Math.Min(stateArray.Length, m_LEDState[id].Length));

			return SetLEDStates(id);
		}

		public bool SetLEDStates(int id)
		{
			if (id >= m_deviceCount)
				return false;

            bool retVal = false;

            if (IsPacDrive(id))
			{
				ushort dataSend = 0;

				for (int i = 0; i < m_LEDState[id].Length; i++)
					if (m_LEDState[id][i])
						dataSend |= (ushort)(1 << i);

                retVal = (m_is64Bit ? PacSetLEDStates64(id, dataSend) : PacSetLEDStates32(id, dataSend));
			}
			else if (IsPac64(id))
			{
				for (int i = 0; i < m_LEDState[id].Length; i += 8)
				{
					int group = (i / 8) + 1;
					byte dataSend = 0;

					for (int j = 0; j < 8; j++)
						if (m_LEDState[id][i + j])
							dataSend |= (byte)(1 << j);

					retVal = (m_is64Bit ? Pac64SetLEDStates64(id, group, dataSend) : Pac64SetLEDStates32(id, group, dataSend));
				}
			}

            return retVal;
		}

		public bool SetLEDStatesRandom(int id)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64SetLEDStatesRandom64(id) : Pac64SetLEDStatesRandom32(id));
		}

        public bool SetLEDIntensity(int id)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64SetLEDIntensities64(id, m_LEDIntensity[id]) : Pac64SetLEDIntensities32(id, m_LEDIntensity[id]));
		}

		public bool SetLEDIntensity(int id, int port, byte intensity)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64SetLEDIntensity64(id, port, intensity) : Pac64SetLEDIntensity32(id, port, intensity));
		}

		public bool SetLEDFadeTime(int id, byte fadeTime)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64SetLEDFadeTime64(id, fadeTime) : Pac64SetLEDFadeTime32(id, fadeTime));
		}

		public bool SetLEDFlashSpeeds(int id, FlashSpeed flashSpeed)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64SetLEDFlashSpeeds64(id, (byte)flashSpeed) : Pac64SetLEDFlashSpeeds32(id, (byte)flashSpeed));
		}

		public bool SetLEDFlashSpeed(int id, int port, FlashSpeed flashSpeed)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64SetLEDFlashSpeed64(id, port, (byte)flashSpeed) : Pac64SetLEDFlashSpeed32(id, port, (byte)flashSpeed));
		}

		public bool StartScriptRecording(int id)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64StartScriptRecording64(id) : Pac64StartScriptRecording32(id));
		}

		public bool StopScriptRecording(int id)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64StopScriptRecording64(id) : Pac64StopScriptRecording32(id));
		}

		public bool SetScriptStepDelay(int id, byte stepDelay)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64SetScriptStepDelay64(id, stepDelay) : Pac64SetScriptStepDelay32(id, stepDelay));
		}

		public bool RunScript(int id)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64RunScript64(id) : Pac64RunScript32(id));
		}

		public bool ClearFlash(int id)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64ClearFlash64(id) : Pac64ClearFlash32(id));
		}

		public bool SetDeviceId(int id, int newId)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? Pac64SetDeviceId64(id, newId) : Pac64SetDeviceId32(id, newId));
		}

		public DeviceType GetDeviceType(int id)
		{
			if (id >= m_deviceCount)
				return DeviceType.Unknown;

			return (DeviceType)(m_is64Bit ? PacGetDeviceType64(id) : PacGetDeviceType32(id));
		}

		public int GetVendorId(int id)
		{
			if (id >= m_deviceCount)
				return 0;

			return (m_is64Bit ? PacGetVendorId64(id) : PacGetVendorId32(id));
		}

		public int GetProductId(int id)
		{
			if (id >= m_deviceCount)
				return 0;

			return (m_is64Bit ? PacGetProductId64(id) : PacGetProductId32(id));
		}

		public int GetVersionNumber(int id)
		{
			if (id >= m_deviceCount)
				return 0;

			return (m_is64Bit ? PacGetVersionNumber64(id) : PacGetVersionNumber32(id));
		}

		public string GetVendorName(int id)
		{
			if (id >= m_deviceCount)
				return String.Empty;

			StringBuilder sb = new StringBuilder(256);

			if (m_is64Bit)
				PacGetVendorName64(id, sb);
			else
				PacGetVendorName32(id, sb);

			return sb.ToString();
		}

		public string GetProductName(int id)
		{
			if (id >= m_deviceCount)
				return String.Empty;

			StringBuilder sb = new StringBuilder(256);

			if (m_is64Bit)
				PacGetProductName64(id, sb);
			else
				PacGetProductName32(id, sb);

			return sb.ToString();
		}

		public string GetSerialNumber(int id)
		{
			if (id >= m_deviceCount)
				return String.Empty;

			StringBuilder sb = new StringBuilder(256);

			if (m_is64Bit)
				PacGetSerialNumber64(id, sb);
			else
				PacGetSerialNumber32(id, sb);

			return sb.ToString();
		}

		public string GetDevicePath(int id)
		{
			if (id >= m_deviceCount)
				return String.Empty;

			StringBuilder sb = new StringBuilder(256);

			if (m_is64Bit)
				PacGetDevicePath64(id, sb);
			else
				PacGetDevicePath32(id, sb);

			return sb.ToString();
		}

		public bool ProgramUHid(int id, string fileName)
		{
			if (id >= m_deviceCount)
				return false;

			StringBuilder sb = new StringBuilder(fileName);

			return (m_is64Bit ? PacProgramUHid64(id, sb) : PacProgramUHid32(id, sb));
		}

		public bool SetServoStik4Way()
		{
			return (m_is64Bit ? PacSetServoStik4Way64() : PacSetServoStik4Way32());
		}

		public bool SetServoStik8Way()
		{
			return (m_is64Bit ? PacSetServoStik8Way64() : PacSetServoStik8Way32());
		}

		public bool SetUSBButtonConfigurePermanent(int id, byte[] dataArray)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? USBButtonConfigurePermanent64(id, dataArray) : USBButtonConfigurePermanent32(id, dataArray));
		}

		public bool SetUSBButtonConfigureTemporary(int id, byte[] dataArray)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? USBButtonConfigureTemporary64(id, dataArray) : USBButtonConfigureTemporary32(id, dataArray));
		}

		public bool SetUSBButtonConfigureColor(int id, byte red, byte green, byte blue)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? USBButtonConfigureColor64(id, red, green, blue) : USBButtonConfigureColor32(id, red, green, blue));
		}

		public bool GetUSBButtonState(int id, ref bool state)
		{
			if (id >= m_deviceCount)
				return false;

			return (m_is64Bit ? USBButtonGetState64(id, ref state) : USBButtonGetState32(id, ref state));
		}

		public void GetLEDControllerInfo(int id, out bool isLEDController, out bool isRGB, out int ledCount, out int rgbLEDCount, out int maxBrightness)
		{
			isLEDController = false;
			isRGB = false;
			ledCount = 0;
			rgbLEDCount = 0;
			maxBrightness = 0;

			if (id >= m_deviceCount)
				return;

			DeviceType deviceType = GetDeviceType(id);

			switch (deviceType)
			{
				case DeviceType.Unknown:
					break;
				case DeviceType.PacDrive:
				case DeviceType.UHID:
				case DeviceType.BlueHID:
					isLEDController = true;
					ledCount = 16;
					break;
				case DeviceType.NanoLED:
					isLEDController = true;
					isRGB = true;
					ledCount = 60;
					rgbLEDCount = 20;
					maxBrightness = 255;
					break;
				case DeviceType.PacLED64:
					isLEDController = true;
					isRGB = true;
					ledCount = 64;
					rgbLEDCount = 21;
					maxBrightness = 255;
					break;
				case DeviceType.IPacUltimateIO:
					isLEDController = true;
					isRGB = true;
					ledCount = 16;
					rgbLEDCount = 32;
					maxBrightness = 255;
					break;
				case DeviceType.ServoStik:
					break;
				case DeviceType.USBButton:
					break;
			}
		}

		public bool IsPacDrive(int id)
		{
			if (id >= m_deviceCount)
				return false;

			DeviceType deviceType = GetDeviceType(id);

			return (deviceType == DeviceType.PacDrive || deviceType == DeviceType.UHID || deviceType == DeviceType.BlueHID);
		}

		public bool IsPac64(int id)
		{
			if (id >= m_deviceCount)
				return false;

			DeviceType deviceType = GetDeviceType(id);

			return (deviceType == DeviceType.NanoLED || deviceType == DeviceType.PacLED64 || deviceType == DeviceType.IPacUltimateIO);
		}

		public void SetSingleLEDState(int id, int port, bool state)
		{
			m_LEDState[id][port] = state;

			SetLEDStates(id);
		}

		public void SetSingleLEDIntensity(int id, int port, byte intensity)
		{
			m_LEDIntensity[id][port] = intensity;

            SetLEDIntensity(id);
		}

		public void SetRGBLEDIntensity(int id, int[] portArray, byte[] intensityArray)
		{
			for (int i = 0; i < portArray.Length; i++)
				m_LEDIntensity[id][portArray[i]] = intensityArray[i];

            SetLEDIntensity(id);
		}

		public void SetRGBLEDState(int id, int[] portArray, bool[] stateArray)
		{
			for (int i = 0; i < portArray.Length; i++)
				m_LEDState[id][portArray[i]] = stateArray[i];

			SetLEDStates(id);
		}

		public void SetLEDStateAll(bool state)
		{
			for (int i = 0; i < m_deviceCount; i++)
			{
				for (int j = 0; j < MAX_LEDCOUNT; j++)
					m_LEDState[i][j] = state;

				SetLEDStates(i);
			}
		}

		public void SetLEDIntensity(int id, byte[] intensityArray)
		{
			Array.Copy(intensityArray, m_LEDIntensity[id], Math.Min(intensityArray.Length, m_LEDIntensity[id].Length));

            SetLEDIntensity(id);
		}

		public void SetLEDIntensityAll(byte intensity)
		{
			for (int i = 0; i < m_deviceCount; i++)
			{
				for (int j = 0; j < MAX_LEDCOUNT; j++)
					m_LEDIntensity[i][j] = intensity;

                SetLEDIntensity(i);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this); // remove this from gc finalizer list
		}

		private void Dispose(bool disposing)
		{
			if (!this.m_disposed) // dispose once only
			{
				if (disposing) // called from Dispose
				{
					// Dispose managed resources.
				}

				// Clean up unmanaged resources here.
				Shutdown();
			}

			m_disposed = true;
		}

		#endregion

		public int DeviceCount
		{
			get { return m_deviceCount; }
		}

		public bool[][] LEDState
		{
			get { return m_LEDState; }
			set { m_LEDState = value; }
		}

		public byte[][] LEDIntensity
		{
			get { return m_LEDIntensity; }
			set { m_LEDIntensity = value; }
		}

		private bool Is64Bit()
		{
			return Marshal.SizeOf(typeof(IntPtr)) == 8;
		}
	}
}
