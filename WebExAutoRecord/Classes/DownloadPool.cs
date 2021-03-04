using PoliDLGUI.Enums;
using PoliDLGUI.Forms;
using PoliDLGUI.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace PoliDLGUI.Classes
{
    public class DownloadPool
    {
        public DownloadInfoList current = new DownloadInfoList();
        public DownloadInfoList waiting = new DownloadInfoList();
        public DownloadInfoList success = new DownloadInfoList();
        public DownloadInfoList fail = new DownloadInfoList();
        private readonly int maxCurrent;
        private readonly DownloadForm downloadForm;
        public ProgressTracker progressTracker;
        public OutputHandlerUtil outputHandler = null;
        public int total;

        public DownloadPool(int maxCurrent, DownloadForm downloadForm, ProgressTracker progressTracker)
        {
            this.maxCurrent = maxCurrent;
            this.downloadForm = downloadForm;
            this.progressTracker = progressTracker;
        }

        internal DownloadInfo Find(Process process)
        {
            try
            {
                lock (this)
                {
                    bool pred(DownloadInfo i)
                    {
                        return i.process == process;
                    }

                    return ((current.Find(pred) ?? waiting.Find(pred)) ?? success.Find(pred)) ?? fail.Find(pred);
                }
            }
            catch
            {
                ;
            }

            return null;
        }

        internal bool? WeHaveSegmentedDownloadsCurrently()
        {
            lock (this)
            {
                try
                {
                    var a1 = this.current.Select(x => x.process.StartInfo.Arguments.Contains("- s"));
                    var a2 = this.current.Select(x => x.process.StartInfo.FileName.Contains("polidown.exe"));
                    return a1.Any(x => (bool)x) || a2.Any(x => (bool)x);
                }
                catch
                {
                    ;
                }
            }

            return null;
        }

        internal void KillAll()
        {
            this.current.KillAll();
        }

        internal void Ended(DownloadInfo downloadInfo, HowEnded howEnded)
        {
            lock (this)
            {
                this.current.Remove(downloadInfo);

                bool b1 = success.Contains(downloadInfo);
                bool b2 = fail.Contains(downloadInfo);

                if (!b1 && !b2)
                {
                    switch (howEnded)
                    {
                        case HowEnded.SUCCESS:
                            {
                                success.Add(downloadInfo);
                                this.progressTracker.OneDownloadHasFinished();
                                break;
                            }
                        case HowEnded.FAIL:
                            {
                                this.progressTracker.OneDownloadHasFailed();
                                fail.Add(downloadInfo);
                                break;
                            }
                        case HowEnded.NOT_ENDED_YET:
                            break;
                    }
                }

                if (this.current.GetCount() < this.maxCurrent && waiting.GetCount() > 0)
                {
                    DownloadInfo p = this.waiting.GetAndRemoveFirst();
                    if (p != null)
                        Add2(p);
                }
            }
        }

        internal void Add(DownloadInfo downloadInfo)
        {
            lock (this)
            {
                if (current.GetCount() < this.maxCurrent)
                {
                    Add2(downloadInfo);
                }
                else
                {
                    waiting.Add(downloadInfo);
                }
            }
        }

        private void Add2(DownloadInfo downloadInfo)
        {
            if (downloadInfo == null)
                return;

            lock (this)
            {
                this.current.Add(downloadInfo);
                progressTracker.UpdateFileNum();
                bool NoOutputRedirect = false;
                ProcessStartInfo oStartInfo;
                downloadInfo.DLError = false;
                if (NoOutputRedirect)
                {
                    oStartInfo = new ProcessStartInfo(downloadInfo.Command, downloadInfo.Arguments)
                    {
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        RedirectStandardInput = false,
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Normal,
                        CreateNoWindow = false,
                        WorkingDirectory = downloadInfo.Command.Substring(0, downloadInfo.Command.LastIndexOf(@"\"))
                    };
                }
                else
                {
                    oStartInfo = new ProcessStartInfo(downloadInfo.Command, downloadInfo.Arguments)
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Normal,
                        CreateNoWindow = true,
                        WorkingDirectory = downloadInfo.Command.Substring(0, downloadInfo.Command.LastIndexOf(@"\", downloadInfo.Command.Length - 3))
                    };
                }

                downloadInfo.process.EnableRaisingEvents = true;
                downloadInfo.process.StartInfo = oStartInfo;
                downloadInfo.currentprogress = downloadInfo.WebexProgress;
                downloadInfo.NotDownloaded = -1;

                if (outputHandler == null)
                {
                    outputHandler = new OutputHandlerUtil(this.downloadForm);
                }

               

                downloadInfo.process.OutputDataReceived += outputHandler.OutputHandler;
                downloadInfo.process.ErrorDataReceived += outputHandler.OutputHandler;
                try
                {
                    downloadInfo.process.Start();
                    if (!NoOutputRedirect)
                        downloadInfo.process.BeginOutputReadLine();
                    if (!NoOutputRedirect)
                        downloadInfo.process.BeginErrorReadLine();
                }
                catch (Exception ex)
                {
                    File.WriteAllText(StartupForm.RootFolder + @"\crashreport.txt", ex.ToString());
                    if (StartupForm.IsItalian)
                    {
                        MessageBox.Show("Errore nell'avvio del processo. Informazioni sull'errore salvate in " + StartupForm.RootFolder + @"\crashreport.txt");
                    }
                    else
                    {
                        MessageBox.Show("Error starting the process. Exception info saved in crashreport.txt");
                    }

                    if (StartupForm.IsItalian)
                    {
                        downloadInfo.CurrentSpeed = "Finito.";
                    }
                    else
                    {
                        downloadInfo.CurrentSpeed = "Finished.";
                    }
                }
            }
        }

      
    }
}