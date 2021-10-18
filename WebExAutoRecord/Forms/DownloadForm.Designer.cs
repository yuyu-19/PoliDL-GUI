using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace PoliDLGUI.Forms
{
    [DesignerGenerated()]
    public partial class DownloadForm : Form
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is object)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.ModeLbl = new System.Windows.Forms.Label();
            this._ModeSelect = new System.Windows.Forms.ComboBox();
            this._Browse = new System.Windows.Forms.Button();
            this.FilePath = new System.Windows.Forms.TextBox();
            this._DLButton = new System.Windows.Forms.Button();
            this.URLlist = new System.Windows.Forms.TextBox();
            this.ExtensionInfo = new System.Windows.Forms.Label();
            this.DLfolderlabel = new System.Windows.Forms.Label();
            this.FolderPath = new System.Windows.Forms.TextBox();
            this._BrowseFolder = new System.Windows.Forms.Button();
            this._CheckSegmented = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ModeLbl
            // 
            this.ModeLbl.AutoSize = true;
            this.ModeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModeLbl.Location = new System.Drawing.Point(12, 30);
            this.ModeLbl.Name = "ModeLbl";
            this.ModeLbl.Size = new System.Drawing.Size(64, 24);
            this.ModeLbl.TabIndex = 0;
            this.ModeLbl.Text = "Mode:";
            // 
            // _ModeSelect
            // 
            this._ModeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._ModeSelect.FormattingEnabled = true;
            this._ModeSelect.Items.AddRange(new object[] {
            "File",
            "Text",
            "Folder"});
            this._ModeSelect.Location = new System.Drawing.Point(82, 35);
            this._ModeSelect.Name = "_ModeSelect";
            this._ModeSelect.Size = new System.Drawing.Size(91, 21);
            this._ModeSelect.TabIndex = 1;
            this._ModeSelect.SelectedIndexChanged += new System.EventHandler(this.ModeSelect_SelectedIndexChanged);
            // 
            // _Browse
            // 
            this._Browse.Location = new System.Drawing.Point(282, 67);
            this._Browse.Name = "_Browse";
            this._Browse.Size = new System.Drawing.Size(75, 23);
            this._Browse.TabIndex = 2;
            this._Browse.Text = "Browse";
            this._Browse.UseVisualStyleBackColor = true;
            this._Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // FilePath
            // 
            this.FilePath.AllowDrop = true;
            this.FilePath.Location = new System.Drawing.Point(16, 69);
            this.FilePath.Name = "FilePath";
            this.FilePath.Size = new System.Drawing.Size(260, 20);
            this.FilePath.TabIndex = 3;
            // 
            // _DLButton
            // 
            this._DLButton.Location = new System.Drawing.Point(480, 67);
            this._DLButton.Name = "_DLButton";
            this._DLButton.Size = new System.Drawing.Size(75, 23);
            this._DLButton.TabIndex = 4;
            this._DLButton.Text = "Download";
            this._DLButton.UseVisualStyleBackColor = true;
            this._DLButton.Click += new System.EventHandler(this.DLButton_Click);
            // 
            // URLlist
            // 
            this.URLlist.AcceptsReturn = true;
            this.URLlist.AllowDrop = true;
            this.URLlist.Location = new System.Drawing.Point(16, 67);
            this.URLlist.Multiline = true;
            this.URLlist.Name = "URLlist";
            this.URLlist.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.URLlist.Size = new System.Drawing.Size(539, 307);
            this.URLlist.TabIndex = 5;
            this.URLlist.Visible = false;
            this.URLlist.TextChanged += new System.EventHandler(this.URLlist_TextChanged);
            // 
            // ExtensionInfo
            // 
            this.ExtensionInfo.AutoSize = true;
            this.ExtensionInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExtensionInfo.Location = new System.Drawing.Point(179, 38);
            this.ExtensionInfo.Name = "ExtensionInfo";
            this.ExtensionInfo.Size = new System.Drawing.Size(346, 16);
            this.ExtensionInfo.TabIndex = 6;
            this.ExtensionInfo.Text = "Supported file types: html, xlsx, docx, zip (of the other files)\r\n";
            // 
            // DLfolderlabel
            // 
            this.DLfolderlabel.AutoSize = true;
            this.DLfolderlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DLfolderlabel.Location = new System.Drawing.Point(12, 8);
            this.DLfolderlabel.Name = "DLfolderlabel";
            this.DLfolderlabel.Size = new System.Drawing.Size(160, 24);
            this.DLfolderlabel.TabIndex = 7;
            this.DLfolderlabel.Text = "Download Folder:";
            // 
            // FolderPath
            // 
            this.FolderPath.Location = new System.Drawing.Point(178, 13);
            this.FolderPath.Name = "FolderPath";
            this.FolderPath.Size = new System.Drawing.Size(260, 20);
            this.FolderPath.TabIndex = 8;
            // 
            // _BrowseFolder
            // 
            this._BrowseFolder.Location = new System.Drawing.Point(444, 12);
            this._BrowseFolder.Name = "_BrowseFolder";
            this._BrowseFolder.Size = new System.Drawing.Size(75, 23);
            this._BrowseFolder.TabIndex = 9;
            this._BrowseFolder.Text = "Browse";
            this._BrowseFolder.UseVisualStyleBackColor = true;
            this._BrowseFolder.Click += new System.EventHandler(this.BrowseFolder_Click);
            // 
            // _CheckSegmented
            // 
            this._CheckSegmented.AutoSize = true;
            this._CheckSegmented.Location = new System.Drawing.Point(16, 382);
            this._CheckSegmented.Name = "_CheckSegmented";
            this._CheckSegmented.Size = new System.Drawing.Size(113, 30);
            this._CheckSegmented.TabIndex = 10;
            this._CheckSegmented.Text = "Run unsegmented\r\n(Fallback option)";
            this._CheckSegmented.UseVisualStyleBackColor = true;
            this._CheckSegmented.Visible = false;
            this._CheckSegmented.CheckedChanged += new System.EventHandler(this.CheckSegmented_CheckedChanged);
            // 
            // DownloadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 424);
            this.Controls.Add(this._CheckSegmented);
            this.Controls.Add(this._BrowseFolder);
            this.Controls.Add(this.FolderPath);
            this.Controls.Add(this.DLfolderlabel);
            this.Controls.Add(this.ExtensionInfo);
            this.Controls.Add(this._DLButton);
            this.Controls.Add(this.FilePath);
            this.Controls.Add(this._Browse);
            this.Controls.Add(this._ModeSelect);
            this.Controls.Add(this.ModeLbl);
            this.Controls.Add(this.URLlist);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DownloadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DownloadForm_FormClosing);
            this.Load += new System.EventHandler(this.DownloadForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal Label ModeLbl;
        private ComboBox _ModeSelect;

        internal ComboBox ModeSelect
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ModeSelect;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_ModeSelect != null)
                {
                    _ModeSelect.SelectedIndexChanged -= ModeSelect_SelectedIndexChanged;
                }

                _ModeSelect = value;
                if (_ModeSelect != null)
                {
                    _ModeSelect.SelectedIndexChanged += ModeSelect_SelectedIndexChanged;
                }
            }
        }

        private Button _Browse;

        internal Button Browse
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Browse;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Browse != null)
                {
                    _Browse.Click -= Browse_Click;
                }

                _Browse = value;
                if (_Browse != null)
                {
                    _Browse.Click += Browse_Click;
                }
            }
        }

        internal TextBox FilePath;
        private Button _DLButton;

        internal Button DLButton
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _DLButton;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_DLButton != null)
                {
                    _DLButton.Click -= DLButton_Click;
                }

                _DLButton = value;
                if (_DLButton != null)
                {
                    _DLButton.Click += DLButton_Click;
                }
            }
        }

        internal TextBox URLlist;
        internal Label ExtensionInfo;
        internal Label DLfolderlabel;
        internal TextBox FolderPath;
        private Button _BrowseFolder;

        internal Button BrowseFolder
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _BrowseFolder;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_BrowseFolder != null)
                {
                    _BrowseFolder.Click -= BrowseFolder_Click;
                }

                _BrowseFolder = value;
                if (_BrowseFolder != null)
                {
                    _BrowseFolder.Click += BrowseFolder_Click;
                }
            }
        }

        private CheckBox _CheckSegmented;

        internal CheckBox CheckSegmented
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _CheckSegmented;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_CheckSegmented != null)
                {
                    _CheckSegmented.CheckedChanged -= CheckSegmented_CheckedChanged;
                }

                _CheckSegmented = value;
                if (_CheckSegmented != null)
                {
                    _CheckSegmented.CheckedChanged += CheckSegmented_CheckedChanged;
                }
            }
        }
    }
}