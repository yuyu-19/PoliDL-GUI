using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace PoliDLGUI.Forms
{
    [DesignerGenerated()]
    public partial class InputForm : Form
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
            this._OK = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.InputText = new System.Windows.Forms.TextBox();
            this._Skip = new System.Windows.Forms.Button();
            this.reUsePW = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _OK
            // 
            this._OK.Location = new System.Drawing.Point(334, 32);
            this._OK.Name = "_OK";
            this._OK.Size = new System.Drawing.Size(75, 23);
            this._OK.TabIndex = 0;
            this._OK.Text = "OK";
            this._OK.UseVisualStyleBackColor = true;
            this._OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(12, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(145, 20);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Please input your X";
            // 
            // InputText
            // 
            this.InputText.Location = new System.Drawing.Point(12, 35);
            this.InputText.Name = "InputText";
            this.InputText.Size = new System.Drawing.Size(293, 20);
            this.InputText.TabIndex = 2;
            // 
            // _Skip
            // 
            this._Skip.Location = new System.Drawing.Point(334, 9);
            this._Skip.Name = "_Skip";
            this._Skip.Size = new System.Drawing.Size(75, 23);
            this._Skip.TabIndex = 3;
            this._Skip.Text = "Skip";
            this._Skip.UseVisualStyleBackColor = true;
            this._Skip.Visible = false;
            this._Skip.Click += new System.EventHandler(this.Skip_Click);
            // 
            // reUsePW
            // 
            this.reUsePW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reUsePW.AutoSize = true;
            this.reUsePW.Location = new System.Drawing.Point(16, 39);
            this.reUsePW.Name = "reUsePW";
            this.reUsePW.Size = new System.Drawing.Size(114, 17);
            this.reUsePW.TabIndex = 4;
            this.reUsePW.Text = "Re-use password?";
            this.reUsePW.UseVisualStyleBackColor = true;
            this.reUsePW.Visible = false;
            this.reUsePW.CheckedChanged += new System.EventHandler(this.reUsePW_CheckedChanged);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 59);
            this.Controls.Add(this.reUsePW);
            this.Controls.Add(this._Skip);
            this.Controls.Add(this.InputText);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this._OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "InputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Input";
            this.Load += new System.EventHandler(this.InputForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Button _OK;

        internal Button OK
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _OK;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_OK != null)
                {
                    _OK.Click -= OK_Click;
                }

                _OK = value;
                if (_OK != null)
                {
                    _OK.Click += OK_Click;
                }
            }
        }

        internal Label Label1;
        internal TextBox InputText;
        private Button _Skip;
        private CheckBox reUsePW;

        internal Button Skip
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Skip;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Skip != null)
                {
                    _Skip.Click -= Skip_Click;
                }

                _Skip = value;
                if (_Skip != null)
                {
                    _Skip.Click += Skip_Click;
                }
            }
        }
    }
}