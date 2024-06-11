Imports System.Net

Public Class FrmSettings
    Private Sub BtnSettingCancel_Click(sender As Object, e As EventArgs) Handles BtnSettingCancel.Click
        Me.Close()
    End Sub

    Private Sub BtnSettingsSave_Click(sender As Object, e As EventArgs) Handles BtnSettingsSave.Click
        '*** On save get current log file path and Update global variable 'StrLogPath'
        FrmMain.StrLogPath = LblLogPath.Text

        '******** Save settings
        Dim fileNumber As Integer
        fileNumber = FreeFile()
        FileOpen(fileNumber, FrmMain.SettingsFile, OpenMode.Output) ' Open file for output.
        Dim StrTemp As String
        '*** Save Default master
        StrTemp = Trim(TxtDefaultMaster.Text)
        FrmMain.StrDefaultMaster = StrTemp
        PrintLine(fileNumber, StrTemp) ' Print text to file.

        '*** Save Log Path
        '*** If path does not end with \ add it
        Dim TmpChar As String = Strings.Right(Trim(LblLogPath.Text), 1)
        If TmpChar <> "\" Then
            StrTemp = Trim(LblLogPath.Text) & "\"
        Else
            StrTemp = Trim(LblLogPath.Text)
        End If
        FrmMain.StrLogPath = StrTemp
        PrintLine(fileNumber, StrTemp) ' Print log path to file.
        '*** Update pat displayed on Tool Strip
        FrmMain.TSLogPath.Text = StrTemp

        FileClose(fileNumber)   ' Close file.
        If FrmMain.StrDefaultMaster <> "" Then FrmMain.ChkSkipMaster.Text = "Skip Master (Use " & FrmMain.StrDefaultMaster & ")"
        '******** End Save
        Me.Close()
    End Sub

    Private Sub BtnSetLogPath_Click(sender As Object, e As EventArgs) Handles BtnSetLogPath.Click
        If LogFolderBrowserDialog.ShowDialog() = DialogResult.OK Then
            LblLogPath.Text = LogFolderBrowserDialog.SelectedPath

        End If
    End Sub
End Class