Public Class F_NDBMessage
    Private Sub PicClose_Click(sender As Object, e As EventArgs) Handles PicClose.Click
        PicClose.Enabled = False
        Me.Close()
    End Sub

    Private Sub F_NDBMessage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PicClose.Enabled = True
    End Sub

    Private Sub F_NDBMessage_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        Dim sRTKeyPressed = e.KeyChar.ToString()
        Debug.Print(">" & sRTKeyPressed & "<")
        Debug.Print(">" & Asc(sRTKeyPressed) & "<")
        Dim I_Key As Integer = Asc(sRTKeyPressed)
        If I_Key = 27 Then Me.Close()
    End Sub
End Class