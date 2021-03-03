using Microsoft.VisualBasic;
using PoliDLGUI.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            /*
            if (downloadForm.CurrentSpeed == "Finished." | downloadForm.CurrentSpeed == "Finito.")
                Close();
            OverallProgressCompleted.Value = (int)Math.Round(downloadForm.currentprogress);
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
            */
        }

        delegate void OneDownloadHasFinishedCallback(object o);

        private void OneDownloadHasFinished2(object o)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.OverallProgressCompleted.InvokeRequired)
            {
                OneDownloadHasFinishedCallback d = new OneDownloadHasFinishedCallback(OneDownloadHasFinished2);
                this.Invoke(d, new object[] { null });
            }
            else
            {
                this.OverallProgressCompleted.Value++;
            }
        }

        internal void OneDownloadHasFinished()
        {
            OneDownloadHasFinished2(null);
        }

        private void ProgressTracker_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (downloadForm.downloadInfoList.Select(x => x.CurrentSpeed == "Processing file...").Any(x => x) ||
                downloadForm.downloadInfoList.Select(x => x.CurrentSpeed == "Sto elaborando il file...").Any(x => x))
            {
                // We don't want the user to stop a file whilst it's processing.
                // TODO: Add the existing file check to poliwebex.js
                MessageBox.Show("Please wait until the file is done processing.");
                e.Cancel = true;
                return;
            }

            if (!(!downloadForm.downloadInfoList.Select(x => x.CurrentSpeed != "Finished.").Any(x => x) ||
                !downloadForm.downloadInfoList.Select(x => x.CurrentSpeed != "Finito.").Any(x => x)))
            {
                int ans;
                if (StartupForm.IsItalian)
                {
                    if (!downloadForm.downloadInfoList.Select(x => x.process.StartInfo.Arguments.Contains("- s")).Any(x => x) &&
                        !downloadForm.downloadInfoList.Select( x => x.process.StartInfo.FileName.Contains("polidown.exe")).Any(x => x))
                    {
                        ans = (int)Interaction.MsgBox("Sei sicuro? Interromperà il download corrente e dovrai ricominciare da capo, dato che sei in modalità unsegmented.", MsgBoxStyle.YesNo, "Exit?");
                    }
                    else
                    {
                        ans = (int)Interaction.MsgBox("Sei sicuro? Interromperà il download corrente.", MsgBoxStyle.YesNo, "Exit?");
                    }
                }
                else if (!downloadForm.downloadInfoList.Select(x => x.process.StartInfo.Arguments.Contains(" -s")).Any() & !downloadForm.downloadInfoList.Select(x => x.process.StartInfo.FileName.Contains("polidown.exe")).Any())
                {
                    ans = (int)Interaction.MsgBox("Are you sure? This will stop the current download and you will have to start from scratch, since you're in unsegmented mode.", MsgBoxStyle.YesNo, "Exit?");
                }
                else
                {
                    ans = (int)Interaction.MsgBox("Are you sure? This will stop the current download.", MsgBoxStyle.YesNo, "Exit?");
                }

                if (ans == (int)DialogResult.Yes)
                {
                    KillAllProcesses(this.downloadForm.downloadInfoList);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public static void KillAllProcesses(List<DownloadInfo> list)
        {
            try
            {
                foreach (var x in list)
                {
                    try
                    {
                        x.process.Kill();
                        x.process.Dispose();
                    }
                    catch
                    {
                        ;
                    }
                }

                foreach (var proc in Process.GetProcessesByName("aria2c"))   // Stop the download.
                {
                    try
                    {
                        if (proc.MainModule.FileName.Contains(StartupForm.RootFolder))
                        {
                            proc.Kill();
                            proc.Dispose();
                        }
                    }
                    catch
                    {
                        ;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ProgressTracker_Load(object sender, EventArgs e)
        {

        }


        delegate void CloseThisCallback(string text);

        public void CloseThis(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                CloseThisCallback d = new CloseThisCallback(CloseThis);
                this.Invoke(d, new object[] { text });
            }
            else
            { 

                this.Close();


            }
        }
        

        delegate void UpdateFileNumCallBack(string text);

        public void UpdateFileNum2(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                UpdateFileNumCallBack d = new UpdateFileNumCallBack(UpdateFileNum2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                int a = this.downloadForm.downloadInfoList.Select(x => x.ended).Count(x => x);
                int b = this.downloadForm.downloadInfoList.Count;
                this.FileNum.Text = "File "+a.ToString()+"/" + (b).ToString();
            }
        }

        internal void UpdateFileNum()
        {
            UpdateFileNum2(null);
        }

        private void ProgressTracker_FormClosing(object sender, FormClosingEventArgs e)
        {
            KillAllProcesses(this.downloadForm.downloadInfoList);
        }
    }
}