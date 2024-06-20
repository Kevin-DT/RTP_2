<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSummary
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
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.DataSummaryGrid = New System.Windows.Forms.DataGridView()
        Me.LblSummaryResult = New System.Windows.Forms.Label()
        CType(Me.DataSummaryGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnClose
        '
        Me.BtnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnClose.BackColor = System.Drawing.Color.Red
        Me.BtnClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClose.ForeColor = System.Drawing.Color.White
        Me.BtnClose.Location = New System.Drawing.Point(1099, 572)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(96, 44)
        Me.BtnClose.TabIndex = 0
        Me.BtnClose.Text = "Close"
        Me.BtnClose.UseVisualStyleBackColor = False
        '
        'DataSummaryGrid
        '
        Me.DataSummaryGrid.AllowUserToAddRows = False
        Me.DataSummaryGrid.AllowUserToDeleteRows = False
        Me.DataSummaryGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataSummaryGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.DataSummaryGrid.Location = New System.Drawing.Point(13, 12)
        Me.DataSummaryGrid.Name = "DataSummaryGrid"
        Me.DataSummaryGrid.ReadOnly = True
        Me.DataSummaryGrid.Size = New System.Drawing.Size(1182, 554)
        Me.DataSummaryGrid.TabIndex = 1
        '
        'LblSummaryResult
        '
        Me.LblSummaryResult.AutoSize = True
        Me.LblSummaryResult.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSummaryResult.ForeColor = System.Drawing.Color.Blue
        Me.LblSummaryResult.Location = New System.Drawing.Point(13, 602)
        Me.LblSummaryResult.Name = "LblSummaryResult"
        Me.LblSummaryResult.Size = New System.Drawing.Size(50, 18)
        Me.LblSummaryResult.TabIndex = 2
        Me.LblSummaryResult.Text = "Result"
        Me.LblSummaryResult.Visible = False
        '
        'FrmSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1207, 628)
        Me.ControlBox = False
        Me.Controls.Add(Me.LblSummaryResult)
        Me.Controls.Add(Me.DataSummaryGrid)
        Me.Controls.Add(Me.BtnClose)
        Me.Name = "FrmSummary"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Summary Info"
        CType(Me.DataSummaryGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents BtnClose As Button
    Friend WithEvents DataSummaryGrid As DataGridView
    Friend WithEvents LblSummaryResult As Label
End Class
