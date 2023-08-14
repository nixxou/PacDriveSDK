Public Class Form1
    Dim pacDrive As PacDrive = Nothing
    Dim random As Random = New Random()
    Dim timer As Timer = New Timer()

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim numDevices As Integer

        pacDrive = New PacDrive(Me)
        numDevices = pacDrive.Initialize()

        If numDevices > 0 Then
            timer.Interval = 100
            AddHandler timer.Tick, AddressOf timer_Tick
            timer.Enabled = True
        End If
    End Sub

    Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Dim data As UShort = CType((random.Next(&HFFFF) And &HFFFF), UShort)

        pacDrive.SetLEDStates(0, data)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        timer.Enabled = False
        pacDrive.Shutdown()
    End Sub
End Class
