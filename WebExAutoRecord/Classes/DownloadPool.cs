using PoliDLGUI.Enums;
using PoliDLGUI.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace PoliDLGUI.Classes
{
    public class DownloadPool
    {
        public List<DownloadInfo> current = new List<DownloadInfo>();
        public List<DownloadInfo> waiting = new List<DownloadInfo>();
        public List<DownloadInfo> success = new List<DownloadInfo>();
        public List<DownloadInfo> fail = new List<DownloadInfo>();
        private readonly int maxCurrent;
        private readonly DownloadForm downloadForm;
        public ProgressTracker progressTracker;

        public DownloadPool(int maxCurrent, DownloadForm downloadForm, ProgressTracker progressTracker)
        {
            this.maxCurrent = maxCurrent;
            this.downloadForm = downloadForm;
            this.progressTracker = progressTracker;
        }

        internal DownloadInfo Find(Process process)
        {
            bool pred(DownloadInfo i)
            {
                return i.process == process;
            }
            return current.Find(pred);
        }

        internal bool WeHaveSegmentedDownloadsCurrently()
        {
            return this.current.Select(x => x.process.StartInfo.Arguments.Contains("- s")).Any(x => x) ||
                        this.current.Select(x => x.process.StartInfo.FileName.Contains("polidown.exe")).Any(x => x);
        }

        internal void KillAll()
        {
            try
            {
                for (int i = 0; i < current.Count; i++)
                {
                    DownloadInfo x = current[i];
                    try
                    {
                        x.process.Kill();
                        x.process.Dispose();
                        current.Remove(x);
                        i--;
                    }
                    catch
                    {
                        ;
                    }
                }
            }
            catch
            {
                ;
            }
        }

        internal void Ended(DownloadInfo downloadInfo, HowEnded howEnded)
        {
            lock (this)
            {
                int a = this.current.IndexOf(downloadInfo);
                if (a >= 0 && a < this.current.Count)
                    this.current.RemoveAt(a);

                switch (howEnded)
                {
                    case HowEnded.SUCCESS:
                        {
                            success.Add(downloadInfo);
                            break;
                        }
                    case HowEnded.FAIL:
                        {
                            fail.Add(downloadInfo);
                            break;
                        }
                    case HowEnded.NOT_ENDED_YET:
                        break;
                }

                if (this.current.Count < this.maxCurrent && waiting.Count > 0)
                {
                    var p = this.waiting[0];
                    this.waiting.RemoveAt(0);
                    Add2(p);
                }
            }
        }

        internal void Add(DownloadInfo downloadInfo)
        {
            lock (this)
            {
                if (this.current.Count < this.maxCurrent)
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
                downloadInfo.process.OutputDataReceived += this.downloadForm.OutputHandler;
                downloadInfo.process.ErrorDataReceived += this.downloadForm.OutputHandler;
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