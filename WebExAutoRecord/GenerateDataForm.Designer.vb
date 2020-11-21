<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class GenerateDataForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Generate = New System.Windows.Forms.Button()
        Me.SavePath = New System.Windows.Forms.TextBox()
        Me.SavePathTitle = New System.Windows.Forms.TextBox()
        Me.Browse = New System.Windows.Forms.Button()
        Me.BrowseFile = New System.Windows.Forms.Button()
        Me.info = New System.Windows.Forms.TextBox()
        Me.HtmlPath = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'Generate
        '
        Me.Generate.Location = New System.Drawing.Point(341, 116)
        Me.Generate.Name = "Generate"
        Me.Generate.Size = New System.Drawing.Size(107, 23)
        Me.Generate.TabIndex = 2
        Me.Generate.Text = "Generate"
        Me.Generate.UseVisualStyleBackColor = True
        '
        'SavePath
        '
        Me.SavePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.SavePath.Location = New System.Drawing.Point(9, 118)
        Me.SavePath.Name = "SavePath"
        Me.SavePath.Size = New System.Drawing.Size(226, 23)
        Me.SavePath.TabIndex = 3
        '
        'SavePathTitle
        '
        Me.SavePathTitle.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.SavePathTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.SavePathTitle.Location = New System.Drawing.Point(9, 92)
        Me.SavePathTitle.Name = "SavePathTitle"
        Me.SavePathTitle.ReadOnly = True
        Me.SavePathTitle.Size = New System.Drawing.Size(439, 20)
        Me.SavePathTitle.TabIndex = 4
        Me.SavePathTitle.Text = "Recordings folder"
        '
        'Browse
        '
        Me.Browse.Location = New System.Drawing.Point(241, 116)
        Me.Browse.Name = "Browse"
        Me.Browse.Size = New System.Drawing.Size(59, 23)
        Me.Browse.TabIndex = 5
        Me.Browse.Text = "Browse"
        Me.Browse.UseVisualStyleBackColor = True
        '
        'BrowseFile
        '
        Me.BrowseFile.Location = New System.Drawing.Point(241, 47)
        Me.BrowseFile.Name = "BrowseFile"
        Me.BrowseFile.Size = New System.Drawing.Size(59, 23)
        Me.BrowseFile.TabIndex = 8
        Me.BrowseFile.Text = "Browse"
        Me.BrowseFile.UseVisualStyleBackColor = True
        '
        'info
        '
        Me.info.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.info.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!)
        Me.info.Location = New System.Drawing.Point(9, 21)
        Me.info.Name = "info"
        Me.info.ReadOnly = True
        Me.info.Size = New System.Drawing.Size(439, 20)
        Me.info.TabIndex = 7
        Me.info.Text = "HTML file location"
        '
        'HtmlPath
        '
        Me.HtmlPath.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.HtmlPath.Location = New System.Drawing.Point(9, 47)
        Me.HtmlPath.Name = "HtmlPath"
        Me.HtmlPath.Size = New System.Drawing.Size(226, 23)
        Me.HtmlPath.TabIndex = 6
        '
        'GenerateDataForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(460, 157)
        Me.Controls.Add(Me.BrowseFile)
        Me.Controls.Add(Me.info)
        Me.Controls.Add(Me.HtmlPath)
        Me.Controls.Add(Me.Browse)
        Me.Controls.Add(Me.SavePathTitle)
        Me.Controls.Add(Me.SavePath)
        Me.Controls.Add(Me.Generate)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "GenerateDataForm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Generate As Button
    Friend WithEvents SavePath As TextBox
    Friend WithEvents SavePathTitle As TextBox
    Friend WithEvents Browse As Button
    Friend WithEvents BrowseFile As Button
    Friend WithEvents info As TextBox
    Friend WithEvents HtmlPath As TextBox
End Class
