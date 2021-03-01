using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PoliDLGUI.Forms
{
    public partial class ProgressTracker
    {
        private readonly DownloadForm downloadForm;

        public ProgressTracker(DownloadForm downloadForm)
        {
            this.downloadForm = downloadForm;
            InitializeComponent();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // I swear I tried for hours to do this properly with multithreading etc, but it just refused.
            // JANK IT UP!

            if (downloadForm.CurrentSpeed == "Finished." | downloadForm.CurrentSpeed == "Finito.")
                Close();
            OverallProgress.Value = (int)Math.Round(downloadForm.currentprogress);
            FileNum.Text = "File " + downloadForm.currentfile + "/" + downloadForm.currentfiletotal;
            if ((DLspeed.Text ?? "") != (downloadForm.CurrentSpeed ?? ""))
            {
                DLspeed.Text = downloadForm.CurrentSpeed;
                var size = TextRenderer.MeasureText(DLspeed.Text, DLspeed.Font);
                DLspeed.Width = size.Width;
                var p = DLspeed.Location;
                p.X = ClientSize.Width - DLspeed.Width - 5;
                DLspeed.Location = p;
            }
        }

        private void ProgressTracker_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (downloadForm.CurrentSpeed == "Processing file..." | downloadForm.CurrentSpeed == "Sto elaborando il file...")
            {
                // We don't want the user to stop a file whilst it's processing.
                // TODO: Add the existing file check to poliwebex.js
                MessageBox.Show("Please wait until the file is done processing.");
                e.Cancel = true;
                return;
            }

            if (downloadForm.CurrentSpeed != "Finished." & downloadForm.CurrentSpeed != "Finito.")
            {
                int ans;
                if (StartupForm.IsItalian)
                {
                    if (!downloadForm.GlobalProcess.StartInfo.Arguments.Contains(" -s") & !downloadForm.GlobalProcess.StartInfo.FileName.Contains("polidown.exe"))
                    {
                        ans = (int)Interaction.MsgBox("Sei sicuro? Interromperà il download corrente e dovrai ricominciare da capo, dato che sei in modalità unsegmented.", MsgBoxStyle.YesNo, "Exit?");
                    }
                    else
                    {
                        ans = (int)Interaction.MsgBox("Sei sicuro? Interromperà il download corrente.", MsgBoxStyle.YesNo, "Exit?");
                    }
                }
                else if (!downloadForm.GlobalProcess.StartInfo.Arguments.Contains(" -s") & !downloadForm.GlobalProcess.StartInfo.FileName.Contains("polidown.exe"))
                {
                    ans = (int)Interaction.MsgBox("Are you sure? This will stop the current download and you will have to start from scratch, since you're in unsegmented mode.", MsgBoxStyle.YesNo, "Exit?");
                }
                else
                {
                    ans = (int)Interaction.MsgBox("Are you sure? This will stop the current download.", MsgBoxStyle.YesNo, "Exit?");
                }

                if (ans == (int)DialogResult.Yes)
                {
                    try
                    {
                        downloadForm.GlobalProcess.Kill();
                        downloadForm.GlobalProcess.Dispose();
                        foreach (var proc in Process.GetProcessesByName("aria2c"))   // Stop the download.
                        {
                            if (proc.MainModule.FileName.Contains(StartupForm.RootFolder))
                            {
                                proc.Kill();
                                proc.Dispose();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void ProgressTracker_Load(object sender, EventArgs e)
        {
        }
    }
}