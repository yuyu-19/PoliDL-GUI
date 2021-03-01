using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PoliDLGUI
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class GenerateDataForm : Form
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
            _Generate = new Button();
            _Generate.Click += new EventHandler(Generate_Click);
            SavePath = new TextBox();
            SavePathTitle = new TextBox();
            _Browse = new Button();
            _Browse.Click += new EventHandler(Browse_Click);
            _BrowseFile = new Button();
            _BrowseFile.Click += new EventHandler(BrowseFile_Click);
            info = new TextBox();
            HtmlPath = new TextBox();
            SuspendLayout();
            // 
            // Generate
            // 
            _Generate.Location = new Point(341, 116);
            _Generate.Name = "_Generate";
            _Generate.Size = new Size(107, 23);
            _Generate.TabIndex = 2;
            _Generate.Text = "Generate";
            _Generate.UseVisualStyleBackColor = true;
            // 
            // SavePath
            // 
            SavePath.AllowDrop = true;
            SavePath.Font = new Font("Microsoft Sans Serif", 10.0f);
            SavePath.Location = new Point(9, 118);
            SavePath.Name = "SavePath";
            SavePath.Size = new Size(226, 23);
            SavePath.TabIndex = 3;
            // 
            // SavePathTitle
            // 
            SavePathTitle.BorderStyle = BorderStyle.None;
            SavePathTitle.Font = new Font("Microsoft Sans Serif", 13.0f);
            SavePathTitle.Location = new Point(9, 92);
            SavePathTitle.Name = "SavePathTitle";
            SavePathTitle.ReadOnly = true;
            SavePathTitle.Size = new Size(439, 20);
            SavePathTitle.TabIndex = 4;
            SavePathTitle.Text = "Recordings folder";
            // 
            // Browse
            // 
            _Browse.Location = new Point(241, 116);
            _Browse.Name = "_Browse";
            _Browse.Size = new Size(59, 23);
            _Browse.TabIndex = 5;
            _Browse.Text = "Browse";
            _Browse.UseVisualStyleBackColor = true;
            // 
            // BrowseFile
            // 
            _BrowseFile.Location = new Point(241, 47);
            _BrowseFile.Name = "_BrowseFile";
            _BrowseFile.Size = new Size(59, 23);
            _BrowseFile.TabIndex = 8;
            _BrowseFile.Text = "Browse";
            _BrowseFile.UseVisualStyleBackColor = true;
            // 
            // info
            // 
            info.BorderStyle = BorderStyle.None;
            info.Font = new Font("Microsoft Sans Serif", 13.0f);
            info.Location = new Point(9, 21);
            info.Name = "info";
            info.ReadOnly = true;
            info.Size = new Size(439, 20);
            info.TabIndex = 7;
            info.Text = "HTML file location";
            // 
            // HtmlPath
            // 
            HtmlPath.AllowDrop = true;
            HtmlPath.Font = new Font("Microsoft Sans Serif", 10.0f);
            HtmlPath.Location = new Point(9, 47);
            HtmlPath.Name = "HtmlPath";
            HtmlPath.Size = new Size(226, 23);
            HtmlPath.TabIndex = 6;
            // 
            // GenerateDataForm
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(460, 157);
            Controls.Add(_BrowseFile);
            Controls.Add(info);
            Controls.Add(HtmlPath);
            Controls.Add(_Browse);
            Controls.Add(SavePathTitle);
            Controls.Add(SavePath);
            Controls.Add(_Generate);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "GenerateDataForm";
            Load += new EventHandler(GenerateDataForm_Load);
            ResumeLayout(false);
            PerformLayout();
        }

        private Button _Generate;

        internal Button Generate
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Generate;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Generate != null)
                {
                    _Generate.Click -= Generate_Click;
                }

                _Generate = value;
                if (_Generate != null)
                {
                    _Generate.Click += Generate_Click;
                }
            }
        }

        internal TextBox SavePath;
        internal TextBox SavePathTitle;
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

        private Button _BrowseFile;

        internal Button BrowseFile
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _BrowseFile;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_BrowseFile != null)
                {
                    _BrowseFile.Click -= BrowseFile_Click;
                }

                _BrowseFile = value;
                if (_BrowseFile != null)
                {
                    _BrowseFile.Click += BrowseFile_Click;
                }
            }
        }

        internal TextBox info;
        internal TextBox HtmlPath;
    }
}