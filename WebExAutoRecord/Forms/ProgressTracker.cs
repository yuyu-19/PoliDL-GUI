using Microsoft.VisualBasic;
using PoliDLGUI.Classes;
using PoliDLGUI.Enums;
using System;
using System.Collections.Generic;
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
            UpdateFileNum2(new ProgressUpdateClass(ProgressUpdate.COMPLETED, false));
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
                MsgBoxResult ans = Program.IsItalian
                    ? Interaction.MsgBox("Sei sicuro? Interromperà i download correnti (certe volte è possible che non interrompa tutti i processi di aria2, è consigliato controllare con task manager)", MsgBoxStyle.YesNo, "Exit?")
                    : Interaction.MsgBox("Are you sure? This will stop all current downloads (sometimes it faills to kill all aria2 processes, checking manually with task manager is recommended)", MsgBoxStyle.YesNo, "Exit?");

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

        internal void OneDownloadHasFailed(bool retry)
        {
            UpdateFileNum2(new ProgressUpdateClass(ProgressUpdate.ERROR, retry));
        }

        public static void KillAllProcesses(DownloadPool list)
        {
            try
            {
                if (list != null)       //Otherwise the program would crash when closing a downloadform without doing anything
                    list.KillAll();
                foreach (var processName in new[] {"chrome","poliwebex","aria2c"}) {
                    foreach (var proc in Process.GetProcessesByName("processName"))   // Stop the download.
                    {
                        try
                        {
                            if (proc.MainModule.FileName.Contains(Program.RootFolder))
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

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
                //this.downloadForm.Close();
                this.Close();
            }
        }

        private delegate void UpdateFileNumCallBack(ProgressUpdateClass text);

        public void UpdateFileNum2(ProgressUpdateClass text)
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
            var FailedCount = this.downloadForm.downloadPool.failWithoutRetry.GetCount();
            var FailedRetried = this.downloadForm.downloadPool.failedButRetried.GetCount();

            switch (text.progressUpdate)
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
            this.FileNumCurrent.Text = "File " + (CurrentCount + SuccessCount + FailedCount) + "/" + (startedDownloads - FailedRetried).ToString();
            this.FileNumTotal.Text = "File " + (SuccessCount + FailedCount).ToString() + "/" + (this.downloadForm.downloadPool.total).ToString();
            this.Text = "File " + (SuccessCount + FailedCount).ToString() + "/" + (this.downloadForm.downloadPool.total).ToString();
            this.OverallProgressCurrent.Maximum = startedDownloads - FailedRetried;
            this.OverallProgressCurrent.Value = CurrentCount + SuccessCount + FailedCount;
            this.OverallProgressTotal.Value = SuccessCount + FailedCount;

            int notDownloaded = this.downloadForm.downloadPool.total - SuccessCount;
            int waitingOrInProgress = this.downloadForm.downloadPool.waiting.GetCount() + this.downloadForm.downloadPool.current.GetCount();
            //MessageBox.Show("Files left: " + waitingOrInProgress + "\n Not downloaded: "+notDownloaded);
            //If there are no downloads waiting and we've failed at least 1
            int failedButRetried = downloadForm.downloadPool.failedButRetried.GetURIs().Count;
            int failedWithoutRetry = downloadForm.downloadPool.failWithoutRetry.GetURIs().Count;
            /*MessageBox.Show("" +
                "failedButRetried " + failedButRetried +
                "\ncurrent " +downloadForm.downloadPool.current.GetURIs().Count+
                "\nfailedWithoutRetry: " + failedWithoutRetry+
                "\ntotal: " + downloadForm.downloadPool.total
                );*/

            if (waitingOrInProgress == 0 && notDownloaded > 0 && text.progressUpdate != ProgressUpdate.STARTED && Convert.ToInt32(this.NumFailed.Text) > 0)
            {
                MsgBoxResult ans = Program.IsItalian
                    ? Interaction.MsgBox("Hai "+ notDownloaded + " download falliti. Vuoi riprovare?", MsgBoxStyle.YesNo, "Riprova?")
                    : Interaction.MsgBox("There are " + notDownloaded + " failed downloads. Would you like to retry them?", MsgBoxStyle.YesNo, "Retry?");
                if (ans == MsgBoxResult.Ok || ans == MsgBoxResult.Yes)
                {
                    //Let's take only the downloads that have failed and were NOT completed.
                    HashSet<string> failedCompletely = new HashSet<string>();
                    
                    foreach (string s in downloadForm.downloadPool.failedButRetried.GetURIs())
                        failedCompletely.Add(s);

                    foreach (string s in downloadForm.downloadPool.failWithoutRetry.GetURIs())
                        failedCompletely.Add(s);
                        
                    foreach (string s in downloadForm.downloadPool.success.GetURIs())
                        failedCompletely.Remove(s);

                        
                    downloadForm.setText(failedCompletely);
                    CloseThis("");
                }
            }
        }

        private int startedDownloads = 0;

        internal void UpdateFileNum()
        {
            UpdateFileNum2(new ProgressUpdateClass(ProgressUpdate.STARTED, false));
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
                    r = this.downloadForm.downloadPool.failWithoutRetry;
                    break;

                case HowEnded.NOT_ENDED_YET:
                    r = this.downloadForm.downloadPool.current;
                    break;
            }

            if (r == null || r.GetCount() == 0)
            {
                if (Program.IsItalian)
                    MessageBox.Show("Nessun risultato!");
                else
                    MessageBox.Show("No results!");
                return;
            }

            ResultsListForm resultsListForm = new ResultsListForm(r, howEnded);
            resultsListForm.Show();
        }

        private void ProgressTracker_Load(object sender, EventArgs e)
        {
            buttonInfoCurrent.Text = buttonInfoCompleted.Text = buttonInfoFailed.Text = Program.IsItalian ? "Ulteriori informazioni" : "More info";
            CurrentDL.Text = Program.IsItalian ? "Download in corso" : "Currently downloading";
            SuccessfulDL.Text = Program.IsItalian ? "Download completati" : "Successfully downloaded";
            FailedDL.Text = Program.IsItalian ? "Download falliti" : "Failed";
            CurrentLbl.Text = Program.IsItalian ? "Corrente" : "Current";
            TotalLbl.Text = Program.IsItalian ? "Totale" : "Overall";
        }
    }
}