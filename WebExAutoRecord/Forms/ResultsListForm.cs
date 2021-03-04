using PoliDLGUI.Classes;
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
    public partial class ResultsListForm : Form
    {
        DownloadInfoList downloadInfoList;
        public ResultsListForm(DownloadInfoList downloadInfoList)
        {
            this.downloadInfoList = downloadInfoList;
            InitializeComponent();
        }

        private void ResultsListForm_Load(object sender, EventArgs e)
        {
            this.downloadInfoList.LoadPanelResults(ref this.panel1);
        }
    }
}
