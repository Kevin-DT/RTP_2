Public Class CRC_16
    Const Polynomial As UShort = &HA001
    Private table As UShort() = New UShort(255) {}


    'create the lookup table
    Public Sub New()
        Dim value As UShort
        Dim temp As UShort
        For i As UShort = 0 To table.Length - 1
            value = 0
            temp = i
            For j As Byte = 0 To 7
                If ((value Xor temp) And &H1) <> 0 Then
                    value = CUShort((value >> 1) Xor Polynomial)
                Else
                    value >>= 1
                End If
                temp >>= 1

            Next
            table(i) = value
            ' Debug.Print("CRC-16: " & table(i))
        Next
    End Sub
    Public Function DisplayResult(sData As String, append As Boolean, pollSuccess As Integer, iErrCode As Integer) As Integer
        If append Then
            FrmMain.TxtDebug.Text = FrmMain.TxtDebug.Text & vbCrLf & sData
        Else
            FrmMain.TxtDebug.Text = sData
        End If
        'Select Case pollSuccess
        '    Case 1
        '        FrmMain.cycleSuccess = FrmMain.cycleSuccess + 1
        '    Case -1
        '        FrmMain.cycleFail = FrmMain.cycleFail + 1
        'End Select


        'Select Case iErrCode
        '    Case 1
        '        FrmMain.iErr(0) = FrmMain.iErr(0) + 1
        '    Case 2
        '        FrmMain.iErr(1) = FrmMain.iErr(1) + 1
        '    Case 3
        '        FrmMain.iErr(2) = FrmMain.iErr(2) + 1
        '    Case Else

        'End Select

        'Select Case FrmMain.iCurrentStep

        '    '**** 14/10/2020
        '    '*** Rewrote this section to try to get rid of and error that was occuring sometimes.
        '    '*** So far so good.
        '    Case 3
        '        '** Read GUID before setting new GUID
        '        If Strings.Len(sData) > 30 Then
        '            Dim sCommand = Strings.Mid(sData, 24, 2)
        '            If Strings.UCase(sCommand) = "2F" Then FrmMain.sOriginalGUID = Strings.Mid(sData, 18, 107)
        '            Application.DoEvents()
        '        End If
        '    Case 5
        '        '** Read GUID after setting new GUID
        '        Debug.Print("***** Start Step 5 *****")
        '        Debug.Print("Initial sData is " & sData)
        '        If Strings.Len(sData) > 30 Then
        '            Dim sCommand = Strings.Mid(sData, 24, 2)
        '            Dim ChCommand = Strings.Mid(sData, 27, 1)
        '            Debug.Print("Processing sData is " & sData)
        '            Debug.Print("SCommand is " & sCommand)
        '            If Strings.UCase(sCommand) = "2F" Then
        '                Select Case ChCommand
        '                    Case "0"
        '                        If Not FrmMain.sCB_SetGUID Then
        '                            FrmMain.sNewGUID = Strings.Mid(sData, 18, 107)
        '                            Debug.Print("SCommand for Master is " & sCommand & " GUID = " & FrmMain.sNewGUID)
        '                        End If
        '                    Case "1", "2" '(CIB's)
        '                        FrmMain.sCB_NewGUID = Strings.Mid(sData, 32, 107)
        '                        Debug.Print("SCommand for CIB is " & sCommand & " GUID = " & FrmMain.sCB_NewGUID)
        '                    Case Else

        '                End Select


        '            End If
        '            Debug.Print("***** Finish Step 5 *****")
        '            Application.DoEvents()
        '        End If
        '    Case 8
        '        '** Read GUID that has been saved
        '        If Strings.Len(sData) > 30 Then
        '            Dim sCommand = Strings.Mid(sData, 24, 2)
        '            If Strings.UCase(sCommand) = "2F" Then FrmMain.sCheckedGUID = Strings.Mid(sData, 18, 107)
        '            Application.DoEvents()
        '        End If
        '    Case 9
        '        If Strings.InStr(sData, "Channel") Then
        '            FrmMain.sTripResult = sData
        '        End If

        '    Case Else

        'End Select


        Return 1

    End Function
    Public Sub SetVersion(iVMin As Integer, iVMaj As Integer)
        FrmMain.iVersionMinor = iVMin
        FrmMain.iVersionMajor = iVMaj
        FrmMain.IntResponseLength = 21
        ' FrmMain.Lbl_Firmware.Text = "Firmware = " & FrmMain.iVersionMajor.ToString & "." & FrmMain.iVersionMinor.ToString
        'Dim SmasterType As String = "Master is " & FrmMain.sMasterType
        'FrmMain.lblMaster.Text = SmasterType

        'Return (SmasterType)

    End Sub
    Public Sub SetChannelCount(iChannels As Integer)
        'FrmMain.TxtChannelLimit.Text = (iChannels).ToString
        'FrmMain.Int_GlobalChannelCount = (iChannels).ToString
    End Sub

    Public Sub GetMasterTopology(iMasterModel)
        Select Case iMasterModel
            Case 3
                FrmMain.SMasterType = "RTMC8"
            Case Else
                FrmMain.SMasterType = "RTMC8F5"
        End Select


        '        FrmMain.lblMaster.Text = "Master is " & FrmMain.sMasterType
    End Sub
    Public Function DisplayTestResults(sData As String)
        '** iType is the element to Update. sData is the result data to use
        '** 0 = Checkbox
        Dim name As String = ""

        '** Find group box and check if the controls are checkboxes.
        For Each GroupBoxCntrol As Control In FrmMain.Controls
            For Each cntrl As Control In GroupBoxCntrol.Controls
                'If cntrl.GetType() = CheckBox Then
                For x = 1 To FrmMain.Controls.Count
                    '** Pad out if only 1 didgit
                    Select Case sData.Length
                        Case 1
                            name = "chk0" & sData
                        Case 2
                            name = "chk" & sData
                        Case Else
                            Debug.Print("Channel tested = " & sData)
                    End Select

                    If cntrl.Name = name Then
                        '** Clear checkboxes
                        Dim abc As CheckBox = CType(cntrl, CheckBox)
                        abc.Checked = True
                        Application.DoEvents()
                    End If
                Next
            Next

        Next

        Return 1
    End Function
    'compute checksum by doing xor and shift with table
    Public Function ComputeChecksum(bytes As Byte()) As UShort
        Dim crc As UShort = 0
        'Dim index As Byte = Convert.ToByte((crc Xor bytes(i)))
        'crc = CUShort((crc >> 8) Xor table(index))
        For Each data As Byte In bytes
            crc = CUShort((crc >> 8) Xor table((crc Xor data) And &HFF))
            Debug.Print("CRC for " & data.ToString & " " & crc.ToString)
        Next
        Return crc
    End Function

    Public Function ComputeChecksumBytes(bytes As Byte()) As Byte()
        Dim crc As UShort = ComputeChecksum(bytes)
        Dim decimalvalue As Byte() = BitConverter.GetBytes(crc)
        Dim hex As Byte() = New Byte(decimalvalue.Length - 1) {}
        Dim tempResult(1) As Byte

        For i As Integer = 0 To decimalvalue.Length - 1

            Dim deciString As String = decimalvalue(i).ToString()
            Dim hexString As String = Convert.ToInt32(deciString).ToString("X")
            Dim HexByte As Byte = Convert.ToByte(hexString, 16)
            hex(i) = HexByte
        Next

        tempResult(0) = hex(0)
        tempResult(1) = hex(1)
        '** Hack to swap the checksum bytes
        'hex(0) = tempResult(1)
        'hex(1) = tempResult(0)
        Return hex

    End Function
End Class
