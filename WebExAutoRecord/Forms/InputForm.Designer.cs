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
            _OK = new Button();
            _OK.Click += new EventHandler(OK_Click);
            Label1 = new Label();
            InputText = new TextBox();
            _Skip = new Button();
            _Skip.Click += new EventHandler(Skip_Click);
            SuspendLayout();
            // 
            // OK
            // 
            _OK.Location = new Point(334, 32);
            _OK.Name = "_OK";
            _OK.Size = new Size(75, 23);
            _OK.TabIndex = 0;
            _OK.Text = "OK";
            _OK.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Font = new Font("Microsoft Sans Serif", 12.0f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            Label1.Location = new Point(12, 9);
            Label1.Name = "Label1";
            Label1.Size = new Size(145, 20);
            Label1.TabIndex = 1;
            Label1.Text = "Please input your X";
            // 
            // InputText
            // 
            InputText.Location = new Point(12, 34);
            InputText.Name = "InputText";
            InputText.Size = new Size(293, 20);
            InputText.TabIndex = 2;
            // 
            // Skip
            // 
            _Skip.Location = new Point(334, 3);
            _Skip.Name = "_Skip";
            _Skip.Size = new Size(75, 23);
            _Skip.TabIndex = 3;
            _Skip.Text = "Skip";
            _Skip.UseVisualStyleBackColor = true;
            _Skip.Visible = false;
            // 
            // InputForm
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(421, 66);
            Controls.Add(_Skip);
            Controls.Add(InputText);
            Controls.Add(Label1);
            Controls.Add(_OK);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "InputForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Input";
            Load += new EventHandler(InputForm_Load);
            ResumeLayout(false);
            PerformLayout();
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