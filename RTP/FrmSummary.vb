Public Class FrmSummary
    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Me.Close()
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles BtnRefresh.Click
        FrmMain.GetSummary()

    End Sub

    Private Sub FrmSummary_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class