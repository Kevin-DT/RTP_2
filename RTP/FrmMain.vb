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
Public Class FrmMain

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

    '*** Wifi 
    Public MyWiFi As New Wifi()
    Public SConnectedWiFi As String = ""
    '** device IP address and port
    Public sIP As String = "192.168.0.1"  '*** Change as required for LAN testing
    Public iPort As Integer = 12000
    '*** Shared elements for the ethernet
    Shared client As New TcpClient
    Shared stream As NetworkStream

    '*** Version specific details
    Public iVersionMinor As Integer
    Public iVersionMajor As Integer
    Public IntResponseLength As Integer

    '*** Step controls
    Public ICurrentStep As Integer
    Public ResponseLength As Integer = 21

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
        '*** Strat Wifi Check]
        TmrWifi.Enabled = True
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
        Else
            GrpMaster.Enabled = True
            TxtMasterSSID.Text = StrCurrentSSID
            ChkConnectToID.Checked = True
        End If
    End Sub

    Private Sub ChkSkipCIB_CheckedChanged(sender As Object, e As EventArgs) Handles ChkSkipCIB.CheckedChanged
        If ChkSkipCIB.Checked Then
            GrpCIB.Enabled = False

        Else
            GrpCIB.Enabled = True
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
        TmrWifi.Interval = 2000

        Try
            If Not MyWiFi.NoWifiAvailable Then
                Dim MyWifiList As List(Of AccessPoint) = MyWiFi.GetAccessPoints()
                Dim Int_listSize As Integer = MyWifiList.Count
                PicTimerHB.Visible = Not PicTimerHB.Visible

                For Each item In MyWifiList
                    Dim S_Item As String = ""
                    If item.Name = "" Then S_Item = "Hidden" Else S_Item = item.Name
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
        '*** Make sure port is closed and disable manual version button
        BtnFirmware.Enabled = False
        MakeConnection(sIP, False, iPort)

        '*** Enable connection
        MakeConnection(sIP, True, iPort)

        GetMasterVersion = GetVersion()

        '*** allow app to update graphics
        Application.DoEvents()

        '*** Disconnect and re-enable manual version button.
        MakeConnection(sIP, False, iPort)
        BtnFirmware.Enabled = True

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
        Debug.Print("Working...")
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
                ' If stream. Then stream.Close()  '*** crashes if the stream is not open. seems to be closed with the client anyway
                If client.Connected Then client.Close()
                Debug.Print("Not Connected.")
                ShowResult = myCRC_16.DisplayResult("Connection closed.", 1, 0, 0)
            End If
            Debug.Print("Done...")
        Catch ex As SocketException
            Debug.Print("Socket Error " & ex.NativeErrorCode & " " & ex.SocketErrorCode & " " & ex.Message)

        End Try

    End Sub
    Private Sub BtnFirmware_Click(sender As Object, e As EventArgs) Handles BtnFirmware.Click
        TxtDebug.Text = ""
        Dim StrResult As String = GetMasterVersion()
        Dim Decoded As String = RT_Decode("92", StrResult)

    End Sub
    Shared Function Connect(server As [String], message As [String], replySize As Integer, MasterChannel As [String], OrderRefText As [String])
        '*** Hop count refers also to the TTL Time To Live value for the MultiCastScope 

        MasterChannel = UCase(MasterChannel)

        Dim data(Len(message) - 1) As Byte
        Dim data2SendFormatted As String = ""
        Dim dataReceivedFormatted As String = ""
        '*** Log details
        Dim debugMode As Boolean = False
        Dim debugFilename As String = FrmMain.StrRootFolder & "RT_" & Format(Now(), "yyyyMMdd") & "_" & Trim(OrderRefText) & MasterChannel & ".log"
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
                                sError = "No Error"
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
        '  Connect = "Result"
    End Function 'Connect
    Private Function RT_Decode(CMD As String, StrPacket As String) As String
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
            '*** NOTE THIS METHOD DOES NOT RETURN THE  SECOD CHECKSUM BYTE, BUT IT ISNOT NEEDED HERE
            '*** AS IT HAS ALREADY BEEN CHECKED
        End While

        Select Case CMD
            Case "EA", "EB"
                Debug.Print(CMD & " or EB Should equal " & StrResultArray(5))
                TxtDebug.Text = "Read Config Result"
            Case "92"
                Debug.Print(CMD & " Should equal " & StrResultArray(5))
                TxtDebug.Text = "Get Firmware Version"
            Case "BE"
                Debug.Print(CMD & " Should equal " & StrResultArray(5))
                TxtDebug.Text = "Get Master or CIB GUID"
            Case "DA"
                Debug.Print(CMD & " Should equal " & StrResultArray(5))
                TxtDebug.Text = "Get Bus Power Status"
        End Select
    End Function
    Private Sub BtnConfig_Click(sender As Object, e As EventArgs) Handles BtnConfig.Click
        Dim StrResult As String = ReadAllTopology()
        Dim Decoded As String = RT_Decode("EA", StrResult)
    End Sub
    Private Function ReadAllTopology()
        '*** Connect to device
        '*** Make sure it is disconnected to start with
        MakeConnection(sIP, False, iPort)

        '** Clear debug window
        Me.TxtDebug.Text = ""

        '*** Enable connection and disable manual connection button
        MakeConnection(sIP, True, iPort)
        BtnConfig.Enabled = False


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

        Debug.Print("Command: Read topology 0xEA")
        TxtDebug.Text = "Command: Read topology 0xEA"

        StrTextToLog = "Getting Configuration"
        S_LogFileText = "_" & TxtMasterSSID.Text & "_" & TxtCIBid.Text
        Dim StrResult As String = Connect(sIP, data2Send, ResponseLength, S_LogFileText, TxtOrderRef.Text)
        Debug.Print("****** Configuration " & StrResult)
        TxtDebug.Text = "****** Configuration " & StrResult

        '************************************
        '*** Return result for decoding
        ReadAllTopology = Trim(StrResult)
        '************************************
        '*** allow app to update graphics
        Application.DoEvents()

        '*** Disconnect and re-enable manual connect button.
        MakeConnection(sIP, False, iPort)
        BtnConfig.Enabled = True

    End Function
    Private Function ReadMGuid(Device As String)
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

        StrTextToLog = " Getting Configuration"
        S_LogFileText = " _" & TxtMasterSSID.Text & " _" & TxtCIBid.Text
        Dim StrResult As String = Connect(sIP, data2Send, 47, S_LogFileText, TxtOrderRef.Text)
        Debug.Print(" ****** GUID " & StrResult)
        TxtDebug.Text = " ****** GUID " & StrResult

        '************************************
        '*** Return result for decoding
        ReadMGuid = Trim(StrResult)
        '************************************
        '*** allow app to update graphics
        Application.DoEvents()

        '*** Disconnect and re-enable manual connect button.
        MakeConnection(sIP, False, iPort)
        BtnMguid.Enabled = True
    End Function

    Private Sub BtnMguid_Click(sender As Object, e As EventArgs) Handles BtnMguid.Click
        Dim StrResult As String = ReadMGuid("0")
        Dim Decoded As String = RT_Decode("BE", StrResult)

    End Sub

    Private Sub BtnCIBID_Click(sender As Object, e As EventArgs) Handles BtnCIBID.Click
        Dim StrResult As String = ReadMGuid("1")
        Dim Decoded As String = RT_Decode("BE", StrResult)
    End Sub

    Private Sub BtnBusPwr_Click(sender As Object, e As EventArgs) Handles BtnBusPwr.Click
        Dim StrResult As String = ReadBusPower()
        Dim Decoded As String = RT_Decode("DA", StrResult)
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

        '*** allow app to update graphics
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

                        ' Threading.Thread.Sleep(500)
                        Debug.Print(" ********************* In WiFiAutoConnect " & item.Name)
                        Application.DoEvents()
                        Select Case UCase(MyWiFi.ConnectionStatus.ToString)
                            Case " CONNECTED"
                                SConnectedWiFi = item.Name
                                LblWifi.ForeColor = Color.Green
                                LblWifi.Text = " WiFi " & MyWiFi.ConnectionStatus.ToString & " to " & SConnectedWiFi
                                Debug.Print(" ********************* CONNECTED WiFiAutoConnect *********************")
                                Application.DoEvents()
                                RetValue = 1
                                Exit For
                            Case Else
                                LblWifi.ForeColor = Color.Red
                                LblWifi.Text = " WiFi " & MyWiFi.ConnectionStatus.ToString
                                Debug.Print(" ********************* NOT CONNECTED WiFiAutoConnect *********************")
                                Application.DoEvents()
                                RetValue = 0
                        End Select
                        Application.DoEvents()
                    End If
                Next
            End If
        Catch Err As Exception
            Debug.Print(" Catch - No Wifi")
            LblWifi.ForeColor = Color.Red
            LblWifi.Text = " No WiFi Card Detected"
        End Try
        Threading.Thread.Sleep(500)

        Debug.Print(" CONNECTED_WIFI " & SConnectedWiFi)
        TmrWifi.Enabled = True
        Return (RetValue)
        ' Use: MyWiFi.Disconnect()
        ' To close the current connection.

    End Function
    Private Sub BtnWifiConnect_Click(sender As Object, e As EventArgs) Handles BtnWifiConnect.Click
        Dim StrConnectionID As String
        If TxtMasterSSID.Text <> "" Then
            If ChkConnectToID.Checked Then
                '*** connect to dispalyed SSID instead of RT_001
                StrConnectionID = "RT_" & Trim(TxtMasterSSID.Text)
            Else
                '*** Connect to default SSID
                StrConnectionID = "RT_001"
            End If
            Dim IntResult As Integer = WiFiAutoConnect(StrConnectionID, "rapidtest")

        Else

        End if
    End Sub
End Class
