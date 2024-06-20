<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F_NDBMessage
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F_NDBMessage))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LBLMessage = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.PicClose = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicClose, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 309)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(160, 20)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Press ""ESC"" to Close"
        '
        'LBLMessage
        '
        Me.LBLMessage.AutoSize = True
        Me.LBLMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBLMessage.Location = New System.Drawing.Point(137, 27)
        Me.LBLMessage.Name = "LBLMessage"
        Me.LBLMessage.Size = New System.Drawing.Size(74, 20)
        Me.LBLMessage.TabIndex = 6
        Me.LBLMessage.Text = "Message"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImage = Global.RTP.My.Resources.Resources.Symbol_Error
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(119, 109)
        Me.PictureBox1.TabIndex = 8
        Me.PictureBox1.TabStop = False
        '
        'PicClose
        '
        Me.PicClose.BackgroundImage = Global.RTP.My.Resources.Resources.Delete
        Me.PicClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PicClose.Enabled = False
        Me.PicClose.Location = New System.Drawing.Point(521, 226)
        Me.PicClose.Name = "PicClose"
        Me.PicClose.Size = New System.Drawing.Size(107, 103)
        Me.PicClose.TabIndex = 7
        Me.PicClose.TabStop = False
        '
        'F_NDBMessage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(655, 352)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.PicClose)
        Me.Controls.Add(Me.LBLMessage)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "F_NDBMessage"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Matching Device found in RT DB"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicClose, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents PicClose As PictureBox
    Friend WithEvents LBLMessage As Label
End Class
