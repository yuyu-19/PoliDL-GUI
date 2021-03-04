using Microsoft.VisualBasic;
using PoliDLGUI.Classes;
using PoliDLGUI.Enums;
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

        internal void OneDownloadHasFinished()
        {
            UpdateFileNum2(ProgressUpdate.COMPLETED);
        }

        private void ProgressTracker_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (downloadForm.downloadPool.current.GetCount() > 0) //downloadForm.downloadInfoList.Select(x => x.CurrentSpeed == "Processing file...").Any(x => x) ||
                                                                  //downloadForm.downloadInfoList.Select(x => x.CurrentSpeed == "Sto elaborando il file...").Any(x => x))
            {
                // We don't want the user to stop a file whilst it's processing.
                // TODO: Add the existing file check to poliwebex.js
                //MessageBox.Show("Please wait until the file is done processing.");
                //e.Cancel = true;
                //return;
                //}

                //if (!(!downloadForm.downloadInfoList.Select(x => x.CurrentSpeed != "Finished.").Any(x => x) ||
                //    !downloadForm.downloadInfoList.Select(x => x.CurrentSpeed != "Finito.").Any(x => x)))
                //{
                MsgBoxResult ans = StartupForm.IsItalian
                    ? Interaction.MsgBox("Sei sicuro? Interromperà i download correnti e dovrai ricominciare da capo", MsgBoxStyle.YesNo, "Exit?")
                    : Interaction.MsgBox("Are you sure? This will stop all current downloads and you will have to start from scratch", MsgBoxStyle.YesNo, "Exit?");

                //bool? isSegmented = downloadForm.downloadPool.WeHaveSegmentedDownloadsCurrently();

                if (ans == MsgBoxResult.Ok || ans == MsgBoxResult.Yes)
                {
                    KillAllProcesses(this.downloadForm.downloadPool);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        internal void OneDownloadHasFailed()
        {
            UpdateFileNum2(ProgressUpdate.ERROR);
        }

        public static void KillAllProcesses(DownloadPool list)
        {
            try
            {
                list.KillAll();

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

        private delegate void CloseThisCallback(string text);

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

        private delegate void UpdateFileNumCallBack(ProgressUpdate text);

        public void UpdateFileNum2(ProgressUpdate text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.

            if (this.InvokeRequired)
            {
                UpdateFileNumCallBack d = new UpdateFileNumCallBack(UpdateFileNum2);
                this.Invoke(d, new object[] { text });
                return;
            }

            var SuccessCount = this.downloadForm.downloadPool.success.GetCount();
            var CurrentCount = this.downloadForm.downloadPool.current.GetCount();
            var FailedCount = this.downloadForm.downloadPool.fail.GetCount();

            switch (text)
            {
                case ProgressUpdate.COMPLETED:
                    {
                        
                        this.NumCompleted.Text = ((Convert.ToInt32(this.NumCompleted.Text)) + 1).ToString();
                        break;
                    }
                case ProgressUpdate.ERROR:
                    {
                        this.NumFailed.Text = ((Convert.ToInt32(this.NumFailed.Text)) + 1).ToString();
                        break;
                    }
                case ProgressUpdate.STARTED:
                    {
                        startedDownloads++;
                        break;
                    }
            }

            this.NumDownloading.Text = CurrentCount.ToString();
            this.FileNumCurrent.Text = "File " + (CurrentCount + SuccessCount + FailedCount) + "/" + (startedDownloads).ToString();
            this.FileNumTotal.Text = "File " + (SuccessCount + FailedCount).ToString() + "/" + (this.downloadForm.downloadPool.total).ToString();
            this.OverallProgressCurrent.Maximum = startedDownloads;
            this.OverallProgressCurrent.Value = (CurrentCount + SuccessCount + FailedCount);
            this.OverallProgressTotal.Value = (SuccessCount + FailedCount);
        }

        private int startedDownloads = 0;

        internal void UpdateFileNum()
        {
            UpdateFileNum2(ProgressUpdate.STARTED);
        }

        private void ProgressTracker_FormClosing(object sender, FormClosingEventArgs e)
        {
            KillAllProcesses(this.downloadForm.downloadPool);
        }

        private void ButtonInfoCompleted_Click(object sender, EventArgs e)
        {
            MoreInfo(Enums.HowEnded.SUCCESS);
        }

        private void ButtonInfoFailed_Click(object sender, EventArgs e)
        {
            MoreInfo(Enums.HowEnded.FAIL);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MoreInfo(Enums.HowEnded.NOT_ENDED_YET);
        }

        private void MoreInfo(HowEnded howEnded)
        {
            DownloadInfoList r = null;
            switch (howEnded)
            {
                case HowEnded.SUCCESS:
                    r = this.downloadForm.downloadPool.success;
                    break;

                case HowEnded.FAIL:
                    r = this.downloadForm.downloadPool.fail;
                    break;

                case HowEnded.NOT_ENDED_YET:
                    r = this.downloadForm.downloadPool.current;
                    break;
            }

            if (r == null || r.GetCount() == 0)
            {
                if (StartupForm.IsItalian)
                    MessageBox.Show("Nessun risultato!");
                else
                    MessageBox.Show("No results!");
                return;
            }

            ResultsListForm resultsListForm = new ResultsListForm(r, howEnded);
            resultsListForm.ShowDialog();
        }
    }
}