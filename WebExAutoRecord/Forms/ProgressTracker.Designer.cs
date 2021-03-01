using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace PoliDLGUI.Forms
{
    [DesignerGenerated()]
    public partial class ProgressTracker : Form
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
            components = new System.ComponentModel.Container();
            OverallProgress = new ProgressBar();
            FileNum = new Label();
            DLspeed = new Label();
            _Timer = new Timer(components);
            _Timer.Tick += new EventHandler(Timer_Tick);
            SuspendLayout();
            // 
            // OverallProgress
            // 
            OverallProgress.Location = new Point(12, 37);
            OverallProgress.Name = "OverallProgress";
            OverallProgress.Size = new Size(424, 23);
            OverallProgress.TabIndex = 0;
            // 
            // FileNum
            // 
            FileNum.AutoSize = true;
            FileNum.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            FileNum.Location = new Point(7, 9);
            FileNum.Name = "FileNum";
            FileNum.Size = new Size(88, 25);
            FileNum.TabIndex = 1;
            FileNum.Text = "File X/Y";
            // 
            // DLspeed
            // 
            DLspeed.AutoSize = true;
            DLspeed.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            DLspeed.Location = new Point(337, 9);
            DLspeed.Name = "DLspeed";
            DLspeed.Size = new Size(0, 25);
            DLspeed.TabIndex = 2;
            // 
            // Timer
            // 
            _Timer.Enabled = true;
            _Timer.Interval = 500;
            // 
            // ProgressTracker
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(448, 72);
            Controls.Add(DLspeed);
            Controls.Add(FileNum);
            Controls.Add(OverallProgress);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ProgressTracker";
            StartPosition = FormStartPosition.CenterParent;
            Closing += new System.ComponentModel.CancelEventHandler(ProgressTracker_Closing);
            Load += new EventHandler(ProgressTracker_Load);
            ResumeLayout(false);
            PerformLayout();
        }

        public Label FileNum;
        public Label DLspeed;
        public ProgressBar OverallProgress;
        private Timer _Timer;

        internal Timer Timer
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Timer;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Timer != null)
                {
                    _Timer.Tick -= Timer_Tick;
                }

                _Timer = value;
                if (_Timer != null)
                {
                    _Timer.Tick += Timer_Tick;
                }
            }
        }
    }
}