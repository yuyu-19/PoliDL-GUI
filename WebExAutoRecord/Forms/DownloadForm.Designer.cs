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
            ModeLbl = new Label();
            _ModeSelect = new ComboBox();
            _ModeSelect.SelectedIndexChanged += new EventHandler(ModeSelect_SelectedIndexChanged);
            _Browse = new Button();
            _Browse.Click += new EventHandler(Browse_Click);
            FilePath = new TextBox();
            _DLButton = new Button();
            _DLButton.Click += new EventHandler(DLButton_Click);
            URLlist = new TextBox();
            ExtensionInfo = new Label();
            DLfolderlabel = new Label();
            FolderPath = new TextBox();
            _BrowseFolder = new Button();
            _BrowseFolder.Click += new EventHandler(BrowseFolder_Click);
            _CheckSegmented = new CheckBox();
            _CheckSegmented.CheckedChanged += new EventHandler(CheckSegmented_CheckedChanged);
            SuspendLayout();
            // 
            // ModeLbl
            // 
            ModeLbl.AutoSize = true;
            ModeLbl.Font = new Font("Microsoft Sans Serif", 14.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            ModeLbl.Location = new Point(12, 30);
            ModeLbl.Name = "ModeLbl";
            ModeLbl.Size = new Size(64, 24);
            ModeLbl.TabIndex = 0;
            ModeLbl.Text = "Mode:";
            // 
            // ModeSelect
            // 
            _ModeSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            _ModeSelect.FormattingEnabled = true;
            _ModeSelect.Items.AddRange(new object[] { "File", "Text", "Folder" });
            _ModeSelect.Location = new Point(82, 35);
            _ModeSelect.Name = "_ModeSelect";
            _ModeSelect.Size = new Size(91, 21);
            _ModeSelect.TabIndex = 1;
            // 
            // Browse
            // 
            _Browse.Location = new Point(282, 67);
            _Browse.Name = "_Browse";
            _Browse.Size = new Size(75, 23);
            _Browse.TabIndex = 2;
            _Browse.Text = "Browse";
            _Browse.UseVisualStyleBackColor = true;
            // 
            // FilePath
            // 
            FilePath.AllowDrop = true;
            FilePath.Location = new Point(16, 69);
            FilePath.Name = "FilePath";
            FilePath.Size = new Size(260, 20);
            FilePath.TabIndex = 3;
            // 
            // DLButton
            // 
            _DLButton.Location = new Point(480, 67);
            _DLButton.Name = "_DLButton";
            _DLButton.Size = new Size(75, 23);
            _DLButton.TabIndex = 4;
            _DLButton.Text = "Download";
            _DLButton.UseVisualStyleBackColor = true;
            // 
            // URLlist
            // 
            URLlist.AcceptsReturn = true;
            URLlist.AllowDrop = true;
            URLlist.Location = new Point(16, 67);
            URLlist.Multiline = true;
            URLlist.Name = "URLlist";
            URLlist.ScrollBars = ScrollBars.Vertical;
            URLlist.Size = new Size(539, 307);
            URLlist.TabIndex = 5;
            URLlist.Visible = false;
            // 
            // ExtensionInfo
            // 
            ExtensionInfo.AutoSize = true;
            ExtensionInfo.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            ExtensionInfo.Location = new Point(179, 38);
            ExtensionInfo.Name = "ExtensionInfo";
            ExtensionInfo.Size = new Size(346, 16);
            ExtensionInfo.TabIndex = 6;
            ExtensionInfo.Text = "Supported file types: html, xlsx, docx, zip (of the other files)" + '\r' + '\n';
            // 
            // DLfolderlabel
            // 
            DLfolderlabel.AutoSize = true;
            DLfolderlabel.Font = new Font("Microsoft Sans Serif", 14.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            DLfolderlabel.Location = new Point(12, 8);
            DLfolderlabel.Name = "DLfolderlabel";
            DLfolderlabel.Size = new Size(160, 24);
            DLfolderlabel.TabIndex = 7;
            DLfolderlabel.Text = "Download Folder:";
            // 
            // FolderPath
            // 
            FolderPath.Location = new Point(178, 13);
            FolderPath.Name = "FolderPath";
            FolderPath.Size = new Size(260, 20);
            FolderPath.TabIndex = 8;
            // 
            // BrowseFolder
            // 
            _BrowseFolder.Location = new Point(444, 12);
            _BrowseFolder.Name = "_BrowseFolder";
            _BrowseFolder.Size = new Size(75, 23);
            _BrowseFolder.TabIndex = 9;
            _BrowseFolder.Text = "Browse";
            _BrowseFolder.UseVisualStyleBackColor = true;
            // 
            // CheckSegmented
            // 
            _CheckSegmented.AutoSize = true;
            _CheckSegmented.Location = new Point(16, 382);
            _CheckSegmented.Name = "_CheckSegmented";
            _CheckSegmented.Size = new Size(113, 30);
            _CheckSegmented.TabIndex = 10;
            _CheckSegmented.Text = "Run unsegmented" + '\r' + '\n' + "(Fallback option)";
            _CheckSegmented.UseVisualStyleBackColor = true;
            // 
            // DownloadForm
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(567, 424);
            Controls.Add(_CheckSegmented);
            Controls.Add(_BrowseFolder);
            Controls.Add(FolderPath);
            Controls.Add(DLfolderlabel);
            Controls.Add(ExtensionInfo);
            Controls.Add(_DLButton);
            Controls.Add(FilePath);
            Controls.Add(_Browse);
            Controls.Add(_ModeSelect);
            Controls.Add(ModeLbl);
            Controls.Add(URLlist);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "DownloadForm";
            StartPosition = FormStartPosition.CenterParent;
            Load += new EventHandler(DownloadForm_Load);
            ResumeLayout(false);
            PerformLayout();
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