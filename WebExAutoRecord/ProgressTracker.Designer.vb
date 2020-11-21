<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProgressTracker
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
        Me.OverallProgress = New System.Windows.Forms.ProgressBar()
        Me.FileNum = New System.Windows.Forms.Label()
        Me.DLspeed = New System.Windows.Forms.Label()
        Me.Timer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'OverallProgress
        '
        Me.OverallProgress.Location = New System.Drawing.Point(12, 37)
        Me.OverallProgress.Name = "OverallProgress"
        Me.OverallProgress.Size = New System.Drawing.Size(424, 23)
        Me.OverallProgress.TabIndex = 0
        '
        'FileNum
        '
        Me.FileNum.AutoSize = True
        Me.FileNum.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FileNum.Location = New System.Drawing.Point(7, 9)
        Me.FileNum.Name = "FileNum"
        Me.FileNum.Size = New System.Drawing.Size(88, 25)
        Me.FileNum.TabIndex = 1
        Me.FileNum.Text = "File X/Y"
        '
        'DLspeed
        '
        Me.DLspeed.AutoSize = True
        Me.DLspeed.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DLspeed.Location = New System.Drawing.Point(337, 9)
        Me.DLspeed.Name = "DLspeed"
        Me.DLspeed.Size = New System.Drawing.Size(0, 25)
        Me.DLspeed.TabIndex = 2
        '
        'Timer
        '
        Me.Timer.Enabled = True
        Me.Timer.Interval = 500
        '
        'ProgressTracker
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(448, 72)
        Me.Controls.Add(Me.DLspeed)
        Me.Controls.Add(Me.FileNum)
        Me.Controls.Add(Me.OverallProgress)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "ProgressTracker"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents FileNum As Label
    Public WithEvents DLspeed As Label
    Public WithEvents OverallProgress As ProgressBar
    Friend WithEvents Timer As Timer
End Class
