Imports System.Net

Public Class Printers
    Private Sub BtnPrintClose_Click(sender As Object, e As EventArgs) Handles BtnPrintClose.Click
        Me.Close()
    End Sub

    Private Sub Printers_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim i As Integer
        Dim pkInstalledPrinters As String

        For i = 0 To System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count - 1
            pkInstalledPrinters = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Item(i)
            Me.ListPrinters.Items.Add(pkInstalledPrinters)
        Next
    End Sub

    Private Sub ListPrinters_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListPrinters.SelectedIndexChanged
        LblSelectedPrinter.Text = Me.ListPrinters.Text

    End Sub

    Private Sub BtnPrintSave_Click(sender As Object, e As EventArgs) Handles BtnPrintSave.Click
        Dim fileNumber As Integer
        fileNumber = FreeFile()
        FrmMain.StrSelectedPrinter = Me.LblSelectedPrinter.Text
        FrmMain.TSPrinterName.ForeColor = Color.Green
        FrmMain.TSPrinterName.Text = FrmMain.StrSelectedPrinter
        FileOpen(fileNumber, FrmMain.S_PrintFile, OpenMode.Output) ' Open file for output.
        PrintLine(fileNumber, FrmMain.StrSelectedPrinter) ' Print text to file.

        FileClose(fileNumber)   ' Close file.
        Me.Close()
    End Sub
End Class