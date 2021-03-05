using PoliDLGUI.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PoliDLGUI.Forms
{
    public partial class ResultsListForm : Form
    {
        private readonly DownloadInfoList downloadInfoList;
        private readonly Enums.HowEnded howEnded;

        public ResultsListForm(DownloadInfoList downloadInfoList, Enums.HowEnded howEnded)
        {
            this.downloadInfoList = downloadInfoList;
            this.howEnded = howEnded;
            InitializeComponent();
        }

        private void ResultsListForm_Load(object sender, EventArgs e)
        {
            this.downloadInfoList.LoadPanelResults(ref this.panel1);
            this.Text += " [" + howEnded.ToString() + "]";
            saveBtn.Text = StartupForm.IsItalian ? "Save URLs as .txt":"Salva link in un .txt";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "TXT|*.txt"
            };
            var r = saveFileDialog.ShowDialog();
            if (r != DialogResult.OK)
            {
                return;
            }

            List<string> Urls = this.downloadInfoList.GetURIs();
            File.WriteAllLines(saveFileDialog.FileName, Urls);
        }
    }
}