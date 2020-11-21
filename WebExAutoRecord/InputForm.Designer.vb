<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InputForm
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
        Me.OK = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.InputText = New System.Windows.Forms.TextBox()
        Me.Skip = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'OK
        '
        Me.OK.Location = New System.Drawing.Point(334, 32)
        Me.OK.Name = "OK"
        Me.OK.Size = New System.Drawing.Size(75, 23)
        Me.OK.TabIndex = 0
        Me.OK.Text = "OK"
        Me.OK.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(145, 20)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Please input your X"
        '
        'InputText
        '
        Me.InputText.Location = New System.Drawing.Point(12, 34)
        Me.InputText.Name = "InputText"
        Me.InputText.Size = New System.Drawing.Size(293, 20)
        Me.InputText.TabIndex = 2
        '
        'Skip
        '
        Me.Skip.Location = New System.Drawing.Point(334, 3)
        Me.Skip.Name = "Skip"
        Me.Skip.Size = New System.Drawing.Size(75, 23)
        Me.Skip.TabIndex = 3
        Me.Skip.Text = "Skip"
        Me.Skip.UseVisualStyleBackColor = True
        Me.Skip.Visible = False
        '
        'InputForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(421, 66)
        Me.Controls.Add(Me.Skip)
        Me.Controls.Add(Me.InputText)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.OK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "InputForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Input"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents OK As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents InputText As TextBox
    Friend WithEvents Skip As Button
End Class
