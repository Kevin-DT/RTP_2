Imports System.Net
Imports Ats.RapidTest.DataConfiguration
Imports Ats.RapidTest.DataConfiguration.Model

Public Class Pack
    Public S_ScannedCode As String = ""
    Public PackRunningTotal As Integer = 0
    Public CIBF_Count As Integer = 0
    Public MC8F_Count As Integer = 0
    Public CIB_Count As Integer = 0
    Public MC8_Count As Integer = 0
    Public MASTERM_Count As Integer = 0
    Public MC8F5_Count As Integer = 0
    Public MASTER_Count As Integer = 0

    Public BIsDirty As Boolean = False
    Public TimeStamp As String

    Private Sub BtnLoadPO_Click(sender As Object, e As EventArgs) Handles BtnLoadPO.Click
        Txt_PackedBy.Text = FrmMain.TxtUser.Text
        Txt_PONumber.Text = FrmMain.TxtOrderRef.Text
        Txt_Purchaser.Text = FrmMain.TxtPurchaser.Text
    End Sub
    Private Sub ClearPackingForm()


        '***** Clear grid and text boxes
        DG_Packing.Rows.Clear()
        Txt_PackedBy.Text = ""
        Txt_PONumber.Text = ""
        Txt_Purchaser.Text = ""
        PackRunningTotal = 0
        Lbl_IItemCount.Text = "0"

        CIBF_Count = 0
        LblCIBF.Text = "0"

        MC8F_Count = 0
        LblMC8F.Text = "0"

        CIB_Count = 0
        LblCIB.Text = "0"

        MC8_Count = 0
        LblMC8.Text = "0"

        MASTERM_Count = 0
        LblMasterM.Text = "0"

        MC8F5_Count = 0
        LblMC8F5.Text = "0"

        MASTER_Count = 0
        LblMasterCount.Text = "0"

        '****** ARRAY for list of Modules
        '*** Empty array 
        Dim I_UpperLimit1 As Integer = UBound(FrmMain.S_Pack_Master, 1)
        Dim I_UpperLimit2 As Integer = UBound(FrmMain.S_Pack_Master, 2)
        For x = 0 To I_UpperLimit1
            For y = 0 To I_UpperLimit2
                FrmMain.S_Pack_Master(x, y) = ""
                FrmMain.S_Pack_MC8F5(x, y) = ""
                FrmMain.S_Pack_MasterM(x, y) = ""
                FrmMain.S_Pack_MC8(x, y) = ""
                FrmMain.S_Pack_CIB(x, y) = ""
                FrmMain.S_Pack_MC8SS(x, y) = ""
                FrmMain.S_Pack_CIBSS(x, y) = ""
            Next
        Next

        '** Array for other Products
        ' ArrayProducts
        I_UpperLimit1 = UBound(FrmMain.ArrayProducts, 1)
        I_UpperLimit2 = UBound(FrmMain.ArrayProducts, 2)

        For x = 0 To I_UpperLimit1
            For y = 0 To I_UpperLimit2
                FrmMain.ArrayProducts(x, y) = ""
            Next
        Next
    End Sub
    Private Sub Btn_ClearForm_Click(sender As Object, e As EventArgs) Handles Btn_ClearForm.Click
        Dim S_Msg = "Continuing will clear the grid and all text boxes." & vbCrLf & "Do you want to continue?"
        Dim S_Style = MsgBoxStyle.YesNo Or MsgBoxStyle.DefaultButton2 Or MsgBoxStyle.Critical
        Dim I_msgResult = MsgBox(S_Msg, S_Style, "WARNING")

        If I_msgResult = MsgBoxResult.Yes Then
            ClearPackingForm()
        End If
    End Sub

    Private Sub Chk_Scan_CheckedChanged(sender As Object, e As EventArgs) Handles Chk_Scan.CheckedChanged
        Dim OK2Scan As Integer = 0
        Dim S_MessageText As String = ""
        If Txt_PackedBy.Text = "" Then OK2Scan += 1
        If Txt_PONumber.Text = "" Then OK2Scan += 2
        If Txt_Purchaser.Text = "" Then OK2Scan += 4

        Txt_PONumber.Focus()

        Select Case OK2Scan
            Case 0
                If Chk_Scan.Checked Then
                    Txt_PackedBy.Text = StrConv(Txt_PackedBy.Text, VbStrConv.ProperCase)
                    S_ScannedCode = ""
                    Txt_PackedBy.Enabled = False
                    Txt_PONumber.Enabled = False
                    Txt_Purchaser.Enabled = False
                    Btn_Cancel.Enabled = False
                    Btn_Pack.Enabled = False
                    Btn_ClearForm.Enabled = False
                    KeyPreview = True
                    Chk_Scan.BackColor = Color.Red
                    Chk_Scan.Text = "Scanning"
                Else
                    Txt_PackedBy.Enabled = True
                    Txt_PONumber.Enabled = True
                    Txt_Purchaser.Enabled = True
                    KeyPreview = False
                    Chk_Scan.Text = "Scan"
                    Btn_Cancel.Enabled = True
                    Btn_Pack.Enabled = True
                    Btn_ClearForm.Enabled = True
                    Chk_Scan.BackColor = SystemColors.Control
                End If
            Case 1
                S_MessageText = "Please provide a name for Packed By:"
            Case 2
                S_MessageText = "Please provide a PO Number"
            Case 3
                S_MessageText = "Please provide a name for Packed By: and a PO Number"
            Case 4
                S_MessageText = "Please provide a Purchaser"
            Case 5
                S_MessageText = "Please provide a name for Packed By: and a Purchaser"
            Case 6
                S_MessageText = "Please provide a PO Number and a Purchaser"
            Case 7
                S_MessageText = "Please provide a name for Packed By:, a PO Number and a Purchaser"
        End Select

        If OK2Scan > 0 Then MsgBox(S_MessageText, vbCritical + vbOKOnly, "Incomplete details")
    End Sub

    Private Sub Btn_Cancel_Click(sender As Object, e As EventArgs) Handles Btn_Cancel.Click
        Dim BOK2Close As Boolean = False

        If BIsDirty Then
            Dim I_Reponse As Integer = MsgBox("Closing will clear all data." & vbCrLf & "Continue Y/N?", vbQuestion + vbYesNo, "Warning")
            If I_Reponse = MsgBoxResult.Yes Then BOK2Close = True
        Else
            BOK2Close = True
        End If

        If BOK2Close Then
            '***** Clear grid and text boxes
            DG_Packing.Rows.Clear()
            Txt_PackedBy.Text = ""
            Txt_PONumber.Text = ""
            Txt_Purchaser.Text = ""

            '****** ARRAY for list of Modules
            '*** Empty array 
            Dim I_UpperLimit1 As Integer = UBound(FrmMain.S_Pack_Master, 1)
            Dim I_UpperLimit2 As Integer = UBound(FrmMain.S_Pack_Master, 2)
            For x = 0 To I_UpperLimit1
                For y = 0 To I_UpperLimit2
                    FrmMain.S_Pack_Master(x, y) = ""
                    FrmMain.S_Pack_MC8F5(x, y) = ""
                    FrmMain.S_Pack_MasterM(x, y) = ""
                    FrmMain.S_Pack_MC8(x, y) = ""
                    FrmMain.S_Pack_CIB(x, y) = ""
                    FrmMain.S_Pack_CIBF(x, y) = ""
                    FrmMain.S_Pack_MC8SS(x, y) = ""
                    FrmMain.S_Pack_CIBSS(x, y) = ""
                Next
            Next

            '** Array for other Products
            ' ArrayProducts
            I_UpperLimit1 = UBound(FrmMain.ArrayProducts, 1)
            I_UpperLimit2 = UBound(FrmMain.ArrayProducts, 2)

            For x = 0 To I_UpperLimit1
                For y = 0 To I_UpperLimit2
                    FrmMain.ArrayProducts(x, y) = ""
                Next
            Next
            PackRunningTotal = 0
            CIBF_Count = 0
            MC8F_Count = 0
            CIB_Count = 0
            MC8_Count = 0
            MASTERM_Count = 0
            MC8F5_Count = 0
            MASTER_Count = 0

            Lbl_IItemCount.Text = "0"
            LblCIB.Text = "0"
            LblCIBF.Text = "0"
            LblMasterCount.Text = "0"
            LblMasterM.Text = "0"
            LblMC8.Text = "0"
            LblMC8F.Text = "0"
            LblMC8F5.Text = "0"

            BIsDirty = False
            Me.Close()
        End If
    End Sub

    Private Sub Btn_Pack_Click(sender As Object, e As EventArgs) Handles Btn_Pack.Click
        Debug.Print("ROWS: " & DG_Packing.RowCount.ToString)
        TimeStamp = Format(Now(), "HHmmss")
        Dim OK2Scan As Integer = 0
        Dim S_MessageText As String = ""
        If Txt_PackedBy.Text = "" Then OK2Scan += 1
        If Txt_PONumber.Text = "" Then OK2Scan += 2
        If Txt_Purchaser.Text = "" Then OK2Scan += 4
        If DG_Packing.RowCount < 1 Then OK2Scan += 8

        Txt_PONumber.Focus()

        Select Case OK2Scan
            Case 0
            Case 1
                S_MessageText = "Please provide a name for Packed By:"
            Case 2
                S_MessageText = "Please provide a PO Number"
            Case 3
                S_MessageText = "Please provide a name for Packed By: and a PO Number"
            Case 4
                S_MessageText = "Please provide a Purchaser"
            Case 5
                S_MessageText = "Please provide a name for Packed By: and a Purchaser"
            Case 6
                S_MessageText = "Please provide a PO Number and a Purchaser"
            Case 7
                S_MessageText = "Please provide a name for Packed By:, a PO Number and a Purchaser"
            Case Else
                S_MessageText = "Nothing to Print"
        End Select

        If OK2Scan > 0 Then MsgBox(S_MessageText, vbCritical + vbOKOnly, "Did you forget something?")

        '*** If everything OK, continue
        If OK2Scan = 0 Then
            Lbl_Updating.Visible = True
            Application.DoEvents()

            '*** Add headings to csv file
            SavePO("Product ID", "Barcode/Description", "Details", "Programmed by", "Date", "", "Quantity")

            '** Update NDB for Masters
            If FrmMain.S_Pack_Master(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_Master(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_Master(x, 1), FrmMain.S_Pack_Master(x, 0), FrmMain.S_Pack_Master(x, 3), FrmMain.S_Pack_Master(x, 4), FrmMain.S_Pack_Master(x, 5), FrmMain.S_Pack_Master(x, 6))
                        SavePO(FrmMain.S_Pack_Master(x, 3), FrmMain.S_Pack_Master(x, 0), FrmMain.S_Pack_Master(x, 1), FrmMain.S_Pack_Master(x, 6), FrmMain.S_Pack_Master(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If

            '** Update NDB for MC8F5
            If FrmMain.S_Pack_MC8F5(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_MC8F5(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_MC8F5(x, 1), FrmMain.S_Pack_MC8F5(x, 0), FrmMain.S_Pack_MC8F5(x, 3), FrmMain.S_Pack_MC8F5(x, 4), FrmMain.S_Pack_MC8F5(x, 5), FrmMain.S_Pack_MC8F5(x, 6))
                        SavePO(FrmMain.S_Pack_MC8F5(x, 3), FrmMain.S_Pack_MC8F5(x, 0), FrmMain.S_Pack_MC8F5(x, 1), FrmMain.S_Pack_MC8F5(x, 6), FrmMain.S_Pack_MC8F5(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If

            '** Update NDB for MastersM
            If FrmMain.S_Pack_MasterM(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_MasterM(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_MasterM(x, 1), FrmMain.S_Pack_MasterM(x, 0), FrmMain.S_Pack_MasterM(x, 3), FrmMain.S_Pack_MasterM(x, 4), FrmMain.S_Pack_MasterM(x, 5), FrmMain.S_Pack_MasterM(x, 6))
                        SavePO(FrmMain.S_Pack_MasterM(x, 3), FrmMain.S_Pack_MasterM(x, 0), FrmMain.S_Pack_MasterM(x, 1), FrmMain.S_Pack_MasterM(x, 6), FrmMain.S_Pack_MasterM(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If
            '** Update NDB for MC8's
            If FrmMain.S_Pack_MC8(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_MC8(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_MC8(x, 1), FrmMain.S_Pack_MC8(x, 0), FrmMain.S_Pack_MC8(x, 3), FrmMain.S_Pack_MC8(x, 4), FrmMain.S_Pack_MC8(x, 5), FrmMain.S_Pack_MC8(x, 6))
                        SavePO(FrmMain.S_Pack_MC8(x, 3), FrmMain.S_Pack_MC8(x, 0), FrmMain.S_Pack_MC8(x, 1), FrmMain.S_Pack_MC8(x, 6), FrmMain.S_Pack_MC8(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If
            '** Update NDB for CIB's
            If FrmMain.S_Pack_CIB(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_CIB(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_CIB(x, 1), FrmMain.S_Pack_CIB(x, 0), FrmMain.S_Pack_CIB(x, 3), FrmMain.S_Pack_CIB(x, 4), FrmMain.S_Pack_CIB(x, 5), FrmMain.S_Pack_CIB(x, 6))
                        SavePO(FrmMain.S_Pack_CIB(x, 3), FrmMain.S_Pack_CIB(x, 0), FrmMain.S_Pack_CIB(x, 1), FrmMain.S_Pack_CIB(x, 6), FrmMain.S_Pack_CIB(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If
            '** Update NDB for MC8SS
            If FrmMain.S_Pack_MC8SS(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_MC8SS(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_MC8SS(x, 1), FrmMain.S_Pack_MC8SS(x, 0), FrmMain.S_Pack_MC8SS(x, 3), FrmMain.S_Pack_MC8SS(x, 4), FrmMain.S_Pack_MC8SS(x, 5), FrmMain.S_Pack_MC8SS(x, 6))
                        SavePO(FrmMain.S_Pack_MC8SS(x, 3), FrmMain.S_Pack_MC8SS(x, 0), FrmMain.S_Pack_MC8SS(x, 1), FrmMain.S_Pack_MC8SS(x, 6), FrmMain.S_Pack_MC8SS(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If
            '** Update NDB for CIBSS
            If FrmMain.S_Pack_CIBSS(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_CIBSS(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_CIBSS(x, 1), FrmMain.S_Pack_CIBSS(x, 0), FrmMain.S_Pack_CIBSS(x, 3), FrmMain.S_Pack_CIBSS(x, 4), FrmMain.S_Pack_CIBSS(x, 5), FrmMain.S_Pack_CIBSS(x, 6))
                        SavePO(FrmMain.S_Pack_CIBSS(x, 3), FrmMain.S_Pack_CIBSS(x, 0), FrmMain.S_Pack_CIBSS(x, 1), FrmMain.S_Pack_CIBSS(x, 6), FrmMain.S_Pack_CIBSS(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If


            '*** 26.11.2019 add Fuse & 0 link support
            '** Update NDB for MC8F
            If FrmMain.S_Pack_MC8F(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_MC8F(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_MC8F(x, 1), FrmMain.S_Pack_MC8F(x, 0), FrmMain.S_Pack_MC8F(x, 3), FrmMain.S_Pack_MC8F(x, 4), FrmMain.S_Pack_MC8F(x, 5), FrmMain.S_Pack_MC8F(x, 6))
                        SavePO(FrmMain.S_Pack_MC8F(x, 3), FrmMain.S_Pack_MC8F(x, 0), FrmMain.S_Pack_MC8F(x, 1), FrmMain.S_Pack_MC8F(x, 6), FrmMain.S_Pack_MC8F(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If

            '** Update NDB for CIBF
            If FrmMain.S_Pack_CIBF(0, 0) <> "" Then
                For x = 0 To 500
                    If FrmMain.S_Pack_CIBF(x, 0) <> "" Then
                        UpdateNDB(FrmMain.S_Pack_CIBF(x, 1), FrmMain.S_Pack_CIBF(x, 0), FrmMain.S_Pack_CIBF(x, 3), FrmMain.S_Pack_CIBF(x, 4), FrmMain.S_Pack_CIBF(x, 5), FrmMain.S_Pack_CIBF(x, 6))
                        SavePO(FrmMain.S_Pack_CIBF(x, 3), FrmMain.S_Pack_CIBF(x, 0), FrmMain.S_Pack_CIBF(x, 1), FrmMain.S_Pack_CIBF(x, 6), FrmMain.S_Pack_CIBF(x, 5), "", "1")
                    Else Exit For
                    End If
                Next
            End If


            '============================================
            '** Additional products
            If FrmMain.ArrayProducts(0, 0) = "YES" Then
                '*** Step through each product in the array currently items are 1-17
                For x = 1 To 20
                    If FrmMain.ArrayProducts(x, 0) <> "" Then
                        ' NO NDB UPDATE 
                        SavePO(FrmMain.ArrayProducts(x, 0), FrmMain.ArrayProducts(x, 1), FrmMain.ArrayProducts(x, 2), "", "", "", FrmMain.ArrayProducts(x, 3))
                    End If
                Next
            End If
            '=============================================

            '** Check if multiple pages need to be printed
            ' Get highest number of lines to print
            ' Need to add additional pages if required.
            Dim DoPoSending As Boolean = False
            If Val(FrmMain.S_Pack_Master(0, 2)) > 0 Then DoPoSending = True
            If Val(FrmMain.S_Pack_MC8F5(0, 2)) > 0 Then DoPoSending = True
            If Val(FrmMain.S_Pack_MasterM(0, 2)) > 0 Then DoPoSending = True
            If Val(FrmMain.S_Pack_MC8(0, 2)) > 0 Then DoPoSending = True
            If Val(FrmMain.S_Pack_CIB(0, 2)) > 0 Then DoPoSending = True
            If Val(FrmMain.S_Pack_MC8SS(0, 2)) > 0 Then DoPoSending = True
            If Val(FrmMain.S_Pack_CIBSS(0, 2)) > 0 Then DoPoSending = True

            If Val(FrmMain.S_Pack_MC8F(0, 2)) > 0 Then DoPoSending = True
            If Val(FrmMain.S_Pack_CIBF(0, 2)) > 0 Then DoPoSending = True

            If FrmMain.ArrayProducts(0, 0) = "YES" Then FrmMain.PrintExtras = True

            ' While (I_Lines2Print > 0) Or (FrmMain.PrintExtras = True)
            If DoPoSending Or (FrmMain.PrintExtras = True) Then
                Dim IntPoDone As Integer = FrmMain.SendPO()
                Debug.Print("Returned from email")
                FrmMain.PrintExtras = False
            End If

            Lbl_Updating.Visible = False
            BIsDirty = False
            Dim IntCloseForm As Integer = MsgBox("Do you wish to Close the packing form?", vbYesNo + vbQuestion, "Are you finished")


            If IntCloseForm = vbYes Then
                '***** After updating DB and printing clear the Grid.
                DG_Packing.Rows.Clear()
                '****** ARRAY for list of Modules
                '*** Empty array 
                Dim I_UpperLimit3 = UBound(FrmMain.S_Pack_Master, 1)
                Dim I_UpperLimit4 = UBound(FrmMain.S_Pack_Master, 2)

                For x = 0 To I_UpperLimit3
                    For y = 0 To I_UpperLimit4
                        FrmMain.S_Pack_Master(x, y) = ""

                        FrmMain.S_Pack_MC8F5(x, y) = ""
                        FrmMain.S_Pack_MasterM(x, y) = ""

                        FrmMain.S_Pack_MC8(x, y) = ""
                        FrmMain.S_Pack_CIB(x, y) = ""
                        FrmMain.S_Pack_MC8SS(x, y) = ""
                        FrmMain.S_Pack_CIBSS(x, y) = ""

                        FrmMain.S_Pack_MC8F(x, y) = ""
                        FrmMain.S_Pack_CIBF(x, y) = ""
                    Next
                Next


                '** Array for other Products
                Dim I_UpperLimit1 As Integer = I_UpperLimit1 = UBound(FrmMain.ArrayProducts, 1)
                Dim I_UpperLimit2 As Integer = I_UpperLimit2 = UBound(FrmMain.ArrayProducts, 2)

                For x = 0 To I_UpperLimit1
                    For y = 0 To I_UpperLimit2
                        FrmMain.ArrayProducts(x, y) = ""
                    Next
                Next
                FrmMain.PrintExtras = False

                ClearPackingForm()
                Me.Close()
            End If
        End If
    End Sub
    Public Sub SavePO(item As String, Description As String, Data1 As String, Data2 As String, Data3 As String, Details As String, Qty As String)

        Dim Spath As String = FrmMain.StrRootFolder

        Dim Sfilename As String = "Purchase Order " & Trim(Txt_PONumber.Text) & TimeStamp
        Dim PO_FileName As String = Spath & Sfilename & ".CSV"
        Dim PO_FileNumber As Integer = FreeFile()
        FrmMain.StrAttachedPO = PO_FileName

        'Update log file
        FileOpen(PO_FileNumber, PO_FileName, OpenMode.Append)
        PrintLine(PO_FileNumber, item & "," & Description & "," & Data1 & "," & Data2 & "," & Data3 & "," & Details & "," & Qty)
        FileClose(PO_FileNumber)
    End Sub
    Private Sub UpdateNDB(S_GUID As String, S_ID As String, type As String, I_channels As Integer, D_Date As Date, S_TestBy As String)
        '*** TEST FOR NDB UPDATE
        '** test to determine if a blank field overwrites the ndb data
        '*** This section writes configuration details for the masters and channel boards into the NDB using a OneBit DLL
        Dim configuration As New BoardConfiguration()

        With configuration
            .EnableLinkConfirmed = True
            .ModuleGUID = Trim(S_GUID)               '** String containg SS detals
            .ModuleID = Trim(S_ID)                 '** ID like MC500083 or C15054
            .ModuleType = Trim(type)                        '** Device type Master, CIB, MC8, MC8SS & CIBSS
            .NumberOfChannelsTested = I_channels    '** Number of channels 8 or 24
            .QATestedDate = D_Date                  '** Current Date & Time
            .QATestedBy = S_TestBy          '** Name of person programming units
            .Purchaser = Txt_Purchaser.Text         '** Company purchasing device
            .OrderNumber = Txt_PONumber.Text         '** PO Number
        End With


        Try

            Dim configurationSaver As New ConfigurationSaver(FrmMain.connectionString)
            configurationSaver.SaveBoardConfiguration(configuration)

        Catch ex As Exception
            Debug.Print("ERROR " & ex.ToString)
        End Try

    End Sub

    Private Sub BtnReloadPO_Click(sender As Object, e As EventArgs) Handles BtnReloadPO.Click
        Dim IntAnswer As Integer = MsgBox("Warning. Continuing will clear any data already in the grid." & vbCrLf & "Do you wish to continue?", vbYesNo + vbQuestion)
        If IntAnswer = MsgBoxResult.Yes Then
            If Txt_PONumber.Text <> "" Then
                ReadPO_Data()
            Else
                MsgBox("Please enter a PO number", vbOKOnly, "Error No PO")
            End If
        End If
    End Sub
    Public Sub ReadPO_Data()
        Dim Spath As String = FrmMain.StrRootFolder
        Dim Sfilename As String = Trim(Txt_PONumber.Text)
        Dim PO_FileName As String = Spath & Sfilename & ".POD"
        Dim PO_FileNumber As Integer = FreeFile()

        'Read PO file
        'PrintLine(PO_FileNumber, item & "," & Description & "," & Data1 & "," & Data2 & "," & Data3 & "," & Details & "," & Qty)
        Dim rowCount As Integer = 0
        '*** Get settings if any
        Try
            Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(PO_FileName)
                MyReader.TextFieldType = FileIO.FieldType.Delimited
                MyReader.SetDelimiters(",")
                Dim currentRow As String()

                Dim fieldCount As Integer = 0
                While Not MyReader.EndOfData
                    Try
                        currentRow = MyReader.ReadFields()

                        Dim currentField As String
                        Dim tmp1 As String = ""
                        Dim tmp2 As String = ""
                        Dim tmp3 As String = ""
                        Dim tmp4 As String = ""
                        Dim tmp5 As String = ""
                        Dim tmp6 As String = ""
                        Dim tmp7 As String = ""
                        For Each currentField In currentRow

                            Debug.Print(currentField)
                            Select Case fieldCount
                                Case 0
                                    tmp1 = currentField
                                Case 1
                                    tmp2 = currentField
                                Case 2
                                    tmp3 = currentField
                                Case 3
                                    tmp4 = currentField
                                Case 4
                                    tmp5 = currentField
                                Case 5
                                    tmp6 = currentField
                                Case 6
                                    tmp7 = currentField

                            End Select

                            fieldCount += 1
                        Next
                        'DG_Packing.Rows.Add(tmp1, tmp2, tmp3, tmp4, tmp5, tmp6, tmp7)
                        If tmp1 = "FL100" Then tmp1 = "FL100X"
                        If tmp1 = "FL300" Then tmp1 = "FL300X"
                        Dim PutPart As Integer = GetNDBData(tmp1, tmp7)
                        rowCount += 1
                        fieldCount = 0
                    Catch ex As _
                    Microsoft.VisualBasic.FileIO.MalformedLineException
                        MsgBox("Line " & ex.Message &
                        "is not valid and will be skipped.")

                    End Try
                End While
                '   MsgBox("Please discover topology to check Master Type", vbCritical + vbOKOnly,"IMPORTANT")
            End Using
        Catch fileNotFound As System.IO.FileNotFoundException
            'Error handling here. Load default values etc
            MsgBox("Existing PO file not found.", vbCritical + vbOKOnly, "File not found")
        End Try
    End Sub

    Private Sub Pack_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Pack_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        '** May need to add a button to activate scanning
        If Chk_Scan.Checked Then
            Chk_Scan.Text = "Scanning"
            Dim S_KeyInput As String = e.KeyChar.ToString()

            '** Filter out progammed scanners
            If S_KeyInput <> "!" And S_KeyInput <> "#" Then
                S_ScannedCode &= Strings.UCase(S_KeyInput)
                Debug.Print(S_ScannedCode)
            End If

            If Strings.Len(S_ScannedCode) > 2 Then
                '*** Check for code type
                Dim S_ModuleCode As String = Strings.Left(S_ScannedCode, 2)
                Select Case UCase(S_ModuleCode)
                    Case "MC"
                        If Strings.Len(S_ScannedCode) = 7 Then
                            '** MC Code received
                            Debug.Print("MC8 " & S_ScannedCode)
                            Dim I_Result As Integer = GetNDBData(S_ScannedCode, "0")
                            S_ScannedCode = ""
                        End If
                    Case "FL", "RT", "US", "LU", "RB", "VT"
                        '** Other product
                        Select Case S_ScannedCode
                            Case "FL100X", "FL300X", "FL500", "FL1000", "FL1500", "FL2000", "FL3000", "FL4000", "FL CUSTOM"
                                Debug.Print("Product: " & S_ScannedCode)
                                Dim I_Result As Integer = GetNDBData(S_ScannedCode, "0")
                                S_ScannedCode = ""

                                '** Other product
                            Case "RTSP", "RTSDC", "USB", "LUSB", "RBL", "RT005", "VTL", "VTS"
                                Debug.Print("Product: " & S_ScannedCode)
                                Dim I_Result As Integer = GetNDBData(S_ScannedCode, "0")
                                S_ScannedCode = ""
                            Case Else
                                '*** Unknown product
                        End Select
                    Case Else
                        If Strings.Len(S_ScannedCode) = 6 Then
                            S_ModuleCode = Strings.Left(S_ModuleCode, 1)
                            Select Case S_ModuleCode
                                Case "M"
                                    Debug.Print("Master " & S_ScannedCode)
                                    Dim I_Result As Integer = GetNDBData(S_ScannedCode, "0")
                                    S_ScannedCode = ""
                                Case Else
                                    '** must be channel module
                                    Debug.Print("CIB " & S_ScannedCode)
                                    Dim I_Result As Integer = GetNDBData(S_ScannedCode, "0")
                                    S_ScannedCode = ""

                            End Select
                        End If

                End Select

            End If

        Else
            Dim S_KeyInput As String = e.KeyChar.ToString()
            Debug.Print(S_KeyInput)

        End If
    End Sub
    Public Sub WritePO_Data(item As String, Description As String, Data1 As String, Data2 As String, Data3 As String, Details As String, Qty As String)
        Dim Spath As String = FrmMain.StrRootFolder
        Dim Sfilename As String = Trim(Txt_PONumber.Text)
        Dim PO_FileName As String = Spath & Sfilename & ".POD"
        Dim PO_FileNumber As Integer = FreeFile()

        ''Update log file
        'FileOpen(PO_FileNumber, PO_FileName, OpenMode.Output)
        'PrintLine(PO_FileNumber, item & "," & Description & "," & Data1 & "," & Data2 & "," & Data3 & "," & Details & "," & Qty)
        'FileClose(PO_FileNumber)
    End Sub
    Private Function GetNDBData(ModuleID As String, OverrideQty As String) As Integer
        '*** Look up scanned items if they are stored in the DB
        '*** For other products just add to the grid
        Dim item As String = ""
        Dim Description As String = ""
        Dim Details As String = ""
        Dim StrReturn As String = ""

        BIsDirty = True

        '*** 20.03.2020
        '*** Yet to be implemented
        '*** Save file copy of scanned items so they can be retrieved if required

        Select Case ModuleID
            Case "FL100X"
                item = "FL100"
                Description = "Fly Lead"
                Details = "100mm"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(1, 0) = item
                        FrmMain.ArrayProducts(1, 1) = Description
                        FrmMain.ArrayProducts(1, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(1, 3) = Val(FrmMain.ArrayProducts(1, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(1, 3))
                        Else
                            FrmMain.ArrayProducts(1, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(1, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "FL300X"
                item = "FL300"
                Description = "Fly Lead"
                Details = "300mm"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(2, 0) = item
                        FrmMain.ArrayProducts(2, 1) = Description
                        FrmMain.ArrayProducts(2, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(2, 3) = Val(FrmMain.ArrayProducts(2, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(2, 3))
                        Else
                            FrmMain.ArrayProducts(2, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(2, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "FL500"
                item = ModuleID
                Description = "Fly Lead"
                Details = "500mm"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(3, 0) = item
                        FrmMain.ArrayProducts(3, 1) = Description
                        FrmMain.ArrayProducts(3, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(3, 3) = Val(FrmMain.ArrayProducts(3, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(3, 3))
                        Else
                            FrmMain.ArrayProducts(3, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(3, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "FL1000"
                item = ModuleID
                Description = "Fly Lead"
                Details = "1000mm"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(4, 0) = item
                        FrmMain.ArrayProducts(4, 1) = Description
                        FrmMain.ArrayProducts(4, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(4, 3) = Val(FrmMain.ArrayProducts(4, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(4, 3))
                        Else
                            FrmMain.ArrayProducts(4, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(4, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "FL1500"
                item = ModuleID
                Description = "Fly Lead"
                Details = "1500mm"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(5, 0) = item
                        FrmMain.ArrayProducts(5, 1) = Description
                        FrmMain.ArrayProducts(5, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(5, 3) = Val(FrmMain.ArrayProducts(5, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(5, 3))
                        Else
                            FrmMain.ArrayProducts(5, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(5, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "FL2000"
                item = ModuleID
                Description = "Fly Lead"
                Details = "2000mm"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(6, 0) = item
                        FrmMain.ArrayProducts(6, 1) = Description
                        FrmMain.ArrayProducts(6, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(6, 3) = Val(FrmMain.ArrayProducts(6, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(6, 3))
                        Else
                            FrmMain.ArrayProducts(6, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(6, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "FL3000"
                item = ModuleID
                Description = "Fly Lead"
                Details = "3000mm"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(7, 0) = item
                        FrmMain.ArrayProducts(7, 1) = Description
                        FrmMain.ArrayProducts(7, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(7, 3) = Val(FrmMain.ArrayProducts(7, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(7, 3))
                        Else
                            FrmMain.ArrayProducts(7, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(7, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "FL4000"
                item = ModuleID
                Description = "Fly Lead"
                Details = "4000mm"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(8, 0) = item
                        FrmMain.ArrayProducts(8, 1) = Description
                        FrmMain.ArrayProducts(8, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(8, 3) = Val(FrmMain.ArrayProducts(8, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(8, 3))
                        Else
                            FrmMain.ArrayProducts(8, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(8, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "FL CUSTOM"
                item = ModuleID
                Description = "Fly Lead"
                Details = "Custom"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(9, 0) = item
                        FrmMain.ArrayProducts(9, 1) = Description
                        FrmMain.ArrayProducts(9, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(9, 3) = Val(FrmMain.ArrayProducts(9, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(9, 3))
                        Else
                            FrmMain.ArrayProducts(9, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(9, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "RTSP"
                item = ModuleID
                Description = "Surge Diverter"
                Details = "1 Set"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(10, 0) = item
                        FrmMain.ArrayProducts(10, 1) = Description
                        FrmMain.ArrayProducts(10, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(10, 3) = Val(FrmMain.ArrayProducts(10, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(10, 3))
                        Else
                            FrmMain.ArrayProducts(10, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(10, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "RTSDC"
                item = ModuleID
                Description = "RJ12 Dust Cover"
                Details = "Pack of 10"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(11, 0) = item
                        FrmMain.ArrayProducts(11, 1) = Description
                        FrmMain.ArrayProducts(11, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(11, 3) = Val(FrmMain.ArrayProducts(11, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(11, 3))
                        Else
                            FrmMain.ArrayProducts(11, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(11, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "USB"
                item = ModuleID
                Description = "Universal Stacker Bracket"
                Details = "1 Set"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(12, 0) = item
                        FrmMain.ArrayProducts(12, 1) = Description
                        FrmMain.ArrayProducts(12, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(12, 3) = Val(FrmMain.ArrayProducts(12, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(12, 3))
                        Else
                            FrmMain.ArrayProducts(12, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(12, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "LUSB"
                item = "USBL"
                Description = "Universal Stacker Bracket - Large"
                Details = "1 Set"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(13, 0) = item
                        FrmMain.ArrayProducts(13, 1) = Description
                        FrmMain.ArrayProducts(13, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(13, 3) = Val(FrmMain.ArrayProducts(13, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(1, 3))
                        Else
                            FrmMain.ArrayProducts(13, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(13, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "RBL"
                item = ModuleID
                Description = "RCD Barcode Labels"
                Details = "24 Pack"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(14, 0) = item
                        FrmMain.ArrayProducts(14, 1) = Description
                        FrmMain.ArrayProducts(14, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(14, 3) = Val(FrmMain.ArrayProducts(14, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(14, 3))
                        Else
                            FrmMain.ArrayProducts(14, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(14, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "RT005"
                item = ModuleID
                Description = "Switchboard Barcode Labels"
                Details = "Pack of 10"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(15, 0) = item
                        FrmMain.ArrayProducts(15, 1) = Description
                        FrmMain.ArrayProducts(15, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(15, 3) = Val(FrmMain.ArrayProducts(15, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(15, 3))
                        Else
                            FrmMain.ArrayProducts(15, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(15, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "VTL"
                item = ModuleID
                Description = "Verification Test Lead"
                Details = "1 piece"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(16, 0) = item
                        FrmMain.ArrayProducts(16, 1) = Description
                        FrmMain.ArrayProducts(16, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(16, 3) = Val(FrmMain.ArrayProducts(16, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(16, 3))
                        Else
                            FrmMain.ArrayProducts(16, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(16, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case "VTS"
                item = ModuleID
                Description = "Verification Test Socket"
                Details = "1 piece"
                If OverrideQty <> "0" Then StrReturn = OverrideQty Else StrReturn = InputBox("Enter a Quantity for " & Details & " " & Description, "How many?")
                StrReturn = Strings.Replace(StrReturn, "#", "")
                If IsNumeric(StrReturn) Then
                    If Val(StrReturn) > 0 Then
                        PackRunningTotal += 1
                        FrmMain.ArrayProducts(0, 0) = "YES"   '** first record in array indicates there are other products to print
                        FrmMain.ArrayProducts(17, 0) = item
                        FrmMain.ArrayProducts(17, 1) = Description
                        FrmMain.ArrayProducts(17, 2) = Details
                        If OverrideQty = "0" Then
                            FrmMain.ArrayProducts(17, 3) = Val(FrmMain.ArrayProducts(17, 3)) + Val(StrReturn)
                            WritePO_Data(item, Description, "", "", "", Details, FrmMain.ArrayProducts(17, 3))
                        Else
                            FrmMain.ArrayProducts(17, 3) = Val(StrReturn)
                        End If
                        Me.DG_Packing.Rows.Add(item, Description, "", "", "", Details, FrmMain.ArrayProducts(17, 3))
                        Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                    End If
                End If

            Case Else


                '*** Connect to data source 
                '*** DB ACCESS VIA DLL ****
                Dim configuration As New BoardConfiguration()
                Dim S_Message As String = ""
                Dim S_ModuleID As String = ""
                Dim S_ModuleType As String = ""
                Dim S_Purchaser As String = ""
                Dim S_TestedBy As String = ""
                Dim S_OrderNumber As String = ""
                Dim S_GUID As String = ""
                Dim S_Channels As String = ""
                Dim S_QADate As String = ""

                Try
                    Dim configurationSaver As New ConfigurationSaver(FrmMain.connectionString)

                    configuration = configurationSaver.GetBoardConfiguration(Trim(ModuleID))
                    With configuration

                        S_GUID = .ModuleGUID.ToString
                        S_ModuleID = .ModuleID.ToString
                        S_ModuleType = .ModuleType.ToString
                        S_Channels = .NumberOfChannelsTested.ToString
                        S_TestedBy = .QATestedBy
                        S_QADate = .QATestedDate.ToString
                        S_Purchaser = .Purchaser
                        S_OrderNumber = .OrderNumber
                    End With


                Catch ex As Exception
                    Debug.Print("ERROR " & ex.ToString)
                    MsgBox(ex.ToString, vbOK + vbExclamation, "Result")
                End Try
                '**** END DB by DLL
                '                Dim NDB_DB As New SqlClient.SqlConnection(FrmMain.connectionString)

                '                NDB_DB.Open()

                '                Dim tmpSQLClause As String = ""
                '                'DISTINCT
                '                tmpSQLClause = "SELECT     dbo.NDB_Board.BRD_ModuleID, dbo.NDB_Board.BRD_ModuleGUID, dbo.NDB_Board.BRD_QATestedDate, dbo.NDB_Board.BRD_QATestedBy, 
                '                      dbo.NDB_Board.BRD_NumberOfChannelsTested, dbo.NDB_Board.BRD_Purchaser, dbo.NDB_Board.BRD_OrderNumber, dbo.NDB_ModuleType.MOD_Type,
                'dbo.NDB_Board.BRD_NumberOfChannelsTested, dbo.NDB_Board.BRD_QATestedDate 
                'FROM         dbo.NDB_Board INNER JOIN
                '                      dbo.NDB_ModuleType ON dbo.NDB_Board.BRD_ModuleType_ID = dbo.NDB_ModuleType.MOD_ID
                'WHERE     (dbo.NDB_Board.BRD_ModuleID = '" & ModuleID & "')"


                '                Dim RT_Sql As New SqlClient.SqlCommand(tmpSQLClause, NDB_DB)
                '                Dim RT_Results As SqlClient.SqlDataReader = RT_Sql.ExecuteReader()

                '                RT_Results.Read()


                '                Dim rowCount As Integer = 0
                Dim I_MasterIndex As Integer = 0

                '                If RT_Results.HasRows Then
                '                    '// Increment number of rows
                '                    rowCount = rowCount + 1
                '                    If IsDBNull(RT_Results.Item("BRD_ModuleID")) Then S_ModuleID = "" Else S_ModuleID = UCase(RT_Results.Item("BRD_ModuleID"))
                '                    If IsDBNull(RT_Results.Item("MOD_Type")) Then S_ModuleType = "" Else S_ModuleType = UCase(RT_Results.Item("MOD_Type"))
                '                    If IsDBNull(RT_Results.Item("BRD_Purchaser")) Then S_Purchaser = "" Else S_Purchaser = UCase(RT_Results.Item("BRD_Purchaser"))
                '                    If IsDBNull(RT_Results.Item("BRD_QATestedBy")) Then S_TestedBy = "" Else S_TestedBy = RT_Results.Item("BRD_QATestedBy")
                '                    If IsDBNull(RT_Results.Item("BRD_OrderNumber")) Then S_OrderNumber = "" Else S_OrderNumber = UCase(RT_Results.Item("BRD_OrderNumber"))
                '                    If IsDBNull(RT_Results.Item("BRD_ModuleGUID")) Then S_GUID = "" Else S_GUID = UCase(RT_Results.Item("BRD_ModuleGUID"))
                '                    If IsDBNull(RT_Results.Item("BRD_NumberOfChannelsTested")) Then S_Channels = "" Else S_Channels = UCase(RT_Results.Item("BRD_NumberOfChannelsTested"))
                '                    If IsDBNull(RT_Results.Item("BRD_QATestedDate")) Then S_QADate = "" Else S_QADate = UCase(RT_Results.Item("BRD_QATestedDate"))
                Debug.Print(S_ModuleType & "  " & S_ModuleID & "  " & S_TestedBy & "  " & S_Purchaser & "  " & S_OrderNumber & "  " & S_ModuleType & "  " & S_GUID)

                'Use an array to store the Module ID's to check they are not already in the grid. If not in the array, add it to the array and the grid.


                Select Case UCase(S_ModuleType)
                    Case "MASTER"
                        For y = 0 To 500
                            If FrmMain.S_Pack_Master(y, 0) = "" Then
                                '** Normal Master
                                FrmMain.S_Pack_Master(0, 2) = Val(FrmMain.S_Pack_Master(0, 2)) + 1 '*** Count modules
                                FrmMain.S_Pack_Master(y, 0) = S_ModuleID
                                FrmMain.S_Pack_Master(y, 1) = S_GUID
                                FrmMain.S_Pack_Master(y, 3) = S_ModuleType
                                FrmMain.S_Pack_Master(y, 4) = S_Channels
                                FrmMain.S_Pack_Master(y, 5) = S_QADate
                                FrmMain.S_Pack_Master(y, 6) = S_TestedBy
                                I_MasterIndex += 1 ' Increment
                                Me.DG_Packing.Rows.Add(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                If OverrideQty = "0" Then WritePO_Data(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                                PackRunningTotal += 1
                                MASTER_Count += 1
                                '* Update count
                                Lbl_IItemCount.Text = PackRunningTotal.ToString
                                LblMasterCount.Text = MASTER_Count.ToString

                                Exit For
                            Else
                                If FrmMain.S_Pack_Master(y, 0) = S_ModuleID Then Exit For
                            End If
                        Next

                    Case "MC8F5"
                        For y = 0 To 500
                            If FrmMain.S_Pack_MC8F5(y, 0) = "" Then
                                '** Normal Master
                                FrmMain.S_Pack_MC8F5(0, 2) = Val(FrmMain.S_Pack_MC8F5(0, 2)) + 1 '*** Count modules
                                FrmMain.S_Pack_MC8F5(y, 0) = S_ModuleID
                                FrmMain.S_Pack_MC8F5(y, 1) = S_GUID
                                FrmMain.S_Pack_MC8F5(y, 3) = S_ModuleType
                                FrmMain.S_Pack_MC8F5(y, 4) = S_Channels
                                FrmMain.S_Pack_MC8F5(y, 5) = S_QADate
                                FrmMain.S_Pack_MC8F5(y, 6) = S_TestedBy
                                I_MasterIndex += 1 ' Increment
                                Me.DG_Packing.Rows.Add(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                If OverrideQty = "0" Then WritePO_Data(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                                PackRunningTotal += 1
                                MC8F5_Count += 1
                                '* Update count
                                Lbl_IItemCount.Text = PackRunningTotal.ToString
                                LblMC8F5.Text = MC8F5_Count.ToString
                                Exit For
                            Else
                                If FrmMain.S_Pack_MC8F5(y, 0) = S_ModuleID Then Exit For
                            End If
                        Next

                    Case "MASTERM"
                        For y = 0 To 500
                            If FrmMain.S_Pack_MasterM(y, 0) = "" Then
                                '** Normal Master
                                FrmMain.S_Pack_MasterM(0, 2) = Val(FrmMain.S_Pack_MasterM(0, 2)) + 1 '*** Count modules
                                FrmMain.S_Pack_MasterM(y, 0) = S_ModuleID
                                FrmMain.S_Pack_MasterM(y, 1) = S_GUID
                                FrmMain.S_Pack_MasterM(y, 3) = S_ModuleType
                                FrmMain.S_Pack_MasterM(y, 4) = S_Channels
                                FrmMain.S_Pack_MasterM(y, 5) = S_QADate
                                FrmMain.S_Pack_MasterM(y, 6) = S_TestedBy
                                I_MasterIndex += 1 ' Increment
                                Me.DG_Packing.Rows.Add(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                If OverrideQty = "0" Then WritePO_Data(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                                PackRunningTotal += 1
                                MASTERM_Count += 1
                                '* Update count
                                Lbl_IItemCount.Text = PackRunningTotal.ToString
                                LblMasterM.Text = MASTERM_Count.ToString
                                Exit For
                            Else
                                If FrmMain.S_Pack_MasterM(y, 0) = S_ModuleID Then Exit For
                            End If
                        Next

                    Case "MC8"
                        For y = 0 To 500
                            If FrmMain.S_Pack_MC8(y, 0) = "" Then
                                '** MC8
                                FrmMain.S_Pack_MC8(0, 2) = Val(FrmMain.S_Pack_MC8(0, 2)) + 1 '*** Count modules
                                FrmMain.S_Pack_MC8(y, 0) = S_ModuleID
                                FrmMain.S_Pack_MC8(y, 1) = S_GUID
                                FrmMain.S_Pack_MC8(y, 3) = S_ModuleType
                                FrmMain.S_Pack_MC8(y, 4) = S_Channels
                                FrmMain.S_Pack_MC8(y, 5) = S_QADate
                                FrmMain.S_Pack_MC8(y, 6) = S_TestedBy
                                I_MasterIndex += 1 ' Increment
                                Me.DG_Packing.Rows.Add(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                If OverrideQty = "0" Then WritePO_Data(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                                PackRunningTotal += 1
                                MC8_Count += 1
                                '* Update count
                                Lbl_IItemCount.Text = PackRunningTotal.ToString
                                LblMC8.Text = MC8_Count.ToString
                                Exit For
                            Else
                                If FrmMain.S_Pack_MC8(y, 0) = S_ModuleID Then Exit For
                            End If
                        Next

                    Case "CIB"
                        For y = 0 To 500
                            If FrmMain.S_Pack_CIB(y, 0) = "" Then
                                '** CIB
                                FrmMain.S_Pack_CIB(0, 2) = Val(FrmMain.S_Pack_CIB(0, 2)) + 1 '*** Count modules
                                FrmMain.S_Pack_CIB(y, 0) = S_ModuleID
                                FrmMain.S_Pack_CIB(y, 1) = S_GUID
                                FrmMain.S_Pack_CIB(y, 3) = S_ModuleType
                                FrmMain.S_Pack_CIB(y, 4) = S_Channels
                                FrmMain.S_Pack_CIB(y, 5) = S_QADate
                                FrmMain.S_Pack_CIB(y, 6) = S_TestedBy
                                I_MasterIndex += 1 ' Increment
                                Me.DG_Packing.Rows.Add(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                If OverrideQty = "0" Then WritePO_Data(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                                PackRunningTotal += 1
                                CIB_Count += 1
                                '* Update count
                                Lbl_IItemCount.Text = PackRunningTotal.ToString
                                LblCIB.Text = CIB_Count.ToString
                                Exit For
                            Else
                                If FrmMain.S_Pack_CIB(y, 0) = S_ModuleID Then Exit For
                            End If
                        Next

                    Case "MC8F"
                        For y = 0 To 500
                            If FrmMain.S_Pack_MC8F(y, 0) = "" Then
                                '** CIB
                                FrmMain.S_Pack_MC8F(0, 2) = Val(FrmMain.S_Pack_MC8F(0, 2)) + 1 '*** Count modules
                                FrmMain.S_Pack_MC8F(y, 0) = S_ModuleID
                                FrmMain.S_Pack_MC8F(y, 1) = S_GUID
                                FrmMain.S_Pack_MC8F(y, 3) = S_ModuleType
                                FrmMain.S_Pack_MC8F(y, 4) = S_Channels
                                FrmMain.S_Pack_MC8F(y, 5) = S_QADate
                                FrmMain.S_Pack_MC8F(y, 6) = S_TestedBy
                                I_MasterIndex += 1 ' Increment
                                Me.DG_Packing.Rows.Add(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                If OverrideQty = "0" Then WritePO_Data(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                                PackRunningTotal += 1
                                MC8F_Count += 1
                                '* Update count
                                Lbl_IItemCount.Text = PackRunningTotal.ToString
                                LblMC8F.Text = MC8F_Count.ToString
                                Exit For
                            Else
                                If FrmMain.S_Pack_MC8F(y, 0) = S_ModuleID Then Exit For
                            End If
                        Next

                    Case "CIBF"
                        For y = 0 To 500
                            If FrmMain.S_Pack_CIBF(y, 0) = "" Then
                                '** CIB
                                FrmMain.S_Pack_CIBF(0, 2) = Val(FrmMain.S_Pack_CIBF(0, 2)) + 1 '*** Count modules
                                FrmMain.S_Pack_CIBF(y, 0) = S_ModuleID
                                FrmMain.S_Pack_CIBF(y, 1) = S_GUID
                                FrmMain.S_Pack_CIBF(y, 3) = S_ModuleType
                                FrmMain.S_Pack_CIBF(y, 4) = S_Channels
                                FrmMain.S_Pack_CIBF(y, 5) = S_QADate
                                FrmMain.S_Pack_CIBF(y, 6) = S_TestedBy
                                I_MasterIndex += 1 ' Increment
                                Me.DG_Packing.Rows.Add(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                If OverrideQty = "0" Then WritePO_Data(S_ModuleID, S_ModuleType, S_TestedBy, S_OrderNumber, S_Purchaser, S_GUID, "1")
                                Me.DG_Packing.FirstDisplayedScrollingRowIndex = DG_Packing.RowCount - 1
                                PackRunningTotal += 1
                                CIBF_Count += 1
                                '* Update count
                                Lbl_IItemCount.Text = PackRunningTotal.ToString
                                LblCIBF.Text = CIBF_Count.ToString
                                Exit For
                            Else
                                If FrmMain.S_Pack_CIBF(y, 0) = S_ModuleID Then Exit For
                            End If
                        Next

                    Case Else

                End Select

                Return 0
                '*** Do remianing assets
                '    For Each item In RT_Results
                '        '// If more than 1 match something is very wrong

                '        MsgBox("ERROR multiple matches found.", vbCritical + vbOKOnly, "Done")
                '    Next
                'Else
                '    MsgBox("No results returned.", vbCritical + vbOKOnly, "Done")

                '    Return -1

                'End If

                'NDB_DB.Close()

        End Select
        '* Update count
        Lbl_IItemCount.Text = PackRunningTotal.ToString

        Return 1
    End Function

    Private Sub Label1_DoubleClick(sender As Object, e As EventArgs) Handles Label1.DoubleClick
        Txt_PackedBy.Text = "Kevin Wilson"
        Txt_PONumber.Text = "DT12345"
        Txt_Purchaser.Text = "Data Transformation"
    End Sub
End Class