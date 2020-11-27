<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DownloadForm
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
        Me.ModeLbl = New System.Windows.Forms.Label()
        Me.ModeSelect = New System.Windows.Forms.ComboBox()
        Me.Browse = New System.Windows.Forms.Button()
        Me.FilePath = New System.Windows.Forms.TextBox()
        Me.DLButton = New System.Windows.Forms.Button()
        Me.URLlist = New System.Windows.Forms.TextBox()
        Me.ExtensionInfo = New System.Windows.Forms.Label()
        Me.DLfolderlabel = New System.Windows.Forms.Label()
        Me.FolderPath = New System.Windows.Forms.TextBox()
        Me.BrowseFolder = New System.Windows.Forms.Button()
        Me.CheckSegmented = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'ModeLbl
        '
        Me.ModeLbl.AutoSize = True
        Me.ModeLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ModeLbl.Location = New System.Drawing.Point(12, 30)
        Me.ModeLbl.Name = "ModeLbl"
        Me.ModeLbl.Size = New System.Drawing.Size(64, 24)
        Me.ModeLbl.TabIndex = 0
        Me.ModeLbl.Text = "Mode:"
        '
        'ModeSelect
        '
        Me.ModeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ModeSelect.FormattingEnabled = True
        Me.ModeSelect.Items.AddRange(New Object() {"File", "URL List", "Folder"})
        Me.ModeSelect.Location = New System.Drawing.Point(82, 35)
        Me.ModeSelect.Name = "ModeSelect"
        Me.ModeSelect.Size = New System.Drawing.Size(91, 21)
        Me.ModeSelect.TabIndex = 1
        '
        'Browse
        '
        Me.Browse.Location = New System.Drawing.Point(282, 67)
        Me.Browse.Name = "Browse"
        Me.Browse.Size = New System.Drawing.Size(75, 23)
        Me.Browse.TabIndex = 2
        Me.Browse.Text = "Browse"
        Me.Browse.UseVisualStyleBackColor = True
        '
        'FilePath
        '
        Me.FilePath.Location = New System.Drawing.Point(16, 69)
        Me.FilePath.Name = "FilePath"
        Me.FilePath.Size = New System.Drawing.Size(260, 20)
        Me.FilePath.TabIndex = 3
        '
        'DLButton
        '
        Me.DLButton.Location = New System.Drawing.Point(480, 67)
        Me.DLButton.Name = "DLButton"
        Me.DLButton.Size = New System.Drawing.Size(75, 23)
        Me.DLButton.TabIndex = 4
        Me.DLButton.Text = "Download"
        Me.DLButton.UseVisualStyleBackColor = True
        '
        'URLlist
        '
        Me.URLlist.AcceptsReturn = True
        Me.URLlist.AllowDrop = True
        Me.URLlist.Location = New System.Drawing.Point(16, 67)
        Me.URLlist.Multiline = True
        Me.URLlist.Name = "URLlist"
        Me.URLlist.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.URLlist.Size = New System.Drawing.Size(539, 307)
        Me.URLlist.TabIndex = 5
        Me.URLlist.Visible = False
        '
        'ExtensionInfo
        '
        Me.ExtensionInfo.AutoSize = True
        Me.ExtensionInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ExtensionInfo.Location = New System.Drawing.Point(179, 38)
        Me.ExtensionInfo.Name = "ExtensionInfo"
        Me.ExtensionInfo.Size = New System.Drawing.Size(346, 16)
        Me.ExtensionInfo.TabIndex = 6
        Me.ExtensionInfo.Text = "Supported file types: html, xlsx, docx, zip (of the other files)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'DLfolderlabel
        '
        Me.DLfolderlabel.AutoSize = True
        Me.DLfolderlabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DLfolderlabel.Location = New System.Drawing.Point(12, 8)
        Me.DLfolderlabel.Name = "DLfolderlabel"
        Me.DLfolderlabel.Size = New System.Drawing.Size(160, 24)
        Me.DLfolderlabel.TabIndex = 7
        Me.DLfolderlabel.Text = "Download Folder:"
        '
        'FolderPath
        '
        Me.FolderPath.Location = New System.Drawing.Point(178, 13)
        Me.FolderPath.Name = "FolderPath"
        Me.FolderPath.Size = New System.Drawing.Size(260, 20)
        Me.FolderPath.TabIndex = 8
        '
        'BrowseFolder
        '
        Me.BrowseFolder.Location = New System.Drawing.Point(444, 12)
        Me.BrowseFolder.Name = "BrowseFolder"
        Me.BrowseFolder.Size = New System.Drawing.Size(75, 23)
        Me.BrowseFolder.TabIndex = 9
        Me.BrowseFolder.Text = "Browse"
        Me.BrowseFolder.UseVisualStyleBackColor = True
        '
        'CheckSegmented
        '
        Me.CheckSegmented.AutoSize = True
        Me.CheckSegmented.Location = New System.Drawing.Point(16, 382)
        Me.CheckSegmented.Name = "CheckSegmented"
        Me.CheckSegmented.Size = New System.Drawing.Size(113, 30)
        Me.CheckSegmented.TabIndex = 10
        Me.CheckSegmented.Text = "Run unsegmented" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Fallback option)"
        Me.CheckSegmented.UseVisualStyleBackColor = True
        '
        'DownloadForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(567, 424)
        Me.Controls.Add(Me.CheckSegmented)
        Me.Controls.Add(Me.BrowseFolder)
        Me.Controls.Add(Me.FolderPath)
        Me.Controls.Add(Me.DLfolderlabel)
        Me.Controls.Add(Me.ExtensionInfo)
        Me.Controls.Add(Me.DLButton)
        Me.Controls.Add(Me.FilePath)
        Me.Controls.Add(Me.Browse)
        Me.Controls.Add(Me.ModeSelect)
        Me.Controls.Add(Me.ModeLbl)
        Me.Controls.Add(Me.URLlist)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "DownloadForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ModeLbl As Label
    Friend WithEvents ModeSelect As ComboBox
    Friend WithEvents Browse As Button
    Friend WithEvents FilePath As TextBox
    Friend WithEvents DLButton As Button
    Friend WithEvents URLlist As TextBox
    Friend WithEvents ExtensionInfo As Label
    Friend WithEvents DLfolderlabel As Label
    Friend WithEvents FolderPath As TextBox
    Friend WithEvents BrowseFolder As Button
    Friend WithEvents CheckSegmented As CheckBox
End Class
