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
            this.SuccessfulDL = new System.Windows.Forms.Label();
            this.NumCompleted = new System.Windows.Forms.Label();
            this.FailedDL = new System.Windows.Forms.Label();
            this.NumFailed = new System.Windows.Forms.Label();
            this.buttonInfoCompleted = new System.Windows.Forms.Button();
            this.buttonInfoFailed = new System.Windows.Forms.Button();
            this.CurrentDL = new System.Windows.Forms.Label();
            this.NumDownloading = new System.Windows.Forms.Label();
            this.buttonInfoCurrent = new System.Windows.Forms.Button();
            this.CurrentLbl = new System.Windows.Forms.Label();
            this.FileNumTotal = new System.Windows.Forms.Label();
            this.OverallProgressTotal = new System.Windows.Forms.ProgressBar();
            this.TotalLbl = new System.Windows.Forms.Label();
            if (this.downloadForm.downloadPool != null)  //Honestly, not sure why this became necessary, but it started freaking out for some reason.
                this.Text = "File 0/" + (this.downloadForm.downloadPool.total).ToString();
            this.SuspendLayout();
            // 
            // OverallProgressCurrent
            // 
            this.OverallProgressCurrent.Location = new System.Drawing.Point(13, 37);
            this.OverallProgressCurrent.Name = "OverallProgressCurrent";
            this.OverallProgressCurrent.Size = new System.Drawing.Size(412, 23);
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
            // SuccessfulDL
            // 
            this.SuccessfulDL.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SuccessfulDL.Location = new System.Drawing.Point(12, 215);
            this.SuccessfulDL.Name = "SuccessfulDL";
            this.SuccessfulDL.Size = new System.Drawing.Size(241, 25);
            this.SuccessfulDL.TabIndex = 3;
            this.SuccessfulDL.Text = "Completed successfully";
            this.SuccessfulDL.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // NumCompleted
            // 
            this.NumCompleted.AutoSize = true;
            this.NumCompleted.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumCompleted.Location = new System.Drawing.Point(259, 215);
            this.NumCompleted.Name = "NumCompleted";
            this.NumCompleted.Size = new System.Drawing.Size(24, 25);
            this.NumCompleted.TabIndex = 4;
            this.NumCompleted.Text = "0";
            // 
            // FailedDL
            // 
            this.FailedDL.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FailedDL.Location = new System.Drawing.Point(18, 250);
            this.FailedDL.Name = "FailedDL";
            this.FailedDL.Size = new System.Drawing.Size(235, 25);
            this.FailedDL.TabIndex = 5;
            this.FailedDL.Text = "Failed";
            this.FailedDL.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // NumFailed
            // 
            this.NumFailed.AutoSize = true;
            this.NumFailed.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumFailed.Location = new System.Drawing.Point(259, 250);
            this.NumFailed.Name = "NumFailed";
            this.NumFailed.Size = new System.Drawing.Size(24, 25);
            this.NumFailed.TabIndex = 6;
            this.NumFailed.Text = "0";
            // 
            // buttonInfoCompleted
            // 
            this.buttonInfoCompleted.Location = new System.Drawing.Point(294, 215);
            this.buttonInfoCompleted.Name = "buttonInfoCompleted";
            this.buttonInfoCompleted.Size = new System.Drawing.Size(131, 23);
            this.buttonInfoCompleted.TabIndex = 7;
            this.buttonInfoCompleted.Text = "More info";
            this.buttonInfoCompleted.UseVisualStyleBackColor = true;
            this.buttonInfoCompleted.Click += new System.EventHandler(this.ButtonInfoCompleted_Click);
            // 
            // buttonInfoFailed
            // 
            this.buttonInfoFailed.Location = new System.Drawing.Point(294, 250);
            this.buttonInfoFailed.Name = "buttonInfoFailed";
            this.buttonInfoFailed.Size = new System.Drawing.Size(131, 23);
            this.buttonInfoFailed.TabIndex = 8;
            this.buttonInfoFailed.Text = "More info";
            this.buttonInfoFailed.UseVisualStyleBackColor = true;
            this.buttonInfoFailed.Click += new System.EventHandler(this.ButtonInfoFailed_Click);
            // 
            // CurrentDL
            // 
            this.CurrentDL.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentDL.Location = new System.Drawing.Point(13, 178);
            this.CurrentDL.Name = "CurrentDL";
            this.CurrentDL.Size = new System.Drawing.Size(240, 25);
            this.CurrentDL.TabIndex = 9;
            this.CurrentDL.Text = "Currently downloading";
            this.CurrentDL.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // NumDownloading
            // 
            this.NumDownloading.AutoSize = true;
            this.NumDownloading.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDownloading.Location = new System.Drawing.Point(259, 178);
            this.NumDownloading.Name = "NumDownloading";
            this.NumDownloading.Size = new System.Drawing.Size(24, 25);
            this.NumDownloading.TabIndex = 10;
            this.NumDownloading.Text = "0";
            // 
            // buttonInfoCurrent
            // 
            this.buttonInfoCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.buttonInfoCurrent.Location = new System.Drawing.Point(294, 178);
            this.buttonInfoCurrent.Name = "buttonInfoCurrent";
            this.buttonInfoCurrent.Size = new System.Drawing.Size(131, 23);
            this.buttonInfoCurrent.TabIndex = 11;
            this.buttonInfoCurrent.Text = "More info";
            this.buttonInfoCurrent.UseVisualStyleBackColor = true;
            this.buttonInfoCurrent.Click += new System.EventHandler(this.Button1_Click);
            // 
            // CurrentLbl
            // 
            this.CurrentLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentLbl.Location = new System.Drawing.Point(294, 13);
            this.CurrentLbl.Name = "CurrentLbl";
            this.CurrentLbl.Size = new System.Drawing.Size(131, 20);
            this.CurrentLbl.TabIndex = 12;
            this.CurrentLbl.Text = "Current";
            this.CurrentLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
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
            this.OverallProgressTotal.Size = new System.Drawing.Size(413, 23);
            this.OverallProgressTotal.TabIndex = 13;
            // 
            // TotalLbl
            // 
            this.TotalLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalLbl.Location = new System.Drawing.Point(305, 91);
            this.TotalLbl.Name = "TotalLbl";
            this.TotalLbl.Size = new System.Drawing.Size(120, 20);
            this.TotalLbl.TabIndex = 15;
            this.TotalLbl.Text = "Total";
            this.TotalLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ProgressTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 285);
            this.Controls.Add(this.TotalLbl);
            this.Controls.Add(this.FileNumTotal);
            this.Controls.Add(this.OverallProgressTotal);
            this.Controls.Add(this.CurrentLbl);
            this.Controls.Add(this.buttonInfoCurrent);
            this.Controls.Add(this.NumDownloading);
            this.Controls.Add(this.CurrentDL);
            this.Controls.Add(this.buttonInfoFailed);
            this.Controls.Add(this.buttonInfoCompleted);
            this.Controls.Add(this.NumFailed);
            this.Controls.Add(this.FailedDL);
            this.Controls.Add(this.NumCompleted);
            this.Controls.Add(this.SuccessfulDL);
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
        public Label SuccessfulDL;
        public Label NumCompleted;
        public Label FailedDL;
        public Label NumFailed;
        private Button buttonInfoCompleted;
        private Button buttonInfoFailed;
        public Label CurrentDL;
        public Label NumDownloading;
        private Button buttonInfoCurrent;
        public Label CurrentLbl;
        public Label FileNumTotal;
        public ProgressBar OverallProgressTotal;
        public Label TotalLbl;
    }
}