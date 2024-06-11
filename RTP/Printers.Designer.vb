<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Printers
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Printers))
        Me.LblSelectedPrinter = New System.Windows.Forms.Label()
        Me.BtnPrintClose = New System.Windows.Forms.Button()
        Me.BtnPrintSave = New System.Windows.Forms.Button()
        Me.ListPrinters = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'LblSelectedPrinter
        '
        Me.LblSelectedPrinter.AutoSize = True
        Me.LblSelectedPrinter.Location = New System.Drawing.Point(12, 426)
        Me.LblSelectedPrinter.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblSelectedPrinter.Name = "LblSelectedPrinter"
        Me.LblSelectedPrinter.Size = New System.Drawing.Size(105, 20)
        Me.LblSelectedPrinter.TabIndex = 0
        Me.LblSelectedPrinter.Text = "Not Selected"
        '
        'BtnPrintClose
        '
        Me.BtnPrintClose.Location = New System.Drawing.Point(448, 453)
        Me.BtnPrintClose.Name = "BtnPrintClose"
        Me.BtnPrintClose.Size = New System.Drawing.Size(108, 50)
        Me.BtnPrintClose.TabIndex = 1
        Me.BtnPrintClose.Text = "&Cancel"
        Me.BtnPrintClose.UseVisualStyleBackColor = True
        '
        'BtnPrintSave
        '
        Me.BtnPrintSave.Location = New System.Drawing.Point(12, 453)
        Me.BtnPrintSave.Name = "BtnPrintSave"
        Me.BtnPrintSave.Size = New System.Drawing.Size(108, 50)
        Me.BtnPrintSave.TabIndex = 2
        Me.BtnPrintSave.Text = "&Save"
        Me.BtnPrintSave.UseVisualStyleBackColor = True
        '
        'ListPrinters
        '
        Me.ListPrinters.FormattingEnabled = True
        Me.ListPrinters.ItemHeight = 20
        Me.ListPrinters.Location = New System.Drawing.Point(12, 12)
        Me.ListPrinters.Name = "ListPrinters"
        Me.ListPrinters.Size = New System.Drawing.Size(544, 404)
        Me.ListPrinters.TabIndex = 3
        '
        'Printers
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(568, 514)
        Me.Controls.Add(Me.ListPrinters)
        Me.Controls.Add(Me.BtnPrintSave)
        Me.Controls.Add(Me.BtnPrintClose)
        Me.Controls.Add(Me.LblSelectedPrinter)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "Printers"
        Me.Text = "Printers"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LblSelectedPrinter As Label
    Friend WithEvents BtnPrintClose As Button
    Friend WithEvents BtnPrintSave As Button
    Friend WithEvents ListPrinters As ListBox
End Class
