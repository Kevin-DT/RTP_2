Imports System.Management
Imports System.Net.Sockets
Imports System.Net.NetworkInformation
Imports System.Drawing.Printing
Imports Ats.RapidTest.DataConfiguration
Imports Ats.RapidTest.DataConfiguration.Model
Imports System.Net.Mail
Imports System.ComponentModel
Imports System.Net
Imports System.Text
Imports System.Runtime.InteropServices
Public Class FrmMain
    '*** Publishing
    'Username: dtsuperuser1
    'Password: dataPublish1$

    '*** Settings
    Public StrRootFolder As String = "C:\ats\rtp\"
    Public S_PrintFile As String = StrRootFolder & "RT_print.cfg"
    Public SettingsFile As String = StrRootFolder & "RT_Settings.cfg"
    Public DataFile As String = StrRootFolder & "RT_Autosave.cfg"
    Public StrSelectedPrinter As String = ""
    Public BoolHasPrinter As Boolean = False
    Public StrDefaultMaster As String = ""
    Public StrLogPath As String = StrRootFolder
    Public StrTextToLog As String = ""

    '*** Save temp value for Skip Master
    Public StrCurrentSSID As String = ""

    '*** Controlled by Staging server setting.
    Public connectionStringServer As String = ""
    Public connectionString As String = ""

    '*** Wifi 
    Public MyWiFi As New Wifi()
    Public SConnectedWiFi As String = ""
    Public StrRTpassword As String = "rapidtest"

    '** device IP address and port
    Public sIP As String = "192.168.0.1"  '*** Change as required for LAN testing
    Public iPort As Integer = 12000

    '*** Shared elements for the ethernet
    Shared client As New TcpClient
    Shared stream As NetworkStream

    '*** Device details
    Public iVersionMinor As Integer
    Public iVersionMajor As Integer
    Public IntResponseLength As Integer
    Public IntMasterType As Integer = 0
    Const MC8F5 As Integer = 1
    Const MC8 As Integer = 2
    Public S_MasterGUID As String
    Public sCB_OriginalGUID As String = ""
    Public sNewGUID As String = ""
    Public sScannerString As String = ""

    '*** Printing
    Public StickerText01 As String = ""
    Public StickerText02 As String = ""

    '*** Array for PO details
    '*** Array x 3 elements
    '*** Unit ID and Date string and count
    Public S_po_Master(500, 2) As String
    Public S_po_MC8(500, 2) As String
    Public S_po_CIB(500, 2) As String

    '*** Step controls
    Public ICurrentStep As Integer = 1
    Public IntMaxStep As Integer = 11
    Public ResponseLength As Integer = 21
    Public IntMasterTestsDone As Integer = 0
    Public IntCIBTestsDone As Integer = 0

    '*** Channel details
    Public IntCurrentChannel As Integer = 1
    Public IntMaxChannel As Integer = 0
    Public RtUnitChannel As Integer = 0 '*** The real zero based RT unit Channel
    Public B_RelayCheck As Boolean = False '*** Use when not tripping but testing channels - May become absolete

    '*** Debug
    Public S_LogFileText As String = ""
    Public BSuppressTimeWarning As Boolean
    Public SMasterType As String

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()

    End Sub

    Private Sub FileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FileToolStripMenuItem.Click

    End Sub

    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        '** Load config
        'create config folder if needed
        Try
            System.IO.Directory.CreateDirectory("C:\ats\RTP")
        Catch ex As Exception
            '** Don't do anything if the directory exists. just catch it so it doesn't cause a crash
        End Try
        Me.Text = "RT Programmer [ " & Me.Width.ToString & " x " & Me.Height.ToString & "]"
        '*** load printer if anyS
        StrSelectedPrinter = GetPrinter()
        If StrSelectedPrinter <> "" Then
            BoolHasPrinter = True
            TSPrinterName.ForeColor = Color.Green
            TSPrinterName.Text = StrSelectedPrinter

        Else
            BoolHasPrinter = False
            TSPrinterName.ForeColor = Color.Red
            TSPrinterName.Text = "No Printer"
        End If
        GetSettings()
        If StrDefaultMaster <> "" Then ChkSkipMaster.Text = "Skip Master (Use " & StrDefaultMaster & ")"
        TSLogPath.Text = StrLogPath
        Initialise()
        ClearResults()
        '*** Start Wifi Check
        TmrWifi.Enabled = True
    End Sub
    Private Sub Initialise()
        RtUnitChannel = IntCurrentChannel - 1 '*** Remove offset of 1 for 0 based unit channels
        LblChannel.Text = IntCurrentChannel.ToString

        '*** Appp version
        Dim SversionText As String = My.Application.Info.Version.Major & "." _
                & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build & "." & My.Application.Info.Version.Revision
        '*** Update step Label
        lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)

        connectionString = "Server=tcp:ats-prod.database.windows.net,1433;Initial Catalog=RapidTest;Persist Security Info=False;User ID=atsadmin;Password=pass@word1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
        Lbl_Server.ForeColor = Color.Blue
        Lbl_Server.Text = "Azure Production"
        Lbl_Server.Visible = True
        Me.Text = "RTP - AZURE Version " & SversionText
    End Sub
    Private Sub GetSettings()
        Dim rowCount As Integer = 0

        '*** Get settings if any
        Try
            Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(SettingsFile)
                MyReader.TextFieldType = FileIO.FieldType.Delimited
                MyReader.SetDelimiters(",")
                Dim currentRow As String()

                Dim fieldCount As Integer = 0
                While Not MyReader.EndOfData
                    Try
                        currentRow = MyReader.ReadFields()

                        Dim currentField As String
                        For Each currentField In currentRow
                            If rowCount = 0 And fieldCount = 0 Then StrDefaultMaster = currentField
                            If rowCount = 1 And fieldCount = 0 Then StrLogPath = currentField
                        Next
                        rowCount += 1
                        fieldCount = 0
                    Catch ex As _
                    Microsoft.VisualBasic.FileIO.MalformedLineException
                        MsgBox("Line " & ex.Message &
                        "is not valid and will be skipped.")

                    End Try
                End While
            End Using
        Catch fileNotFound As System.IO.FileNotFoundException
            'Error handling here. Load default values etc
            MsgBox("No Settings file found." & vbNewLine & "Please save default settings before continuing.", vbCritical + vbOKOnly, "File not found")
        End Try
        '*** END get settings
    End Sub
    Private Function GetPrinter() As String
        Dim rowCount As Integer = 0
        Dim StrPrinterName As String = ""
        Dim BoolSkip2ndMessage As Boolean = False

        '*** Get printer if any
        Try
            Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(S_PrintFile)
                MyReader.TextFieldType = FileIO.FieldType.Delimited
                MyReader.SetDelimiters(",")
                Dim currentRow As String()

                Dim fieldCount As Integer = 0
                While Not MyReader.EndOfData
                    Try
                        currentRow = MyReader.ReadFields()

                        Dim currentField As String
                        For Each currentField In currentRow
                            'MsgBox(currentField & " " & rowCount)
                            If rowCount = 0 Then StrPrinterName = currentField
                        Next
                        rowCount += 1
                        fieldCount = 0
                    Catch ex As _
                    Microsoft.VisualBasic.FileIO.MalformedLineException
                        MsgBox("Line " & ex.Message &
                        "is not valid and will be skipped.")

                    End Try
                End While
            End Using
        Catch fileNotFound As System.IO.FileNotFoundException
            'Error handling here. Load default values etc
            MsgBox("No printer selected." & vbNewLine & "Please select a printer before continuing.", vbCritical + vbOKOnly, "File not found")
            BoolSkip2ndMessage = True
        End Try
        '*** END get printer
        If Not BoolSkip2ndMessage Then
            If StrPrinterName = "" Then
                MsgBox("Please select a printer before continuing.", vbCritical + vbOKOnly, "Printer not selected")
            End If
        End If
        Return StrPrinterName
    End Function
    Private Sub FrmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Me.Text = "RT Programmer [ " & Me.Width.ToString & " x " & Me.Height.ToString & "]"

    End Sub

    Private Sub PrinterToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Printers.LblSelectedPrinter.Text = StrSelectedPrinter
        Printers.ShowDialog()
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        FrmSettings.TxtDefaultMaster.Text = StrDefaultMaster
        FrmSettings.LblLogPath.Text = StrLogPath
        FrmSettings.ShowDialog()
    End Sub

    Private Sub ChkSkipMaster_CheckedChanged(sender As Object, e As EventArgs) Handles ChkSkipMaster.CheckedChanged
        If ChkSkipMaster.Checked Then
            StrCurrentSSID = Trim(TxtMasterSSID.Text)
            TxtMasterSSID.Text = StrDefaultMaster
            ChkConnectToID.Checked = True
            GrpMaster.Enabled = False
            ChkSkipCIB.Checked = False
            '*** Clear label from printing
            ChkMasterLbl.Checked = False
        Else
            GrpMaster.Enabled = True
            TxtMasterSSID.Text = StrCurrentSSID
            ChkConnectToID.Checked = True
            '*** Add label for printing
            ChkMasterLbl.Checked = True
        End If
    End Sub

    Private Sub ChkSkipCIB_CheckedChanged(sender As Object, e As EventArgs) Handles ChkSkipCIB.CheckedChanged
        If ChkSkipCIB.Checked Then
            GrpCIB.Enabled = False
            ChkSkipMaster.Checked = False
            '*** Clear label from printing
            ChkCIBLbl.Checked = False
        Else
            GrpCIB.Enabled = True
            '*** Add label to printing
            ChkCIBLbl.Checked = True
        End If
    End Sub
    Private Function SaveData() As Integer
        Dim ReturnData As Integer = 1
        '*** Get availabel file number
        Dim AutoSaveFileNumber As Integer = FreeFile()
        '*** Write data to file
        FileOpen(AutoSaveFileNumber, DataFile, OpenMode.Output)
        PrintLine(AutoSaveFileNumber, "User" & "," & TxtUser.Text)
        PrintLine(AutoSaveFileNumber, "OrderNo" & "," & TxtOrderRef.Text)
        PrintLine(AutoSaveFileNumber, "Purchaser" & "," & TxtPurchaser.Text)
        PrintLine(AutoSaveFileNumber, "Skip_Master" & "," & ChkSkipMaster.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "SSID" & "," & TxtMasterSSID.Text)
        PrintLine(AutoSaveFileNumber, "ConnectID" & "," & ChkConnectToID.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "MasterManCode" & "," & TxtMasterManCode.Text)
        PrintLine(AutoSaveFileNumber, "Skip_CIB" & "," & ChkSkipCIB.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "CIB_ID" & "," & TxtCIBid.Text)
        PrintLine(AutoSaveFileNumber, "CIB_ID_Only" & "," & ChkCIBidOnly.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "CIBManCode" & "," & TxtCIBManCode.Text)
        PrintLine(AutoSaveFileNumber, "NHP" & "," & ChkNHP.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "InvLabel" & "," & ChkInventoryLbl.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "MasterLbl" & "," & ChkMasterLbl.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "CIBLbl" & "," & ChkCIBLbl.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "AutoPrint" & "," & ChkAutoPrint.Checked.ToString)
        PrintLine(AutoSaveFileNumber, "AutoStep" & "," & ChkAutoStep.Checked.ToString)
        '*** Close file

        FileClose(AutoSaveFileNumber)
        Return ReturnData
    End Function
    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim SaveResult As Integer = SaveData()

    End Sub
    Function ReloadDataFile() As Integer
        Dim RowCount As Integer = 1
        Dim ReturnData As Integer = 1
        '*** Get data
        Try
            Dim StrTempSSID As String = ""
            Dim StrTempCIB As String = ""
            Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(DataFile)
                MyReader.TextFieldType = FileIO.FieldType.Delimited
                MyReader.SetDelimiters(",")
                Dim currentRow As String()

                Dim fieldCount As Integer = 0
                While Not MyReader.EndOfData
                    Try
                        currentRow = MyReader.ReadFields()

                        Dim currentField As String
                        For Each currentField In currentRow
                            'User
                            If RowCount = 1 And fieldCount = 1 Then TxtUser.Text = currentField
                            'Order number and purchaser
                            If RowCount = 2 And fieldCount = 1 Then TxtOrderRef.Text = currentField
                            If RowCount = 3 And fieldCount = 1 Then TxtPurchaser.Text = currentField
                            'Master info
                            If RowCount = 4 And fieldCount = 1 Then ChkSkipMaster.Checked = CBool(currentField)
                            If RowCount = 5 And fieldCount = 1 Then StrTempSSID = currentField
                            If RowCount = 6 And fieldCount = 1 Then ChkConnectToID.Checked = CBool(currentField)
                            If RowCount = 7 And fieldCount = 1 Then TxtMasterManCode.Text = currentField
                            'CIB info
                            If RowCount = 8 And fieldCount = 1 Then ChkSkipCIB.Checked = CBool(currentField)
                            If RowCount = 9 And fieldCount = 1 Then StrTempCIB = currentField
                            If RowCount = 10 And fieldCount = 1 Then ChkCIBidOnly.Checked = CBool(currentField)
                            If RowCount = 11 And fieldCount = 1 Then TxtCIBManCode.Text = currentField
                            '*** Labels
                            If RowCount = 12 And fieldCount = 1 Then ChkNHP.Checked = CBool(currentField)
                            If RowCount = 13 And fieldCount = 1 Then ChkInventoryLbl.Checked = CBool(currentField)
                            If RowCount = 14 And fieldCount = 1 Then ChkMasterLbl.Checked = CBool(currentField)
                            If RowCount = 15 And fieldCount = 1 Then ChkCIBLbl.Checked = CBool(currentField)
                            If RowCount = 16 And fieldCount = 1 Then ChkAutoPrint.Checked = CBool(currentField)
                            If RowCount = 17 And fieldCount = 1 Then ChkAutoStep.Checked = CBool(currentField)

                            fieldCount = +1
                        Next
                        RowCount += 1
                        fieldCount = 0
                    Catch ex As _
                    Microsoft.VisualBasic.FileIO.MalformedLineException
                        MsgBox("Line " & ex.Message &
                        "is not valid and will be skipped.")

                    End Try
                End While
            End Using
            'Update SSID text here so the NHP label check doesn't get triggered
            Application.DoEvents()
            TxtMasterSSID.Text = StrTempSSID
            TxtCIBid.Text = StrTempCIB
        Catch fileNotFound As System.IO.FileNotFoundException
            'Error handling here. Load default values etc
            MsgBox("No printer selected." & vbNewLine & "Please select a printer before continuing.", vbCritical + vbOKOnly, "File not found")
        End Try
        '*** END get printer

        Return ReturnData
    End Function
    Private Sub BtnReload_Click(sender As Object, e As EventArgs) Handles BtnReload.Click
        Dim ReloadData As Integer = ReloadDataFile()
    End Sub

    Private Sub ChkManCntrls_CheckedChanged(sender As Object, e As EventArgs) Handles ChkManCntrls.CheckedChanged
        If ChkManCntrls.Checked Then
            GrpManual.Enabled = False

        Else
            GrpManual.Enabled = True
        End If
    End Sub

    Private Sub TmrWifi_Tick(sender As Object, e As EventArgs) Handles TmrWifi.Tick
        TmrWifi.Enabled = False
        TmrWifi.Interval = 1000
        LstWiFi.Items.Clear()

        Try
            If Not MyWiFi.NoWifiAvailable Then
                Dim MyWifiList As List(Of AccessPoint) = MyWiFi.GetAccessPoints()
                Dim Int_listSize As Integer = MyWifiList.Count
                PicTimerHB.Visible = Not PicTimerHB.Visible

                For Each item In MyWifiList
                    Dim S_Item As String = ""
                    If item.Name = "" Then S_Item = "Hidden" Else S_Item = item.Name

                    '***
                    If Strings.Left(S_Item, 3) = "RT_" Then
                        LstWiFi.Items.Add(S_Item)
                        ' Allow the ListBox to repaint and display the new items.
                        LstWiFi.EndUpdate()
                        '*****
                    End If
                    '  Debug.Print(S_Item & " " & item.IsConnected)
                    If item.IsConnected = True Then SConnectedWiFi = S_Item
                Next

                If MyWiFi.NoWifiAvailable Then
                    LblWifi.ForeColor = Color.Red
                    LblWifi.Text = "No WiFi Card Detected"
                Else
                    Select Case UCase(MyWiFi.ConnectionStatus.ToString)
                        Case "CONNECTED"
                            If Strings.Left(SConnectedWiFi, 3) = "RT_" Then LblWifi.ForeColor = Color.Green Else LblWifi.ForeColor = Color.Blue
                            LblWifi.Text = "WiFi " & MyWiFi.ConnectionStatus.ToString & " to " & SConnectedWiFi
                        Case Else
                            LblWifi.ForeColor = Color.Red
                            LblWifi.Text = "WiFi " & MyWiFi.ConnectionStatus.ToString
                    End Select
                End If
            End If
        Catch Err As Exception
            Debug.Print("Catch - No Wifi")
            LblWifi.ForeColor = Color.Red
            LblWifi.Text = "No WiFi Card Detected"
        End Try
        Application.DoEvents()
        TmrWifi.Enabled = True

        '    Debug.Print("****************************** Timer Loop Count: " & TimerLoopCount.ToString)
    End Sub

    Private Sub FrmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        TmrWifi.Enabled = False
    End Sub

    Private Function GetMasterVersion()
        '*** Make sure port is closed
        MakeConnection(sIP, False, iPort)

        '*** Enable connection
        MakeConnection(sIP, True, iPort)

        GetMasterVersion = GetVersion()

        '*** allow app to Update graphics
        Application.DoEvents()

        '*** Disconnect
        MakeConnection(sIP, False, iPort)

    End Function

    Private Function GetVersion()

        '*** Read RCD Mapping - single channel
        Dim data2Send As String = ""
        Dim commandLength As Integer = 4   '*** 0 based Length less 2 x Check sum bytes

        Dim bytesForCRC(commandLength) As Byte

        Dim myCRC_16 As New CRC_16()
        '********** Command bytes
        bytesForCRC(0) = 85         '** Header A
        bytesForCRC(1) = 170        '** Header B
        bytesForCRC(2) = &H7  '** Command length with 2 x checksum
        bytesForCRC(3) = &H0   '** Device address 0 for tester
        bytesForCRC(4) = &H92  '** Command instruction - Read version

        '********** Data - No data for this command

        '*********** START Checksum calculation
        Dim csum() As Byte = myCRC_16.ComputeChecksumBytes(bytesForCRC)
        '************ END Checksum calculation

        For x = 0 To commandLength
            data2Send &= Chr(bytesForCRC(x))
        Next
        data2Send = data2Send & Chr(csum(0)) & Chr(csum(1))

        Dim data2SendFormatted As String = ""
        For x = 1 To Len(data2Send)
            data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
        Next

        Debug.Print("Command: Read Firmware Version 0x92")
        StrTextToLog = "Getting Frimware Version"
        S_LogFileText = "_" & TxtMasterSSID.Text & "_" & TxtCIBid.Text
        Dim StrResult As String = Connect(sIP, data2Send, 9, S_LogFileText, TxtOrderRef.Text)
        Debug.Print("****** Firmware " & StrResult)
        TxtDebug.Text = "****** Firmware " & StrResult
        '****************************************************
        '*** Return result to be decoded
        GetVersion = Trim(StrResult)
        '****************************************************
    End Function

    Shared Sub MakeConnection(server As [String], isAlive As Boolean, port As Int32)
        Debug.Print("Working on connection...")
        Dim myCRC_16 As New CRC_16()
        Dim ShowResult As Integer
        Try
            '  Dim port As Int32 = ' 80 '12000
            If isAlive Then
                client = New TcpClient(server, port)
                If client.Connected Then
                    Debug.Print("Connected.")
                    ShowResult = myCRC_16.DisplayResult("Connected", 1, 0, 0)
                Else
                    Debug.Print("Not Connected.")
                    ShowResult = myCRC_16.DisplayResult("Not Connected.", 1, 0, 0)
                End If
            Else
                Try
                    ' If stream. Then stream.Close()  '*** crashes if the stream is not open. seems to be closed with the client anyway
                    If client.Connected Then client.Close()
                    Debug.Print("Not Connected - Else.")
                    ShowResult = myCRC_16.DisplayResult("Connection closed.", 1, 0, 0)
                Catch ex As NullReferenceException
                    Debug.Print("Make Connection - Null Reference Exception caught.")
                End Try
            End If
            Debug.Print("Done...")
        Catch ex As SocketException
            Debug.Print("Socket Error " & ex.NativeErrorCode & " " & ex.SocketErrorCode & " " & ex.Message)

        End Try

    End Sub
    Private Sub BtnFirmware_Click(sender As Object, e As EventArgs) Handles BtnFirmware.Click
        '*** Disable manual version button
        BtnFirmware.Enabled = False
        TxtDebug.Text = ""

        '*** Log activity
        StrTextToLog = "Get Firmware"
        Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
        Dim debugFileNumber As Integer = FreeFile()
        FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
        Dim debugTime As String = Format(Now(), "hh:mm:ss")
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim StrResult As String = GetMasterVersion()

        '*** If logging to this file is needed in other areas either replicate the file name or make it global
        '*** OK here only for now.
        '*** Log data
        debugTime = Format(Now(), "hh:mm:ss")
        StrTextToLog = StrResult
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim Decoded As String = RT_Decode("92", StrResult)

        '*** Log data
        StrTextToLog = Decoded
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
        FileClose(debugFileNumber)

        '*** Enable button
        BtnFirmware.Enabled = True

    End Sub
    Shared Function Connect(server As [String], message As [String], replySize As Integer, MasterChannel As [String], OrderRefText As [String])
        '*** Hop count refers also to the TTL Time To Live value for the MultiCastScope 

        MasterChannel = UCase(MasterChannel)

        Dim data(Len(message) - 1) As Byte
        Dim data2SendFormatted As String = ""
        Dim dataReceivedFormatted As String = ""

        '*** Log details
        Dim debugMode As Boolean = False
        Dim debugFilename As String = FrmMain.StrRootFolder & "Debug RT_" & Format(Now(), "yyyyMMdd") & "_" & Trim(OrderRefText) & MasterChannel & ".log"
        Dim debugFileNumber As Integer = FreeFile()
        Dim debugTime As String = Format(Now(), "hh:mm:ss")
        Dim IUnderscore As Integer = InStrRev(MasterChannel, "_")
        Dim SMasterID As String = Microsoft.VisualBasic.Mid(MasterChannel, 2, IUnderscore - 2)
        Dim SChannelID As String = ""


        If Len(MasterChannel) > 2 Then SChannelID = Microsoft.VisualBasic.Right(MasterChannel, Len(MasterChannel) - IUnderscore)
        Debug.Print("IO's " & SMasterID & " - " & SChannelID)
        Try

            'Open log file
            FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
            PrintLine(debugFileNumber, debugTime & " " & FrmMain.StrTextToLog)
            data2SendFormatted = ""
            For x = 1 To Len(message)
                '*** Store message into data byte array for transmission
                data(x - 1) = Asc(Mid(message, x, 1))
                data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(message, x, 1)))
            Next

            Debug.Print("Sending packet >" & data2SendFormatted & "< " & Len(message).ToString & " > " & debugTime)
            '*** data() is zero based 4 = 5 = Command ID
            Select Case data(4).ToString
                Case "32" '20
                    PrintLine(debugFileNumber, debugTime & ": Disabled Wi-Fi")
                Case "128" '80
                    PrintLine(debugFileNumber, debugTime & ": Start trip Test on Channel " & (data(5) + 1).ToString & " [" & data(5).ToString & "]")
                Case "129" '81
                    PrintLine(debugFileNumber, debugTime & ": Fetch Result")
                Case "145" '91
                    PrintLine(debugFileNumber, debugTime & ": Set Wi-Fi Settings [" & SMasterID & "]")
                Case "146" '92
                    PrintLine(debugFileNumber, "****************************************************")
                    PrintLine(debugFileNumber, debugTime & ": Order Reference  : " & OrderRefText)
                    '** Record Master & Channel board details if any
                    If SMasterID <> "" Then PrintLine(debugFileNumber, debugTime & ": Master ID        : " & SMasterID)
                    If SChannelID <> "" Then PrintLine(debugFileNumber, debugTime & ": Channel Board ID : " & SChannelID)

                    PrintLine(debugFileNumber, debugTime & ": Get firmware version.")
                Case "186" 'BA
                    PrintLine(debugFileNumber, debugTime & ": Setting RCD Mapping for channel " & (data(5) + 1).ToString & " [" & data(5).ToString & "] = " & data(6))
                Case "187" 'BB
                    PrintLine(debugFileNumber, debugTime & ": Reading RCD Mapping for channel " & (data(5) + 1).ToString & " [" & data(5).ToString & "]")
                Case "188" 'BC
                    PrintLine(debugFileNumber, debugTime & ": Reading All RCD Mappings.")
                Case "189" 'BD
                    Dim sBoard As String = ""
                    Dim Iboard As Integer = data(3)
                    Select Case Iboard
                        Case 0
                            sBoard = "Master"
                        Case Else
                            sBoard = "Channel board " & data(3).ToString
                    End Select
                    Dim StempGUID As String = Microsoft.VisualBasic.Mid(message, 6, 36)
                    PrintLine(debugFileNumber, debugTime & ": Set GUID for " & sBoard & " = " & StempGUID)
                Case "190" 'BE
                    Dim sBoard As String = ""
                    Dim Iboard As Integer = data(3)
                    Select Case Iboard
                        Case 0
                            sBoard = "Master"
                        Case Else
                            sBoard = "Channel board " & data(3).ToString
                    End Select
                    PrintLine(debugFileNumber, debugTime & ": Read " & sBoard & " GUID.")
                Case "218" 'DA
                    PrintLine(debugFileNumber, debugTime & ": Read Bus Power.")
                Case "234" 'EA
                    PrintLine(debugFileNumber, debugTime & ": Discovering Topology.")
                Case "235" 'EB
                    PrintLine(debugFileNumber, debugTime & ": Reading Topology.")
                Case Else

                    PrintLine(debugFileNumber, debugTime & "   >>> SENDING >" & data2SendFormatted & "< " & Len(message).ToString)
            End Select
            'PrintLine(debugFileNumber, debugTime & ": Raw data >" & data2SendFormatted & "< " & Len(message).ToString)

            '*** Test Reply should be 55 aa 08 00 da 01 8c dd to 0xDA Read Test Bus Powered

            ' Get a client stream for reading and writing.
            'Dim stream As NetworkStream = client.GetStream()
            If client.Connected Then

                '***********************************************
                '*** SEND COMMAND FROM HERE
                '***********************************************
                stream = client.GetStream()

                '** Delay to open stream
                Threading.Thread.Sleep(Microsoft.VisualBasic.Val(100))

                ' Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length)
                Application.DoEvents()

                '*** Delay reading response buffer
                Dim sleepDelay As Integer = replySize * 20
                Debug.Print("Calculated Delay " & sleepDelay.ToString)
                If sleepDelay < 1200 Then sleepDelay = 1200
                If sleepDelay > 5000 Then sleepDelay = 5000
                Debug.Print("Reply Size " & replySize.ToString)

                '*** LAN ONLY CODE
                '*** Calculate delay overide for LAN
                If FrmMain.ChkLAN.Checked Then
                    Select Case data(4)
                        Case "234", "129"
                            sleepDelay = 4500 '*** Only required for LAN version
                    End Select
                End If

                '*** BLE ONLY CODE
                If FrmMain.ChkBLE.Checked Then

                End If

                Debug.Print("Final Delay " & sleepDelay.ToString)
                System.Threading.Thread.Sleep(sleepDelay) '*
                'System.Threading.Thread.Sleep(5000) '*

                '***********************************************
                '*** RESPONSE FROM HERE
                '***********************************************

                data = New [Byte](replySize - 1) {}
                ' String to store the response ASCII representation.
                Dim responseData As [String] = [String].Empty

                ' Read the first batch of the TcpServer response bytes.
                '*** MAY CHECK DATA IS AVAILABLE FIRST TO PREVENT THE READ FROM BLOCKING (DataAvailable)

                '** Only check if data is expected
                If replySize > 0 Then
                    Debug.Print("********** Checking for Data of Length " & data.Length.ToString)

                    Dim bDataAvailable As Boolean = stream.DataAvailable
                    Dim bytes As Integer = stream.Read(data, 0, data.Length) ' 
                    Debug.Print("********** Data Checked " & bytes.ToString & " found.")
                    Debug.Print("data available = " & bDataAvailable.ToString)


                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)
                    Debug.Print("Received: {0}", responseData)
                    Debug.Print(vbCrLf)
                    '*** Check incomming checksum
                    '*** Assign data only - no checksum
                    '*** 0 based byte array = reply size - 3. 2 for checksum and 1 to offset to 0
                    Dim dataByteCount As Integer = replySize - 3
                    Dim dataOnly = New [Byte](dataByteCount) {}
                    For x = 0 To dataByteCount
                        dataOnly(x) = data(x)

                        Debug.Print("Data Only " & Hex(dataOnly(x)))

                    Next

                    Dim csumOnly = New [Byte](2) {}
                    csumOnly(0) = data(dataByteCount + 1)
                    csumOnly(1) = data(dataByteCount + 2)

                    '*** Calculate checksum for comparisson
                    Dim myCRC_16_2 As New CRC_16()

                    Dim showSendnData As Integer = myCRC_16_2.DisplayResult("Sending packet >" & data2SendFormatted & "< ", 0, 0, 0)

                    Dim csum() As Byte = myCRC_16_2.ComputeChecksumBytes(dataOnly)
                    Debug.Print("Calculated CheckSum " & Hex(csum(0)) & " " & Hex(csum(1)))
                    Debug.Print("  Received CheckSum " & Hex(csumOnly(0)) & " " & Hex(csumOnly(1)))
                    Dim checkSumIsOK As Boolean = False
                    If Hex(csum(0)) = Hex(csumOnly(0)) And Hex(csum(1)) = Hex(csumOnly(1)) Then
                        checkSumIsOK = True
                    End If
                    dataReceivedFormatted = [String].Empty
                    For x = 0 To bytes - 1
                        dataReceivedFormatted = dataReceivedFormatted & " " & Hex(data(x))
                    Next

                    '****************************************************
                    '**** SAVE RESPONSE TO RETURN FROM FUNCTION
                    '****************************************************
                    Connect = dataReceivedFormatted
                    '****************************************************

                    '*** decode for result packet 0x81 = 129. This is the trip test result
                    Dim sError As String = ""
                    Dim sChannel As String = ""
                    Dim sDuration As String = ""
                    Dim sCurrent As String = ""
                    Dim iChTested As Integer = 0
                    '*** Fetch results 0x81
                    If data(4) = 129 And replySize > 7 Then
                        Debug.Print("Response processing...")

                        '*** Channel Tested
                        sChannel = "Channel " & (data(6) + 1).ToString
                        '** Update Checkboxes on main form
                        iChTested = data(6) + 1
                        myCRC_16_2.DisplayTestResults(iChTested.ToString)

                        '*** duration
                        Dim iDurData As Integer = 0
                        If data(8) > 0 Then iDurData = data(8) * 256 Else iDurData = 0
                        iDurData += data(7)
                        Dim sTemp As String = iDurData.ToString
                        sDuration = "Duration = " & sTemp

                        '*** Current  
                        Dim iCurData As Integer = 0
                        Dim tmpData As String = data(10).ToString("X2") & data(9).ToString("X2")
                        Dim RealResult As Long = Convert.ToUInt64(tmpData, 16)
                        sCurrent = (RealResult / 10).ToString

                        '*** Command errors if any
                        Select Case data(5)
                            Case 0
                                sError = "No Error:"
                                FrmMain.BSuppressTimeWarning = False
                            Case Else
                                Dim S_ErrorCode As String = Hex(data(5)).ToString
                                Select Case S_ErrorCode
                                    Case "10", "11", "12", "13"
                                        sError = "ERROR " & S_ErrorCode & " - Invalid test parameter"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "20"
                                        sError = "ERROR " & S_ErrorCode & " - Power on Test Bus before board powered"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "21"
                                        sError = "ERROR " & S_ErrorCode & " - Power on Test Bus before enable"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "22"
                                        sError = "ERROR " & S_ErrorCode & " - Power on Test Bus before channel Set"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "23"
                                        sError = "ERROR " & S_ErrorCode & " - No response from set channel"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "24"
                                        sError = "ERROR " & S_ErrorCode & " - Invalid Channel selected"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "25"
                                        sError = "ERROR " & S_ErrorCode & " - Channel Conflict"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "26"
                                        sError = "ERROR " & S_ErrorCode & " - No response to set power relay"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "27"
                                        sError = "ERROR " & S_ErrorCode & " - Power on Test Bus relay set"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "28"
                                        sError = "ERROR " & S_ErrorCode & " - No power before test"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "29"
                                        sError = "ERROR " & S_ErrorCode & " - ADC not cleared - Master"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case "30"
                                        sError = "ERROR " & S_ErrorCode & " - Phase not detected - Master"
                                        FrmMain.BSuppressTimeWarning = True
                                    Case Else
                                        sError = "ERROR " & S_ErrorCode & " - Undefined error code"
                                        FrmMain.BSuppressTimeWarning = True
                                End Select

                                '** IF there are any erorrs ignore and time or current data
                                iDurData = 0
                                iCurData = 0
                        End Select

                        Debug.Print(sError & " " & sChannel & " " & sDuration & " " & sCurrent)
                        Connect += vbNewLine & sError & " " & sChannel & " " & sDuration & " " & "Current leaked " & sCurrent
                        ' If debugGUID <> "" Then PrintLine(debugFileNumber, debugTime & "SSID [RT_" & debugSSID & "] GUID [" & debugGUID & "]")
                    End If

                    If data(4) = 146 Then
                        myCRC_16_2.SetVersion(data(5), data(6))
                        'PrintLine(debugFileNumber, debugTime & ": " & SMasterType)
                    End If
                    debugTime = Format(Now(), "hh:mm:ss")
                    Debug.Print("Received packet >" & dataReceivedFormatted & "< checksum is " & checkSumIsOK.ToString & " for " &
                            dataOnly.Length.ToString & " bytes" & " > " & debugTime)
                    debugTime = (Format(Now(), "hh:mm:ss"))

                    '*********************  DECODE results for logging

                    Dim BoolTopologyError As Boolean

                    Select Case data(4).ToString
                        Case "32" '20 - Should never get a response to this. This is 0x20 sleep wifi
                            PrintLine(debugFileNumber, debugTime & ": Disabled Wi-Fi - No Resposne")
                        Case "128" '80 - Done
                            PrintLine(debugFileNumber, debugTime & "  > Test started")
                        Case "129" '81 - Done
                            PrintLine(debugFileNumber, debugTime & ": Test Result:")
                            PrintLine(debugFileNumber, debugTime & "  > " & sError)
                            PrintLine(debugFileNumber, debugTime & "  > Channel = " & (data(6) + 1).ToString & " [" & data(6).ToString & "]")
                            Dim iDurData As Integer = 0
                            If data(8) > 0 Then iDurData = data(8) * 256 Else iDurData = 0
                            iDurData += data(7)
                            PrintLine(debugFileNumber, debugTime & "  > Duration (ms) = " & iDurData)  '*** duration is 2 bytes 
                            Dim iCurData As Integer = 0
                            If data(10) > 0 Then iCurData = data(9) * 256 Else iCurData = 0
                            iCurData += data(9)

                            '****
                            '*** Current  
                            sCurrent = "0"
                            Dim tmpData As String = data(10).ToString("X2") & data(9).ToString("X2")
                            Dim RealResult As Long = Convert.ToUInt64(tmpData, 16)
                            sCurrent = (RealResult / 10).ToString

                            '****
                            '*** 18/12/2019
                            If sError <> "No Error" Then sCurrent = "0"
                            ' PrintLine(debugFileNumber, debugTime & "  > Set Val (mA) = " & FrmMain.TxtSetCurrent.Text)
                            PrintLine(debugFileNumber, debugTime & "  > Current (mA) = " & sCurrent & " (" & data(10).ToString & "," & data(9).ToString & ")") '** Current is 2 bytes
                            'PrintLine(debugFileNumber, debugTime & "  > Current (mA) = " & iCurData & " (" & data(10).ToString & "," & data(9).ToString & ")") '** Current is 2 bytes
                            If data(11) = 1 Then
                                PrintLine(debugFileNumber, debugTime & "  > Tripped = Yes")
                                'FrmMain.B_tripped = True
                                'If FrmMain.B_RelayCheck Then
                                '    PrintLine(debugFileNumber, debugTime & "  > SHOULD NOT TRIP - Relay Test Mode")
                                '    ' MsgBox("Check channel - should not have returned as tripped.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Something Fishy Here")
                                'End If
                                'Else
                                '    PrintLine(debugFileNumber, debugTime & "  > Tripped = No")
                                '    If FrmMain.B_RelayCheck Then
                                '        PrintLine(debugFileNumber, debugTime & "  > Relay Test Mode")
                                '    End If
                                '    FrmMain.B_tripped = False
                            End If
                        Case "145" '91 - Done
                            PrintLine(debugFileNumber, debugTime & ": Set Wi-Fi Settings [RT_" & SMasterID & "]")
                        Case "146" '92 - Done
                            PrintLine(debugFileNumber, debugTime & ": Firmware version is " & data(6).ToString & "." & data(5).ToString)
                            PrintLine(debugFileNumber, "****************************************************")
                        Case "186" 'BA - Done
                            PrintLine(debugFileNumber, debugTime & ": Set single RCD for channel " & (data(6) + 1).ToString & " [" & data(6).ToString & "]")
                            If data(5) = 0 Then
                                PrintLine(debugFileNumber, debugTime & "  > RCD Mapping succeeded.")
                            Else
                                PrintLine(debugFileNumber, debugTime & "  > RCD Mapping Error. Channel out of range.")
                            End If

                        Case "187" 'BB - Done
                            PrintLine(debugFileNumber, debugTime & ": RCD Mapping for channel " & (data(5) + 1).ToString & " [" & data(5).ToString & "]")
                            PrintLine(debugFileNumber, debugTime & "  > RCD number = " & data(6).ToString)
                        Case "188" 'BC 
                            PrintLine(debugFileNumber, debugTime & ": Reading All RCD Mappings. >> " & dataReceivedFormatted)
                        Case "189" 'BD - Done
                            PrintLine(debugFileNumber, debugTime & "  > GUID set.")
                        Case "190" 'BE - Read GUID
                            Dim StempGUID As String = ""
                            For x = 5 To 40
                                StempGUID &= Chr(data(x))
                            Next
                            PrintLine(debugFileNumber, debugTime & "  > GUID = " & StempGUID)
                            PrintLine(debugFileNumber, debugTime & "  > GUID Packet = >> " & dataReceivedFormatted)
                            '*** Display ASCII version of the GUID
                            'Dim I_showResult As Integer = myCRC_16_2.displayResult(vbCrLf & "Guid String = " & StempGUID & vbCrLf, 1, 1, 0)
                            Dim I_showResult As Integer = myCRC_16_2.DisplayResult("Guid String = " & StempGUID & vbCrLf, 1, 1, 0)
                        Case "218" 'DA
                            PrintLine(debugFileNumber, debugTime & ": Bus Power Status.")
                            If data(5) = 1 Then
                                PrintLine(debugFileNumber, debugTime & "  > Bus is Live")
                                '*** Display bus status
                                Dim I_showResult As Integer = myCRC_16_2.DisplayResult(vbCrLf & "  > Bus is Live" & vbCrLf, 1, 1, 0)
                            Else
                                PrintLine(debugFileNumber, debugTime & "  > Bus is Safe")
                                '*** Display bus status
                                Dim I_showResult As Integer = myCRC_16_2.DisplayResult(vbCrLf & "  > Bus is Safe" & vbCrLf, 1, 1, 0)
                            End If
                        Case "234" 'EA - discover topoplgy. Response uses EB packet below
                            PrintLine(debugFileNumber, debugTime & ": Discovering Topology.")
                            If data(5) = 0 Then
                                PrintLine(debugFileNumber, debugTime & "  > No Error.")
                            Else
                                PrintLine(debugFileNumber, debugTime & "  > Topology not yet discovered.")
                            End If
                            PrintLine(debugFileNumber, debugTime & "  > Number of boards detected = " & data(6).ToString)
                            PrintLine(debugFileNumber, debugTime & "  > Number of Channels = " & data(7).ToString)
                            If FrmMain.iVersionMajor >= 3 Then
                                For x = 7 To 19
                                    Select Case x
                                        Case 7
                                            myCRC_16_2.SetChannelCount(data(7))
                                        Case 8  ' Master type
                                            myCRC_16_2.GetMasterTopology(data(8))

                                    End Select
                                Next
                            Else
                                If FrmMain.iVersionMajor = 2 And FrmMain.iVersionMinor < 6 Then


                                Else
                                    '** Master is new standard with microAmp
                                    For x = 7 To 19
                                        Select Case x
                                            Case 7
                                                myCRC_16_2.SetChannelCount(data(7))
                                            Case 8  ' Master type
                                                myCRC_16_2.GetMasterTopology(data(8))

                                        End Select
                                    Next
                                End If
                            End If

                        Case "235" 'EB. 
                            '**** 21/09/2018
                            '** 25/09/*2018 also added to discover topology above
                            'If the version number is above 3.0 a new response is expected here
                            '** New response has the same data in bytes 5, 6 & 7 but adds the following
                            '******** zero based byte count
                            '** Byte 8 Model of tester. 01, 03, 05 or 06
                            '           0x01 Tester with no integrated channels - original standard master
                            '           0x02 24 channel CIB
                            '           0x03 Tester with 8 integrated channels - MC8 - RELEY
                            '           0x04 8 channel CIB - Does not exist as a seperate unit
                            '           0x00 No device in that location
                            '           0x05 Tester with 8 integrated channel - MC8 SOLID STATE
                            '           0x06 Tester with 4 integrated - MC8 SOLID STATE
                            '           0x07 internally used for 4 channel CIB section of MC4
                            '           0x08 12 Channel CIB - SOLID STATE
                            '** Bytes 9 - 19 Map of Attached CIB devices 
                            '           Using the same codes above
                            PrintLine(debugFileNumber, debugTime & ": Topology.")
                            If data(5) = 0 Then
                                PrintLine(debugFileNumber, debugTime & "  > No Error.")
                                BoolTopologyError = False
                            Else
                                PrintLine(debugFileNumber, debugTime & "  > Topology not yet discovered or error reading topology.")
                                BoolTopologyError = True
                            End If
                            PrintLine(debugFileNumber, debugTime & "  > Number of boards detected = " & data(6).ToString)
                            PrintLine(debugFileNumber, debugTime & "  > Number of Channels = " & data(7).ToString)
                            If FrmMain.iVersionMajor >= 3 Then
                                For x = 7 To 17
                                    Select Case x
                                        Case 7
                                            myCRC_16_2.SetChannelCount(data(7))
                                        Case 8  ' Master type
                                            myCRC_16_2.GetMasterTopology(data(8))
                                        Case 9  'First Channel board
                                            If data(9) = 8 Then
                                                '*** 12 channel SS CIB
                                                '** 14.11.2019  NO MORE SOLID STATE
                                                'FrmMain.ChkSS.Checked = True
                                            End If
                                    End Select
                                Next
                            Else
                                If FrmMain.iVersionMajor = 2 And FrmMain.iVersionMinor < 6 Then

                                Else
                                    '** Master is new standard with microAmp
                                    For x = 7 To 19
                                        Select Case x
                                            Case 7
                                                myCRC_16_2.SetChannelCount(data(7))
                                            Case 8  ' Master type
                                                myCRC_16_2.GetMasterTopology(data(8))

                                        End Select
                                    Next
                                End If
                            End If

                        Case Else
                            PrintLine(debugFileNumber, debugTime & "   >>> RECEIVED >" & dataReceivedFormatted & "< checksum is " & checkSumIsOK.ToString &
                         " for " & dataOnly.Length.ToString & " bytes")
                    End Select
                    '**********************
                    '** Display error or result
                    If BoolTopologyError Then
                        Dim showResult As Integer = myCRC_16_2.DisplayResult("Received Reply =>" & dataReceivedFormatted & "< checksum is " & checkSumIsOK.ToString &
                                                                          " for " & dataOnly.Length.ToString & " bytes" & vbCrLf & "*** ERROR READING TOPOLOGY ***", 1, 1, 0)
                    Else
                        Dim showResult As Integer = myCRC_16_2.DisplayResult("Received Reply =>" & dataReceivedFormatted & "< checksum is " & checkSumIsOK.ToString &
                                                                      " for " & dataOnly.Length.ToString & " bytes", 1, 1, 0)
                    End If

                    If sError <> "No Error" Then
                        sCurrent = "0"
                    End If

                    If data(4) <> 146 Then sCurrent = "Current = " & sCurrent.ToString & " mA"

                    Debug.Print(sError & " " & sChannel & " " & sDuration & " " & sCurrent)
                    'If sChannel <> "" Then

                    '    Dim S_Trip As String = "NO"
                    '    If FrmMain.B_RelayCheck Then S_Trip = "Relay Test Mode"
                    '    '*** Change message if relay test mode
                    '    If FrmMain.B_tripped Then
                    '        S_Trip = "YES"
                    '        If FrmMain.B_RelayCheck Then S_Trip = "YES - Relay Test Mode"
                    '        FrmMain.SetStatus(sError, iChTested.ToString, S_Trip)
                    '        If (iChTested = 1) Or (iChTested = 9) Then
                    '            Dim BlockMessage As Boolean = FrmMain.ChkSkipCIB.Checked And iChTested = 9
                    '            If Not BlockMessage Then
                    '                Dim IntContinue As Integer = MsgBox("Do you wish to continue in automatic mode?", vbQuestion + vbYesNo, "Continue ?")
                    '                If IntContinue = vbYes Then
                    '                    FrmMain.AutoStartTesting = True
                    '                    FrmMain.bAutoEnabled = True
                    '                    FrmMain.chkAuto.Text = "Auto ON"
                    '                    FrmMain.chkAuto.Checked = True
                    '                Else
                    '                    FrmMain.AutoStartTesting = False
                    '                    FrmMain.bAutoEnabled = False
                    '                    FrmMain.chkAuto.Text = "Auto OFF"
                    '                    FrmMain.chkAuto.Checked = False
                    '                End If
                    '            End If
                    '        End If
                    '    Else    '**** NOT tripped
                    '        If Not FrmMain.B_RelayCheck Then
                    '            '*** Show no trip image if not in relay test mode
                    '            FrmMain.ImgError.Visible = True
                    '            If FrmMain.iCurrentStep = 9 Then
                    '                If (iChTested = 1) Or (iChTested = 9) Then
                    '                    Dim BlockMessage As Boolean = FrmMain.ChkSkipCIB.Checked And iChTested = 9
                    '                    If Not BlockMessage Then
                    '                        Dim IntContinue As Integer = MsgBox("RCD " & iChTested.ToString & " did not trip. Do you wish to continue in automatic mode?", vbQuestion + vbYesNo, "Continue ?")
                    '                        If IntContinue = vbYes Then
                    '                            FrmMain.AutoStartTesting = True
                    '                            FrmMain.bAutoEnabled = True
                    '                            FrmMain.chkAuto.Text = "Auto ON"
                    '                            FrmMain.chkAuto.Checked = True
                    '                        Else
                    '                            FrmMain.AutoStartTesting = False
                    '                            FrmMain.bAutoEnabled = False
                    '                            FrmMain.chkAuto.Text = "Auto OFF"
                    '                            FrmMain.chkAuto.Checked = False
                    '                        End If
                    '                    End If
                    '                End If
                    '            End If
                    '        End If
                    '        FrmMain.SetStatus(sError, iChTested.ToString, S_Trip)
                    '    End If
                    '    Dim showResultData As Integer = myCRC_16_2.DisplayResult(sError & vbCrLf & sChannel & ":   " & sDuration & "   " & sCurrent & vbCrLf & "Trip: " & S_Trip, 1, 0, 0)
                    'Else
                    '    Dim showResultData As Integer = myCRC_16_2.DisplayResult("Waiting for response . . . ", 1, 0, 0)
                    'End If
                    ' Close everything.
                    'stream.Close()
                    'client.Close()
                Else    '*** Else Reply is not > 0
                    Debug.Print("This command does not get a response...")
                    Dim myCRC_16 As New CRC_16()
                    '*** Successful poll no response expected.
                    Dim showResult2 As Integer = myCRC_16.DisplayResult("This command does not get a response", True, 1, 3)
                End If ' *** Reply > 0

            Else
                Debug.Print("Skip send. Not Connected")
                debugTime = (Format(Now(), "hh:mm:ss"))
                PrintLine(debugFileNumber, debugTime & ": Skip send. Not Connected")
                Dim myCRC_16 As New CRC_16()
                Dim showResult2 As Integer = myCRC_16.DisplayResult("Failed. No Connection", True, -1, 1)
            End If '*** If ethernet client is connected

            'makeConnection(server, False)
            FileClose(debugFileNumber)
        Catch e As ArgumentNullException
            Debug.Print("ArgumentNullException: {0}", e)
            debugTime = (Format(Now(), "hh:mm:ss"))
            PrintLine(debugFileNumber, debugTime & " In Connect 1: ArgumentNullException: {0}", e)
        Catch e As SocketException
            Debug.Print("SocketException: {0}", e)
            debugTime = (Format(Now(), "hh:mm:ss"))
            PrintLine(debugFileNumber, debugTime & " In Connect 2: ArgumentNullException: {0}", e)

        Catch err As Exception
            MessageBox.Show(err.Message & vbCrLf & err.Source)
        End Try
        '    Connect = "Result"
    End Function 'Connect
    Private Function RT_Decode(CMD As String, StrPacket As String) As String
        Dim StrRawPacket As String = StrPacket
        Dim StrResultArray(250) As String
        Dim IntSpacePos As Integer
        Dim IntCounter As Integer = 0
        StrResultArray(IntCounter) = CMD
        IntSpacePos = InStr(StrPacket, " ")
        Debug.Print(StrPacket)

        While IntSpacePos > 0
            IntCounter += 1
            StrResultArray(IntCounter) = Strings.Left(StrPacket, IntSpacePos)
            Debug.Print("Byte saved " & StrResultArray(IntCounter))
            StrPacket = Strings.Right(StrPacket, Len(StrPacket) - IntSpacePos)
            IntSpacePos = InStr(StrPacket, " ")
            '*** NOTE THIS METHOD DOES NOT RETURN THE SECOD CHECKSUM BYTE, BUT IT ISNOT NEEDED HERE
            '*** AS IT HAS ALREADY BEEN CHECKED
        End While

        Select Case CMD

            Case "EA", "EB" '*** Discover or get topology
                Debug.Print(CMD & " or EB Should equal " & StrResultArray(5))
                TxtDebug.Text = "Read Config Result"
                Dim TmpString As String = ""
                Dim IntResponseLength As Integer = CInt("&H" & (Trim(StrResultArray(3)))) - 1 '2
                For x = 1 To IntResponseLength
                    TmpString += StrResultArray(x)
                Next
                TxtDebug.Text += vbNewLine & "Configuration " & TmpString
                '*** decode some info
                Select Case Trim(StrResultArray(9))
                    Case "9"
                        LblType.Text = "Type: MC8F5"
                        IntMasterType = MC8F5
                        IntMaxChannel = CInt("&H" & (Trim(StrResultArray(8))))
                        LblChLimit.Text = IntMaxChannel.ToString
                    Case Else
                        LblType.Text = "Type: MC8"
                        IntMasterType = MC8
                End Select

                RT_Decode = "Response to command " & Trim(StrResultArray(5)) & " = " & TmpString

            Case "80", "81"
                Debug.Print(CMD & " or 81 Should equal " & StrResultArray(5))
                '*** Do test - Trip RCD
                TxtDebug.Text = "Trip RCD " & StrRawPacket

                RT_Decode = StrResultArray(12)

                '*** Overide result if B_RelayCheck is active as these don't trip
                If B_RelayCheck Then
                    If Trim(StrResultArray(6)) = "0" Then   '** No Error
                        Select Case Trim(StrResultArray(12))
                            Case "0" '*** relay check - no trip
                                RT_Decode = 3
                            Case "1" '*** relay check - False trip
                                RT_Decode = 4
                        End Select
                    Else
                        RT_Decode = -1
                    End If
                End If

            Case "91"
                '*** Set master GUID
                TxtDebug.Text = "Set Master GUID " & StrRawPacket
                RT_Decode = "Response to Set Master GUID command " & StrRawPacket

            Case "92"   '***  Get firmware version
                Debug.Print(CMD & " Should equal " & StrResultArray(5))
                TxtDebug.Text = "Get Firmware Version"
                iVersionMinor = StrResultArray(6)
                iVersionMajor = StrResultArray(7)
                LblVersion.Text = "Version: " & iVersionMajor & "." & iVersionMinor
                TxtDebug.Text += vbNewLine & "Version " & iVersionMajor & "." & iVersionMinor
                RT_Decode = "Response to command " & Trim(StrResultArray(5)) & " = Version: " & iVersionMajor & "." & iVersionMinor

            Case "BE" '*** Get GUIDs
                Debug.Print(CMD & " Should equal " & StrResultArray(5) & " " & StrRawPacket)
                Dim StempGUID As String = ""
                For x = 6 To 41
                    StempGUID &= Chr(CInt("&H" & Trim(StrResultArray(x))))
                Next
                RT_Decode = StempGUID
                'RT_Decode = "Response to command " & Trim(StrResultArray(5)) & " = GUID: = " & StempGUID
                TxtDebug.Text = "Get Master or CIB GUID = " & StempGUID

            Case "DA" '*** Get BUS power state
                Debug.Print(CMD & " Should equal " & StrResultArray(5))
                Dim StrResultText As String = ""
                Select Case Trim(StrResultArray(6))
                    Case "0"
                        TxtDebug.Text = "Get Bus Power Status = Safe"
                        StrResultText = "Safe"
                    Case "1"
                        TxtDebug.Text = "Get Bus Power Status = Live - DO NOT CONTINUE"
                        StrResultText = "DO NOT CONTINUE"
                    Case Else
                        TxtDebug.Text = "Get Bus Power Status = ERROR - DO NOT CONTINUE"
                        StrResultText = "DO NOT CONTINUE"
                End Select
                RT_Decode = "Response to command " & Trim(StrResultArray(5)) & " = Bus = " & StrResultText

            Case Else
                '*** CHECK FOR ERRORS

        End Select
    End Function
    Private Sub BtnConfig_Click(sender As Object, e As EventArgs) Handles BtnConfig.Click
        '*** Disable button
        BtnConfig.Enabled = False

        '*** Log activity
        StrTextToLog = "Discover Config"
        Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
        Dim debugFileNumber As Integer = FreeFile()
        FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
        Dim debugTime As String = Format(Now(), "hh:mm:ss")
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim StrResult As String = ReadAllTopology()
        '*** If logging to this file is needed in other areas either replicate the file name or make it global
        '*** OK here only for now.
        '*** Log data
        debugTime = Format(Now(), "hh:mm:ss")
        StrTextToLog = StrResult
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim Decoded As String = RT_Decode("EA", StrResult)
        '*** Log data
        StrTextToLog = Decoded
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
        FileClose(debugFileNumber)
        '*** Enable button
        BtnConfig.Enabled = True
    End Sub
    Private Function ReadAllTopology()
        '*** Connect to device
        '*** Make sure it is disconnected to start with
        MakeConnection(sIP, False, iPort)

        '** Clear debug window
        Me.TxtDebug.Text = ""

        '*** Enable connection
        MakeConnection(sIP, True, iPort)


        '*** Discover topology
        Dim data2Send As String = ""
        Dim commandLength As Integer = 6   '*** 0 based Length less 2 x Check sum bytes

        Dim bytesForCRC(commandLength) As Byte

        Dim myCRC_16 As New CRC_16()
        '********** Command bytes
        bytesForCRC(0) = 85         '** Header A
        bytesForCRC(1) = 170        '** Header B
        bytesForCRC(2) = &H9  '** Command length with 2 x checksum
        bytesForCRC(3) = &H0   '** Device address 0 for tester
        bytesForCRC(4) = &HEA  '** Command instruction - Discover Topology
        '********** Data
        bytesForCRC(5) = &HFF  '** Command instruction - Timeout A
        bytesForCRC(6) = &H2  '** Command instruction - Timeout B

        '*********** START Checksum calculation
        Dim csum() As Byte = myCRC_16.ComputeChecksumBytes(bytesForCRC)
        '************ END Checksum calculation

        For x = 0 To commandLength
            data2Send &= Chr(bytesForCRC(x))
        Next
        data2Send = data2Send & Chr(csum(0)) & Chr(csum(1))

        Dim data2SendFormatted As String = ""
        For x = 1 To Len(data2Send)
            data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
        Next

        Debug.Print("Command: Discover topology 0xEA")
        TxtDebug.Text = "Command: Discover topology 0xEA"

        StrTextToLog = "Discovering Configuration"
        S_LogFileText = "_" & TxtMasterSSID.Text & "_" & TxtCIBid.Text
        Dim StrResult As String = Connect(sIP, data2Send, ResponseLength, S_LogFileText, TxtOrderRef.Text)
        Debug.Print("****** Discovering Configuration " & StrResult)
        TxtDebug.Text = "****** Discovering Configuration " & StrResult

        '************************************
        '*** Return result for decoding
        ReadAllTopology = Trim(StrResult)
        '************************************
        '*** allow app to Update graphics
        Application.DoEvents()

        '*** Disconnect
        MakeConnection(sIP, False, iPort)

    End Function
    Private Function ReadGuid(Device As String)
        '*** Connect to device
        '*** Make sure it is disconnected to start with
        MakeConnection(sIP, False, iPort)

        '** Clear debug window
        Me.TxtDebug.Text = ""

        '*** Enable connection and disable manual connection button
        MakeConnection(sIP, True, iPort)
        BtnMguid.Enabled = False

        '*** Read GUID
        Dim data2Send As String = ""
        Dim commandLength As Integer = 4   '*** 0 based Length less 2 x Check sum bytes

        Dim bytesForCRC(commandLength) As Byte

        Dim myCRC_16 As New CRC_16()
        '********** Command bytes
        bytesForCRC(0) = 85         '** Header A
        bytesForCRC(1) = 170        '** Header B
        bytesForCRC(2) = &H7  '** Command length with 2 x checksum
        bytesForCRC(3) = "&H" & Device   '** Device address 0 for tester
        bytesForCRC(4) = &HBE  '** Command instruction - Read GUID

        '*********** START Checksum calculation
        Dim csum() As Byte = myCRC_16.ComputeChecksumBytes(bytesForCRC)
        '************ END Checksum calculation

        For x = 0 To commandLength
            data2Send &= Chr(bytesForCRC(x))
        Next
        data2Send = data2Send & Chr(csum(0)) & Chr(csum(1))

        Dim data2SendFormatted As String = ""
        For x = 1 To Len(data2Send)
            data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
        Next

        Debug.Print("Command:Read GUID 0xBE")
        TxtDebug.Text = "Command:Read GUID 0xBE >" & data2SendFormatted & "< " & Len(data2Send).ToString

        StrTextToLog = " Getting GUID from device " & Device
        S_LogFileText = " _" & TxtMasterSSID.Text & " _" & TxtCIBid.Text
        Dim StrResult As String = Connect(sIP, data2Send, 47, S_LogFileText, TxtOrderRef.Text)
        Debug.Print(" ****** GUID " & StrResult)
        TxtDebug.Text = " ****** GUID " & StrResult

        '************************************
        '*** Return result for decoding
        ReadGuid = Trim(StrResult)
        '************************************
        '*** allow app to Update graphics
        Application.DoEvents()

        '*** Disconnect and re-enable manual connect button.
        MakeConnection(sIP, False, iPort)
        BtnMguid.Enabled = True
    End Function

    Private Sub BtnMguid_Click(sender As Object, e As EventArgs) Handles BtnMguid.Click
        '*** Log activity
        StrTextToLog = "Get Master GUID"
        Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
        Dim debugFileNumber As Integer = FreeFile()
        FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
        Dim debugTime As String = Format(Now(), "hh:mm:ss")
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim StrResult As String = ReadGuid("0")

        '*** If logging to this file is needed in other areas either replicate the file name or make it global
        '*** OK here only for now.
        '*** Log data
        debugTime = Format(Now(), "hh:mm:ss")
        StrTextToLog = StrResult
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim Decoded As String = RT_Decode("BE", StrResult)
        '*** Log data
        StrTextToLog = Decoded
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
        FileClose(debugFileNumber)

    End Sub

    Private Sub BtnCIBID_Click(sender As Object, e As EventArgs) Handles BtnCIBID.Click
        '*** Log activity
        StrTextToLog = "Get CIB GUID"
        Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
        Dim debugFileNumber As Integer = FreeFile()
        FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
        Dim debugTime As String = Format(Now(), "hh:mm:ss")
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim StrResult As String = ReadGuid("1")

        '*** If logging to this file is needed in other areas either replicate the file name or make it global
        '*** OK here only for now.
        '*** Log data
        debugTime = Format(Now(), "hh:mm:ss")
        StrTextToLog = StrResult
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim Decoded As String = RT_Decode("BE", StrResult)
        '*** Log data
        StrTextToLog = Decoded
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
        FileClose(debugFileNumber)
    End Sub

    Private Sub BtnBusPwr_Click(sender As Object, e As EventArgs) Handles BtnBusPwr.Click
        '*** Log activity
        StrTextToLog = "Check BUS Power"
        Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
        Dim debugFileNumber As Integer = FreeFile()
        FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
        Dim debugTime As String = Format(Now(), "hh:mm:ss")
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim StrResult As String = ReadBusPower()

        '*** If logging to this file is needed in other areas either replicate the file name or make it global
        '*** OK here only for now.
        '*** Log data
        debugTime = Format(Now(), "hh:mm:ss")
        StrTextToLog = StrResult
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim Decoded As String = RT_Decode("DA", StrResult)
        '*** Log data
        StrTextToLog = Decoded
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
        FileClose(debugFileNumber)
    End Sub
    Private Function ReadBusPower()
        MakeConnection(sIP, False, iPort)

        '** Clear debug window
        Me.TxtDebug.Text = ""

        '*** Enable connection and disable manual connection button
        MakeConnection(sIP, True, iPort)
        BtnBusPwr.Enabled = False

        '*** Read Bus power active state
        Dim commandLength As Integer = 4   '*** 0 based Length less 2 x Check sum bytes
        Dim data2Send As String = ""

        '*********** START Checksum calculation
        Dim bytesForCRC(4) As Byte

        Dim myCRC_16 As New CRC_16()

        bytesForCRC(0) = 85
        bytesForCRC(1) = 170
        bytesForCRC(2) = &H7
        bytesForCRC(3) = &H0
        bytesForCRC(4) = &HDA

        Dim csum() As Byte = myCRC_16.ComputeChecksumBytes(bytesForCRC)
        '************ END Checksum calculation

        For x = 0 To commandLength
            data2Send &= Chr(bytesForCRC(x))
        Next
        data2Send = data2Send & Chr(csum(0)) & Chr(csum(1)) '& Chr(&HDA) '

        Dim data2SendFormatted As String = ""
        For x = 1 To Len(data2Send)
            data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
        Next
        '*** Test Reply should be 55 aa 08 00 da 01 8c dd to 0xDA Read Test Bus Powered

        Debug.Print(" Command:Read Bus Powered 0xDA")
        TxtDebug.Text = " Command:Read Bus Powered 0xDA"
        'Connect(" 169.254.1.1", data2Send, 8)


        StrTextToLog = " Check Bus Power"
        S_LogFileText = " _" & TxtMasterSSID.Text & " _" & TxtCIBid.Text
        Dim StrResult As String = Connect(sIP, data2Send, 8, S_LogFileText, TxtOrderRef.Text)
        Debug.Print(" ****** Bus Power " & StrResult)
        TxtDebug.Text = " ****** Bus Power " & StrResult

        '************************************
        '*** Return result for decoding
        ReadBusPower = Trim(StrResult)
        '************************************

        '*** allow app to Update graphics
        Application.DoEvents()

        '*** Disconnect and re-enable manual connect button.
        MakeConnection(sIP, False, iPort)
        BtnBusPwr.Enabled = True
    End Function
    Private Function WiFiAutoConnect(AP_Name As String, AP_Password As String) As Integer
        TmrWifi.Enabled = False

        Dim RetValue As Integer = 0
        Dim auth_request As AuthRequest

        Try
            Dim MyWifiList As List(Of AccessPoint) = MyWiFi.GetAccessPoints()
            If Not MyWiFi.NoWifiAvailable Then
                For Each item In MyWifiList
                    Dim S_Item As String = ""
                    If item.Name = "" Then S_Item = " Hidden" Else S_Item = item.Name
                    If item.Name = AP_Name Then
                        auth_request = New AuthRequest(item) With {
                            .Password = AP_Password
                        }
                        item.ConnectAsync(auth_request, False)
                        RetValue = 1
                        Application.DoEvents()
                    End If
                Next
            End If
        Catch Err As Exception
            Debug.Print(" Catch - No Wifi")
            LblWifi.ForeColor = Color.Red
            LblWifi.Text = " No WiFi Card Detected"
        End Try
        TmrWifi.Enabled = True
        Application.DoEvents()
        Threading.Thread.Sleep(1000)

        Debug.Print("WiFiAutoConnect " & SConnectedWiFi & " - " & RetValue.ToString)
        Application.DoEvents()
        Return RetValue
        ' Use: MyWiFi.Disconnect()
        ' To close the current connection.

    End Function
    Function ConnectWiFi(Ignore001 As Boolean) As Integer
        Dim StrConnectionID As String
        If ChkConnectToID.Checked Then
            '*** connect to dispalyed SSID instead of RT_001
            StrConnectionID = "RT_" & Trim(TxtMasterSSID.Text)
        Else
            If Ignore001 Then
                '*** connect to dispalyed SSID instead of RT_001
                StrConnectionID = "RT_" & Trim(TxtMasterSSID.Text)
            Else
                '*** Connect to default SSID
                StrConnectionID = "RT_001"
            End If
        End If
        ConnectWiFi = WiFiAutoConnect(StrConnectionID, StrRTpassword)
    End Function
    Private Sub BtnWifiConnect_Click(sender As Object, e As EventArgs) Handles BtnWifiConnect.Click

        If TxtMasterSSID.Text <> "" Then
            ConnectWiFi(False)
        Else

        End If
    End Sub
    Private Function ReadTopology()
        '*** Connect to device
        '*** Make sure it is disconnected to start with
        MakeConnection(sIP, False, iPort)

        '** Clear debug window
        Me.TxtDebug.Text = ""

        '*** Enable connection and disable manual connection button
        MakeConnection(sIP, True, iPort)
        BtnReadConfig.Enabled = False


        '*** Discover topology
        Dim data2Send As String = ""
        Dim commandLength As Integer = 6   '*** 0 based Length less 2 x Check sum bytes

        Dim bytesForCRC(commandLength) As Byte

        Dim myCRC_16 As New CRC_16()
        '********** Command bytes
        bytesForCRC(0) = 85         '** Header A
        bytesForCRC(1) = 170        '** Header B
        bytesForCRC(2) = &H9  '** Command length with 2 x checksum
        bytesForCRC(3) = &H0   '** Device address 0 for tester
        bytesForCRC(4) = &HEB  '** Command instruction - Discover Topology
        '********** Data
        bytesForCRC(5) = &HFF  '** Command instruction - Timeout A
        bytesForCRC(6) = &H2  '** Command instruction - Timeout B

        '*********** START Checksum calculation
        Dim csum() As Byte = myCRC_16.ComputeChecksumBytes(bytesForCRC)
        '************ END Checksum calculation

        For x = 0 To commandLength
            data2Send &= Chr(bytesForCRC(x))
        Next
        data2Send = data2Send & Chr(csum(0)) & Chr(csum(1))

        Dim data2SendFormatted As String = ""
        For x = 1 To Len(data2Send)
            data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
        Next

        Debug.Print("Command: Read topology 0xEB")
        TxtDebug.Text = "Command: Read topology 0xEB"

        StrTextToLog = "Reading Configuration"
        S_LogFileText = "_" & TxtMasterSSID.Text & "_" & TxtCIBid.Text
        Dim StrResult As String = Connect(sIP, data2Send, ResponseLength, S_LogFileText, TxtOrderRef.Text)
        Debug.Print("****** Read Configuration " & StrResult)
        TxtDebug.Text = "****** Read Configuration " & StrResult

        '************************************
        '*** Return result for decoding
        ReadTopology = Trim(StrResult)
        '************************************
        '*** allow app to Update graphics
        Application.DoEvents()

        '*** Disconnect and re-enable manual connect button.
        MakeConnection(sIP, False, iPort)
        BtnReadConfig.Enabled = True

    End Function

    Private Sub BtnReadConfig_Click(sender As Object, e As EventArgs) Handles BtnReadConfig.Click
        '*** Log activity
        StrTextToLog = "Read Config"
        Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
        Dim debugFileNumber As Integer = FreeFile()
        FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
        Dim debugTime As String = Format(Now(), "hh:mm:ss")
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)


        Dim StrResult As String = ReadTopology()
        '*** If logging to this file is needed in other areas either replicate the file name or make it global
        '*** OK here only for now.
        '*** Log data
        debugTime = Format(Now(), "hh:mm:ss")
        StrTextToLog = StrResult
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim Decoded As String = RT_Decode("EB", StrResult)
        '*** Log data
        StrTextToLog = Decoded
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
        FileClose(debugFileNumber)
    End Sub

    Private Sub BtnTrip_Click(sender As Object, e As EventArgs) Handles BtnTrip.Click
        BtnTrip.Enabled = False
        Dim StrResult As String = TripRCD()
        BtnTrip.Enabled = True
    End Sub
    Private Function TripRCD() As String
        If IntMasterType > 0 Then
            '*** Log activity
            StrTextToLog = "Trip RCD"
            Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
            Dim debugFileNumber As Integer = FreeFile()
            FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
            Dim debugTime As String = Format(Now(), "hh:mm:ss")
            PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

            '*** Connect first
            '*** Make sure it is disconnected
            MakeConnection(sIP, False, iPort)

            '** Clear debug window
            Me.TxtDebug.Text = ""

            '*** Enable connection and disable manual connection button
            MakeConnection(sIP, True, iPort)

            Dim StrResult As String = DoTest()
            '*** allow app to Update graphics
            Application.DoEvents()

            '*** Disconnect and re-enable manual connect button.
            MakeConnection(sIP, False, iPort)

            '*** If logging to this file is needed in other areas either replicate the file name or make it global
            '*** OK here only for now.
            '*** Log data
            debugTime = Format(Now(), "hh:mm:ss")
            StrTextToLog = StrResult
            PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

            Dim Decoded As String = RT_Decode("80", StrResult)
            TripRCD = Decoded


            '*** Log data
            StrTextToLog = Decoded
            PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
            FileClose(debugFileNumber)
        Else
            MsgBox("Please get configuration first", vbOKOnly + vbCritical, "Cannot continue")
        End If
    End Function
    Function DoTest() As String
        Dim iResult As String = 0
        Dim iSelectedChannel As Integer = Microsoft.VisualBasic.Val(LblChannel.Text)
        Dim iChannelLimit As Integer = Microsoft.VisualBasic.Val(LblChLimit.Text) ' - 1

        '*** If channel number is valid
        If IsNumeric(LblChannel.Text) Then
            If iSelectedChannel >= 0 And iSelectedChannel <= iChannelLimit Then
                '*** Commence Test
                Dim data2Send As String = ""
                Dim commandLength As Integer = 10   '*** 0 based Length less 2 x Check sum bytes

                Dim bytesForCRC(commandLength) As Byte

                Dim myCRC_16 As New CRC_16()

                ' Determine Phase angle
                ' RadioButton()
                Dim iPhase As Integer = 1

                If rbAnyPhase.Checked Then
                    iPhase = 0
                End If
                If rbPositive.Checked Then
                    iPhase = 1
                End If
                If rbNegative.Checked Then
                    iPhase = 2
                End If
                If rbRamp.Checked Then
                    iPhase = 3
                End If


                bytesForCRC(0) = 85         '** Header A
                bytesForCRC(1) = 170        '** Header B
                bytesForCRC(2) = &HD  '** Command length with 2 x checksum (13)
                bytesForCRC(3) = &H0   '** Device address 0 for tester
                bytesForCRC(4) = &H80       '** Command instruction - Commence test 
                '****************** Above is standard
                Dim shtCurrent As Single = (Val(TxtSetCurrent.Text))
                Dim shtMulti As Single = (Val(TxtMultiplier.Text))

                '*** 12/10/2020
                '** Relay check mode
                '*** If just testing relay operation reduce trip current
                If B_RelayCheck Then
                    shtCurrent = 3  'mA
                End If

                '*** 12/06/2024 - Always microamp
                '  If UseMicroAmp Then
                '****
                '**** 18/03/2020  Disable override for testing
                '   shtMulti = 1
                shtCurrent = shtCurrent / shtMulti
                '****
                shtCurrent *= 10
                ' End If


                ''** Also standard MC8 with fuse option
                'If SMasterType = "RTMC8" And Val(LblChannel.Text) < 8 And Chk_FuseMaster.Checked Then
                '    '****
                '    '**** 18/03/2020  Disable override for testing
                '    shtMulti = 1
                '    '****
                'End If


                '****
                '**** 18/03/2020  Disable override for testing
                If shtMulti = 1 Then
                    LblScaleInUse.Text = "(1)"
                Else
                    LblScaleInUse.Text = "(" & TxtMultiplier.Text & ")"
                End If
                'Added to Update display
                'LblScaleInUse.Text = "(" & TxtMultiplier.Text & ")"
                '****

                Dim shtTestCurrent As Integer = shtCurrent ' Math.Round((shtCurrent / shtMulti), 0, MidpointRounding.AwayFromZero) 
                ' Commented out for programmin units '
                ' Dim shtTestCurrent As Integer = Math.Round((shtCurrent / shtMulti), 0, MidpointRounding.AwayFromZero) 

                Debug.Print("Value to send to tester = " & shtTestCurrent.ToString)
                ' If shtMulti < 1 And shtTestCurrent > 150 Then shtTestCurrent = 150
                Dim IntTimeout As Integer

                '*** Override Test Current and time out if applicable
                '*** This is so the remaining RCD's (> channel 1) are not tripped just checked for power


                Dim hexS As String = shtTestCurrent.ToString("X4")

                Dim Sbyte7 As String = Strings.Right(hexS, 2)
                Dim Sbyte8 As String = Strings.Left(hexS, 2)

                '** Update label showing actual value sent including any modifications due to multiplier
                TxtCurrentSent.Text = "(" & shtTestCurrent.ToString & " " & Sbyte7.ToString & ":" & Sbyte8.ToString & ")"

                'Apply timeout

                If IsNumeric(TxtTimeout.Text) Then
                    IntTimeout = Val(TxtTimeout.Text)
                    Select Case IntTimeout
                        Case < 1
                            IntTimeout = 500
                        Case > 500
                            IntTimeout = 500
                    End Select
                Else
                    IntTimeout = 500
                End If
                '*** If just testing relay operation reduce time to wait
                If B_RelayCheck Then
                    IntTimeout = 30
                End If


                '***
                Dim hexTimeout As String = IntTimeout.ToString("X4")

                Dim Sbyte9 As String = Strings.Right(hexTimeout, 2)
                Dim Sbyte10 As String = Strings.Left(hexTimeout, 2)
                '***


                bytesForCRC(5) = RtUnitChannel '*** Use zero based unit channel
                bytesForCRC(6) = "&H" & iPhase     '** Command parameter - Test type 0, 1, 2 [any, positive, negative]
                bytesForCRC(7) = "&H" & Sbyte7 '(Val(TxtSetCurrent.Text)) '1F    '** Command parameter - Test current mA 0-150  - set to trip test RCD
                bytesForCRC(8) = "&H" & Sbyte8   '** Command parameter - Test current mA 0-150
                bytesForCRC(9) = "&H" & Sbyte9 '&HF4 '2C    '** Command parameter - Test max duration in ms
                bytesForCRC(10) = "&H" & Sbyte10 '&H1   '** Command parameter - Test max duration in ms   - 1F4 = 500ms

                '*********** START Checksum calculation
                Dim csum() As Byte = myCRC_16.ComputeChecksumBytes(bytesForCRC)
                '************ END Checksum calculation

                For x = 0 To commandLength
                    data2Send &= Chr(bytesForCRC(x))
                Next
                data2Send = data2Send & Chr(csum(0)) & Chr(csum(1))

                Dim data2SendFormatted As String = ""
                For x = 1 To Len(data2Send)
                    data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
                Next
                '*** Test Reply should be 55 aa 08 00 da 01 8c dd to 0xDA Read Test Bus Powered

                Debug.Print("Command: Commence Test 0x80 on channel " & (Hex(Val(LblChannel.Text)))) 'Chr("&H" & boardNumber.ToString)
                ' Connect("169.254.1.1", data2Send, 7)

                S_LogFileText = "_" & TxtMasterSSID.Text & "_" & TxtCIBid.Text
                iResult = Connect(sIP, data2Send, 7, S_LogFileText, TxtOrderRef.Text)
                Application.DoEvents()

                '*** Fetch Result
                data2Send = ""
                Dim bytesForCRC2(4) As Byte

                bytesForCRC2(0) = 85   '** Header A
                bytesForCRC2(1) = 170  '** Header B
                bytesForCRC2(2) = &H7  '** Command length with 2 x checksum
                bytesForCRC2(3) = &H0  '** Device address 0 for tester
                bytesForCRC2(4) = &H81 '** Command instruction - Get result

                '*********** START Checksum calculation
                csum = myCRC_16.ComputeChecksumBytes(bytesForCRC2)
                '************ END Checksum calculation

                For x = 0 To 4
                    data2Send += Chr(bytesForCRC2(x))
                Next
                data2Send = data2Send & Chr(csum(0)) & Chr(csum(1))

                data2SendFormatted = ""
                For x = 1 To Len(data2Send)
                    data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
                Next

                Debug.Print("Command: Fetch Result 0x81")

                S_LogFileText = "_" & TxtMasterSSID.Text & "_" & TxtCIBid.Text
                iResult = Connect(sIP, data2Send, 14, S_LogFileText, TxtOrderRef.Text)
                iResult = Trim(iResult)
                '   iResult = "-3"

            Else    '** number out of range

                iResult = "-2 " & iSelectedChannel.ToString & " " & iChannelLimit.ToString
            End If  '*** Channel number in range
        Else    '*** Not numeric
            iResult = "-1 not a number"
        End If '*** Numeric channel number
        Application.DoEvents()

        Return iResult
    End Function
    Private Sub BtnChUp_Click(sender As Object, e As EventArgs) Handles BtnChUp.Click
        If IntMaxChannel = 0 Then
            MsgBox("Please discover config.", vbOKOnly + vbExclamation, "System Not Ready")
        Else '** OK to go
            Select Case Val(LblChannel.Text)
                Case < IntMaxChannel
                    RtUnitChannel += 1
                    LblChannel.Text = (RtUnitChannel + 1).ToString
                Case Else
                    MsgBox("Cannot exceed maximum Channel.", vbOKOnly + vbExclamation, "Error")
            End Select
        End If
    End Sub

    Private Sub BtnChDn_Click(sender As Object, e As EventArgs) Handles BtnChDn.Click
        If IntMaxChannel = 0 Then
            MsgBox("Please discover config.", vbOKOnly + vbExclamation, "System Not Ready")
        Else '** OK to go
            Select Case Val(LblChannel.Text)
                Case > 1
                    RtUnitChannel -= 1
                    LblChannel.Text = (RtUnitChannel + 1).ToString
                Case Else
                    MsgBox("Channel cannot be less than 1.", vbOKOnly + vbExclamation, "Error")
            End Select
        End If
    End Sub
    Private Function ValidatePage() As Integer
        Dim IntValidation As Integer = 0
        If TxtUser.Text = "" Then IntValidation += 1
        If TxtUser.Text = "" Then IntValidation += 2
        If TxtPurchaser.Text = "" Then IntValidation += 4

        If Not ChkSkipMaster.Checked Then
            If TxtMasterSSID.Text = "" Then IntValidation += 8
            If TxtMasterManCode.Text = "" Then IntValidation += 16
        End If

        If Not ChkSkipCIB.Checked Then
            If TxtCIBid.Text = "" Then IntValidation += 32
            If TxtCIBManCode.Text = "" Then IntValidation += 64
        End If
        ValidatePage = IntValidation
    End Function
    Private Function TestStep() As Integer
        Select Case ICurrentStep
            Case 1  '*** Connect to WIFI
                '*** If skipping master, assume it is already connected
                If ChkSkipMaster.Checked And Strings.Left(SConnectedWiFi, 3) = "RT_" Then
                    If ChkAutoStep.Checked Then
                        ICurrentStep += 1
                        BtnNext.PerformClick()
                    Else
                        '*** Display Prompt
                        LblPrompt.Visible = True
                    End If
                Else
                    LblStatus.Text = "Connecting to Wi-Fi"
                    Application.DoEvents()
                    Debug.Print("In Step " & ICurrentStep.ToString)
                    '*** Update step Label
                    lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                    SaveData()  '*** Auto save current setup
                    Dim IntConWifi As Integer = 0
                    Dim IntConnectCount As Integer = 0 '** counter to exit while if not connecting
                    Dim IntConnectLimit As Integer = 8

                    '*** IF connecting to not RT_001
                    '*** Check if wifi is already connected
                    If ChkConnectToID.Checked And (SConnectedWiFi = "RT_" & Trim(TxtMasterSSID.Text)) Then
                        '*** No need to reconnect
                        LblStatus.Text = "Already connected"
                    Else
                        If SConnectedWiFi <> "RT_001" Then '*** Skip connection if already on RT_001
                            While IntConWifi = 0
                                IntConWifi = ConnectWiFi(False) '*** Connect to master - do not override checkbox
                                If IntConWifi = 1 Then Threading.Thread.Sleep(1000)
                                If Strings.Left(SConnectedWiFi, 3) = "RT_" Then
                                    Debug.Print("S01 - Connected Wifi " & SConnectedWiFi & " - " & IntConWifi)
                                    Application.DoEvents()
                                    Exit While
                                End If
                                IntConnectCount += 1
                                If IntConnectCount = IntConnectLimit Then Exit While
                            End While
                        End If
                    End If
                        Application.DoEvents()

                    If IntConnectCount = IntConnectLimit Then
                        MsgBox("Could not connect. " & IntConnectCount.ToString, vbCritical + vbOKOnly, "Connection issue.")
                    Else
                        Debug.Print("Wifi attempts " & IntConnectCount.ToString)

                        If ChkAutoStep.Checked Then
                            ICurrentStep += 1
                            ' Threading.Thread.Sleep(1000)
                            BtnNext.PerformClick()
                        Else
                            '*** Display Prompt
                            LblPrompt.Visible = True
                        End If
                    End If
                End If
            Case 2  '*** Get firmware version
                LblStatus.Text = "Getting Firmware Version"
                Application.DoEvents()
                '*** Hide Prompt
                LblPrompt.Visible = False
                Debug.Print("In Step " & ICurrentStep.ToString)
                '*** Update step Label
                lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)

                '*** Log activity
                StrTextToLog = "Get firmware version"
                Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                Dim debugFileNumber As Integer = FreeFile()
                FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                Dim debugTime As String = Format(Now(), "hh:mm:ss")
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                Dim StrResult As String = GetMasterVersion()

                '*** Log data
                debugTime = Format(Now(), "hh:mm:ss")
                StrTextToLog = StrResult
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                Dim Decoded As String = RT_Decode("92", StrResult)

                '*** Log data
                StrTextToLog = Decoded
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                FileClose(debugFileNumber)
                If ChkAutoStep.Checked Then
                    ICurrentStep += 1
                    BtnNext.PerformClick()
                Else
                    '*** Display Prompt
                    LblPrompt.Visible = True
                End If
                '*** 
            Case 3  '*** Discover topology (Config)
                LblStatus.Text = "Getting configuration"
                Application.DoEvents()
                '*** Hide Prompt
                LblPrompt.Visible = False
                Debug.Print("In Step " & ICurrentStep.ToString)
                '*** Update step Label
                lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                '*** Log activity
                StrTextToLog = "Discover Config"
                Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                Dim debugFileNumber As Integer = FreeFile()
                FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                Dim debugTime As String = Format(Now(), "hh:mm:ss")
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                Dim StrResult As String = ReadAllTopology()
                '*** Log data
                debugTime = Format(Now(), "hh:mm:ss")
                StrTextToLog = StrResult
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                Dim Decoded As String = RT_Decode("EA", StrResult)
                '*** Log data
                StrTextToLog = Decoded
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                FileClose(debugFileNumber)

                '*** Print all Labels
                If ChkAutoPrint.Checked Then BtnPrintEquipLbl.PerformClick()

                '*** 
                If ChkAutoStep.Checked Then
                    ICurrentStep += 1
                    BtnNext.PerformClick()
                Else
                    '*** Display Prompt
                    LblPrompt.Visible = True
                End If
            Case 4  '*** If not skipping master, read the current GUID
                LblStatus.Text = "Getting Master GUID"
                Application.DoEvents()
                Debug.Print("In Step " & ICurrentStep.ToString)
                '*** Update step Label
                lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                '*** Hide Prompt
                LblPrompt.Visible = False
                If ChkSkipMaster.Checked = False Then
                    '*** Log activity
                    StrTextToLog = "Get Master GUID"
                    Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                    Dim debugFileNumber As Integer = FreeFile()
                    FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                    Dim debugTime As String = Format(Now(), "hh:mm:ss")
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                    Dim StrResult As String = ReadGuid("0")

                    '*** Log data
                    debugTime = Format(Now(), "hh:mm:ss")
                    StrTextToLog = StrResult
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                    Dim Decoded As String = RT_Decode("BE", StrResult)
                    '  S_MasterGUID = Decoded
                    '*** Log data
                    StrTextToLog = Decoded
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                    FileClose(debugFileNumber)
                    '*** 
                    If ChkAutoStep.Checked Then
                        ICurrentStep += 1
                        BtnNext.PerformClick()
                    Else
                        '*** Display Prompt
                        LblPrompt.Visible = True
                    End If
                Else
                    '*** Skip next few steps as I don't have to program the master every time or trip the RCD
                    If ChkAutoStep.Checked Then
                        ICurrentStep += 5
                        BtnNext.PerformClick()
                    Else
                        '*** Display Prompt
                        LblPrompt.Visible = True
                    End If
                End If
            Case 5 '*** Update & compare GUIDS
                '*** Hide Prompt
                LblPrompt.Visible = False
                LblStatus.Text = "Update and compare GUIDS"

                '***** WRITE MASTER GUID
                Dim G As Guid
                Dim GuidString As String = g.ToString
                GuidString = Microsoft.VisualBasic.Left("MR5FMC8MasterModule_" & Trim(TxtMasterSSID.Text) & "_" & Format(Now(), "ddMMyyyy"), 36)

                '*** Log activity
                StrTextToLog = "Write Master GUID"
                Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                Dim debugFileNumber As Integer = FreeFile()
                FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                Dim debugTime As String = Format(Now(), "hh:mm:ss")
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                '*** Write GUID
                WriteGUID(GuidString, "0")
                StrTextToLog = "Writing " & guidString
                debugTime = Format(Now(), "hh:mm:ss")
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                Dim StrResult As String = ReadGuid("0")
                Dim Decoded As String = RT_Decode("BE", StrResult)
                S_MasterGUID = Decoded
                '*** Log data
                debugTime = Format(Now(), "hh:mm:ss")
                StrTextToLog = "Read back GUID " & S_MasterGUID
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                '*** 

                '**** COMPARE Before & After GUIDS
                If S_MasterGUID = guidString Then

                    LblStatus.Text = "GUIDS Match"
                    '*** Log data
                    debugTime = Format(Now(), "hh:mm:ss")
                    StrTextToLog = "GUIDS Match"
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                Else
                    LblStatus.Text = "GUIDS DO NOT Match"
                    '*** Log data
                    debugTime = Format(Now(), "hh:mm:ss")
                    StrTextToLog = "GUIDS DO NOT Match"
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                    Dim MsgResponse As Integer = MsgBox("GUIDs do not match!" & vbNewLine & "Do you wish to continue?", vbYesNo + vbQuestion, "Warning")
                    If MsgResponse = vbNo Then
                        ChkAutoStep.Checked = False
                    End If
                End If
                LblMguid.Text = S_MasterGUID
                LblMguid.Visible = True

                FileClose(debugFileNumber)
                '*** 
                If ChkAutoStep.Checked Then
                    ICurrentStep += 1
                    BtnNext.PerformClick()
                Else
                    '*** Display Prompt
                    LblPrompt.Visible = True
                End If

            Case 6 ' Set New Master SSID If required
                If ChkSkipMaster.Checked = False And Strings.Left(SConnectedWiFi, 3) = "RT_" Then
                    LblStatus.Text = "Setting Master SSID"
                    Application.DoEvents()

                    '*** Update step Label
                    lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                    '*** Hide Prompt
                    LblPrompt.Visible = False

                    '*** Log activity
                    StrTextToLog = "Set Master SSID"
                    Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                    Dim debugFileNumber As Integer = FreeFile()
                    FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                    Dim debugTime As String = Format(Now(), "hh:mm:ss")
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                    Dim StrResult As String = SetSSID()

                    '*** Log data
                    debugTime = Format(Now(), "hh:mm:ss")
                    StrTextToLog = StrResult
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                    Dim Decoded As String = RT_Decode("91", StrResult)
                    '*** Log data
                    StrTextToLog = Decoded
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                    FileClose(debugFileNumber)
                    MsgBox("When the master ID appears in the WiFi list," & vbNewLine & "Click OK to connect and continue.", vbOKOnly, "SSID Updated.")

                    '*******
                    LblStatus.Text = "Connecting to New ID"
                    Application.DoEvents()

                    '*** CONNECT TO NEW SSID - Start *****

                    Dim IntConWifi As Integer = 0
                    Dim IntConnectCount As Integer = 0 '** counter to exit while if not connecting
                    Dim IntConnectLimit As Integer = 8
                    While IntConWifi = 0
                        IntConWifi = ConnectWiFi(True) '*** Ignore 001 and connect to new master 
                        If IntConWifi = 1 Then Threading.Thread.Sleep(2000)
                        If Strings.Left(SConnectedWiFi, 3) = "RT_" Then
                            Debug.Print("S01 - Connected Wifi " & SConnectedWiFi & " - " & IntConWifi)
                            Application.DoEvents()
                            Exit While
                        End If
                        IntConnectCount += 1
                        If IntConnectCount = IntConnectLimit Then Exit While
                    End While
                    Application.DoEvents()

                    If IntConnectCount = IntConnectLimit Then
                        MsgBox("Could not connect. " & IntConnectCount.ToString, vbCritical + vbOKOnly, "Connection issue.")
                    End If
                    '*** CONNECT TO NEW SSID - Finish
                    Threading.Thread.Sleep(2200)
                End If
                '*** 
                If ChkAutoStep.Checked Then
                    ICurrentStep += 1
                    BtnNext.PerformClick()
                Else
                    '*** Display Prompt
                    LblPrompt.Visible = True
                End If

            Case 7 '*** Trip Master RCD
                LblStatus.Text = "Tripping MC8 RCDs"
                Application.DoEvents()

                '*** Update step Label
                lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                '*** Hide Prompt
                LblPrompt.Visible = False

                '*** Log activity
                StrTextToLog = "Tripping MC8 RCDs"
                Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                Dim debugFileNumber As Integer = FreeFile()
                FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                Dim debugTime As String = Format(Now(), "hh:mm:ss")
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                FileClose(debugFileNumber)

                '*** Update step Label
                lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                '*** Hide Prompt
                LblPrompt.Visible = False
                LblChannel.Text = "1"
                Dim StrResult As String = TripRCD()
                IntMasterTestsDone += 1

                Dim Decoded As String = RT_Decode("80", StrResult)
                Dim IntContinueTrip As Integer = 0

                Select Case Val(StrResult)
                    Case -1 '*** Error in command
                        PicStatM1.Image = My.Resources.StatusRed
                        Application.DoEvents()
                        StrTextToLog += "No trip on ch 1"
                        IntContinueTrip = MsgBox("Error on operation tripping Master Channel 1." & vbNewLine & "Cannot Continue.", vbOKOnly + vbCritical, "ERROR")
                        ToolTip1.SetToolTip(PicStatM1, "Fail - Error in command response")

                        '*** Clear auto step to stop process
                        ChkAutoStep.Checked = False

                    Case 0  '*** RCD did not trip
                        PicStatM1.Image = My.Resources.StatusRed
                        Application.DoEvents()
                        StrTextToLog += "No trip on ch 1"
                        IntContinueTrip = MsgBox("RCD did not trip on channel 1." & vbNewLine & "Do you wish to continue. Y/N", vbYesNo + vbQuestion, "Question")
                        ToolTip1.SetToolTip(PicStatM1, "Fail - RCD did NOT trip")
                    Case 1  '*** RCD did trip
                        PicStatM1.Image = My.Resources.StatusGreen
                        Application.DoEvents()
                        StrTextToLog += "Trip on ch 1"
                        '  IntContinueTrip = MsgBox("RCD tripped on channel 1." & vbNewLine & "Do you wish to continue. Y/N", vbYesNo + vbQuestion, "Question")
                        ToolTip1.SetToolTip(PicStatM1, "PASS - RCD tripped")
                End Select

                If IntContinueTrip = 6 Then
                    B_RelayCheck = True
                    While RtUnitChannel < 7
                        IntMasterTestsDone += 1
                        RtUnitChannel += 1 '*** Use zero based unit channel
                        Dim Schannel As String = (RtUnitChannel + 1).ToString
                        LblChannel.Text = Schannel
                        Application.DoEvents()
                        StrResult = TripRCD()
                        For Each cntrl As Control In GrpMaster.Controls
                            'Debug.Print(cntrl.Name)

                            If Strings.Left(cntrl.Name, 9) = "PicStatM" & Schannel Then
                                Dim abc As PictureBox = CType(cntrl, PictureBox)
                                With abc
                                    Select Case Trim(StrResult)
                                        Case -1 '*** Error in command
                                            .Image = My.Resources.StatusRed
                                            Application.DoEvents()
                                            ToolTip1.SetToolTip(abc, "Error in command response.")
                                            IntContinueTrip = MsgBox("Error on operation tripping Master Channel " & Schannel & vbNewLine & "Cannot Continue.", vbOKOnly + vbCritical, "ERROR")

                                            '*** Clear auto step to stop process and exit loop
                                            ChkAutoStep.Checked = False
                                            Exit While
                                        Case "3"
                                            .Image = My.Resources.StatusBlue
                                            ToolTip1.SetToolTip(abc, "OK - Relay Test")
                                        Case "4" ' Relay Test mode
                                            .Image = My.Resources.StatusYellow
                                            ToolTip1.SetToolTip(abc, "FAIL - False Trip")
                                        Case Else
                                            .Image = My.Resources.StatusRed
                                            ToolTip1.SetToolTip(abc, "Unknown status")
                                    End Select

                                End With
                            End If
                        Next
                    End While
                End If
                B_RelayCheck = False

                '*** Log data
                debugFileNumber = FreeFile()
                FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                debugTime = Format(Now(), "hh:mm:ss")
                StrTextToLog = "Result from trip " & StrResult
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                FileClose(debugFileNumber)

                '*** 
                If ChkAutoStep.Checked Then
                    ICurrentStep += 1
                    BtnNext.PerformClick()
                Else
                    '*** Display Prompt
                    LblPrompt.Visible = True
                End If

            Case 8  '*** Update database for Master
                LblStatus.Text = "Update database for Master"
                Application.DoEvents()

                '*** Update step Label
                lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                '*** Hide Prompt
                LblPrompt.Visible = False

                '*** Log activity
                StrTextToLog = "Update database for Master"
                Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                Dim debugFileNumber As Integer = FreeFile()
                FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                Dim debugTime As String = Format(Now(), "hh:mm:ss")
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                Dim TmpType As String = "MC8F5"
                Select Case IntMasterType
                    Case 1
                        TmpType = "MC8F5"
                    Case 2
                        TmpType = "MC8"
                End Select
                StrTextToLog = "Writing to NDB - Master info"
                WriteNDB(S_MasterGUID, Trim(TxtMasterSSID.Text), TmpType, IntMasterTestsDone, Trim(TxtMasterManCode.Text))
                '*** Log data
                StrTextToLog = S_MasterGUID & " " & Trim(TxtMasterSSID.Text) & " " & TmpType & " " & IntMasterTestsDone & " " & Trim(TxtMasterManCode.Text)
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                FileClose(debugFileNumber)

                '*** 
                If ChkAutoStep.Checked Then
                    ICurrentStep += 1
                    BtnNext.PerformClick()
                Else
                    '*** Display Prompt
                    LblPrompt.Visible = True
                End If
            Case 9 '*** Commence Channel board
                If ChkSkipCIB.Checked Then

                    '*** 
                    If ChkAutoStep.Checked Then
                        ICurrentStep = 11
                        BtnNext.PerformClick()
                    Else
                        ICurrentStep = 11
                        '*** Display Prompt
                        LblPrompt.Visible = True
                    End If
                Else    '*** TEST CIB

                    LblStatus.Text = "Commence CIB"
                    Application.DoEvents()

                    '*** Update step Label
                    lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                    '*** Hide Prompt
                    LblPrompt.Visible = False
                    IntCIBTestsDone = 0
                    '*** Update & compare GUIDS
                    '*** Hide Prompt
                    LblPrompt.Visible = False
                    LblStatus.Text = "Update and compare CIB GUIDS"

                    '***** WRITE CIB GUID
                    Dim G As Guid
                    Dim GuidString As String = G.ToString
                    GuidString = Microsoft.VisualBasic.Left("MRF_MECHANICAL_RELAY_" & Trim(TxtCIBid.Text) & "_" & Format(Now(), "ddMMyyyy"), 36)

                    '*** Log activity
                    StrTextToLog = "Write CIB GUID"
                    Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                    Dim debugFileNumber As Integer = FreeFile()
                    FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                    Dim debugTime As String = Format(Now(), "hh:mm:ss")
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                    '*** Write GUID
                    WriteGUID(GuidString, "1")
                    StrTextToLog = "Writing " & GuidString
                    debugTime = Format(Now(), "hh:mm:ss")
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

                    Dim StrResult As String = ReadGuid("1")
                    Dim Decoded As String = RT_Decode("BE", StrResult)
                    sCB_OriginalGUID = Decoded
                    '*** Log data
                    debugTime = Format(Now(), "hh:mm:ss")
                    StrTextToLog = "Read back GUID " & sCB_OriginalGUID
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                    '*** 

                    '**** COMPARE Before & After GUIDS
                    If sCB_OriginalGUID = GuidString Then

                        LblStatus.Text = "GUIDS Match"
                        '*** Log data
                        debugTime = Format(Now(), "hh:mm:ss")
                        StrTextToLog = "GUIDS Match"
                        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                    Else
                        LblStatus.Text = "GUIDS DO NOT Match"
                        '*** Log data
                        debugTime = Format(Now(), "hh:mm:ss")
                        StrTextToLog = "GUIDS DO NOT Match"
                        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                        Dim MsgResponse As Integer = MsgBox("GUIDs do not match!" & vbNewLine & "Do you wish to continue?", vbYesNo + vbQuestion, "Warning")
                        If MsgResponse = vbNo Then
                            ChkAutoStep.Checked = False
                        End If
                    End If
                    sNewGUID = GuidString
                    LblCIBguid.Text = sCB_OriginalGUID
                    LblCIBguid.Visible = True

                    FileClose(debugFileNumber)

                    '********* Check if set to only update GUID
                    '*** If not trip channels
                    '   If ChkCIBidOnly.Checked = False Then    '*** Test channels also '*** Trip RCD

                    LblStatus.Text = "Tripping CIB RCDs"
                    Application.DoEvents()

                    '*** Update step Label
                    lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                    '*** Hide Prompt
                    LblPrompt.Visible = False

                    '*** Log activity
                    StrTextToLog = "Tripping CIB RCDs"
                    debugFileNumber = FreeFile()
                    FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                    debugTime = Format(Now(), "hh:mm:ss")
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                    FileClose(debugFileNumber)

                    '*** Update step Label
                    lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
                    '*** Hide Prompt
                    LblPrompt.Visible = False
                    LblChannel.Text = "9"
                    RtUnitChannel = 8   '0 based channel = first on CIB
                    StrResult = TripRCD()
                    Debug.Print("Picture channel " & (RtUnitChannel - 7).ToString & " Testing real channel " & RtUnitChannel.ToString)
                    IntCIBTestsDone += 1

                    Decoded = RT_Decode("80", StrResult)
                    Dim IntContinueTrip As Integer = 0

                    Select Case Val(StrResult)
                        Case -1 '*** Error in command
                            PicStatCIB01.Image = My.Resources.StatusRed
                            Application.DoEvents()
                            StrTextToLog += "No trip on ch 9 [1]"
                            IntContinueTrip = MsgBox("Error on operation tripping CIB Channel 1." & vbNewLine & "Cannot Continue.", vbOKOnly + vbCritical, "ERROR")
                            ToolTip1.SetToolTip(PicStatCIB01, "Fail - Error in command response")

                            '*** Clear auto step to stop process
                            ChkAutoStep.Checked = False

                        Case 0  '*** RCD did not trip
                            PicStatCIB01.Image = My.Resources.StatusRed
                            Application.DoEvents()
                            StrTextToLog += "No trip on ch 9 [1]"
                            IntContinueTrip = MsgBox("RCD did not trip on channel 1." & vbNewLine & "Do you wish to continue. Y/N", vbYesNo + vbQuestion, "Question")
                            ToolTip1.SetToolTip(PicStatCIB01, "Fail - RCD did NOT trip")
                        Case 1  '*** RCD did trip
                            PicStatCIB01.Image = My.Resources.StatusGreen
                            Application.DoEvents()
                            StrTextToLog += "Trip on ch 9 [1]"
                            IntContinueTrip = MsgBox("RCD tripped on channel 1." & vbNewLine & "Do you wish to continue. Y/N", vbYesNo + vbQuestion, "Question")
                            ToolTip1.SetToolTip(PicStatCIB01, "PASS - RCD tripped")
                    End Select

                    If ChkCIBidOnly.Checked = False Then    '*** If Not ID Only test all channels
                        If IntContinueTrip = 6 Then
                            B_RelayCheck = True
                            While RtUnitChannel <= 30
                                IntCIBTestsDone += 1
                                RtUnitChannel += 1 '*** Use zero based unit channel
                                Dim Schannel As String = (RtUnitChannel + 1).ToString
                                Dim StrPicChannel As String = (RtUnitChannel - 7).ToString
                                LblChannel.Text = Schannel
                                Application.DoEvents()
                                StrResult = TripRCD()
                                If Strings.Len(StrPicChannel) = 1 Then StrPicChannel = "0" & StrPicChannel
                                Debug.Print("Picture channel " & StrPicChannel & " Testing real channel " & RtUnitChannel.ToString)
                                For Each cntrl As Control In GrpCIB.Controls
                                    If Strings.Left(cntrl.Name, 12) = "PicStatCIB" & StrPicChannel Then
                                        Dim abc As PictureBox = CType(cntrl, PictureBox)
                                        With abc
                                            Select Case Trim(StrResult)
                                                Case -1 '*** Error in command
                                                    .Image = My.Resources.StatusRed
                                                    Application.DoEvents()
                                                    ToolTip1.SetToolTip(abc, "Error in command response.")
                                                    IntContinueTrip = MsgBox("Error on operation tripping CIB Channel " & Schannel & vbNewLine & "Cannot Continue.", vbOKOnly + vbCritical, "ERROR")

                                                '*** Clear auto step to stop process and exit loop
                                                'ChkAutoStep.Checked = False
                                                'Exit While
                                                Case "3"
                                                    .Image = My.Resources.StatusBlue
                                                    ToolTip1.SetToolTip(abc, "OK - Relay Test")
                                                Case "4" ' Relay Test mode
                                                    .Image = My.Resources.StatusYellow
                                                    ToolTip1.SetToolTip(abc, "FAIL - False Trip")
                                                Case Else
                                                    .Image = My.Resources.StatusRed
                                                    ToolTip1.SetToolTip(abc, "Unknown status")
                                            End Select

                                        End With
                                    End If
                                Next
                            End While
                        End If
                    End If
                    B_RelayCheck = False

                    '*** Log data
                    debugFileNumber = FreeFile()
                    FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                    debugTime = Format(Now(), "hh:mm:ss")
                    StrTextToLog = "Result from trip " & StrResult
                    PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                    FileClose(debugFileNumber)

                    '   End If
                    '*** 
                    If ChkAutoStep.Checked Then
                        ICurrentStep += 1
                        BtnNext.PerformClick()
                    Else
                        '*** Display Prompt 
                        LblPrompt.Visible = True
                    End If

                End If '*** END CIB

            Case 10 '*** Update database for CIB
                LblStatus.Text = "Update database for CIB"
                Application.DoEvents()

                '*** Update step Label
                lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)

                Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
                Dim debugFileNumber As Integer = FreeFile()
                FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
                Dim debugTime As String = Format(Now(), "hh:mm:ss")
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                Dim TmpType As String = "CIBF"

                StrTextToLog = "Writing to NDB - CIB info"
                WriteNDB(sNewGUID, Trim(TxtCIBid.Text), TmpType, IntCIBTestsDone, Trim(TxtCIBManCode.Text))
                '*** Log data
                StrTextToLog = S_MasterGUID & " " & Trim(TxtMasterSSID.Text) & " " & TmpType & " " & IntCIBTestsDone & " " & Trim(TxtMasterManCode.Text)
                PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
                FileClose(debugFileNumber)
                '*** 
                If ChkAutoStep.Checked Then
                    ICurrentStep += 1
                    BtnNext.PerformClick()
                Else
                    '*** Display Prompt
                    LblPrompt.Visible = True
                End If

            Case 11 '*** Process completed
                '*** Update step Label
                lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)

                LblStatus.Text = "Finished"
                If ChkSound.Checked Then
                    If ChkAutoStep.Checked Then My.Computer.Audio.Play(My.Resources.RT_End, AudioPlayMode.Background)
                End If


        End Select
        Debug.Print("Finished " & ICurrentStep.ToString)
        TestStep = 1
    End Function
    Private Sub WriteGUID(guidString As String, device As String)
        '*** Make sure it is disconnected to start with
        MakeConnection(sIP, False, iPort)

        '*** Enable connection and disable manual connection button
        MakeConnection(sIP, True, iPort)

        '*** Write the GUID
        Dim gChar As Char = Microsoft.VisualBasic.Left(guidString, 1)
        Debug.Print(Hex(Asc(gChar)))
        Dim guidArray(40)
        For x = 1 To 40
            guidArray(x) = "0"
        Next

        '*** Use manual GUID if present
        If Len(TxtGUID.Text) = 36 Then
            For x = 1 To Len(TxtGUID.Text)
                guidArray(x) = Microsoft.VisualBasic.Mid(TxtGUID.Text, x, 1)
                Debug.Print(" Manual GUID " & guidArray(x) & " - " & Asc(guidArray(x)) & " - " & " &H" & Hex(Asc(guidArray(x))))
            Next
        Else
            For x = 1 To Len(guidString)
                guidArray(x) = Microsoft.VisualBasic.Mid(guidString, x, 1)
                Debug.Print(" New GUID " & guidArray(x) & " - " & Asc(guidArray(x)) & " - " & " &H" & Hex(Asc(guidArray(x))))
            Next
        End If

        '***********************
        '*** Set GUID
        Dim data2Send As String = ""
        Dim commandLength As Integer = 44   '*** 0 based Length less 2 x Check sum bytes

        Dim bytesForCRC(commandLength) As Byte

        Dim myCRC_16 As New CRC_16()
        '********** Command bytes
        bytesForCRC(0) = 85         '** Header A
        bytesForCRC(1) = 170        '** Header B
        bytesForCRC(2) = &H2F  '** Command length with 2 x checksum
        bytesForCRC(3) = "&H" & Trim(device)   '** Device address 0 for tester
        bytesForCRC(4) = &HBD  '** Command instruction - Set GUID
        Dim guidIndex As Integer = 1
        bytesForCRC(5) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(6) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(7) = Asc(guidArray(guidIndex)) '** GUID
        guidIndex += 1
        bytesForCRC(8) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(9) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(10) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(11) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(12) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(13) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(14) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(15) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(16) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(17) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(18) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(19) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(20) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(21) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(22) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(23) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(24) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(25) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(26) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(27) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(28) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(29) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(30) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(31) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(32) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(33) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(34) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(35) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(36) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(37) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(38) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(39) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(40) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(41) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(42) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(43) = Asc(guidArray(guidIndex))  '** GUID
        guidIndex += 1
        bytesForCRC(44) = Asc(guidArray(guidIndex))  '** GUID


        '*********** START Checksum calculation
        Dim csum() As Byte = myCRC_16.ComputeChecksumBytes(bytesForCRC)
        '************ END Checksum calculation

        For x = 0 To commandLength
            data2Send &= Chr(bytesForCRC(x))
        Next
        data2Send = data2Send & Chr(csum(0)) & Chr(csum(1))

        Dim data2SendFormatted As String = ""
        For x = 1 To Len(data2Send)
            data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
        Next

        Debug.Print("Command:Set GUID 0xBD")
        TxtDebug.Text = "Command:Set GUID 0xBD >" & data2SendFormatted & "< " & Len(data2Send).ToString

        S_LogFileText = "_" & TxtMasterSSID.Text & "_" & TxtCIBid.Text
        Connect(sIP, data2Send, 7, S_LogFileText, TxtOrderRef.Text)

        '*** Close connection
        '*** Disconnect and re-enable manual connect button.
        MakeConnection(sIP, False, iPort)
    End Sub
    Private Sub BtnNext_Click(sender As Object, e As EventArgs) Handles BtnNext.Click
        Dim IntValidation As Integer = ValidatePage()
        '*** Check required fields are filled out
        If IntValidation > 0 Then
            MsgBox("Please complete all required fields. [" & IntValidation.ToString & "]", vbCritical + vbOKOnly, "Computer says NO")
        Else
            Dim IntTestStep As Integer = TestStep()

        End If

    End Sub
    Private Sub WriteNDB(S_GUID As String, S_ID As String, type As String, I_channels As Integer, S_ManCode As String)

        '*** This section writes configuration details for the masters and channel boards into the NDB using a OneBit DLL
        '*** This function is used by the automatic NDB update used during programming of modules.

        Dim Configuration As New BoardConfiguration()

        With Configuration
            .EnableLinkConfirmed = True
            .ModuleGUID = Trim(S_GUID)              '** String containg SS detals
            .ModuleID = Trim(S_ID)                  '** ID like MC500083 or C15054
            .ModuleType = type                      '** Device type Master, CIB, MC8, MC8SS & CIBSS
            .NumberOfChannelsTested = I_channels    '** Number of channels 8 or 24
            .QATestedDate = Now()                   '** Current Date & Time
            .QATestedBy = TxtUser.Text             '** Name of person programming units
            .Purchaser = TxtPurchaser.Text         '** Company purchasing device
            .OrderNumber = TxtOrderRef.Text         '** PO Number
            .ManufacturerCode = S_ManCode   '** Manufacturing code - Serial Number, model & batch date
        End With

        Try
            Dim configurationSaver As New ConfigurationSaver(connectionString)
            configurationSaver.SaveBoardConfiguration(Configuration)

        Catch ex As Exception
            MsgBox("Take screen shot for Kevin" & vbCrLf & "Module WriteNDB()" & vbCrLf & ex.ToString, vbOKOnly, "Error updating remote database.")
            Debug.Print("ERROR " & ex.ToString)
        End Try

    End Sub
    Private Sub BtnRestart_Click(sender As Object, e As EventArgs) Handles BtnRestart.Click
        ICurrentStep = 1
        LblType.Text = "Type:"
        LblVersion.Text = "Version: "
        LblStatus.Text = "Ready"
        IntMasterType = 0
        IntMasterTestsDone = 0
        LblMguid.Visible = False
        LblCIBguid.Visible = False
        RtUnitChannel = 0
        ClearResults()
        '*** Update step Label
        lblCurrentStep.Text = String.Format("Step {0} of {1}", ICurrentStep, IntMaxStep)
    End Sub
    Private Sub ClearResults()

        For Each cntrl As Control In Me.GrpMaster.Controls
            'Debug.Print(cntrl.Name)
            If Strings.Left(cntrl.Name, 8) = "PicStatM" Then
                Dim abc As PictureBox = CType(cntrl, PictureBox)
                With abc
                    .Image = My.Resources.StatusGrey
                    ToolTip1.SetToolTip(abc, "Not Tested")
                End With

            End If

        Next
        For Each cntrl As Control In Me.GrpCIB.Controls
            'Debug.Print(cntrl.Name)
            If Strings.Left(cntrl.Name, 8) = "PicStatC" Then
                Dim abc As PictureBox = CType(cntrl, PictureBox)
                With abc
                    .Image = My.Resources.StatusGrey
                    ToolTip1.SetToolTip(abc, "Not Tested")
                End With

            End If

        Next
    End Sub
    Private Function SetSSID() As String
        '** Check SSID and password are valid
        '**
        If ((TxtMasterSSID.Text.Length > 5) Or (TxtMasterSSID.Text = "001")) Then
            '*** Make sure it is disconnected to start with
            MakeConnection(sIP, False, iPort)

            '** Clear debug window
            Me.TxtDebug.Text = ""

            '*** Enable connection and disable manual connection button
            MakeConnection(sIP, True, iPort)

            '*** Set Wifi SSID and Passphrase
            Dim data2Send As String = ""
            Dim commandLength As Integer = 88   '*** 0 based Length less 2 x Check sum bytes 16/12/2015

            Dim bytesForCRC(commandLength) As Byte
            For x = 0 To commandLength
                bytesForCRC(x) = Val(Chr(0))

            Next
            Dim myCRC_16 As New CRC_16()
            '********** Command bytes
            bytesForCRC(0) = 85    '** Header A
            bytesForCRC(1) = 170   '** Header B
            bytesForCRC(2) = &H5B   '** Command length with 2 x checksum 91 NOT 0 Based
            bytesForCRC(3) = &H0   '** Device address 0 for tester
            bytesForCRC(4) = &H91  '** Command instruction - Set mapping

            '********** Data
            '*** First 20 byts are SSID
            '*** unused bytes must be filled with NULL
            Dim sSSID As String = Microsoft.VisualBasic.Left(TxtMasterSSID.Text, 17)

            '*** Clear any previous data
            Dim CharCount As Integer = 0

            For CharCount = 8 To 24
                bytesForCRC(CharCount) = Asc(vbNullChar)
            Next
            '*** Fix first 3 to RT_
            bytesForCRC(5) = Asc("R")
            bytesForCRC(6) = Asc("T")
            bytesForCRC(7) = Asc("_")

            '  CharCount = 0
            For CharCount = 1 To Len(sSSID)
                bytesForCRC(CharCount + 7) = Asc(Microsoft.VisualBasic.Mid(sSSID, CharCount, 1))
            Next

            '*** Second 64 bytes are password
            Dim sPassPhrase As String = Microsoft.VisualBasic.Left(StrRTpassword, 64)

            '' CharCount = 0
            'For CharCount = 25 To 88
            '    bytesForCRC(CharCount) = Asc(vbNullChar)
            'Next

            'CharCount = 0
            For CharCount = 1 To Len(sPassPhrase)
                bytesForCRC(CharCount + 24) = Asc(Microsoft.VisualBasic.Mid(sPassPhrase, CharCount, 1))
            Next

            '*********** START Checksum calculation
            Dim csum() As Byte = myCRC_16.ComputeChecksumBytes(bytesForCRC)
            '************ END Checksum calculation

            For x = 0 To commandLength
                data2Send &= Chr(bytesForCRC(x))
            Next
            data2Send = data2Send & Chr(csum(0)) & Chr(csum(1))

            Dim data2SendFormatted As String = ""
            For x = 1 To Len(data2Send)
                data2SendFormatted = data2SendFormatted & " " & Hex(Asc(Mid(data2Send, x, 1)))
            Next

            Debug.Print("Command:Set WiFi Settings " & data2SendFormatted.ToString)

            S_LogFileText = "_" & TxtMasterSSID.Text & "_" & TxtCIBid.Text
            Dim StrResult As String = Connect(sIP, data2Send, 10, S_LogFileText, TxtOrderRef.Text)

            '*** allow app to update graphics
            Application.DoEvents()

            '*** Disconnect and re-enable manual connect button.
            MakeConnection(sIP, False, iPort)

            '************************************
            '*** Return result for decoding
            SetSSID = "Data " & StrResult
            '************************************
        Else
            Dim iresult As Integer = MsgBox("Please enter a valid SSID & Password", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Wi-Fi Settings Error")
        End If
    End Function

    Private Sub BtnSetMasterID_Click(sender As Object, e As EventArgs) Handles BtnSetMasterID.Click
        BtnSetMasterID.Enabled = False

        '*** Log activity
        StrTextToLog = "Set Master GUID"
        Dim debugFilename As String = StrLogPath & "Debug " & Format(Now(), "yyyyMMdd") & "_" & Trim(TxtOrderRef.Text) & "_" & TxtMasterSSID.Text & ".log"
        Dim debugFileNumber As Integer = FreeFile()
        FileOpen(debugFileNumber, debugFilename, OpenMode.Append)
        Dim debugTime As String = Format(Now(), "hh:mm:ss")
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim StrResultVal As String = SetSSID()

        Debug.Print("SetSSID result in Button " & StrResultVal)
        '*** Log data
        debugTime = Format(Now(), "hh:mm:ss")
        StrTextToLog = StrResultVal
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)

        Dim Decoded As String = RT_Decode("91", StrResultVal)
        '*** Log data
        StrTextToLog = Decoded
        PrintLine(debugFileNumber, debugTime & " " & StrTextToLog)
        FileClose(debugFileNumber)

        BtnSetMasterID.Enabled = True

    End Sub

    Private Sub ChkProdDev_CheckedChanged(sender As Object, e As EventArgs) Handles ChkProdDev.CheckedChanged
        '*** Set to value as required 
        Dim ServerToUse As Integer
        Select Case ChkProdDev.Checked
            Case True
                ServerToUse = 2
                ChkProdDev.Image = My.Resources.sliderProd
            Case Else
                ServerToUse = 1
                ChkProdDev.Image = My.Resources.sliderDev
        End Select

        Select Case ServerToUse
            Case 1  'Azure DEV
                connectionString = "Server=tcp:ats-dev.database.windows.net,1433;Initial Catalog=RapidTest;Persist Security Info=False;User ID=onebit;Password=1qaz@WSX;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
                Lbl_Server.ForeColor = Color.Red
                Lbl_Server.Text = "Azure DEV"
                Lbl_Server.Visible = True
                Me.Text = "RTP - AZURE DEVELOPMENT Version"
            Case 2  'Azure Production
                connectionString = "Server=tcp:ats-prod.database.windows.net,1433;Initial Catalog=RapidTest;Persist Security Info=False;User ID=atsadmin;Password=pass@word1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
                Lbl_Server.ForeColor = Color.Blue
                Lbl_Server.Text = "Azure Production"
                Lbl_Server.Visible = True
                Me.Text = "RTP - AZURE Version"
        End Select
    End Sub

    Private Sub BtnWriteMGuid_Click(sender As Object, e As EventArgs) Handles BtnWriteMGuid.Click
        '***** WRITE MASTER GUID
        Dim G As Guid
        Dim GuidString As String = G.ToString
        GuidString = Microsoft.VisualBasic.Left("MR5FMC8MasterModule_" & Trim(TxtMasterSSID.Text) & "_" & Format(Now(), "ddMMyyyy"), 36)
        '*** Write GUID
        WriteGUID(GuidString, "0")
    End Sub

    Private Sub BtnTestAudio_Click(sender As Object, e As EventArgs) Handles BtnTestAudio.Click
        My.Computer.Audio.Play(My.Resources.RT_End, AudioPlayMode.Background)
    End Sub

    Private Sub BtnSummary_Click(sender As Object, e As EventArgs) Handles BtnSummary.Click
        Dim DateStart As String = Format(DateSummary.Value, "yyyy-MM-dd")
        Dim DateFinish As String = Format(DateSummary.Value.AddDays(1), "yyyy-MM-dd")
        Dim StrWhereClause As String = ""

        Select Case ChkSummary.Checked
            Case True 'PO Search
                If Trim(TxtSummary.Text) = "" Then
                    MsgBox("Please enter a PO Number to search for.", vbOKOnly + vbCritical, "No PO.")
                    Exit Sub
                End If
                StrWhereClause = " WHERE dbo.NDB_Board.BRD_OrderNumber Like '" & Trim(TxtSummary.Text) & "%'"
            Case False 'Date Search
                StrWhereClause = " WHERE (BRD_QATestedDate >= CONVERT(DATETIME, '" & DateStart & "', 102)) AND (BRD_QATestedDate < CONVERT(DATETIME, '" & DateFinish & "', 102))"
        End Select

        Dim NDB_DB As New SqlClient.SqlConnection(connectionString)

        NDB_DB.Open()

        Dim tmpSQLClause As String = ""
        tmpSQLClause = "SELECT TOP (2000) dbo.NDB_Board.BRD_ID, dbo.NDB_Board.BRD_ModuleID, dbo.NDB_Board.BRD_ModuleGUID, dbo.NDB_Board.BRD_ModuleType_ID, dbo.NDB_Board.BRD_QATestedDate, dbo.NDB_Board.BRD_QATestedBy, 
                         dbo.NDB_Board.BRD_NumberOfChannelsTested, dbo.NDB_Board.BRD_EnableLinkConfirmed, dbo.NDB_Board.BRD_Purchaser, dbo.NDB_Board.BRD_OrderNumber, dbo.NDB_Board.BRD_ManufacturerCode, 
                         dbo.NDB_ModuleType.MOD_Type
FROM            dbo.NDB_Board INNER JOIN
                         dbo.NDB_ModuleType ON dbo.NDB_Board.BRD_ModuleType_ID = dbo.NDB_ModuleType.MOD_ID" & StrWhereClause


        Dim RT_Sql As New SqlClient.SqlCommand(tmpSQLClause, NDB_DB)
        Dim RT_Results As SqlClient.SqlDataReader = RT_Sql.ExecuteReader()

        RT_Results.Read()


        Dim RowCount As Integer = 0
        Dim StrModuleID As String
        Dim StrModuleType As String
        Dim StrPurchaser As String
        Dim StrTestedBy As String
        Dim StrOrderNumber As String
        Dim StrGUID As String
        Dim StrQADate As String


        With FrmSummary.DataSummaryGrid
            .Rows.Clear()   '** Clear previous data

            'Set number of columns
            .ColumnCount = 7

            'Set names and widths
            Dim x As Integer = 0
            .Columns(x).Name = "Module ID"
            .Columns(x).Width = 80
            x += 1
            .Columns(x).Name = "Module Type"
            .Columns(x).Width = 80
            x += 1

            .Columns(x).Name = "Purchaser"
            .Columns(x).Width = 150
            x += 1

            .Columns(x).Name = "Tested By"
            .Columns(x).Width = 100
            x += 1

            .Columns(x).Name = "Order #"
            .Columns(x).Width = 100
            x += 1

            .Columns(x).Name = "GUID"
            .Columns(x).Width = 300
            x += 1

            .Columns(x).Name = "Date"
            .Columns(x).Width = 180
            x += 1

            '** Display style
            With .DefaultCellStyle
                .Font = New Font("Century Gothic", 8)
                .ForeColor = Color.Blue
                .BackColor = Color.White
                .SelectionForeColor = Color.Yellow
                .SelectionBackColor = Color.Black
            End With
        End With
        If RT_Results.HasRows Then

            'Get First result
            If IsDBNull(RT_Results.Item("BRD_ModuleID")) Then StrModuleID = "" Else StrModuleID = UCase(RT_Results.Item("BRD_ModuleID"))
            If IsDBNull(RT_Results.Item("MOD_Type")) Then StrModuleType = "" Else StrModuleType = UCase(RT_Results.Item("MOD_Type"))
            If IsDBNull(RT_Results.Item("BRD_Purchaser")) Then StrPurchaser = "" Else StrPurchaser = UCase(RT_Results.Item("BRD_Purchaser"))
            If IsDBNull(RT_Results.Item("BRD_QATestedBy")) Then StrTestedBy = "" Else StrTestedBy = RT_Results.Item("BRD_QATestedBy")
            If IsDBNull(RT_Results.Item("BRD_OrderNumber")) Then StrOrderNumber = "" Else StrOrderNumber = UCase(RT_Results.Item("BRD_OrderNumber"))
            If IsDBNull(RT_Results.Item("BRD_ModuleGUID")) Then StrGUID = "" Else StrGUID = UCase(RT_Results.Item("BRD_ModuleGUID"))
            'If IsDBNull(RT_Results.Item("BRD_NumberOfChannelsTested")) Then StrChannels = "" Else StrChannels = UCase(RT_Results.Item("BRD_NumberOfChannelsTested")) ' not used here
            If IsDBNull(RT_Results.Item("BRD_QATestedDate")) Then StrQADate = "" Else StrQADate = UCase(RT_Results.Item("BRD_QATestedDate"))
            Debug.Print(StrModuleID)
            '*** Increment number of rows
            RowCount += 1

            FrmSummary.DataSummaryGrid.Rows.Add(StrModuleID, StrModuleType, StrPurchaser, StrTestedBy, StrOrderNumber, StrGUID, StrQADate)
            For Each item In RT_Results
                If IsDBNull(RT_Results.Item("BRD_ModuleID")) Then StrModuleID = "" Else StrModuleID = UCase(RT_Results.Item("BRD_ModuleID"))
                If IsDBNull(RT_Results.Item("MOD_Type")) Then StrModuleType = "" Else StrModuleType = UCase(RT_Results.Item("MOD_Type"))
                If IsDBNull(RT_Results.Item("BRD_Purchaser")) Then StrPurchaser = "" Else StrPurchaser = UCase(RT_Results.Item("BRD_Purchaser"))
                If IsDBNull(RT_Results.Item("BRD_QATestedBy")) Then StrTestedBy = "" Else StrTestedBy = RT_Results.Item("BRD_QATestedBy")
                If IsDBNull(RT_Results.Item("BRD_OrderNumber")) Then StrOrderNumber = "" Else StrOrderNumber = UCase(RT_Results.Item("BRD_OrderNumber"))
                If IsDBNull(RT_Results.Item("BRD_ModuleGUID")) Then StrGUID = "" Else StrGUID = UCase(RT_Results.Item("BRD_ModuleGUID"))
                '   If IsDBNull(RT_Results.Item("BRD_NumberOfChannelsTested")) Then StrChannels = "" Else StrChannels = UCase(RT_Results.Item("BRD_NumberOfChannelsTested"))
                If IsDBNull(RT_Results.Item("BRD_QATestedDate")) Then StrQADate = "" Else StrQADate = UCase(RT_Results.Item("BRD_QATestedDate"))
                Debug.Print(StrModuleID)
                '*** Increment number of rows
                RowCount += 1

                '*** Update data grid on form before loading
                FrmSummary.DataSummaryGrid.Rows.Add(StrModuleID, StrModuleType, StrPurchaser, StrTestedBy, StrOrderNumber, StrGUID, StrQADate)
                Application.DoEvents() 'Allow UI to refresh
            Next
        End If

        FrmSummary.LblSummaryResult.Visible = True
        FrmSummary.LblSummaryResult.Text = RowCount.ToString & " Items found."
        NDB_DB.Close()
        Application.DoEvents() 'Allow UI to refresh
        Dim IntResult As DialogResult = FrmSummary.ShowDialog()
    End Sub

    Private Sub ChkSummary_CheckedChanged(sender As Object, e As EventArgs) Handles ChkSummary.CheckedChanged
        Select Case ChkSummary.Checked
            Case True ' Selected PO search
                LblSearch.Text = "Search PO"
                TxtSummary.Visible = True
                LblPOText.Visible = True
                DateSummary.Visible = False
            Case False ' Date search
                LblSearch.Text = "Search Date"
                TxtSummary.Visible = False
                LblPOText.Visible = False
                DateSummary.Visible = True
        End Select
    End Sub
    Private Sub AddToPOLog()

        Dim Sfilename As String = Trim(TxtOrderRef.Text)
        Dim PO_FileName As String = StrRootFolder & Sfilename & ".POlog"
        Dim PO_FileNumber As Integer = FreeFile()
        Dim S_Suffix As String

        'Open log file
        FileOpen(PO_FileNumber, PO_FileName, OpenMode.Append)
        If ChkMasterLbl.Checked Then
            Dim S_Type As String
            If Strings.Left(TxtMasterSSID.Text, 2) = "MC" Then S_Type = "RTMC8" Else S_Type = "Master"

            S_Suffix = ",F"

            PrintLine(PO_FileNumber, TxtMasterSSID.Text & "," & Format(Now(), "dd-MM-yyyy") & S_Suffix)
        End If
        If ChkCIBLbl.Checked Then
            S_Suffix = ",F"

            PrintLine(PO_FileNumber, TxtCIBid.Text & "," & Format(Now(), "dd-MM-yyyy") & S_Suffix)
        End If
        FileClose(PO_FileNumber)
    End Sub

    Private Sub BtnPrintInvLbl_Click(sender As Object, e As EventArgs) Handles BtnPrintInvLbl.Click
        '**** Print inventory label

        PrintDocument11.PrinterSettings.PrinterName = StrSelectedPrinter
        PrintDocument12.PrinterSettings.PrinterName = StrSelectedPrinter

        '*** Check for PO number
        If TxtOrderRef.Text <> "" Then
            '*** Check if master label is to be printed
            If ChkMasterLbl.Checked Then
                If UCase(SMasterType) <> "UNKNOWN" Then
                    If TxtMasterSSID.Text <> "" Then
                        AddToPOLog()
                        PrintDocument11.Print()
                    Else
                        MsgBox("No Master Board ID present. Cannot Print", vbOKOnly + vbCritical, "Error. Cannot Print")
                    End If

                Else
                    MsgBox("No Master Type set. Cannot Print", vbOKOnly + vbCritical, "Error. Cannot Print")
                End If
            End If
            '*** Check if slave label is to be printed
            If ChkCIBLbl.Checked Then
                If TxtCIBid.Text <> "" Then
                    AddToPOLog()
                    PrintDocument12.Print()
                Else
                    MsgBox("No Channel Board ID present. Cannot Print", vbOKOnly + vbCritical, "Error. Cannot Print")
                End If
            End If
        Else
            MsgBox("No Order Reference present. Cannot Print", vbOKOnly + vbCritical, "Error. Cannot Print")
        End If
    End Sub

    Private Sub PrintDocument11_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument11.PrintPage
        '** This prints the master inventory label

        '*** Print Master Info
        Dim stdFont = New Font("Arial", 14, FontStyle.Bold)
        Dim boldFont = New Font("Arial", 14, FontStyle.Bold)
        Dim italicFont = New Font("Arial", 14, FontStyle.Italic)
        Dim underlineFont = New Font("Arial", 14, FontStyle.Underline)
        Dim heading01Font = New Font("Arial", 16, FontStyle.Bold)
        Dim BarCodeFont As New Font("IDAHC39M Code 39 Barcode", 16, FontStyle.Regular)
        Dim centeredText As StringFormat
        Dim leftText As StringFormat
        Dim rightText As StringFormat

        Dim labelHeading As String = TxtMasterSSID.Text

        centeredText = New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        leftText = New StringFormat With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Near
        }

        rightText = New StringFormat With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Far
        }

        Dim printLine As Integer = 120
        Dim pLogo As New RectangleF(New PointF(300.0F, 10.0F), New SizeF(64.0F, 50.0F))

        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RT.png"), pLogo)

        e.Graphics.DrawString("Rapid Test Systems Pty Ltd", stdFont, Brushes.Black, 10, 20, leftText)

        e.Graphics.DrawString("*** Inventory Label ***", heading01Font, Brushes.Black, 30, 50, leftText)
        e.Graphics.DrawString("Master: " & labelHeading, boldFont, Brushes.Black, 50, 90, leftText)

        Dim Sprefix As String
        If ChkNHP.Checked Then Sprefix = "N" Else Sprefix = ""
        Dim SpartNumber As String = Sprefix & "RTMC8F5"


        e.Graphics.DrawString("Part: " & SpartNumber, boldFont, Brushes.Black, 50, printLine, leftText)
        e.Graphics.DrawString("*" & labelHeading & "*", BarCodeFont, Brushes.Black, 50, printLine + 20, leftText)
    End Sub

    Private Sub PrintDocument12_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument12.PrintPage
        '** This prints the Channel board inventory label

        '*** Print channel board Info
        Dim stdFont = New Font("Arial", 14, FontStyle.Bold)
        Dim boldFont = New Font("Arial", 14, FontStyle.Bold)
        Dim italicFont = New Font("Arial", 14, FontStyle.Italic)
        Dim underlineFont = New Font("Arial", 14, FontStyle.Underline)
        Dim heading01Font = New Font("Arial", 16, FontStyle.Bold)
        Dim BarCodeFont As New Font("IDAHC39M Code 39 Barcode", 16, FontStyle.Regular)
        Dim centeredText As StringFormat
        Dim leftText As StringFormat
        Dim rightText As StringFormat

        Dim labelHeading As String = TxtCIBid.Text

        centeredText = New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        leftText = New StringFormat With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Near
        }

        rightText = New StringFormat With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Far
        }

        Dim printLine As Integer = 120
        Dim pLogo As New RectangleF(New PointF(300.0F, 10.0F), New SizeF(64.0F, 50.0F))

        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RT.png"), pLogo)

        e.Graphics.DrawString("Rapid Test Systems Pty Ltd", stdFont, Brushes.Black, 10, 20, leftText)


        e.Graphics.DrawString("*** Inventory Label ***", heading01Font, Brushes.Black, 30, 50, leftText)
        e.Graphics.DrawString("C/B ID: " & labelHeading, boldFont, Brushes.Black, 50, 90, leftText)


        Dim Sprefix As String
        If ChkNHP.Checked Then Sprefix = "N" Else Sprefix = ""

        Dim SrelayType As String = "F"

        e.Graphics.DrawString("Part: " & Sprefix & "RTRB24" & SrelayType, boldFont, Brushes.Black, 50, printLine, leftText)
        e.Graphics.DrawString("*" & labelHeading & "*", BarCodeFont, Brushes.Black, 50, printLine + 20, leftText)
    End Sub

    Private Sub BtnPrintEquipLbl_Click(sender As Object, e As EventArgs) Handles BtnPrintEquipLbl.Click
        '*** PRINT MASTER & CIB BOX LABELS
        TxtMasterSSID.Text = UCase(TxtMasterSSID.Text)
        TxtCIBid.Text = UCase(TxtCIBid.Text)

        PrintDocument2.PrinterSettings.PrinterName = StrSelectedPrinter '*** Master box label with QR code
        PrintDocument3.PrinterSettings.PrinterName = StrSelectedPrinter '*** CIB box label with QR code
        PrintDocument4.PrinterSettings.PrinterName = StrSelectedPrinter '*** Master box label with barcode
        PrintDocument5.PrinterSettings.PrinterName = StrSelectedPrinter '*** CIB box label with Barcode
        PrintDocument11.PrinterSettings.PrinterName = StrSelectedPrinter    '*** Master Inventory label
        PrintDocument12.PrinterSettings.PrinterName = StrSelectedPrinter    '*** CIB Inventory label

        '*** Stickers warning the board builder not to throw out the various identifying stickers
        PrintStickerLabel.PrinterSettings.PrinterName = StrSelectedPrinter

        '*** Check for PO number
        If TxtOrderRef.Text <> "" Then
            '*** Check if master label is to be printed
            If ChkMasterLbl.Checked Then
                If UCase(SMasterType) <> "UNKNOWN" Then
                    If TxtMasterSSID.Text <> "" Then
                        AddToPOLog()
                        PrintDocument2.Print()
                        PrintDocument4.Print()
                        If ChkInventoryLbl.Checked Then PrintDocument11.Print()
                        StickerText01 = "Master Indentification Sticker"
                        StickerText02 = "Please place label on the escutcheon " & vbCrLf & "for Master identification"
                        PrintStickerLabel.Print()
                        If UCase(Strings.Left(SMasterType, 4)) = "RTMC" Then
                            StickerText01 = "RCD Indentification Sticker"
                            StickerText02 = "Please place labels to identify each " & vbCrLf & "Rapid Test connected RCD"
                            PrintStickerLabel.Print()
                        End If
                    Else
                        MsgBox("No Master Board ID present. Cannot Print", vbOKOnly + vbCritical, "Error. Cannot Print")
                    End If

                Else
                    MsgBox("No Master Type set. Cannot Print", vbOKOnly + vbCritical, "Error. Cannot Print")
                End If
            End If
            '*** Check if slave label is to be printed
            If ChkCIBLbl.Checked Then
                If TxtCIBid.Text <> "" Then
                    AddToPOLog()
                    PrintDocument3.Print()
                    PrintDocument5.Print()
                    If ChkInventoryLbl.Checked Then PrintDocument12.Print()
                    StickerText01 = "RCD Indentification Sticker"
                    StickerText02 = "Please place labels to identify each " & vbCrLf & "Rapid Test connected RCD"
                    PrintStickerLabel.Print()
                Else
                    MsgBox("No Channel Board ID present. Cannot Print", vbOKOnly + vbCritical, "Error. Cannot Print")
                End If
            End If
        Else
            MsgBox("No Order Reference present. Cannot Print", vbOKOnly + vbCritical, "Error. Cannot Print")
        End If
    End Sub

    Private Sub PrintDocument1_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
        '*** Prints summary of PO from log file

        Dim stdFont = New Font("Arial", 10)
        Dim smlFont = New Font("Arial", 8)
        Dim smlboldFont = New Font("Arial", 8, FontStyle.Bold)
        Dim boldFont = New Font("Arial", 10, FontStyle.Bold)
        Dim italicFont = New Font("Arial", 10, FontStyle.Italic)
        Dim underlineFont = New Font("Arial", 10, FontStyle.Underline)
        Dim heading01Font = New Font("Arial", 16, FontStyle.Bold)
        Dim centeredText As StringFormat
        Dim leftText As StringFormat
        Dim rightText As StringFormat
        Dim generalFont As New Font("Arial", 16, FontStyle.Bold)

        centeredText = New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        leftText = New StringFormat With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Near
        }

        rightText = New StringFormat With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Far
        }

        '*** Body
        '*** Divide into Master, MC8 & Channel
        '*** Get totals
        Dim I_Masters As Integer = Val(S_po_Master(0, 2))
        Dim I_MC8 As Integer = Val(S_po_MC8(0, 2))
        Dim I_CIB As Integer = Val(S_po_CIB(0, 2))
        Dim I_LineIndex As Integer = 100

        '*** Heading
        e.Graphics.DrawString("Rapid Test. Order reference " & UCase(Trim(TxtOrderRef.Text)), heading01Font, Brushes.Black, 50, 30, leftText)

        '*** Print Logo
        Dim pLogo As New RectangleF(New PointF(600.0F, 10.0F), New SizeF(120.0F, 94.0F))
        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RT.png"), pLogo)

        e.Graphics.DrawString("Print Date: " & Format(Now(), "dd/MM/yyyy"), boldFont, Brushes.Black, 50, 70, leftText)
        e.Graphics.DrawString("Page: 1", boldFont, Brushes.Black, 350, 70, leftText)

        If I_Masters > 0 Then
            e.Graphics.DrawString("Standard Master Module (RTTUWF)", boldFont, Brushes.Black, 50, I_LineIndex, leftText)
            e.Graphics.DrawString("Quantity " & I_Masters.ToString, boldFont, Brushes.Black, 350, I_LineIndex, leftText)
            I_LineIndex += 20
        End If

        If I_MC8 > 0 Then
            e.Graphics.DrawString("MC8 Master Module (RTMC8)", boldFont, Brushes.Black, 50, I_LineIndex, leftText)
            e.Graphics.DrawString("Quantity " & I_MC8.ToString, boldFont, Brushes.Black, 350, I_LineIndex, leftText)
            I_LineIndex += 20
        End If

        If I_CIB > 0 Then
            e.Graphics.DrawString("Channel Board Module (RTRB24)", boldFont, Brushes.Black, 50, I_LineIndex, leftText)
            e.Graphics.DrawString("Quantity " & I_CIB.ToString, boldFont, Brushes.Black, 350, I_LineIndex, leftText)
            I_LineIndex += 20
        End If


        Application.DoEvents()

    End Sub

    Private Sub BtnPrintPO_Click(sender As Object, e As EventArgs) Handles BtnPrintPO.Click
        '*** PRINT PO SHEET LISTING

        Dim I_UpperLimit1 As Integer = UBound(S_po_Master, 1)
        Dim I_UpperLimit2 As Integer = UBound(S_po_Master, 2)
        '*** Clear arrays all the same size

        For x = 0 To I_UpperLimit1
            For y = 0 To I_UpperLimit2
                S_po_MC8(x, y) = ""
            Next
        Next

        For x = 0 To I_UpperLimit1
            For y = 0 To I_UpperLimit2
                S_po_CIB(x, y) = ""
            Next
        Next

        Dim Sfilename As String = Trim(TxtOrderRef.Text)
        Dim PO_FileName As String = StrRootFolder & Sfilename & ".POlog"
        ' Dim PO_FileNumber As Integer = FreeFile()
        Dim rowCount As Integer = 0

        If Sfilename <> "" Then


            Dim currentField As String
            Dim S_TempModule As String = ""
            Dim S_TempDate As String = ""
            Dim S_SolidState As String = ""
            Dim I_MasterIndex As Integer = 0
            Dim I_MC8Index As Integer = 0
            Dim I_CIBIndex As Integer = 0
            Dim I_MC8SSIndex As Integer = 0
            Dim I_CIBSSIndex As Integer = 0
            'Open log file
            Try
                Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(PO_FileName)
                    MyReader.TextFieldType = FileIO.FieldType.Delimited
                    MyReader.SetDelimiters(",")
                    Dim currentRow As String()

                    Dim fieldCount As Integer = 0
                    While Not MyReader.EndOfData
                        Try
                            currentRow = MyReader.ReadFields()

                            For Each currentField In currentRow
                                If rowCount >= 0 Then Debug.Print(currentField)

                                '*** Collect module ID & date
                                If fieldCount = 0 Then S_TempModule = (currentField)
                                If fieldCount = 1 Then S_TempDate = (currentField)
                                If fieldCount = 2 Then S_SolidState = UCase((currentField))

                                '*** Only store the module ID if it is not already in the array
                                If fieldCount = 2 Then
                                    ' For x = 0 To 500        ' Possible lines in PO file
                                    '*** Check if next array element is empty
                                    '*** If it is, the module ID has not been processed
                                    '*** Additionally store the count of each module type in the first array element 2 of each array
                                    '*** MasterID, CIB_ID, count Master, count MC8, Count CIB, Count SS MC8, Count SS CIB

                                    '** Determine module type  


                                    If Strings.Left(S_TempModule, 2) = "MC" Then
                                                For y = 0 To 500
                                                    If S_po_MC8(y, 0) = "" Then
                                                        S_po_MC8(0, 2) = Val(S_po_MC8(0, 2)) + 1
                                                        S_po_MC8(I_MC8Index, 0) = S_TempModule
                                                        S_po_MC8(I_MC8Index, 1) = S_TempDate
                                                        I_MC8Index += 1 ' Increment SS index for MC8
                                                        Exit For
                                                    Else
                                                        If S_po_MC8(y, 0) = S_TempModule Then Exit For
                                                    End If

                                                Next
                                            ElseIf Strings.Left(S_TempModule, 1) = "C" Then   '*** Not MC8 or Master must be CIB
                                                For y = 0 To 500
                                                    If S_po_CIB(y, 0) = "" Then
                                                        '** CIB
                                                        S_po_CIB(0, 2) = Val(S_po_CIB(0, 2)) + 1
                                                        S_po_CIB(I_CIBIndex, 0) = S_TempModule
                                                        S_po_CIB(I_CIBIndex, 1) = S_TempDate
                                                        I_CIBIndex += 1 ' Increment SS index for CIB
                                                        Exit For
                                                    Else
                                                        If S_po_CIB(y, 0) = S_TempModule Then Exit For
                                                    End If
                                                Next

                                            End If

                                End If

                                '** increment for next field after checking
                                fieldCount += 1

                            Next

                            rowCount += 1
                            fieldCount = 0
                        Catch ex As _
                        Microsoft.VisualBasic.FileIO.MalformedLineException
                            MsgBox("Line " & ex.Message &
                            "is not valid and will be skipped.")

                        End Try
                    End While
                End Using

                '*** Print array for testing only

                For x = 0 To 500
                    If S_po_MC8(x, 0) = "" Then Exit For
                    Debug.Print("MC8    : " & S_po_MC8(x, 0) & " - " & S_po_MC8(x, 1) & " - " & S_po_MC8(x, 2))
                Next

                For x = 0 To 500
                    If S_po_CIB(x, 0) = "" Then Exit For
                    Debug.Print("CIB    : " & S_po_CIB(x, 0) & " - " & S_po_CIB(x, 1) & " - " & S_po_CIB(x, 2))
                Next

                '** Print the List
                PrintDocument1.Print()

            Catch fileNotFound As System.IO.FileNotFoundException
                'Error handling here. Load default values etc
                Dim Iresult As Integer = MsgBox("File not Found; " & fileNotFound.Message, vbCritical + vbOKOnly, "File error.")
                Debug.Print(fileNotFound.FileName)

            End Try
        Else
            Dim Iresult As Integer = MsgBox("Please enter a Order Reference to open.", vbCritical + vbOKOnly, "Error.")
        End If
    End Sub

    Private Sub PrintDocument2_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument2.PrintPage
        '** This prints the master box label

        '*** Print Master Info
        Dim stdFont = New Font("Arial", 14)
        Dim TinyFont = New Font("Arial", 8, FontStyle.Regular)
        Dim boldFont = New Font("Arial", 14, FontStyle.Bold)
        Dim italicFont = New Font("Arial", 14, FontStyle.Italic)
        Dim underlineFont = New Font("Arial", 14, FontStyle.Underline)
        Dim heading01Font = New Font("Arial", 14, FontStyle.Bold)
        Dim centeredText As StringFormat
        Dim leftText As StringFormat
        Dim rightText As StringFormat

        Dim labelHeading As String = TxtMasterSSID.Text

        centeredText = New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        leftText = New StringFormat With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Near
        }

        rightText = New StringFormat With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Far
        }

        Dim printLine As Integer = 120
        Dim pLogo As New RectangleF(New PointF(300.0F, 10.0F), New SizeF(64.0F, 50.0F))

        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RT.png"), pLogo)
        'Place QR Code
        Dim QRCode As New RectangleF(New PointF(300.0F, 120.0F), New SizeF(60.0F, 60.0F))
        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RTQR.png"), QRCode)
        e.Graphics.DrawString("Installation Manual", TinyFont, Brushes.Black, 280, 180, leftText)
        e.Graphics.DrawString("Rapid Test Systems Pty Ltd", heading01Font, Brushes.Black, 10, 20, leftText)

        e.Graphics.DrawString("QA checked Master : " & SMasterType, heading01Font, Brushes.Black, 10, 50, leftText)

        e.Graphics.DrawString("Date: " & Format(Now(), "dd/MM/yyyy"), boldFont, Brushes.Black, 10, 80, leftText)
        e.Graphics.DrawString("Master Unit ID: " & labelHeading, heading01Font, Brushes.Black, 10, printLine, leftText)


        Dim Sprefix As String
        If ChkNHP.Checked Then Sprefix = "N" Else Sprefix = ""

        Dim SpartNumber As String = Sprefix & "RTMC8F5"

        e.Graphics.DrawString("Part: " & SpartNumber, boldFont, Brushes.Black, 180, 80, leftText)

        printLine += 30
        e.Graphics.DrawString("Order: " & TxtOrderRef.Text, boldFont, Brushes.Black, 10, printLine, leftText)


    End Sub

    Private Sub PrintDocument4_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument4.PrintPage
        '** This prints the master box label with barcode

        '*** Print Master Info
        Dim stdFont = New Font("Arial", 14, FontStyle.Bold)
        Dim TinyFont = New Font("Arial", 8, FontStyle.Regular)
        Dim boldFont = New Font("Arial", 20, FontStyle.Bold)
        Dim italicFont = New Font("Arial", 14, FontStyle.Italic)
        Dim underlineFont = New Font("Arial", 14, FontStyle.Underline)
        Dim heading01Font = New Font("Arial", 25, FontStyle.Bold)
        Dim BarCodeFont As New Font("IDAHC39M Code 39 Barcode", 16, FontStyle.Regular)
        Dim centeredText As StringFormat
        Dim leftText As StringFormat
        Dim rightText As StringFormat

        Dim labelHeading As String = TxtMasterSSID.Text

        centeredText = New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        leftText = New StringFormat With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Near
        }

        rightText = New StringFormat With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Far
        }

        Dim printLine As Integer = 100
        Dim pLogo As New RectangleF(New PointF(300.0F, 10.0F), New SizeF(64.0F, 50.0F))
        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RT.png"), pLogo)

        'Place QR Code
        Dim QRCode As New RectangleF(New PointF(300.0F, 120.0F), New SizeF(60.0F, 60.0F))
        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RTQR.png"), QRCode)
        e.Graphics.DrawString("Installation Manual", TinyFont, Brushes.Black, 280, 180, leftText)

        e.Graphics.DrawString("Rapid Test Systems Pty Ltd", stdFont, Brushes.Black, 10, 20, leftText)

        e.Graphics.DrawString("Master: " & labelHeading, heading01Font, Brushes.Black, 10, 60, leftText)

        Dim Sprefix As String
        If ChkNHP.Checked Then Sprefix = "N" Else Sprefix = ""

        Dim SpartNumber As String = Sprefix & "RTMC8F5"


        e.Graphics.DrawString("Part: " & SpartNumber, boldFont, Brushes.Black, 10, printLine, leftText)
        e.Graphics.DrawString("*" & labelHeading & "*", BarCodeFont, Brushes.Black, 10, printLine + 40, leftText)
    End Sub

    Private Sub PrintStickerLabel_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintStickerLabel.PrintPage
        '** This prints the Stickers warning the board builder
        '** not to throw out the various identifying stickers

        '*** Print channel board Info
        Dim stdFont = New Font("Arial", 14, FontStyle.Regular)
        Dim boldFont = New Font("Arial", 14, FontStyle.Bold)
        Dim italicFont = New Font("Arial", 14, FontStyle.Italic)
        Dim underlineFont = New Font("Arial", 14, FontStyle.Underline)
        Dim heading01Font = New Font("Arial", 25, FontStyle.Bold)
        Dim BarCodeFont As New Font("IDAHC39M Code 39 Barcode", 16, FontStyle.Regular)
        Dim centeredText As StringFormat
        Dim leftText As StringFormat
        Dim rightText As StringFormat


        centeredText = New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }

        leftText = New StringFormat With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Near
        }

        rightText = New StringFormat With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Far
        }

        Dim printLine As Integer = 140
        Dim pLogo As New RectangleF(New PointF(300.0F, 10.0F), New SizeF(64.0F, 50.0F))

        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RT.png"), pLogo)

        e.Graphics.DrawString("Rapid Test Systems Pty Ltd", stdFont, Brushes.Black, 10, 20, leftText)

        e.Graphics.DrawString(StickerText01, boldFont, Brushes.Black, 10, 60, leftText)

        e.Graphics.DrawString(StickerText02, stdFont, Brushes.Black, 10, 90, leftText)

        e.Graphics.DrawString("(Do not throw out, end user identifier)", boldFont, Brushes.Black, 10, printLine, leftText)
    End Sub

    Private Sub PrintDocument3_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument3.PrintPage
        '** This prints the Channel board box label

        '*** Print channel board Info
        Dim stdFont = New Font("Arial", 12)
        Dim TinyFont = New Font("Arial", 8, FontStyle.Regular)
        Dim boldFont = New Font("Arial", 14, FontStyle.Bold)
        Dim italicFont = New Font("Arial", 14, FontStyle.Italic)
        Dim underlineFont = New Font("Arial", 14, FontStyle.Underline)
        Dim heading01Font = New Font("Arial", 14, FontStyle.Bold)
        Dim centeredText As StringFormat
        Dim leftText As StringFormat
        Dim rightText As StringFormat

        Dim labelHeading As String = TxtCIBid.Text

        centeredText = New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        leftText = New StringFormat With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Near
        }

        rightText = New StringFormat With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Far
        }

        Dim printLine As Integer = 120
        Dim pLogo As New RectangleF(New PointF(300.0F, 10.0F), New SizeF(64.0F, 50.0F))

        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RT.png"), pLogo)
        'Place QR Code
        Dim QRCode As New RectangleF(New PointF(300.0F, 120.0F), New SizeF(60.0F, 60.0F))
        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RTQR.png"), QRCode)
        e.Graphics.DrawString("Installation Manual", TinyFont, Brushes.Black, 280, 180, leftText)

        e.Graphics.DrawString("Rapid Test Systems Pty Ltd", heading01Font, Brushes.Black, 10, 20, leftText)
        e.Graphics.DrawString("QA checked Channel Board", heading01Font, Brushes.Black, 10, 50, leftText)

        e.Graphics.DrawString("Date: " & Format(Now(), "dd/MM/yyyy"), boldFont, Brushes.Black, 10, 80, leftText)

        e.Graphics.DrawString("Channel Board ID: " & labelHeading, heading01Font, Brushes.Black, 10, printLine, leftText)


        Dim Sprefix As String
        If ChkNHP.Checked Then Sprefix = "N" Else Sprefix = ""

        Dim SrelayType As String = "F"

        e.Graphics.DrawString("Part: " & Sprefix & "RTRB24" & SrelayType, boldFont, Brushes.Black, 180, 80, leftText)

        printLine += 30
        e.Graphics.DrawString("Order: " & TxtOrderRef.Text, boldFont, Brushes.Black, 10, printLine, leftText)
    End Sub

    Private Sub PrintDocument5_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument5.PrintPage
        '** This prints the Channel board box label with QR code

        '*** Print channel board Info
        Dim stdFont = New Font("Arial", 14, FontStyle.Bold)
        Dim TinyFont = New Font("Arial", 8, FontStyle.Regular)
        Dim boldFont = New Font("Arial", 20, FontStyle.Bold)
        Dim italicFont = New Font("Arial", 14, FontStyle.Italic)
        Dim underlineFont = New Font("Arial", 14, FontStyle.Underline)
        Dim heading01Font = New Font("Arial", 25, FontStyle.Bold)
        Dim BarCodeFont As New Font("IDAHC39M Code 39 Barcode", 16, FontStyle.Regular)
        Dim centeredText As StringFormat
        Dim leftText As StringFormat
        Dim rightText As StringFormat

        Dim labelHeading As String = TxtCIBid.Text

        centeredText = New StringFormat With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        leftText = New StringFormat With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Near
        }

        rightText = New StringFormat With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Far
        }

        Dim printLine As Integer = 100
        Dim pLogo As New RectangleF(New PointF(300.0F, 10.0F), New SizeF(64.0F, 50.0F))

        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RT.png"), pLogo)
        e.Graphics.DrawString("Rapid Test Systems Pty Ltd", stdFont, Brushes.Black, 10, 20, leftText)

        'Place QR Code
        Dim QRCode As New RectangleF(New PointF(300.0F, 120.0F), New SizeF(60.0F, 60.0F))
        e.Graphics.DrawImage(Image.FromFile("C:\Temp\RTQR.png"), QRCode)
        e.Graphics.DrawString("Installation Manual", TinyFont, Brushes.Black, 280, 180, leftText)

        e.Graphics.DrawString("C/B ID: " & labelHeading, heading01Font, Brushes.Black, 10, 60, leftText)


        Dim Sprefix As String
        If ChkNHP.Checked Then Sprefix = "N" Else Sprefix = ""

        Dim SrelayType As String = "F"

        e.Graphics.DrawString("Part: " & Sprefix & "RTRB24" & SrelayType, boldFont, Brushes.Black, 10, printLine, leftText)
        e.Graphics.DrawString("*" & labelHeading & "*", BarCodeFont, Brushes.Black, 10, printLine + 40, leftText)
    End Sub
    Private Function CheckNDBData(ModuleID As String, DisplayResult As Boolean) As Integer
        Dim item As String = ""
        Dim Description As String = ""
        Dim Details As String = ""
        Dim NoMatch As Boolean = False

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
        Dim S_ManCode As String = ""

        Try
            Dim configurationSaver As New ConfigurationSaver(connectionString)

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
                S_ManCode = .ManufacturerCode
            End With


        Catch ex As Exception
            Debug.Print("No Match")
            'MsgBox(ex.ToString, vbOK + vbExclamation, "Result")
            NoMatch = True
        End Try

        Dim I_MasterIndex As Integer = 0

        Debug.Print(S_ModuleType & "  " & S_ModuleID & "  " & S_TestedBy & "  " & S_Purchaser & "  " & S_OrderNumber & "  " & S_ModuleType & "  " & S_GUID)

        If DisplayResult Then
            If NoMatch Then
                MsgBox("No macthing record was found", vbInformation + vbOKOnly, "Sorry . . .")
            Else
                MsgBox("Module ID: " & S_ModuleID & vbCrLf & "Module Type: " & S_ModuleType & vbCrLf &
                       "Manufacturing Code: " & S_ManCode & vbCrLf &
                       "Checked By: " & S_TestedBy & vbCrLf & "Purchaser: " & S_Purchaser & vbCrLf &
                       "Order Number: " & S_OrderNumber & vbCrLf & "Date:  " & S_QADate & vbCrLf &
                       "GUID: " & S_GUID, vbInformation + vbOKOnly, "Got It !!")
            End If
        End If

        '** Determine return value based on the module type 
        ' 0 Nothing found
        Dim IreturnValue As Integer = 0


        Select Case UCase(S_ModuleType)
            Case "MASTER"
                IreturnValue = 1
            Case "MC8", "MC4"
                IreturnValue = 3
            Case "CIB"
                IreturnValue = 2
            Case "MC8SS", "MC4SS"
                IreturnValue = 4
            Case "CIBSS"
                IreturnValue = 5
            Case "MC8F"
                IreturnValue = 8
            Case "CIBF"
                IreturnValue = 9
            Case "CIB0L"
                IreturnValue = 10
            Case "MC8F5"
                IreturnValue = 11
            Case Else

        End Select

        Return IreturnValue

    End Function
    Private Sub TxtMasterSSID_TextChanged(sender As Object, e As EventArgs) Handles TxtMasterSSID.TextChanged
        Dim ImoduleFound As Integer = 0


        TxtMasterSSID.Text = Strings.Replace(TxtMasterSSID.Text, vbCrLf, "")
        TxtMasterSSID.Text = Strings.Replace(TxtMasterSSID.Text, vbCr, "")
        TxtMasterSSID.Text = Strings.Replace(TxtMasterSSID.Text, vbLf, "")
        TxtMasterSSID.Text = Strings.Replace(TxtMasterSSID.Text, "!", "")
        TxtMasterSSID.Text = Strings.Replace(TxtMasterSSID.Text, "#", "")

        sScannerString = Trim(TxtMasterSSID.Text)

        Debug.Print("Scanned " & sScannerString)

        Select Case Len(sScannerString)
            Case 6
                '** Filter out nonstandard masters (Masters are in the format M50123)
                If Strings.Left(sScannerString, 2) <> "MC" Then
                    ImoduleFound = CheckNDBData(Trim(sScannerString), False)
                    Select Case ImoduleFound
                        Case 0
                        Case 1
                            '**** Show warning
                            F_NDBMessage.LBLMessage.Text = "Exisitng Master found in database." & sScannerString & vbCrLf & "Only continue if reprogramming."
                            F_NDBMessage.PictureBox1.BackgroundImage = My.Resources.Symbol_Information
                            F_NDBMessage.ShowDialog()
                        Case Else
                            '**** Show warning for unknown module
                            F_NDBMessage.LBLMessage.Text = "Unknown Master device type found in database. " & sScannerString & vbCrLf & "Check unit is not Solid State and apply new label."
                            F_NDBMessage.PictureBox1.BackgroundImage = My.Resources.Symbol_Error
                            F_NDBMessage.ShowDialog()
                    End Select

                    '***** Check if code matches NHP checkbox setting
                    Dim IntModuleNumber As Integer = Val(Strings.Right(sScannerString, 5))
                    Select Case IntModuleNumber
                        Case >= 50000
                            '** NHP Unit
                            If ChkNHP.Checked = False Then
                                MsgBox("Master Unit Number range indicates NHP Product but NHP checkbox is not selected", vbOKOnly + vbCritical, "ALERT")
                            End If
                        Case Else
                            '**** RT Direct sales unit
                            If ChkNHP.Checked = True Then
                                MsgBox("Master Unit Number range indicates RT supplied product but NHP checkbox is selected", vbOKOnly + vbCritical, "ALERT")
                            End If

                    End Select

                    TxtMasterManCode.Focus()
                End If
            Case 7

                If Strings.Left(sScannerString, 2) = "MC" Then
                    ImoduleFound = CheckNDBData(Trim(sScannerString), False)
                    Select Case ImoduleFound
                        Case 0
                        Case 4, 6
                            '**** Show warning for Solid State MC8 or MC4
                            F_NDBMessage.LBLMessage.Text = "Solid State Master found in database. " & sScannerString & vbCrLf & "Check unit is not Solid State and apply new label."
                            F_NDBMessage.PictureBox1.BackgroundImage = My.Resources.Symbol_Error
                            F_NDBMessage.ShowDialog()
                        Case 3, 8, 11
                            '**** Show warning for standard MC8 or Fused if already programmed
                            F_NDBMessage.LBLMessage.Text = "Exisitng Master found in database." & sScannerString & vbCrLf & "Only continue if reprogramming."
                            F_NDBMessage.PictureBox1.BackgroundImage = My.Resources.Symbol_Information
                            F_NDBMessage.ShowDialog()
                        Case Else
                            '**** Show warning for unknown module
                            F_NDBMessage.LBLMessage.Text = "Unknown Master device type found in database. " & sScannerString & vbCrLf & "Check unit is not Solid State and apply new label."
                            F_NDBMessage.PictureBox1.BackgroundImage = My.Resources.Symbol_Error
                            F_NDBMessage.ShowDialog()
                    End Select

                    '** All MC8 > 50000 except MCF
                    If Strings.Left(sScannerString, 3) = "MCF" Then
                        '** NHP Unit
                        If ChkNHP.Checked = False Then
                            MsgBox("Master Unit Number range indicates NHP Product but NHP checkbox is not selected", vbOKOnly + vbCritical, "ALERT")
                        End If

                    Else

                        '***** Check if code matches NHP checkbox setting
                        Dim IntModuleNumber As Integer = Val(Strings.Right(sScannerString, 5))
                        Select Case IntModuleNumber
                            Case >= 50000
                                '** NHP Unit
                                If ChkNHP.Checked = False Then
                                    MsgBox("Master Unit Number range indicates NHP Product but NHP checkbox is not selected", vbOKOnly + vbCritical, "ALERT")
                                End If
                            Case Else
                                '**** RT Direct sales unit
                                If ChkNHP.Checked = True Then
                                    MsgBox("Master Unit Number range indicates RT supplied product but NHP checkbox is selected", vbOKOnly + vbCritical, "ALERT")
                                End If

                        End Select
                    End If
                    TxtCIBManCode.Focus()
                End If
            Case Else

        End Select
    End Sub

    Private Sub TxtCIBid_TextChanged(sender As Object, e As EventArgs) Handles TxtCIBid.TextChanged
        Dim ImoduleFound As Integer = 0
        TxtCIBid.Text = Strings.Replace(TxtCIBid.Text, vbCrLf, "")
        TxtCIBid.Text = Strings.Replace(TxtCIBid.Text, vbCr, "")
        TxtCIBid.Text = Strings.Replace(TxtCIBid.Text, vbLf, "")
        TxtCIBid.Text = Strings.Replace(TxtCIBid.Text, "!", "")
        TxtCIBid.Text = Strings.Replace(TxtCIBid.Text, "#", "")

        sScannerString = Trim(TxtCIBid.Text)

        If sScannerString <> "" Then

            Debug.Print(sScannerString)

            Select Case Len(sScannerString)
                Case 6
                    '** Filter out nonstandard masters
                    If Strings.Left(sScannerString, 1) = "C" Then
                        ImoduleFound = CheckNDBData(Trim(sScannerString), False)
                        Select Case ImoduleFound
                            Case 0
                            Case 2, 9, 10

                                '**** Show warning
                                F_NDBMessage.LBLMessage.Text = "Exisitng Channel Board found in database." & sScannerString & vbCrLf & "Only continue if reprogramming."
                                F_NDBMessage.PictureBox1.BackgroundImage = My.Resources.Symbol_Information
                                F_NDBMessage.ShowDialog()
                            Case 5
                                '**** Show warning for Solid State CIB
                                F_NDBMessage.LBLMessage.Text = "Solid State Channel Board found in database. " & sScannerString & vbCrLf & "Check unit is not Solid State and apply new label."
                                F_NDBMessage.PictureBox1.BackgroundImage = My.Resources.Symbol_Error
                                F_NDBMessage.ShowDialog()
                            Case Else
                                '**** Show warning for unknown module
                                F_NDBMessage.LBLMessage.Text = "Unknown device type found in database. " & sScannerString & vbCrLf & "Check unit is not Solid State and apply new label."
                                F_NDBMessage.PictureBox1.BackgroundImage = My.Resources.Symbol_Error
                                F_NDBMessage.ShowDialog()
                        End Select


                        '***** Check if code matches NHP checkbox setting
                        Dim IntModuleNumber As Integer = Val(Strings.Right(sScannerString, 5))
                        Select Case IntModuleNumber
                            Case >= 50000
                                '** NHP Unit
                                If ChkNHP.Checked = False Then
                                    MsgBox("CIB Unit Number range indicates NHP Product but NHP checkbox is not selected", vbOKOnly + vbCritical, "ALERT")
                                End If
                            Case Else
                                '**** RT Direct sales unit
                                If ChkNHP.Checked = True Then
                                    MsgBox("CIB Unit Number range indicates RT supplied product but NHP checkbox is selected", vbOKOnly + vbCritical, "ALERT")
                                End If

                        End Select
                        TxtCIBManCode.Focus()
                    End If
                Case Else

            End Select
        End If
    End Sub
End Class
