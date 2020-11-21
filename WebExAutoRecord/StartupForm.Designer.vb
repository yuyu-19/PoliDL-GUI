<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StartupForm
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
        Me.Question = New System.Windows.Forms.Label()
        Me.localmode = New System.Windows.Forms.Button()
        Me.downloadmode = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Question
        '
        Me.Question.AutoSize = True
        Me.Question.Location = New System.Drawing.Point(343, 19)
        Me.Question.Name = "Question"
        Me.Question.Size = New System.Drawing.Size(39, 13)
        Me.Question.TabIndex = 0
        Me.Question.Text = "Label1"
        '
        'localmode
        '
        Me.localmode.Location = New System.Drawing.Point(656, 46)
        Me.localmode.Name = "localmode"
        Me.localmode.Size = New System.Drawing.Size(77, 19)
        Me.localmode.TabIndex = 1
        Me.localmode.Text = "Button1"
        Me.localmode.UseVisualStyleBackColor = True
        '
        'downloadmode
        '
        Me.downloadmode.Location = New System.Drawing.Point(3, 46)
        Me.downloadmode.Name = "downloadmode"
        Me.downloadmode.Size = New System.Drawing.Size(77, 19)
        Me.downloadmode.TabIndex = 2
        Me.downloadmode.Text = "Button2"
        Me.downloadmode.UseVisualStyleBackColor = True
        '
        'StartupForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 89)
        Me.Controls.Add(Me.downloadmode)
        Me.Controls.Add(Me.localmode)
        Me.Controls.Add(Me.Question)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "StartupForm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Question As Label
    Friend WithEvents localmode As Button
    Friend WithEvents downloadmode As Button
End Class
