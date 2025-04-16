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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSummary))
        Me.DataSummaryGrid = New System.Windows.Forms.DataGridView()
        Me.LblSummaryResult = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.BtnRefresh = New System.Windows.Forms.Button()
        CType(Me.DataSummaryGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataSummaryGrid
        '
        Me.DataSummaryGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataSummaryGrid.Location = New System.Drawing.Point(12, 12)
        Me.DataSummaryGrid.Name = "DataSummaryGrid"
        Me.DataSummaryGrid.Size = New System.Drawing.Size(1057, 485)
        Me.DataSummaryGrid.TabIndex = 0
        '
        'LblSummaryResult
        '
        Me.LblSummaryResult.AutoSize = True
        Me.LblSummaryResult.Location = New System.Drawing.Point(12, 511)
        Me.LblSummaryResult.Name = "LblSummaryResult"
        Me.LblSummaryResult.Size = New System.Drawing.Size(37, 13)
        Me.LblSummaryResult.TabIndex = 1
        Me.LblSummaryResult.Text = "Result"
        '
        'BtnClose
        '
        Me.BtnClose.ForeColor = System.Drawing.Color.Red
        Me.BtnClose.Location = New System.Drawing.Point(988, 503)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(81, 29)
        Me.BtnClose.TabIndex = 2
        Me.BtnClose.Text = "&Close"
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'BtnRefresh
        '
        Me.BtnRefresh.Location = New System.Drawing.Point(803, 503)
        Me.BtnRefresh.Name = "BtnRefresh"
        Me.BtnRefresh.Size = New System.Drawing.Size(81, 29)
        Me.BtnRefresh.TabIndex = 3
        Me.BtnRefresh.Text = "Refresh"
        Me.BtnRefresh.UseVisualStyleBackColor = True
        '
        'FrmSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1081, 538)
        Me.Controls.Add(Me.BtnRefresh)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.LblSummaryResult)
        Me.Controls.Add(Me.DataSummaryGrid)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FrmSummary"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Summary"
        CType(Me.DataSummaryGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataSummaryGrid As DataGridView
    Friend WithEvents LblSummaryResult As Label
    Friend WithEvents BtnClose As Button
    Friend WithEvents BtnRefresh As Button
End Class
