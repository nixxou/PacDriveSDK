' * ----------------------------------------------------------------------------
' * Author: Ben Baker
' * Website: headsoft.com.au
' * E-Mail: benbaker@headsoft.com.au
' * Copyright (C) 2015 Headsoft. All Rights Reserved.
' * ----------------------------------------------------------------------------

Imports System.Windows.Forms
Imports System.IO
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Collections.Generic
Imports System

Public Class PacDrive
    Implements IDisposable
    ' ****************************************************************
    ' PacDrive / U-HID / Blue-HID
    ' ****************************************************************
    ' - 16 LED channels

    ' ****************************************************************
    ' Nano-LED
    ' ****************************************************************
    ' - 60 LED channels
    ' - 256 brightness levels
    ' - 20 RGB LEDs

    ' ****************************************************************
    ' PacLED64
    ' ****************************************************************
    ' - 64 LED channels
    ' - 256 brightness levels
    ' - 21 RGB LEDs

    ' ****************************************************************
    ' I-Pac Ultimate I/O
    ' ****************************************************************
    ' - 96 LED channels
    ' - 256 brightness levels
    ' - 32 RGB LEDs

    Public Const MAX_DEVICES As Integer = 16
    Public Const MAX_LEDCOUNT As Integer = 96
    Public Const MAX_INTENSITY As Integer = 255

    Public Enum DeviceType
        Unknown
        PacDrive
        UHID
        BlueHID
        NanoLED
        PacLED64
        IPacUltimateIO
        ServoStik
        USBButton
    End Enum

    Public Enum FlashSpeed As Byte
        AlwaysOn = 0
        Seconds_2 = 1
        Seconds_1 = 2
        Seconds_0_5 = 3
    End Enum

    ' ================== 32-bit ====================

    <DllImport("PacDrive32.dll", EntryPoint:="PacSetCallbacks", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacSetCallbacks32(ByVal usbDeviceAttachedCallback As USBDEVICE_ATTACHED_CALLBACK, ByVal usbDeviceRemovedCallback As USBDEVICE_REMOVED_CALLBACK)
    End Sub

    <DllImport("PacDrive32.dll", EntryPoint:="PacInitialize", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacInitialize32() As Integer
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacShutdown", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacShutdown32()
    End Sub

    <DllImport("PacDrive32.dll", EntryPoint:="PacSetLEDStates", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacSetLEDStates32(ByVal id As Integer, ByVal data As UShort) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacSetLEDState", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacSetLEDState32(ByVal id As Integer, ByVal port As Integer, <MarshalAs(UnmanagedType.Bool)> ByVal state As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetLEDStates", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDStates32(ByVal id As Integer, ByVal group As Integer, ByVal data As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetLEDState", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDState32(ByVal id As Integer, ByVal group As Integer, ByVal port As Integer, <MarshalAs(UnmanagedType.Bool)> ByVal state As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetLEDStatesRandom", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDStatesRandom32(ByVal id As Integer) As Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetLEDIntensities", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDIntensities32(ByVal id As Integer, ByVal dataArray As Byte()) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetLEDIntensity", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDIntensity32(ByVal id As Integer, ByVal port As Integer, ByVal intensity As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetLEDFadeTime", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDFadeTime32(ByVal id As Integer, ByVal fadeTime As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetLEDFlashSpeeds", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDFlashSpeeds32(ByVal id As Integer, ByVal flashSpeed As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetLEDFlashSpeed", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDFlashSpeed32(ByVal id As Integer, ByVal port As Integer, ByVal flashSpeed As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64StartScriptRecording", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64StartScriptRecording32(ByVal id As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64StopScriptRecording", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64StopScriptRecording32(ByVal id As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetScriptStepDelay", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetScriptStepDelay32(ByVal id As Integer, ByVal stepDelay As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64RunScript", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64RunScript32(ByVal id As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64ClearFlash", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64ClearFlash32(ByVal id As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="Pac64SetDeviceId", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetDeviceId32(ByVal id As Integer, ByVal newId As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacGetDeviceType", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacGetDeviceType32(ByVal id As Integer) As Integer
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacGetVendorId", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacGetVendorId32(ByVal id As Integer) As Integer
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacGetProductId", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacGetProductId32(ByVal id As Integer) As Integer
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacGetVersionNumber", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacGetVersionNumber32(ByVal id As Integer) As Integer
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacGetVendorName", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacGetVendorName32(ByVal id As Integer, ByVal vendorName As StringBuilder)
    End Sub

    <DllImport("PacDrive32.dll", EntryPoint:="PacGetProductName", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacGetProductName32(ByVal id As Integer, ByVal productName As StringBuilder)
    End Sub

    <DllImport("PacDrive32.dll", EntryPoint:="PacGetSerialNumber", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacGetSerialNumber32(ByVal id As Integer, ByVal serialNumber As StringBuilder)
    End Sub

    <DllImport("PacDrive32.dll", EntryPoint:="PacGetDevicePath", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacGetDevicePath32(ByVal id As Integer, ByVal devicePath As StringBuilder)
    End Sub

    <DllImport("PacDrive32.dll", EntryPoint:="PacProgramUHid", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacProgramUHid32(ByVal id As Integer, ByVal fileName As StringBuilder) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacSetServoStik4Way", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacSetServoStik4Way32() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="PacSetServoStik8Way", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacSetServoStik8Way32() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="USBButtonConfigurePermanent", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function USBButtonConfigurePermanent32(ByVal id As Integer, ByVal dataArray As Byte()) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="USBButtonConfigureTemporary", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function USBButtonConfigureTemporary32(ByVal id As Integer, ByVal dataArray As Byte()) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="USBButtonConfigureColor", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function USBButtonConfigureColor32(ByVal id As Integer, ByVal red As Byte, ByVal green As Byte, ByVal blue As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive32.dll", EntryPoint:="USBButtonGetState", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function USBButtonGetState32(ByVal id As Integer, <MarshalAs(UnmanagedType.Bool)> ByRef state As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    ' ================== 64-bit ====================

    <DllImport("PacDrive64.dll", EntryPoint:="PacSetCallbacks", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacSetCallbacks64(ByVal usbDeviceAttachedCallback As USBDEVICE_ATTACHED_CALLBACK, ByVal usbDeviceRemovedCallback As USBDEVICE_REMOVED_CALLBACK)
    End Sub

    <DllImport("PacDrive64.dll", EntryPoint:="PacInitialize", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacInitialize64() As Integer
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacShutdown", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacShutdown64()
    End Sub

    <DllImport("PacDrive64.dll", EntryPoint:="PacSetLEDStates", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacSetLEDStates64(ByVal id As Integer, ByVal data As UShort) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacSetLEDState", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacSetLEDState64(ByVal id As Integer, ByVal port As Integer, <MarshalAs(UnmanagedType.Bool)> ByVal state As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetLEDStates", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDStates64(ByVal id As Integer, ByVal group As Integer, ByVal data As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetLEDState", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDState64(ByVal id As Integer, ByVal group As Integer, ByVal port As Integer, <MarshalAs(UnmanagedType.Bool)> ByVal state As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetLEDStatesRandom", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDStatesRandom64(ByVal id As Integer) As Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetLEDIntensities", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDIntensities64(ByVal id As Integer, ByVal dataArray As Byte()) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetLEDIntensity", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDIntensity64(ByVal id As Integer, ByVal port As Integer, ByVal intensity As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetLEDFadeTime", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDFadeTime64(ByVal id As Integer, ByVal fadeTime As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetLEDFlashSpeeds", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDFlashSpeeds64(ByVal id As Integer, ByVal flashSpeed As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetLEDFlashSpeed", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetLEDFlashSpeed64(ByVal id As Integer, ByVal port As Integer, ByVal flashSpeed As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64StartScriptRecording", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64StartScriptRecording64(ByVal id As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64StopScriptRecording", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64StopScriptRecording64(ByVal id As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetScriptStepDelay", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetScriptStepDelay64(ByVal id As Integer, ByVal stepDelay As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64RunScript", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64RunScript64(ByVal id As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64ClearFlash", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64ClearFlash64(ByVal id As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="Pac64SetDeviceId", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function Pac64SetDeviceId64(ByVal id As Integer, ByVal newId As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacGetDeviceType", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacGetDeviceType64(ByVal id As Integer) As Integer
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacGetVendorId", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacGetVendorId64(ByVal id As Integer) As Integer
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacGetProductId", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacGetProductId64(ByVal id As Integer) As Integer
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacGetVersionNumber", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacGetVersionNumber64(ByVal id As Integer) As Integer
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacGetVendorName", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacGetVendorName64(ByVal id As Integer, ByVal vendorName As StringBuilder)
    End Sub

    <DllImport("PacDrive64.dll", EntryPoint:="PacGetProductName", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacGetProductName64(ByVal id As Integer, ByVal productName As StringBuilder)
    End Sub

    <DllImport("PacDrive64.dll", EntryPoint:="PacGetSerialNumber", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacGetSerialNumber64(ByVal id As Integer, ByVal serialNumber As StringBuilder)
    End Sub

    <DllImport("PacDrive64.dll", EntryPoint:="PacGetDevicePath", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Sub PacGetDevicePath64(ByVal id As Integer, ByVal devicePath As StringBuilder)
    End Sub

    <DllImport("PacDrive64.dll", EntryPoint:="PacProgramUHid", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacProgramUHid64(ByVal id As Integer, ByVal fileName As StringBuilder) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacSetServoStik4Way", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacSetServoStik4Way64() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="PacSetServoStik8Way", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function PacSetServoStik8Way64() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="USBButtonConfigurePermanent", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function USBButtonConfigurePermanent64(ByVal id As Integer, ByVal dataArray As Byte()) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="USBButtonConfigureTemporary", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function USBButtonConfigureTemporary64(ByVal id As Integer, ByVal dataArray As Byte()) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="USBButtonConfigureColor", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function USBButtonConfigureColor64(ByVal id As Integer, ByVal red As Byte, ByVal green As Byte, ByVal blue As Byte) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("PacDrive64.dll", EntryPoint:="USBButtonGetState", CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function USBButtonGetState64(ByVal id As Integer, <MarshalAs(UnmanagedType.Bool)> ByRef state As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Private Delegate Sub USBDEVICE_ATTACHED_CALLBACK(ByVal id As Integer)
    Private Delegate Sub USBDEVICE_REMOVED_CALLBACK(ByVal id As Integer)

    Public Delegate Sub UsbDeviceAttachedDelegate(ByVal id As Integer)
    Public Delegate Sub UsbDeviceRemovedDelegate(ByVal id As Integer)

    Public Event OnUsbDeviceAttached As UsbDeviceAttachedDelegate
    Public Event OnUsbDeviceRemoved As UsbDeviceRemovedDelegate

    <MarshalAs(UnmanagedType.FunctionPtr)> _
    Private UsbDeviceAttachedCallbackPtr As USBDEVICE_ATTACHED_CALLBACK

    <MarshalAs(UnmanagedType.FunctionPtr)> _
    Private UsbDeviceRemovedCallbackPtr As USBDEVICE_REMOVED_CALLBACK

    Private m_ctrl As Control

    Private m_is64Bit As Boolean = False

    Private m_LEDState As Boolean()()
    Private m_LEDIntensity As Byte()()

    Private m_deviceCount As Integer = 0

    Private m_disposed As Boolean = False

    Public Sub New(ByVal ctrl As Control)
        m_ctrl = ctrl
        m_is64Bit = Is64Bit()

        UsbDeviceAttachedCallbackPtr = New USBDEVICE_ATTACHED_CALLBACK(AddressOf UsbDeviceAttachedCallback)
        UsbDeviceRemovedCallbackPtr = New USBDEVICE_REMOVED_CALLBACK(AddressOf UsbDeviceRemovedCallback)

        If m_is64Bit Then
            PacSetCallbacks64(UsbDeviceAttachedCallbackPtr, UsbDeviceRemovedCallbackPtr)
        Else
            PacSetCallbacks32(UsbDeviceAttachedCallbackPtr, UsbDeviceRemovedCallbackPtr)
        End If

        m_LEDState = New Boolean(MAX_DEVICES)() {}
        m_LEDIntensity = New Byte(MAX_DEVICES)() {}

        For i As Integer = 0 To MAX_DEVICES - 1
            m_LEDState(i) = New Boolean(MAX_LEDCOUNT) {}
            m_LEDIntensity(i) = New Byte(MAX_LEDCOUNT) {}

            For j As Integer = 0 To MAX_LEDCOUNT - 1
                m_LEDState(i)(j) = False
                m_LEDIntensity(i)(j) = 0
            Next
        Next
    End Sub

    Private Sub UsbDeviceAttachedCallback(ByVal id As Integer)
        m_deviceCount += 1

        RaiseEvent OnUsbDeviceAttached(id)
    End Sub

    Private Sub UsbDeviceRemovedCallback(ByVal id As Integer)
        m_deviceCount -= 1

        RaiseEvent OnUsbDeviceRemoved(id)
    End Sub

    Public Function Initialize() As Integer
        m_deviceCount = If(m_is64Bit, PacInitialize64(), PacInitialize32())

        Return m_deviceCount
    End Function

    Public Sub Shutdown()
        If m_is64Bit Then
            PacShutdown64()
        Else
            PacShutdown32()
        End If
    End Sub

    Public Function SetLEDStates(ByVal id As Integer, ByVal data As UShort) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, PacSetLEDStates64(id, data), PacSetLEDStates32(id, data))
    End Function

    Public Function SetLEDState(ByVal id As Integer, ByVal port As Integer, ByVal state As Boolean) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, PacSetLEDState64(id, port, state), PacSetLEDState32(id, port, state))
    End Function

    Public Function SetLEDStates(ByVal id As Integer, ByVal stateArray As Boolean()) As Boolean
        Array.Copy(stateArray, m_LEDState(id), Math.Min(stateArray.Length, m_LEDState(id).Length))

        Return SetLEDStates(id)
    End Function

    Public Function SetLEDStates(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Dim retVal As Boolean = False

        If IsPacDrive(id) Then
            Dim dataSend As UShort = 0

            For i As Integer = 0 To m_LEDState(id).Length - 1
                If m_LEDState(id)(i) Then
                    dataSend = dataSend Or CType(1 << i, UShort)
                End If
            Next

            retVal = If(m_is64Bit, PacSetLEDStates64(id, dataSend), PacSetLEDStates32(id, dataSend))
        ElseIf IsPac64(id) Then
            For i As Integer = 0 To m_LEDState(id).Length - 1 Step 8
                Dim group As Integer = i / 8
                Dim dataSend As Byte = 0

                For j As Integer = 0 To 7
                    If m_LEDState(id)(i + j) Then
                        dataSend = dataSend Or CType(1 << j, Byte)
                    End If
                Next

                retVal = If(m_is64Bit, Pac64SetLEDStates64(id, group, dataSend), Pac64SetLEDStates32(id, group, dataSend))
            Next
        End If

        Return retVal
    End Function

    Public Function SetLEDStatesRandom(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64SetLEDStatesRandom64(id), Pac64SetLEDStatesRandom32(id))
    End Function

    Public Function SetLEDIntensity(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64SetLEDIntensities64(id, m_LEDIntensity(id)), Pac64SetLEDIntensities32(id, m_LEDIntensity(id)))
    End Function

    Public Function SetLEDIntensity(ByVal id As Integer, ByVal port As Integer, ByVal intensity As Byte) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64SetLEDIntensity64(id, port, intensity), Pac64SetLEDIntensity32(id, port, intensity))
    End Function

    Public Function SetLEDFadeTime(ByVal id As Integer, ByVal fadeTime As Byte) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64SetLEDFadeTime64(id, fadeTime), Pac64SetLEDFadeTime32(id, fadeTime))
    End Function

    Public Function SetLEDFlashSpeeds(ByVal id As Integer, ByVal flashSpeed As FlashSpeed) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64SetLEDFlashSpeeds64(id, CType(flashSpeed, Byte)), Pac64SetLEDFlashSpeeds32(id, CType(flashSpeed, Byte)))
    End Function

    Public Function SetLEDFlashSpeed(ByVal id As Integer, ByVal port As Integer, ByVal flashSpeed As FlashSpeed) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64SetLEDFlashSpeed64(id, port, CType(flashSpeed, Byte)), Pac64SetLEDFlashSpeed32(id, port, CType(flashSpeed, Byte)))
    End Function

    Public Function StartScriptRecording(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64StartScriptRecording64(id), Pac64StartScriptRecording32(id))
    End Function

    Public Function StopScriptRecording(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64StopScriptRecording64(id), Pac64StopScriptRecording32(id))
    End Function

    Public Function SetScriptStepDelay(ByVal id As Integer, ByVal stepDelay As Byte) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64SetScriptStepDelay64(id, stepDelay), Pac64SetScriptStepDelay32(id, stepDelay))
    End Function

    Public Function RunScript(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64RunScript64(id), Pac64RunScript32(id))
    End Function

    Public Function ClearFlash(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64ClearFlash64(id), Pac64ClearFlash32(id))
    End Function

    Public Function SetDeviceId(ByVal id As Integer, ByVal newId As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, Pac64SetDeviceId64(id, newId), Pac64SetDeviceId32(id, newId))
    End Function

    Public Function GetDeviceType(ByVal id As Integer) As DeviceType
        If id >= m_deviceCount Then
            Return DeviceType.Unknown
        End If

        Return CType(If(m_is64Bit, PacGetDeviceType64(id), PacGetDeviceType32(id)), DeviceType)
    End Function

    Public Function GetVendorId(ByVal id As Integer) As Integer
        If id >= m_deviceCount Then
            Return 0
        End If

        Return If(m_is64Bit, PacGetVendorId64(id), PacGetVendorId32(id))
    End Function

    Public Function GetProductId(ByVal id As Integer) As Integer
        If id >= m_deviceCount Then
            Return 0
        End If

        Return If(m_is64Bit, PacGetProductId64(id), PacGetProductId32(id))
    End Function

    Public Function GetVersionNumber(ByVal id As Integer) As Integer
        If id >= m_deviceCount Then
            Return 0
        End If

        Return If(m_is64Bit, PacGetVersionNumber64(id), PacGetVersionNumber32(id))
    End Function

    Public Function GetVendorName(ByVal id As Integer) As String
        If id >= m_deviceCount Then
            Return String.Empty
        End If

        Dim sb As New StringBuilder(256)

        If m_is64Bit Then
            PacGetVendorName64(id, sb)
        Else
            PacGetVendorName32(id, sb)
        End If

        Return sb.ToString()
    End Function

    Public Function GetProductName(ByVal id As Integer) As String
        If id >= m_deviceCount Then
            Return String.Empty
        End If

        Dim sb As New StringBuilder(256)

        If m_is64Bit Then
            PacGetProductName64(id, sb)
        Else
            PacGetProductName32(id, sb)
        End If

        Return sb.ToString()
    End Function

    Public Function GetSerialNumber(ByVal id As Integer) As String
        If id >= m_deviceCount Then
            Return String.Empty
        End If

        Dim sb As New StringBuilder(256)

        If m_is64Bit Then
            PacGetSerialNumber64(id, sb)
        Else
            PacGetSerialNumber32(id, sb)
        End If

        Return sb.ToString()
    End Function

    Public Function GetDevicePath(ByVal id As Integer) As String
        If id >= m_deviceCount Then
            Return String.Empty
        End If

        Dim sb As New StringBuilder(256)

        If m_is64Bit Then
            PacGetDevicePath64(id, sb)
        Else
            PacGetDevicePath32(id, sb)
        End If

        Return sb.ToString()
    End Function

    Public Function ProgramUHid(ByVal id As Integer, ByVal fileName As String) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Dim sb As New StringBuilder(fileName)

        Return If(m_is64Bit, PacProgramUHid64(id, sb), PacProgramUHid32(id, sb))
    End Function

    Public Function SetServoStik4Way() As Boolean
        Return If(m_is64Bit, PacSetServoStik4Way64(), PacSetServoStik4Way32())
    End Function

    Public Function SetServoStik8Way() As Boolean
        Return If(m_is64Bit, PacSetServoStik8Way64(), PacSetServoStik8Way32())
    End Function

    Public Function SetUSBButtonConfigurePermanent(ByVal id As Integer, ByVal dataArray As Byte()) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, USBButtonConfigurePermanent64(id, dataArray), USBButtonConfigurePermanent32(id, dataArray))
    End Function

    Public Function SetUSBButtonConfigureTemporary(ByVal id As Integer, ByVal dataArray As Byte()) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, USBButtonConfigureTemporary64(id, dataArray), USBButtonConfigureTemporary32(id, dataArray))
    End Function

    Public Function SetUSBButtonConfigureColor(ByVal id As Integer, ByVal red As Byte, ByVal green As Byte, ByVal blue As Byte) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, USBButtonConfigureColor64(id, red, green, blue), USBButtonConfigureColor32(id, red, green, blue))
    End Function

    Public Function GetUSBButtonState(ByVal id As Integer, ByRef state As Boolean) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Return If(m_is64Bit, USBButtonGetState64(id, state), USBButtonGetState32(id, state))
    End Function

    Public Sub GetLEDControllerInfo(ByVal id As Integer, ByRef isLEDController As Boolean, ByRef isRGB As Boolean, ByRef ledCount As Integer, ByRef rgbLEDCount As Integer, ByRef maxBrightness As Integer)
        isLEDController = False
        isRGB = False
        ledCount = 0
        rgbLEDCount = 0
        maxBrightness = 0

        If id >= m_deviceCount Then
            Return
        End If

        Dim deviceType As DeviceType = GetDeviceType(id)

        Select Case deviceType
            Case DeviceType.Unknown
                Exit Select
            Case DeviceType.PacDrive, DeviceType.UHID, DeviceType.BlueHID
                isLEDController = True
                ledCount = 16
                Exit Select
            Case DeviceType.NanoLED
                isLEDController = True
                isRGB = True
                ledCount = 60
                rgbLEDCount = 20
                maxBrightness = 255
                Exit Select
            Case DeviceType.PacLED64
                isLEDController = True
                isRGB = True
                ledCount = 64
                rgbLEDCount = 21
                maxBrightness = 255
                Exit Select
            Case DeviceType.IPacUltimateIO
                isLEDController = True
                isRGB = True
                ledCount = 16
                rgbLEDCount = 32
                maxBrightness = 255
                Exit Select
            Case DeviceType.ServoStik
                Exit Select
            Case DeviceType.USBButton
                Exit Select
        End Select
    End Sub

    Public Function IsPacDrive(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Dim deviceType As DeviceType = GetDeviceType(id)

        Return (deviceType = DeviceType.PacDrive OrElse deviceType = DeviceType.UHID OrElse deviceType = DeviceType.BlueHID)
    End Function

    Public Function IsPac64(ByVal id As Integer) As Boolean
        If id >= m_deviceCount Then
            Return False
        End If

        Dim deviceType As DeviceType = GetDeviceType(id)

        Return (deviceType = DeviceType.NanoLED OrElse deviceType = DeviceType.PacLED64 OrElse deviceType = DeviceType.IPacUltimateIO)
    End Function

    Public Sub SetSingleLEDState(ByVal id As Integer, ByVal port As Integer, ByVal state As Boolean)
        m_LEDState(id)(port) = state

        SetLEDStates(id)
    End Sub

    Public Sub SetSingleLEDIntensity(ByVal id As Integer, ByVal port As Integer, ByVal intensity As Byte)
        m_LEDIntensity(id)(port) = intensity

        SetLEDIntensity(id)
    End Sub

    Public Sub SetRGBLEDIntensity(ByVal id As Integer, ByVal portArray As Integer(), ByVal intensityArray As Byte())
        For i As Integer = 0 To portArray.Length - 1
            m_LEDIntensity(id)(portArray(i)) = intensityArray(i)
        Next

        SetLEDIntensity(id)
    End Sub

    Public Sub SetRGBLEDState(ByVal id As Integer, ByVal portArray As Integer(), ByVal stateArray As Boolean())
        For i As Integer = 0 To portArray.Length - 1
            m_LEDState(id)(portArray(i)) = stateArray(i)
        Next

        SetLEDStates(id)
    End Sub

    Public Sub SetLEDStateAll(ByVal state As Boolean)
        For i As Integer = 0 To m_deviceCount - 1
            For j As Integer = 0 To MAX_LEDCOUNT
                m_LEDState(i)(j) = state
            Next
            SetLEDStates(i)
        Next
    End Sub


    Public Sub SetLEDIntensity(ByVal id As Integer, ByVal intensityArray As Byte())
        Array.Copy(intensityArray, m_LEDIntensity(id), Math.Min(intensityArray.Length, m_LEDIntensity(id).Length))

        SetLEDIntensity(id)
    End Sub

    Public Sub SetLEDIntensityAll(ByVal intensity As Byte)
        For i As Integer = 0 To m_deviceCount - 1
            For j As Integer = 0 To MAX_LEDCOUNT
                m_LEDIntensity(i)(j) = intensity
            Next
            SetLEDIntensity(i)
        Next
    End Sub

#Region "IDisposable Members"

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
        ' remove this from gc finalizer list
    End Sub

    Private Sub Dispose(ByVal disposing As Boolean)
        If Not Me.m_disposed Then
            ' dispose once only
            If disposing Then
                ' Dispose managed resources.
                ' called from Dispose
            End If

            ' Clean up unmanaged resources here.
            Shutdown()
        End If

        m_disposed = True
    End Sub

#End Region

    Public ReadOnly Property DeviceCount() As Integer
        Get
            Return m_deviceCount
        End Get
    End Property

    Public Property LEDState() As Boolean()()
        Get
            Return m_LEDState
        End Get
        Set(ByVal value As Boolean()())
            m_LEDState = value
        End Set
    End Property

    Public Property LEDIntensity() As Byte()()
        Get
            Return m_LEDIntensity
        End Get
        Set(ByVal value As Byte()())
            m_LEDIntensity = value
        End Set
    End Property

    Private Function Is64Bit() As Boolean
        Return Marshal.SizeOf(GetType(IntPtr)) = 8
    End Function
End Class
