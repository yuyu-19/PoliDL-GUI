using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace PoliDLGUI.Forms
{
    [DesignerGenerated()]
    public partial class StartupForm : Form
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
            this.Question = new System.Windows.Forms.Label();
            this.localmode = new System.Windows.Forms.Button();
            this.downloadmode = new System.Windows.Forms.Button();
            this.CreditLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Question
            // 
            this.Question.Location = new System.Drawing.Point(324, 19);
            this.Question.Name = "Question";
            this.Question.Size = new System.Drawing.Size(39, 13);
            this.Question.TabIndex = 0;
            this.Question.Text = "This is a test to see how the text displays";
            // 
            // localmode
            // 
            this.localmode.Location = new System.Drawing.Point(656, 46);
            this.localmode.Name = "localmode";
            this.localmode.Size = new System.Drawing.Size(77, 19);
            this.localmode.TabIndex = 1;
            this.localmode.Text = "Local";
            this.localmode.UseVisualStyleBackColor = true;
            this.localmode.Click += new System.EventHandler(this.Localmode_Click_1);
            // 
            // downloadmode
            // 
            this.downloadmode.Location = new System.Drawing.Point(3, 46);
            this.downloadmode.Name = "downloadmode";
            this.downloadmode.Size = new System.Drawing.Size(77, 19);
            this.downloadmode.TabIndex = 2;
            this.downloadmode.Text = "Download";
            this.downloadmode.UseVisualStyleBackColor = true;
            this.downloadmode.Click += new System.EventHandler(this.Downloadmode_Click_1);
            // 
            // CreditLabel
            // 
            this.CreditLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreditLabel.Location = new System.Drawing.Point(201, 46);
            this.CreditLabel.Name = "CreditLabel";
            this.CreditLabel.Size = new System.Drawing.Size(299, 42);
            this.CreditLabel.TabIndex = 3;
            this.CreditLabel.Text = "PoliWebex and PoliDown by @sup3rgiu GUI by @yuyu-19";
            // 
            // StartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 89);
            this.Controls.Add(this.CreditLabel);
            this.Controls.Add(this.downloadmode);
            this.Controls.Add(this.localmode);
            this.Controls.Add(this.Question);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "StartupForm";
            this.Text = "PoliDL-GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StartupForm_FormClosing);
            this.Load += new System.EventHandler(this.StartupForm_Load);
            this.ResumeLayout(false);

        }

        internal Label Question;
        internal Button localmode;
        internal Button downloadmode;
        internal Label CreditLabel;
    }
}