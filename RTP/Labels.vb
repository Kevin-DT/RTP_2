Public Class Labels
    Private Sub chkFL100_CheckedChanged(sender As Object, e As EventArgs) Handles chkFL100.CheckedChanged
        If chkFL100.Checked Then
            TxtQtyFL100.Text = "1"
        Else
            TxtQtyFL100.Text = ""
        End If
    End Sub

    Private Sub chkFL300_CheckedChanged(sender As Object, e As EventArgs) Handles chkFL300.CheckedChanged
        If chkFL300.Checked Then
            TxtQtyFL300.Text = "1"
        Else
            TxtQtyFL300.Text = ""
        End If
    End Sub

    Private Sub chkFL500_CheckedChanged(sender As Object, e As EventArgs) Handles chkFL500.CheckedChanged
        If chkFL500.Checked Then
            TxtQtyFL500.Text = "1"
        Else
            TxtQtyFL500.Text = ""
        End If
    End Sub

    Private Sub chkFL1000_CheckedChanged(sender As Object, e As EventArgs) Handles chkFL1000.CheckedChanged
        If chkFL1000.Checked Then
            TxtQtyFL1000.Text = "1"
        Else
            TxtQtyFL1000.Text = ""
        End If
    End Sub

    Private Sub chkFL1500_CheckedChanged(sender As Object, e As EventArgs) Handles chkFL1500.CheckedChanged
        If chkFL1500.Checked Then
            TxtQtyFL1500.Text = "1"
        Else
            TxtQtyFL1500.Text = ""
        End If
    End Sub

    Private Sub chkFL2000_CheckedChanged(sender As Object, e As EventArgs) Handles chkFL2000.CheckedChanged
        If chkFL2000.Checked Then
            TxtQtyFL2000.Text = "1"
        Else
            TxtQtyFL2000.Text = ""
        End If
    End Sub

    Private Sub chkFL3000_CheckedChanged(sender As Object, e As EventArgs) Handles chkFL3000.CheckedChanged
        If chkFL3000.Checked Then
            TxtQtyFL3000.Text = "1"
        Else
            TxtQtyFL3000.Text = ""
        End If
    End Sub

    Private Sub chkFL4000_CheckedChanged(sender As Object, e As EventArgs) Handles chkFL4000.CheckedChanged
        If chkFL4000.Checked Then
            TxtQtyFL4000.Text = "1"
        Else
            TxtQtyFL4000.Text = ""
        End If
    End Sub

    Private Sub chkFLCuston_CheckedChanged(sender As Object, e As EventArgs) Handles chkFLCuston.CheckedChanged
        If chkFLCuston.Checked Then
            FLCustomLength.Visible = True
            lblCustom.Visible = True
            TxtQtyFLCustom.Text = "1"
        Else
            FLCustomLength.Visible = False
            lblCustom.Visible = False
            FLCustomLength.Text = ""
            TxtQtyFLCustom.Text = ""
        End If
    End Sub

    Private Sub ChkSurge_CheckedChanged(sender As Object, e As EventArgs) Handles ChkSurge.CheckedChanged
        If ChkSurge.Checked Then
            TxtQtySurge.Text = "1"
        Else
            TxtQtySurge.Text = ""
        End If
    End Sub

    Private Sub ChkDustCovers_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDustCovers.CheckedChanged
        If ChkDustCovers.Checked Then
            TxtQtyDustCovers.Text = "1"
            TxtQuantityDC.Text = "1"
        Else
            TxtQtyDustCovers.Text = ""
            TxtQuantityDC.Text = ""
        End If
    End Sub

    Private Sub ChkVTSDustCover_CheckedChanged(sender As Object, e As EventArgs) Handles ChkVTSDustCover.CheckedChanged
        If ChkVTSDustCover.Checked Then
            TxtQtyVTSDCovers.Text = "1"
            TxtVTSDC.Text = "1"
        Else
            TxtQtyVTSDCovers.Text = ""
            TxtVTSDC.Text = ""
        End If
    End Sub

    Private Sub chkStackers_CheckedChanged(sender As Object, e As EventArgs) Handles chkStackers.CheckedChanged
        If chkStackers.Checked Then
            TxtQtyStackers.Text = "1"
        Else
            TxtQtyStackers.Text = ""
        End If
    End Sub

    Private Sub ChkStackerLarge_CheckedChanged(sender As Object, e As EventArgs) Handles ChkStackerLarge.CheckedChanged
        If ChkStackerLarge.Checked Then
            TxtQtyStackersLarge.Text = "1"
        Else
            TxtQtyStackersLarge.Text = ""
        End If
    End Sub

    Private Sub chkRCD_BCLabels_CheckedChanged(sender As Object, e As EventArgs) Handles chkRCD_BCLabels.CheckedChanged
        If chkRCD_BCLabels.Checked Then
            TxtQtyRCD_BCLabels.Text = "1"
        Else
            TxtQtyRCD_BCLabels.Text = ""
        End If
    End Sub

    Private Sub chkSwbdSticker_CheckedChanged(sender As Object, e As EventArgs) Handles chkSwbdSticker.CheckedChanged
        If chkSwbdSticker.Checked Then
            TxtQtySwbdSticker.Text = "1"
        Else
            TxtQtySwbdSticker.Text = ""
        End If
    End Sub

    Private Sub Chk_VTL_CheckedChanged(sender As Object, e As EventArgs) Handles Chk_VTL.CheckedChanged
        If Chk_VTL.Checked Then
            Txt_QtyVTL.Text = "1"
        Else
            Txt_QtyVTL.Text = ""
        End If
    End Sub

    Private Sub Chk_VTS_CheckedChanged(sender As Object, e As EventArgs) Handles Chk_VTS.CheckedChanged
        If Chk_VTS.Checked Then
            Txt_QtyVTS.Text = "1"
        Else
            Txt_QtyVTS.Text = ""
        End If
    End Sub

    Private Sub chkShipping_CheckedChanged(sender As Object, e As EventArgs) Handles chkShipping.CheckedChanged
        If chkShipping.Checked Then
            TxtQtyShipping.Text = "1"
        Else
            TxtQtyShipping.Text = ""
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        TxtShip1.Text = ""
        TxtShip2.Text = ""
        TxtShip3.Text = ""
        TxtShip4.Text = ""
    End Sub

    Private Sub btnCancelLabels_Click(sender As Object, e As EventArgs) Handles btnCancelLabels.Click
        Me.Close()
    End Sub

    Private Sub btnPrintLabels_Click(sender As Object, e As EventArgs) Handles btnPrintLabels.Click
        Dim bOKtoClose As Boolean = True  '** Allow to close if printing is ok


        '*** Do printing
        Me.lblWaitPrinting.Visible = True

        '*** Set printer for all labels
        FrmMain.PrintDocument6.PrinterSettings.PrinterName = FrmMain.StrSelectedPrinter
        FrmMain.PrintDocument13.PrinterSettings.PrinterName = FrmMain.StrSelectedPrinter

        Application.DoEvents()

        '*** FLY LEADS
        '*** Check individual sizes.
        If chkFL100.Checked Then
            If IsNumeric(TxtQtyFL100.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL100"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFL100.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFL100.Checked = False
                TxtQtyFL100.Text = ""
            Else
                bOKtoClose = False
            End If
        End If

        If chkFL300.Checked Then
            If IsNumeric(TxtQtyFL300.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL300"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFL300.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFL300.Checked = False
                TxtQtyFL300.Text = ""
            Else
                bOKtoClose = False
            End If
        End If

        If chkFL500.Checked Then
            If IsNumeric(TxtQtyFL500.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL500"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFL500.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFL500.Checked = False
                TxtQtyFL500.Text = ""
            Else
                bOKtoClose = False
            End If

        End If

        If chkFL1000.Checked Then
            If IsNumeric(TxtQtyFL1000.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL1000"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFL1000.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFL1000.Checked = False
                TxtQtyFL1000.Text = ""
            Else
                bOKtoClose = False
            End If
        End If

        If chkFL1500.Checked Then
            If IsNumeric(TxtQtyFL1500.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL1500"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFL1500.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFL1500.Checked = False
                TxtQtyFL1500.Text = ""
            Else
                bOKtoClose = False
            End If
        End If

        If chkFL2000.Checked Then
            If IsNumeric(TxtQtyFL2000.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL2000"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFL2000.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFL2000.Checked = False
                TxtQtyFL2000.Text = ""
            Else
                bOKtoClose = False
            End If
        End If

        If chkFL3000.Checked Then
            If IsNumeric(TxtQtyFL3000.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL3000"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFL3000.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFL3000.Checked = False
                TxtQtyFL3000.Text = ""
            Else
                bOKtoClose = False
            End If
        End If

        If chkFL4000.Checked Then
            If IsNumeric(TxtQtyFL4000.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL4000"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFL4000.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFL4000.Checked = False
                TxtQtyFL4000.Text = ""
            Else
                bOKtoClose = False
            End If
        End If

        '*** Custom Length Fly Lead
        If chkFLCuston.Checked Then
            If IsNumeric(TxtQtyFLCustom.Text) And IsNumeric(FLCustomLength.Text) Then
                FrmMain.S_LabelHeading = "Communications Fly Lead"
                FrmMain.S_PartNumber = "FL_Custom     (Length = " & FLCustomLength.Text & " mm)"
                FrmMain.S_PartDesc = "Communications Cable Between Masters and" & vbCrLf & "Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyFLCustom.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                        FrmMain.S_PartNumber = "FL_Custom     (Length = " & FLCustomLength.Text & " mm)"
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkFLCuston.Checked = False
                TxtQtyFLCustom.Text = ""
                FLCustomLength.Text = ""
            Else
                bOKtoClose = False
            End If
        End If


        '*** SURGE DIVERTER
        If ChkSurge.Checked Then
            If IsNumeric(TxtQtySurge.Text) Then
                FrmMain.S_LabelHeading = "Surge Diverter - 1 Set"
                FrmMain.S_PartNumber = "RTSP"
                FrmMain.S_PartDesc = "Cattegory III Surge Device."
                Dim iNumtoPrint As Integer = Val(TxtQtySurge.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                ChkSurge.Checked = False
                TxtQtySurge.Text = ""
            Else
                bOKtoClose = False
            End If
        End If

        '*** DUST COVERS
        If ChkDustCovers.Checked Then
            If IsNumeric(TxtQtyDustCovers.Text) And IsNumeric(TxtQuantityDC.Text) Then
                FrmMain.S_LabelHeading = "RJ12 Dust Cover - Quantity: " & TxtQuantityDC.Text
                FrmMain.S_PartNumber = "RTSDC"
                FrmMain.S_PartDesc = "Communications Port Dust Cover."
                Dim iNumtoPrint As Integer = Val(TxtQtyDustCovers.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                ChkDustCovers.Checked = False
                TxtQtyDustCovers.Text = ""
            Else
                bOKtoClose = False
            End If

        End If

        '*** VTS DUST COVERS
        If ChkVTSDustCover.Checked Then
            If IsNumeric(TxtQtyVTSDCovers.Text) And IsNumeric(TxtVTSDC.Text) Then
                FrmMain.S_LabelHeading = "VSD Dust Cover - Quantity: " & TxtVTSDC.Text
                FrmMain.S_PartNumber = "DSVTS"
                FrmMain.S_PartDesc = "Verification Socket Dust Cover."
                Dim iNumtoPrint As Integer = Val(TxtQtyVTSDCovers.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                ChkVTSDustCover.Checked = False
                TxtQtyVTSDCovers.Text = ""
            Else
                bOKtoClose = False
            End If

        End If

        '*** UNIVERSAL STACKING BRACKETS
        If chkStackers.Checked Then
            If IsNumeric(TxtQtyStackers.Text) Then
                FrmMain.S_LabelHeading = "Universal Stacking Brackets - 1 Set"
                FrmMain.S_PartNumber = "USB"
                FrmMain.S_PartDesc = "Stackers for Wi-Fi Master & Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyStackers.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkStackers.Checked = False
                TxtQtyStackers.Text = ""
            Else
                bOKtoClose = False
            End If

        End If

        '*** UNIVERSAL STACKING BRACKETS - LARGE
        If ChkStackerLarge.Checked Then
            If IsNumeric(TxtQtyStackersLarge.Text) Then
                FrmMain.S_LabelHeading = "Universal Stacking Brackets - Large - 1 Set"
                FrmMain.S_PartNumber = "USBL"
                FrmMain.S_PartDesc = "Large Stackers for Wi-Fi Master & Channel Boards."
                Dim iNumtoPrint As Integer = Val(TxtQtyStackersLarge.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                ChkStackerLarge.Checked = False
                TxtQtyStackersLarge.Text = ""
            Else
                bOKtoClose = False
            End If

        End If

        '*** RCD BARCODE LABELS
        If chkRCD_BCLabels.Checked Then
            If IsNumeric(TxtQtyRCD_BCLabels.Text) Then
                FrmMain.S_LabelHeading = "RCD Barcode Labels - 24 Pk"
                FrmMain.S_PartNumber = "RBL"
                FrmMain.S_PartDesc = "Labels for RCD Identification."
                Dim iNumtoPrint As Integer = Val(TxtQtyRCD_BCLabels.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkRCD_BCLabels.Checked = False
                TxtQtyRCD_BCLabels.Text = ""
            Else
                bOKtoClose = False
            End If

        End If

        '*** SWITCHBOARD BARCODE LABELS
        If chkSwbdSticker.Checked Then
            If IsNumeric(TxtQtySwbdSticker.Text) Then
                FrmMain.S_LabelHeading = "Switchboard Barcode Labels - 10 Pk"
                FrmMain.S_PartNumber = "RT005"
                FrmMain.S_PartDesc = "Labels for Switchboard Identification and Test Log."
                Dim iNumtoPrint As Integer = Val(TxtQtySwbdSticker.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                chkSwbdSticker.Checked = False
                TxtQtySwbdSticker.Text = ""
            Else
                bOKtoClose = False
            End If

        End If

        '*** VTL Verification Test Lead
        If Chk_VTL.Checked Then
            If IsNumeric(Txt_QtyVTL.Text) Then
                FrmMain.S_LabelHeading = "Verification Test Lead"
                FrmMain.S_PartNumber = "VTL"
                FrmMain.S_PartDesc = "Lead for connecting verifier unit."
                Dim iNumtoPrint As Integer = Val(Txt_QtyVTL.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                Chk_VTL.Checked = False
                Txt_QtyVTL.Text = ""
            Else
                bOKtoClose = False
            End If

        End If

        '*** VTL Verification Test Socket
        If Chk_VTS.Checked Then
            If IsNumeric(Txt_QtyVTS.Text) Then
                FrmMain.S_LabelHeading = "Verification Test Socket"
                FrmMain.S_PartNumber = "VTS"
                FrmMain.S_PartDesc = "Socket for connecting verifier unit."
                Dim iNumtoPrint As Integer = Val(Txt_QtyVTS.Text)
                If Not ChkOnlySummaryLabel.Checked Then
                    For x = 1 To iNumtoPrint
                        FrmMain.PrintDocument6.Print()
                    Next
                End If

                '** IF option to print summary labels is checked print a label with the QTY on it.
                If ChkSummaryLabel.Checked Then
                    FrmMain.IntSummaryNoToPrint = iNumtoPrint.ToString
                    FrmMain.PrintDocument13.Print()
                    FrmMain.PrintDocument13.Print()
                End If

                '** Clear values in case printo is halted for other items.
                Chk_VTS.Checked = False
                Txt_QtyVTS.Text = ""
            Else
                bOKtoClose = False
            End If

        End If


        '*** SHIPPING LABELS    
        If chkShipping.Checked Then

            If IsNumeric(TxtQtyShipping.Text) Then
                FrmMain.S_ShipLine1 = TxtShip1.Text
                FrmMain.S_ShipLine2 = TxtShip2.Text
                FrmMain.S_ShipLine3 = TxtShip3.Text
                FrmMain.S_ShipLine4 = TxtShip4.Text
                FrmMain.PrintDocument13.PrinterSettings.PrinterName = FrmMain.StrSelectedPrinter
                Dim iNumtoPrint As Integer = Val(TxtQtyShipping.Text)
                For x = 1 To iNumtoPrint
                    FrmMain.PrintDocument13.Print()
                Next
                '** Clear values in case print is halted for other items.
                TxtShip1.Text = ""
                TxtShip2.Text = ""
                TxtShip3.Text = ""
                TxtShip4.Text = ""
                TxtQtyShipping.Text = ""
                chkShipping.Checked = False
            Else
                bOKtoClose = False
            End If

        End If


        Application.DoEvents()
        Me.lblWaitPrinting.Visible = False
        'Close
        If bOKtoClose Then
            Me.Close()
        Else
            MsgBox("One or more errors occured." & vbCrLf & "Please check quantities for items still ticked.", vbCritical + vbOKOnly, "Errors were encountered")
        End If
    End Sub
End Class