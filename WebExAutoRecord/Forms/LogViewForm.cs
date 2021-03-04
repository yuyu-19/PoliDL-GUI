using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoliDLGUI.Forms
{
    public partial class LogViewForm : Form
    {
        readonly List<string> s;
        public LogViewForm(List<string> s)
        {
            this.s = s;
            InitializeComponent();
        }

        private void LogViewForm_Load(object sender, EventArgs e)
        {
            this.textBox1.Lines = s.ToArray();
            this.textBox1.ScrollBars = ScrollBars.Both;
        }
    }
}
