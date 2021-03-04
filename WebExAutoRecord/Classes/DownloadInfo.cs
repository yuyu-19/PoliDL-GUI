using PoliDLGUI.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PoliDLGUI.Classes
{
    public class DownloadInfo
    {
        public int currentsegmenttotal;
        public int currentfiletotalS;
        public int currentfiletotal;
        internal Process process;
        public string StreamArgs;  // Why? Because I need to access it from outside where it was declared. Sue me. I'm tired of working on this godforsaken program.
        public int currentfile;
        public double currentprogress = 0d;
        public string CurrentSpeed = "";
        public ProgressTracker progressTracker;

        public bool StreamIsVideo = false;
        public int NotDownloadedW;

        //public Process GlobalProcess;
        public bool DLError;

        public double WebexProgress = 0d;
        public int NotDownloaded = -1;
        public PoliDLGUI.Enums.HowEnded ended = Enums.HowEnded.NOT_ENDED_YET;
        internal string Command;
        internal string Arguments;

        public DownloadPool owner;
        public Uri uri;

        public List<string> Log = new List<string>();

        public DownloadInfo(ProgressTracker progressTracker, DownloadPool owner, Uri uri)
        {
            this.progressTracker = progressTracker;
            this.owner = owner;
            this.uri = uri;
        }

        internal void EndedSuccessfully(bool isItalian)
        {
            if (isItalian)
            {
                this.CurrentSpeed = "Finito.";
            }
            else
            {
                this.CurrentSpeed = "Finished.";
            }

            this.ended = Enums.HowEnded.SUCCESS;

            this.owner.Ended(this, Enums.HowEnded.SUCCESS);
        }

        internal void AppendLog(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                this.Log.Add(data);
            }
        }

        internal void Failed(bool segmented)
        {
            this.NotDownloaded = Math.Max(this.NotDownloaded, 0) + Math.Max(this.NotDownloadedW, 0);
            if (segmented)
            {
                if (StartupForm.IsItalian)
                {
                    Console.WriteLine("È fallito il download di " + this.NotDownloaded + " video. Riprova più tardi, oppure prova in modalità unsegmented.");
                }
                else
                {
                    Console.WriteLine("Could not download " + this.NotDownloaded + " videos. Please try again later, or try unsegmented mode.");
                }
            }
            else if (StartupForm.IsItalian)
            {
                Console.WriteLine("È fallito il download di " + this.NotDownloaded + " video. Riprova più tardi.");
            }
            else
            {
                Console.WriteLine("Could not download " + this.NotDownloaded + " videos. Please try again later.");
            }

            this.ended = Enums.HowEnded.FAIL;

            this.owner.Ended(this, Enums.HowEnded.FAIL);
        }

        internal void ClickedMoreInfo()
        {
            LogViewForm logViewForm = new LogViewForm(this.Log);
            logViewForm.ShowDialog();
        }
    }
}