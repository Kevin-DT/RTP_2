<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim PrinterToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LblWifi = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtPurchaser = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtOrderRef = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtUser = New System.Windows.Forms.TextBox()
        Me.BtnReload = New System.Windows.Forms.Button()
        Me.BtnSave = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.TSPrinterName = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TSLogPath = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ChkSkipMaster = New System.Windows.Forms.CheckBox()
        Me.GrpMaster = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtMasterManCode = New System.Windows.Forms.TextBox()
        Me.TxtMasterSSID = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ChkConnectToID = New System.Windows.Forms.CheckBox()
        Me.ChkSkipCIB = New System.Windows.Forms.CheckBox()
        Me.GrpCIB = New System.Windows.Forms.GroupBox()
        Me.ChkCIBidOnly = New System.Windows.Forms.CheckBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtCIBManCode = New System.Windows.Forms.TextBox()
        Me.TxtCIBid = New System.Windows.Forms.TextBox()
        Me.ChkManCntrls = New System.Windows.Forms.CheckBox()
        Me.GrpManual = New System.Windows.Forms.GroupBox()
        Me.BtnBusPwr = New System.Windows.Forms.Button()
        Me.BtnCIBID = New System.Windows.Forms.Button()
        Me.BtnMguid = New System.Windows.Forms.Button()
        Me.BtnConfig = New System.Windows.Forms.Button()
        Me.BtnFirmware = New System.Windows.Forms.Button()
        Me.TxtDebug = New System.Windows.Forms.TextBox()
        Me.ChkNHP = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.BtnPrintEquipLbl = New System.Windows.Forms.Button()
        Me.BtnPrintInvLbl = New System.Windows.Forms.Button()
        Me.ChkAutoPrint = New System.Windows.Forms.CheckBox()
        Me.ChkCIBLbl = New System.Windows.Forms.CheckBox()
        Me.ChkMasterLbl = New System.Windows.Forms.CheckBox()
        Me.ChkInventoryLbl = New System.Windows.Forms.CheckBox()
        Me.ChkAutoStep = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.BtnPrintLabels = New System.Windows.Forms.Button()
        Me.BtnPrintPO = New System.Windows.Forms.Button()
        Me.BtnPackStock = New System.Windows.Forms.Button()
        Me.TmrWifi = New System.Windows.Forms.Timer(Me.components)
        Me.PicTimerHB = New System.Windows.Forms.PictureBox()
        Me.PicRTbarcode = New System.Windows.Forms.PictureBox()
        Me.ChkLAN = New System.Windows.Forms.CheckBox()
        Me.ChkBLE = New System.Windows.Forms.CheckBox()
        Me.BtnWifiConnect = New System.Windows.Forms.Button()
        PrinterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.GrpMaster.SuspendLayout()
        Me.GrpCIB.SuspendLayout()
        Me.GrpManual.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.PicTimerHB, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicRTbarcode, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PrinterToolStripMenuItem
        '
        PrinterToolStripMenuItem.Name = "PrinterToolStripMenuItem"
        PrinterToolStripMenuItem.Size = New System.Drawing.Size(224, 28)
        PrinterToolStripMenuItem.Text = "&Printer"
        AddHandler PrinterToolStripMenuItem.Click, AddressOf Me.PrinterToolStripMenuItem_Click
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(10, 2, 0, 2)
        Me.MenuStrip1.Size = New System.Drawing.Size(1465, 31)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SettingsToolStripMenuItem, PrinterToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(49, 27)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(224, 28)
        Me.SettingsToolStripMenuItem.Text = "&Settings"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(221, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(224, 28)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'LblWifi
        '
        Me.LblWifi.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblWifi.AutoSize = True
        Me.LblWifi.ForeColor = System.Drawing.Color.Red
        Me.LblWifi.Location = New System.Drawing.Point(1021, 11)
        Me.LblWifi.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblWifi.Name = "LblWifi"
        Me.LblWifi.Size = New System.Drawing.Size(120, 20)
        Me.LblWifi.TabIndex = 1
        Me.LblWifi.Text = "Not Connected"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.TxtPurchaser)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.TxtOrderRef)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.TxtUser)
        Me.GroupBox1.Location = New System.Drawing.Point(15, 39)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(1055, 134)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = " PO Details "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(645, 34)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(153, 20)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Purchaser or Stock"
        '
        'TxtPurchaser
        '
        Me.TxtPurchaser.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtPurchaser.Location = New System.Drawing.Point(805, 31)
        Me.TxtPurchaser.Margin = New System.Windows.Forms.Padding(4)
        Me.TxtPurchaser.Name = "TxtPurchaser"
        Me.TxtPurchaser.Size = New System.Drawing.Size(220, 26)
        Me.TxtPurchaser.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(298, 34)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(88, 20)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Order Ref:"
        '
        'TxtOrderRef
        '
        Me.TxtOrderRef.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtOrderRef.Location = New System.Drawing.Point(393, 31)
        Me.TxtOrderRef.Margin = New System.Windows.Forms.Padding(4)
        Me.TxtOrderRef.Name = "TxtOrderRef"
        Me.TxtOrderRef.Size = New System.Drawing.Size(220, 26)
        Me.TxtOrderRef.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(29, 34)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 20)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "User:"
        '
        'TxtUser
        '
        Me.TxtUser.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtUser.Location = New System.Drawing.Point(97, 31)
        Me.TxtUser.Margin = New System.Windows.Forms.Padding(4)
        Me.TxtUser.Name = "TxtUser"
        Me.TxtUser.Size = New System.Drawing.Size(173, 26)
        Me.TxtUser.TabIndex = 0
        '
        'BtnReload
        '
        Me.BtnReload.Location = New System.Drawing.Point(1280, 715)
        Me.BtnReload.Name = "BtnReload"
        Me.BtnReload.Size = New System.Drawing.Size(86, 58)
        Me.BtnReload.TabIndex = 3
        Me.BtnReload.Text = "&Reload"
        Me.BtnReload.UseVisualStyleBackColor = True
        '
        'BtnSave
        '
        Me.BtnSave.Location = New System.Drawing.Point(1372, 715)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(86, 58)
        Me.BtnSave.TabIndex = 4
        Me.BtnSave.Text = "&Save"
        Me.BtnSave.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSPrinterName, Me.TSLogPath})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 854)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1465, 29)
        Me.StatusStrip1.TabIndex = 5
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'TSPrinterName
        '
        Me.TSPrinterName.Name = "TSPrinterName"
        Me.TSPrinterName.Size = New System.Drawing.Size(89, 23)
        Me.TSPrinterName.Text = "No Printer"
        '
        'TSLogPath
        '
        Me.TSLogPath.Name = "TSLogPath"
        Me.TSLogPath.Size = New System.Drawing.Size(31, 23)
        Me.TSLogPath.Text = "C:\"
        '
        'ChkSkipMaster
        '
        Me.ChkSkipMaster.AutoSize = True
        Me.ChkSkipMaster.Location = New System.Drawing.Point(15, 197)
        Me.ChkSkipMaster.Name = "ChkSkipMaster"
        Me.ChkSkipMaster.Size = New System.Drawing.Size(120, 24)
        Me.ChkSkipMaster.TabIndex = 6
        Me.ChkSkipMaster.Text = "Skip Master"
        Me.ChkSkipMaster.UseVisualStyleBackColor = True
        '
        'GrpMaster
        '
        Me.GrpMaster.Controls.Add(Me.Label6)
        Me.GrpMaster.Controls.Add(Me.Label5)
        Me.GrpMaster.Controls.Add(Me.TxtMasterManCode)
        Me.GrpMaster.Controls.Add(Me.TxtMasterSSID)
        Me.GrpMaster.Controls.Add(Me.Label4)
        Me.GrpMaster.Controls.Add(Me.ChkConnectToID)
        Me.GrpMaster.Location = New System.Drawing.Point(15, 227)
        Me.GrpMaster.Name = "GrpMaster"
        Me.GrpMaster.Size = New System.Drawing.Size(1057, 141)
        Me.GrpMaster.TabIndex = 7
        Me.GrpMaster.TabStop = False
        Me.GrpMaster.Text = " Master "
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(30, 40)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(59, 25)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "SSID"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(21, 97)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(152, 20)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Manufacturer Code"
        '
        'TxtMasterManCode
        '
        Me.TxtMasterManCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtMasterManCode.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtMasterManCode.Location = New System.Drawing.Point(195, 87)
        Me.TxtMasterManCode.Name = "TxtMasterManCode"
        Me.TxtMasterManCode.Size = New System.Drawing.Size(135, 30)
        Me.TxtMasterManCode.TabIndex = 3
        '
        'TxtMasterSSID
        '
        Me.TxtMasterSSID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtMasterSSID.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtMasterSSID.Location = New System.Drawing.Point(195, 34)
        Me.TxtMasterSSID.Name = "TxtMasterSSID"
        Me.TxtMasterSSID.Size = New System.Drawing.Size(135, 30)
        Me.TxtMasterSSID.TabIndex = 2
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Blue
        Me.Label4.Location = New System.Drawing.Point(140, 37)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(49, 25)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "RT_"
        '
        'ChkConnectToID
        '
        Me.ChkConnectToID.AutoSize = True
        Me.ChkConnectToID.ForeColor = System.Drawing.Color.Blue
        Me.ChkConnectToID.Location = New System.Drawing.Point(372, 40)
        Me.ChkConnectToID.Name = "ChkConnectToID"
        Me.ChkConnectToID.Size = New System.Drawing.Size(188, 24)
        Me.ChkConnectToID.TabIndex = 0
        Me.ChkConnectToID.Text = "Connect to this SSID"
        Me.ChkConnectToID.UseVisualStyleBackColor = True
        '
        'ChkSkipCIB
        '
        Me.ChkSkipCIB.AutoSize = True
        Me.ChkSkipCIB.Location = New System.Drawing.Point(15, 385)
        Me.ChkSkipCIB.Name = "ChkSkipCIB"
        Me.ChkSkipCIB.Size = New System.Drawing.Size(96, 24)
        Me.ChkSkipCIB.TabIndex = 8
        Me.ChkSkipCIB.Text = "Skip CIB"
        Me.ChkSkipCIB.UseVisualStyleBackColor = True
        '
        'GrpCIB
        '
        Me.GrpCIB.Controls.Add(Me.ChkCIBidOnly)
        Me.GrpCIB.Controls.Add(Me.Label8)
        Me.GrpCIB.Controls.Add(Me.Label7)
        Me.GrpCIB.Controls.Add(Me.TxtCIBManCode)
        Me.GrpCIB.Controls.Add(Me.TxtCIBid)
        Me.GrpCIB.Location = New System.Drawing.Point(13, 415)
        Me.GrpCIB.Name = "GrpCIB"
        Me.GrpCIB.Size = New System.Drawing.Size(1057, 141)
        Me.GrpCIB.TabIndex = 9
        Me.GrpCIB.TabStop = False
        Me.GrpCIB.Text = " CIB "
        '
        'ChkCIBidOnly
        '
        Me.ChkCIBidOnly.AutoSize = True
        Me.ChkCIBidOnly.ForeColor = System.Drawing.Color.Blue
        Me.ChkCIBidOnly.Location = New System.Drawing.Point(374, 40)
        Me.ChkCIBidOnly.Name = "ChkCIBidOnly"
        Me.ChkCIBidOnly.Size = New System.Drawing.Size(164, 24)
        Me.ChkCIBidOnly.TabIndex = 8
        Me.ChkCIBidOnly.Text = "Configure ID Only"
        Me.ChkCIBidOnly.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(32, 39)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(69, 25)
        Me.Label8.TabIndex = 7
        Me.Label8.Text = "CIB ID"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(23, 96)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(152, 20)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "Manufacturer Code"
        '
        'TxtCIBManCode
        '
        Me.TxtCIBManCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtCIBManCode.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCIBManCode.Location = New System.Drawing.Point(197, 89)
        Me.TxtCIBManCode.Name = "TxtCIBManCode"
        Me.TxtCIBManCode.Size = New System.Drawing.Size(135, 30)
        Me.TxtCIBManCode.TabIndex = 5
        '
        'TxtCIBid
        '
        Me.TxtCIBid.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtCIBid.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCIBid.Location = New System.Drawing.Point(197, 36)
        Me.TxtCIBid.Name = "TxtCIBid"
        Me.TxtCIBid.Size = New System.Drawing.Size(135, 30)
        Me.TxtCIBid.TabIndex = 4
        '
        'ChkManCntrls
        '
        Me.ChkManCntrls.AutoSize = True
        Me.ChkManCntrls.Location = New System.Drawing.Point(15, 572)
        Me.ChkManCntrls.Name = "ChkManCntrls"
        Me.ChkManCntrls.Size = New System.Drawing.Size(150, 24)
        Me.ChkManCntrls.TabIndex = 10
        Me.ChkManCntrls.Text = "Enable Controls"
        Me.ChkManCntrls.UseVisualStyleBackColor = True
        '
        'GrpManual
        '
        Me.GrpManual.Controls.Add(Me.BtnWifiConnect)
        Me.GrpManual.Controls.Add(Me.BtnBusPwr)
        Me.GrpManual.Controls.Add(Me.BtnCIBID)
        Me.GrpManual.Controls.Add(Me.BtnMguid)
        Me.GrpManual.Controls.Add(Me.BtnConfig)
        Me.GrpManual.Controls.Add(Me.BtnFirmware)
        Me.GrpManual.Controls.Add(Me.TxtDebug)
        Me.GrpManual.Location = New System.Drawing.Point(13, 603)
        Me.GrpManual.Name = "GrpManual"
        Me.GrpManual.Size = New System.Drawing.Size(1057, 248)
        Me.GrpManual.TabIndex = 11
        Me.GrpManual.TabStop = False
        Me.GrpManual.Text = " Manual Controls "
        '
        'BtnBusPwr
        '
        Me.BtnBusPwr.Location = New System.Drawing.Point(7, 87)
        Me.BtnBusPwr.Name = "BtnBusPwr"
        Me.BtnBusPwr.Size = New System.Drawing.Size(115, 46)
        Me.BtnBusPwr.TabIndex = 20
        Me.BtnBusPwr.Text = "BUS"
        Me.BtnBusPwr.UseVisualStyleBackColor = True
        '
        'BtnCIBID
        '
        Me.BtnCIBID.Location = New System.Drawing.Point(491, 35)
        Me.BtnCIBID.Name = "BtnCIBID"
        Me.BtnCIBID.Size = New System.Drawing.Size(115, 46)
        Me.BtnCIBID.TabIndex = 19
        Me.BtnCIBID.Text = "CIB ID"
        Me.BtnCIBID.UseVisualStyleBackColor = True
        '
        'BtnMguid
        '
        Me.BtnMguid.Location = New System.Drawing.Point(370, 35)
        Me.BtnMguid.Name = "BtnMguid"
        Me.BtnMguid.Size = New System.Drawing.Size(115, 46)
        Me.BtnMguid.TabIndex = 18
        Me.BtnMguid.Text = "Master ID"
        Me.BtnMguid.UseVisualStyleBackColor = True
        '
        'BtnConfig
        '
        Me.BtnConfig.Location = New System.Drawing.Point(249, 35)
        Me.BtnConfig.Name = "BtnConfig"
        Me.BtnConfig.Size = New System.Drawing.Size(115, 46)
        Me.BtnConfig.TabIndex = 1
        Me.BtnConfig.Text = "Read Config"
        Me.BtnConfig.UseVisualStyleBackColor = True
        '
        'BtnFirmware
        '
        Me.BtnFirmware.Location = New System.Drawing.Point(128, 35)
        Me.BtnFirmware.Name = "BtnFirmware"
        Me.BtnFirmware.Size = New System.Drawing.Size(115, 46)
        Me.BtnFirmware.TabIndex = 0
        Me.BtnFirmware.Text = "Firmware"
        Me.BtnFirmware.UseVisualStyleBackColor = True
        '
        'TxtDebug
        '
        Me.TxtDebug.Location = New System.Drawing.Point(7, 176)
        Me.TxtDebug.Multiline = True
        Me.TxtDebug.Name = "TxtDebug"
        Me.TxtDebug.Size = New System.Drawing.Size(1044, 66)
        Me.TxtDebug.TabIndex = 17
        '
        'ChkNHP
        '
        Me.ChkNHP.AutoSize = True
        Me.ChkNHP.Location = New System.Drawing.Point(28, 25)
        Me.ChkNHP.Name = "ChkNHP"
        Me.ChkNHP.Size = New System.Drawing.Size(67, 24)
        Me.ChkNHP.TabIndex = 6
        Me.ChkNHP.Text = "NHP"
        Me.ChkNHP.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.BtnPrintEquipLbl)
        Me.GroupBox2.Controls.Add(Me.BtnPrintInvLbl)
        Me.GroupBox2.Controls.Add(Me.ChkAutoPrint)
        Me.GroupBox2.Controls.Add(Me.ChkCIBLbl)
        Me.GroupBox2.Controls.Add(Me.ChkMasterLbl)
        Me.GroupBox2.Controls.Add(Me.ChkInventoryLbl)
        Me.GroupBox2.Controls.Add(Me.ChkNHP)
        Me.GroupBox2.Location = New System.Drawing.Point(1110, 39)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(329, 250)
        Me.GroupBox2.TabIndex = 12
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = " Labels "
        '
        'BtnPrintEquipLbl
        '
        Me.BtnPrintEquipLbl.ForeColor = System.Drawing.Color.Red
        Me.BtnPrintEquipLbl.Location = New System.Drawing.Point(192, 114)
        Me.BtnPrintEquipLbl.Name = "BtnPrintEquipLbl"
        Me.BtnPrintEquipLbl.Size = New System.Drawing.Size(101, 44)
        Me.BtnPrintEquipLbl.TabIndex = 12
        Me.BtnPrintEquipLbl.Text = "Print"
        Me.BtnPrintEquipLbl.UseVisualStyleBackColor = True
        '
        'BtnPrintInvLbl
        '
        Me.BtnPrintInvLbl.ForeColor = System.Drawing.Color.Blue
        Me.BtnPrintInvLbl.Location = New System.Drawing.Point(192, 54)
        Me.BtnPrintInvLbl.Name = "BtnPrintInvLbl"
        Me.BtnPrintInvLbl.Size = New System.Drawing.Size(101, 44)
        Me.BtnPrintInvLbl.TabIndex = 11
        Me.BtnPrintInvLbl.Text = "Print"
        Me.BtnPrintInvLbl.UseVisualStyleBackColor = True
        '
        'ChkAutoPrint
        '
        Me.ChkAutoPrint.AutoSize = True
        Me.ChkAutoPrint.Location = New System.Drawing.Point(28, 201)
        Me.ChkAutoPrint.Name = "ChkAutoPrint"
        Me.ChkAutoPrint.Size = New System.Drawing.Size(244, 24)
        Me.ChkAutoPrint.TabIndex = 10
        Me.ChkAutoPrint.Text = "Auto Print Equipment Labels"
        Me.ChkAutoPrint.UseVisualStyleBackColor = True
        '
        'ChkCIBLbl
        '
        Me.ChkCIBLbl.AutoSize = True
        Me.ChkCIBLbl.ForeColor = System.Drawing.Color.Red
        Me.ChkCIBLbl.Location = New System.Drawing.Point(28, 142)
        Me.ChkCIBLbl.Name = "ChkCIBLbl"
        Me.ChkCIBLbl.Size = New System.Drawing.Size(105, 24)
        Me.ChkCIBLbl.TabIndex = 9
        Me.ChkCIBLbl.Text = "CIB Label"
        Me.ChkCIBLbl.UseVisualStyleBackColor = True
        '
        'ChkMasterLbl
        '
        Me.ChkMasterLbl.AutoSize = True
        Me.ChkMasterLbl.ForeColor = System.Drawing.Color.Red
        Me.ChkMasterLbl.Location = New System.Drawing.Point(28, 103)
        Me.ChkMasterLbl.Name = "ChkMasterLbl"
        Me.ChkMasterLbl.Size = New System.Drawing.Size(129, 24)
        Me.ChkMasterLbl.TabIndex = 8
        Me.ChkMasterLbl.Text = "Master Label"
        Me.ChkMasterLbl.UseVisualStyleBackColor = True
        '
        'ChkInventoryLbl
        '
        Me.ChkInventoryLbl.AutoSize = True
        Me.ChkInventoryLbl.ForeColor = System.Drawing.Color.Blue
        Me.ChkInventoryLbl.Location = New System.Drawing.Point(28, 64)
        Me.ChkInventoryLbl.Name = "ChkInventoryLbl"
        Me.ChkInventoryLbl.Size = New System.Drawing.Size(98, 24)
        Me.ChkInventoryLbl.TabIndex = 7
        Me.ChkInventoryLbl.Text = "Inventory"
        Me.ChkInventoryLbl.UseVisualStyleBackColor = True
        '
        'ChkAutoStep
        '
        Me.ChkAutoStep.AutoSize = True
        Me.ChkAutoStep.Checked = True
        Me.ChkAutoStep.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChkAutoStep.Location = New System.Drawing.Point(778, 200)
        Me.ChkAutoStep.Name = "ChkAutoStep"
        Me.ChkAutoStep.Size = New System.Drawing.Size(150, 24)
        Me.ChkAutoStep.TabIndex = 13
        Me.ChkAutoStep.Text = "Auto Step Mode"
        Me.ChkAutoStep.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.BtnPrintLabels)
        Me.GroupBox3.Controls.Add(Me.BtnPrintPO)
        Me.GroupBox3.Controls.Add(Me.BtnPackStock)
        Me.GroupBox3.Location = New System.Drawing.Point(1110, 295)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(329, 156)
        Me.GroupBox3.TabIndex = 15
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = " Pack Stock "
        '
        'BtnPrintLabels
        '
        Me.BtnPrintLabels.Location = New System.Drawing.Point(170, 87)
        Me.BtnPrintLabels.Name = "BtnPrintLabels"
        Me.BtnPrintLabels.Size = New System.Drawing.Size(123, 47)
        Me.BtnPrintLabels.TabIndex = 2
        Me.BtnPrintLabels.Text = "Labels"
        Me.BtnPrintLabels.UseVisualStyleBackColor = True
        '
        'BtnPrintPO
        '
        Me.BtnPrintPO.Location = New System.Drawing.Point(170, 27)
        Me.BtnPrintPO.Name = "BtnPrintPO"
        Me.BtnPrintPO.Size = New System.Drawing.Size(123, 47)
        Me.BtnPrintPO.TabIndex = 1
        Me.BtnPrintPO.Text = "P.O."
        Me.BtnPrintPO.UseVisualStyleBackColor = True
        '
        'BtnPackStock
        '
        Me.BtnPackStock.BackgroundImage = Global.RTP.My.Resources.Resources.Package
        Me.BtnPackStock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BtnPackStock.Location = New System.Drawing.Point(28, 27)
        Me.BtnPackStock.Name = "BtnPackStock"
        Me.BtnPackStock.Size = New System.Drawing.Size(111, 107)
        Me.BtnPackStock.TabIndex = 0
        Me.BtnPackStock.UseVisualStyleBackColor = True
        '
        'TmrWifi
        '
        '
        'PicTimerHB
        '
        Me.PicTimerHB.BackgroundImage = Global.RTP.My.Resources.Resources.circle_blue
        Me.PicTimerHB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PicTimerHB.Location = New System.Drawing.Point(985, 7)
        Me.PicTimerHB.Name = "PicTimerHB"
        Me.PicTimerHB.Size = New System.Drawing.Size(29, 24)
        Me.PicTimerHB.TabIndex = 16
        Me.PicTimerHB.TabStop = False
        '
        'PicRTbarcode
        '
        Me.PicRTbarcode.Image = Global.RTP.My.Resources.Resources.barcode
        Me.PicRTbarcode.Location = New System.Drawing.Point(1110, 779)
        Me.PicRTbarcode.Name = "PicRTbarcode"
        Me.PicRTbarcode.Size = New System.Drawing.Size(348, 72)
        Me.PicRTbarcode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PicRTbarcode.TabIndex = 14
        Me.PicRTbarcode.TabStop = False
        '
        'ChkLAN
        '
        Me.ChkLAN.AutoSize = True
        Me.ChkLAN.Location = New System.Drawing.Point(279, 203)
        Me.ChkLAN.Name = "ChkLAN"
        Me.ChkLAN.Size = New System.Drawing.Size(64, 24)
        Me.ChkLAN.TabIndex = 18
        Me.ChkLAN.Text = "LAN"
        Me.ChkLAN.UseVisualStyleBackColor = True
        '
        'ChkBLE
        '
        Me.ChkBLE.AutoSize = True
        Me.ChkBLE.Location = New System.Drawing.Point(422, 206)
        Me.ChkBLE.Name = "ChkBLE"
        Me.ChkBLE.Size = New System.Drawing.Size(64, 24)
        Me.ChkBLE.TabIndex = 19
        Me.ChkBLE.Text = "BLE"
        Me.ChkBLE.UseVisualStyleBackColor = True
        '
        'BtnWifiConnect
        '
        Me.BtnWifiConnect.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.BtnWifiConnect.Location = New System.Drawing.Point(7, 35)
        Me.BtnWifiConnect.Name = "BtnWifiConnect"
        Me.BtnWifiConnect.Size = New System.Drawing.Size(115, 46)
        Me.BtnWifiConnect.TabIndex = 21
        Me.BtnWifiConnect.Text = "Connect"
        Me.BtnWifiConnect.UseVisualStyleBackColor = False
        '
        'FrmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1465, 883)
        Me.Controls.Add(Me.ChkBLE)
        Me.Controls.Add(Me.ChkLAN)
        Me.Controls.Add(Me.PicTimerHB)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.PicRTbarcode)
        Me.Controls.Add(Me.ChkAutoStep)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GrpManual)
        Me.Controls.Add(Me.ChkManCntrls)
        Me.Controls.Add(Me.GrpCIB)
        Me.Controls.Add(Me.ChkSkipCIB)
        Me.Controls.Add(Me.GrpMaster)
        Me.Controls.Add(Me.ChkSkipMaster)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.BtnSave)
        Me.Controls.Add(Me.BtnReload)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.LblWifi)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "FrmMain"
        Me.Text = "RT Programmer"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.GrpMaster.ResumeLayout(False)
        Me.GrpMaster.PerformLayout()
        Me.GrpCIB.ResumeLayout(False)
        Me.GrpCIB.PerformLayout()
        Me.GrpManual.ResumeLayout(False)
        Me.GrpManual.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        CType(Me.PicTimerHB, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicRTbarcode, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LblWifi As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents TxtUser As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtPurchaser As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtOrderRef As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents BtnReload As Button
    Friend WithEvents BtnSave As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents TSPrinterName As ToolStripStatusLabel
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents TSLogPath As ToolStripStatusLabel
    Friend WithEvents ChkSkipMaster As CheckBox
    Friend WithEvents GrpMaster As GroupBox
    Friend WithEvents ChkConnectToID As CheckBox
    Friend WithEvents TxtMasterSSID As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents TxtMasterManCode As TextBox
    Friend WithEvents ChkSkipCIB As CheckBox
    Friend WithEvents GrpCIB As GroupBox
    Friend WithEvents ChkCIBidOnly As CheckBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents TxtCIBManCode As TextBox
    Friend WithEvents TxtCIBid As TextBox
    Friend WithEvents ChkManCntrls As CheckBox
    Friend WithEvents GrpManual As GroupBox
    Friend WithEvents ChkNHP As CheckBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents ChkInventoryLbl As CheckBox
    Friend WithEvents ChkAutoPrint As CheckBox
    Friend WithEvents ChkCIBLbl As CheckBox
    Friend WithEvents ChkMasterLbl As CheckBox
    Friend WithEvents BtnPrintEquipLbl As Button
    Friend WithEvents BtnPrintInvLbl As Button
    Friend WithEvents ChkAutoStep As CheckBox
    Friend WithEvents PicRTbarcode As PictureBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents BtnPackStock As Button
    Friend WithEvents BtnPrintPO As Button
    Friend WithEvents BtnPrintLabels As Button
    Friend WithEvents TmrWifi As Timer
    Friend WithEvents PicTimerHB As PictureBox
    Friend WithEvents BtnFirmware As Button
    Friend WithEvents BtnConfig As Button
    Friend WithEvents TxtDebug As TextBox
    Friend WithEvents ChkLAN As CheckBox
    Friend WithEvents ChkBLE As CheckBox
    Friend WithEvents BtnMguid As Button
    Friend WithEvents BtnCIBID As Button
    Friend WithEvents BtnBusPwr As Button
    Friend WithEvents BtnWifiConnect As Button
End Class
