using PoliDLGUI.Forms;
using System;
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
        public bool ended = false;

        public DownloadInfo(ProgressTracker progressTracker)
        {
            this.progressTracker = progressTracker;
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

            this.ended = true;
            this.progressTracker.OneDownloadHasFinished();

        }
    }
}