VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   1635
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   2985
   LinkTopic       =   "Form1"
   ScaleHeight     =   1635
   ScaleWidth      =   2985
   StartUpPosition =   2  'CenterScreen
   Begin VB.CommandButton Command2 
      Caption         =   "Stop"
      Height          =   495
      Left            =   240
      TabIndex        =   1
      Top             =   840
      Width           =   2535
   End
   Begin VB.CommandButton Command1 
      Caption         =   "Start"
      Height          =   495
      Left            =   240
      TabIndex        =   0
      Top             =   240
      Width           =   2535
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Declare Function PacInitialize Lib "PacDrive32" () As Long
Private Declare Sub PacShutdown Lib "PacDrive32" ()
Private Declare Function PacSetLEDStates Lib "PacDrive32" (ByVal id As Long, ByVal data As Integer) As Long
Private Declare Function PacSetLEDState Lib "PacDrive32" (ByVal id As Long, ByVal port As Long, ByVal state As Long) As Long
Private Declare Function PacGetDeviceType Lib "PacDrive32" (ByVal id As Long) As Long
Private Declare Function PacGetVersionNumber Lib "PacDrive32" (ByVal id As Long) As Long

Private Declare Function Pac64SetLEDIntensities Lib "PacDrive32" (ByVal id As Long, data As Byte) As Long
Private Declare Function Pac64SetLEDIntensity Lib "PacDrive32" (ByVal id As Long, ByVal port As Long, ByVal intensity As Byte) As Long

Private Declare Function Pac64SetLEDStates Lib "PacDrive32" (ByVal id As Long, ByVal group As Long, ByVal data As Byte) As Long
Private Declare Function Pac64SetLEDState Lib "PacDrive32" (ByVal id As Long, ByVal group As Long, ByVal port As Long, ByVal state As Long) As Long

Private Declare Function Pac64SetLEDStatesRandom Lib "PacDrive32" (ByVal id As Long) As Long
Private Declare Function Pac64SetLEDFadeTime Lib "PacDrive32" (ByVal id As Long, ByVal fadeTime As Byte) As Long
Private Declare Function Pac64SetScriptStepDelay Lib "PacDrive32" (ByVal id As Long, ByVal stepDelay As Byte) As Long

Private Declare Function Pac64StartScriptRecording Lib "PacDrive32" (ByVal id As Long) As Long
Private Declare Function Pac64StopScriptRecording Lib "PacDrive32" (ByVal id As Long) As Long
Private Declare Function Pac64RunScript Lib "PacDrive32" (ByVal id As Long) As Long
Private Declare Function Pac64ClearFlash Lib "PacDrive32" (ByVal id As Long) As Long

Private Declare Function Pac64SetDeviceId Lib "PacDrive32" (ByVal id As Long, ByVal newId As Long) As Long

Private Declare Function PacProgramUHid Lib "PacDrive32" (ByVal id As Long, ByVal sFilePath As String) As Long

Private Sub Command1_Click()
    Dim numdevices As Integer
    
    numdevices = PacInitialize
    
    If numdevices > 0 Then
        PacSetLEDStates 0, &HAAAA
    End If
End Sub

Private Sub Command2_Click()
    PacSetLEDStates 0, 0
    
    PacShutdown
End Sub
