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
            this.OverallProgressCurrent = new System.Windows.Forms.ProgressBar();
            this.FileNumCurrent = new System.Windows.Forms.Label();
            this.DLspeed = new System.Windows.Forms.Label();
            this.l1 = new System.Windows.Forms.Label();
            this.NumCompleted = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NumFailed = new System.Windows.Forms.Label();
            this.buttonInfoCompleted = new System.Windows.Forms.Button();
            this.buttonInfoFailed = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NumDownloading = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.FileNumTotal = new System.Windows.Forms.Label();
            this.OverallProgressTotal = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OverallProgressCurrent
            // 
            this.OverallProgressCurrent.Location = new System.Drawing.Point(12, 37);
            this.OverallProgressCurrent.Name = "OverallProgressCurrent";
            this.OverallProgressCurrent.Size = new System.Drawing.Size(508, 23);
            this.OverallProgressCurrent.TabIndex = 0;
            // 
            // FileNumCurrent
            // 
            this.FileNumCurrent.AutoSize = true;
            this.FileNumCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileNumCurrent.Location = new System.Drawing.Point(7, 9);
            this.FileNumCurrent.Name = "FileNumCurrent";
            this.FileNumCurrent.Size = new System.Drawing.Size(88, 25);
            this.FileNumCurrent.TabIndex = 1;
            this.FileNumCurrent.Text = "File X/Y";
            // 
            // DLspeed
            // 
            this.DLspeed.AutoSize = true;
            this.DLspeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DLspeed.Location = new System.Drawing.Point(337, 9);
            this.DLspeed.Name = "DLspeed";
            this.DLspeed.Size = new System.Drawing.Size(0, 25);
            this.DLspeed.TabIndex = 2;
            // 
            // l1
            // 
            this.l1.AutoSize = true;
            this.l1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l1.Location = new System.Drawing.Point(14, 215);
            this.l1.Name = "l1";
            this.l1.Size = new System.Drawing.Size(239, 25);
            this.l1.TabIndex = 3;
            this.l1.Text = "Completed successfully";
            // 
            // NumCompleted
            // 
            this.NumCompleted.AutoSize = true;
            this.NumCompleted.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumCompleted.Location = new System.Drawing.Point(264, 215);
            this.NumCompleted.Name = "NumCompleted";
            this.NumCompleted.Size = new System.Drawing.Size(24, 25);
            this.NumCompleted.TabIndex = 4;
            this.NumCompleted.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(182, 250);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Failed";
            // 
            // NumFailed
            // 
            this.NumFailed.AutoSize = true;
            this.NumFailed.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumFailed.Location = new System.Drawing.Point(264, 250);
            this.NumFailed.Name = "NumFailed";
            this.NumFailed.Size = new System.Drawing.Size(24, 25);
            this.NumFailed.TabIndex = 6;
            this.NumFailed.Text = "0";
            // 
            // buttonInfoCompleted
            // 
            this.buttonInfoCompleted.Location = new System.Drawing.Point(363, 218);
            this.buttonInfoCompleted.Name = "buttonInfoCompleted";
            this.buttonInfoCompleted.Size = new System.Drawing.Size(157, 23);
            this.buttonInfoCompleted.TabIndex = 7;
            this.buttonInfoCompleted.Text = "More info";
            this.buttonInfoCompleted.UseVisualStyleBackColor = true;
            this.buttonInfoCompleted.Click += new System.EventHandler(this.ButtonInfoCompleted_Click);
            // 
            // buttonInfoFailed
            // 
            this.buttonInfoFailed.Location = new System.Drawing.Point(363, 253);
            this.buttonInfoFailed.Name = "buttonInfoFailed";
            this.buttonInfoFailed.Size = new System.Drawing.Size(157, 23);
            this.buttonInfoFailed.TabIndex = 8;
            this.buttonInfoFailed.Text = "More info";
            this.buttonInfoFailed.UseVisualStyleBackColor = true;
            this.buttonInfoFailed.Click += new System.EventHandler(this.ButtonInfoFailed_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 25);
            this.label1.TabIndex = 9;
            this.label1.Text = "Currently downloading";
            // 
            // NumDownloading
            // 
            this.NumDownloading.AutoSize = true;
            this.NumDownloading.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDownloading.Location = new System.Drawing.Point(264, 178);
            this.NumDownloading.Name = "NumDownloading";
            this.NumDownloading.Size = new System.Drawing.Size(24, 25);
            this.NumDownloading.TabIndex = 10;
            this.NumDownloading.Text = "0";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(363, 181);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(157, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "More info";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(458, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 20);
            this.label3.TabIndex = 12;
            this.label3.Text = "Current";
            // 
            // FileNumTotal
            // 
            this.FileNumTotal.AutoSize = true;
            this.FileNumTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileNumTotal.Location = new System.Drawing.Point(7, 86);
            this.FileNumTotal.Name = "FileNumTotal";
            this.FileNumTotal.Size = new System.Drawing.Size(88, 25);
            this.FileNumTotal.TabIndex = 14;
            this.FileNumTotal.Text = "File X/Y";
            // 
            // OverallProgressTotal
            // 
            this.OverallProgressTotal.Location = new System.Drawing.Point(12, 114);
            this.OverallProgressTotal.Name = "OverallProgressTotal";
            this.OverallProgressTotal.Size = new System.Drawing.Size(508, 23);
            this.OverallProgressTotal.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(476, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 20);
            this.label5.TabIndex = 15;
            this.label5.Text = "Total";
            // 
            // ProgressTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 285);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.FileNumTotal);
            this.Controls.Add(this.OverallProgressTotal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.NumDownloading);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonInfoFailed);
            this.Controls.Add(this.buttonInfoCompleted);
            this.Controls.Add(this.NumFailed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NumCompleted);
            this.Controls.Add(this.l1);
            this.Controls.Add(this.DLspeed);
            this.Controls.Add(this.FileNumCurrent);
            this.Controls.Add(this.OverallProgressCurrent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ProgressTracker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ProgressTracker_Closing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressTracker_FormClosing);
            this.Load += new System.EventHandler(this.ProgressTracker_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public Label FileNumCurrent;
        public Label DLspeed;
        public ProgressBar OverallProgressCurrent;
        public Label l1;
        public Label NumCompleted;
        public Label label2;
        public Label NumFailed;
        private Button buttonInfoCompleted;
        private Button buttonInfoFailed;
        public Label label1;
        public Label NumDownloading;
        private Button button1;
        public Label label3;
        public Label FileNumTotal;
        public ProgressBar OverallProgressTotal;
        public Label label5;
    }
}