<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSettings))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtDefaultMaster = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LblLogPath = New System.Windows.Forms.Label()
        Me.BtnSetLogPath = New System.Windows.Forms.Button()
        Me.BtnSettingCancel = New System.Windows.Forms.Button()
        Me.BtnSettingsSave = New System.Windows.Forms.Button()
        Me.LogFolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(28, 31)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(120, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Default Master"
        '
        'TxtDefaultMaster
        '
        Me.TxtDefaultMaster.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtDefaultMaster.Location = New System.Drawing.Point(165, 31)
        Me.TxtDefaultMaster.Name = "TxtDefaultMaster"
        Me.TxtDefaultMaster.Size = New System.Drawing.Size(100, 26)
        Me.TxtDefaultMaster.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(32, 78)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(76, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Log Path"
        '
        'LblLogPath
        '
        Me.LblLogPath.AutoSize = True
        Me.LblLogPath.ForeColor = System.Drawing.Color.Blue
        Me.LblLogPath.Location = New System.Drawing.Point(165, 78)
        Me.LblLogPath.Name = "LblLogPath"
        Me.LblLogPath.Size = New System.Drawing.Size(15, 20)
        Me.LblLogPath.TabIndex = 3
        Me.LblLogPath.Text = "-"
        '
        'BtnSetLogPath
        '
        Me.BtnSetLogPath.Location = New System.Drawing.Point(165, 113)
        Me.BtnSetLogPath.Name = "BtnSetLogPath"
        Me.BtnSetLogPath.Size = New System.Drawing.Size(119, 45)
        Me.BtnSetLogPath.TabIndex = 4
        Me.BtnSetLogPath.Text = "Set Path"
        Me.BtnSetLogPath.UseVisualStyleBackColor = True
        '
        'BtnSettingCancel
        '
        Me.BtnSettingCancel.Location = New System.Drawing.Point(886, 193)
        Me.BtnSettingCancel.Name = "BtnSettingCancel"
        Me.BtnSettingCancel.Size = New System.Drawing.Size(102, 51)
        Me.BtnSettingCancel.TabIndex = 5
        Me.BtnSettingCancel.Text = "&Cancel"
        Me.BtnSettingCancel.UseVisualStyleBackColor = True
        '
        'BtnSettingsSave
        '
        Me.BtnSettingsSave.Location = New System.Drawing.Point(12, 193)
        Me.BtnSettingsSave.Name = "BtnSettingsSave"
        Me.BtnSettingsSave.Size = New System.Drawing.Size(102, 51)
        Me.BtnSettingsSave.TabIndex = 6
        Me.BtnSettingsSave.Text = "&Save"
        Me.BtnSettingsSave.UseVisualStyleBackColor = True
        '
        'FrmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 262)
        Me.Controls.Add(Me.BtnSettingsSave)
        Me.Controls.Add(Me.BtnSettingCancel)
        Me.Controls.Add(Me.BtnSetLogPath)
        Me.Controls.Add(Me.LblLogPath)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxtDefaultMaster)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "FrmSettings"
        Me.Text = "Settings"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents TxtDefaultMaster As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents LblLogPath As Label
    Friend WithEvents BtnSetLogPath As Button
    Friend WithEvents BtnSettingCancel As Button
    Friend WithEvents BtnSettingsSave As Button
    Friend WithEvents LogFolderBrowserDialog As FolderBrowserDialog
End Class
