using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PoliDLGUI.Classes;
using PoliDLGUI.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PoliDLGUI.Utils
{
    public class OutputHandlerUtil
    {
        private readonly DownloadForm downloadForm;

        public OutputHandlerUtil(DownloadForm downloadForm)
        {
            this.downloadForm = downloadForm;
        }

        public void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (downloadForm.badCredentials)
            {
                try
                {
                    downloadForm.CloseThis(null);
                }
                catch
                {
                    ;
                }
                return;
            }

            Process process = (Process)sendingProcess;
            bool segmented = process.StartInfo.Arguments.Contains(" -s");
            if (process.StartInfo.FileName.Contains("polidown.exe"))
                segmented = true;    // polidown is always in segmented mode

            if (string.IsNullOrEmpty(outLine.Data))
            {
                return;
            }

            DownloadInfo downloadinfo = this.downloadForm.downloadPool.Find(process);

            try
            {
                downloadinfo.AppendLog(outLine.Data);
            }
            catch
            {
                ;
            }

            if (outLine.Data.Contains("Bad credentials"))   // Output is same on both.
            {
                downloadForm.badCredentials = true;

                try
                {
                    ProgressTracker.KillAllProcesses(this.downloadForm.downloadPool);
                }
                catch
                {
                    ;
                }

                try
                {
                    File.Delete(StartupForm.RootFolder + @"\Poli-pkg\dist\config.json");
                    MessageBox.Show(
                        StartupForm.IsItalian ?
                        "Credenziali errate. Riprova, ti verrà chiesto di reinserirle." :
                        @"Bad credentials. Please try again, you will be prompted to input them. If that didn't happen, please delete the file at %APPDATA%\WebExRec\Poli-pkg\dist\config.json");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(StartupForm.IsItalian ?
                        @"Credenziali errate. Non è stato possibile cancellare il file %APPDATA%\WebExRec\Poli-pkg\dist\config.json, per favore fallo manualmente" :
                        @"Bad credentials. We could not delete the file at %APPDATA%\WebExRec\Poli-pkg\dist\config.json, please do so manually");

                    Console.WriteLine(ex);
                }

                // Might as well stay on the safe side - if one of them is outdated, it's likely the other one is as well.

                downloadinfo.CurrentSpeed = StartupForm.IsItalian ? "Finito." : "Finished.";
                downloadinfo.ended = Enums.HowEnded.FAIL;

                // I'm just going to completely erase the config.json file and kick the user back to the form.
                try
                {
                    downloadForm.CloseThis(null);
                }
                catch
                {
                    ;
                }

                return;
            }
            else if (outLine.Data.Contains("Start downloading video"))    // Output is same on both
            {
                if (downloadinfo.DLError)
                {
                    downloadinfo.DLError = false;
                }
                else
                {
                    downloadinfo.currentprogress = downloadinfo.currentfile / (double)downloadinfo.currentfiletotal * 100d;  // Let's ensure we're at the correct progress.
                    downloadinfo.currentfile += 1;
                }
            }
            // JANK IT UP
            else if (outLine.Data.Contains("Downloading") & outLine.Data.Contains("item(s)") & segmented) // aria2c output - differs slightly with polidown.
            {
                int Temp = Conversions.ToInteger(outLine.Data.Substring(outLine.Data.IndexOf("Downloading") + "Downloading".Length, outLine.Data.IndexOf("item(s)") - (outLine.Data.IndexOf("Downloading") + "Downloading".Length)).Trim());
                if (process.StartInfo.FileName.Contains("polidown.exe"))
                {
                    downloadinfo.StreamIsVideo = !downloadinfo.StreamIsVideo;
                    if (downloadinfo.StreamIsVideo)
                    {
                        downloadinfo.currentsegmenttotal = Temp * 2 + 10; // polidown has to download audio and video separately.
                    } // So I multiply it by two and add 10 for safety in case there's a mismatch between the two
                }
                else
                {
                    downloadinfo.currentsegmenttotal = Temp;
                }
            }
            else if (outLine.Data.Contains("0B CN") & !segmented)    // aria2c output
            {
                // Means it's an update. We can get the speed from here.
                downloadinfo.CurrentSpeed = outLine.Data.Substring(outLine.Data.IndexOf("DL:") + "DL:".Length, outLine.Data.Length - 1 - (outLine.Data.IndexOf("DL:") + "DL:".Length)) + "/s";
            }
            else if (outLine.Data.Contains("[DL:") & segmented)     // aria2c output
            {
                try
                {
                    downloadinfo.CurrentSpeed = outLine.Data.Substring("[DL:".Length, outLine.Data.IndexOf("]") - "[DL:".Length) + "/s";
                }
                catch
                {
                    ;
                }
                try
                {
                    if (downloadinfo.CurrentSpeed == "0B/s")
                    {
                        downloadinfo.CurrentSpeed = StartupForm.IsItalian ? "Sto leggendo dal disco..." : "Reading from disk...";
                    }
                }
                catch
                {
                    ;
                }
            }
            else if (outLine.Data.Contains("Download complete:")) // aria2c output
            {
                downloadinfo.currentprogress += segmented ?
                    (1d / downloadinfo.currentfiletotal / downloadinfo.currentsegmenttotal * 100d) :
                    (1d / downloadinfo.currentfiletotal * 100d);
            }
            else if (outLine.Data.Contains("Download has already completed:") & segmented)   // aria2c output
            {
                // Hey, as long as it works.
                downloadinfo.currentprogress -= 1d / downloadinfo.currentfiletotal / downloadinfo.currentsegmenttotal * 100d;
            }
            // MessageBox.Show(outLine.Data)
            else if (outLine.Data.Contains("These videos have not been downloaded:"))
            {
                downloadinfo.NotDownloaded = 0;
                if (outLine.Data.Contains("https://"))
                {
                    downloadinfo.NotDownloaded += 1;
                }
            }
            else if (outLine.Data.Contains("https://") & downloadinfo.NotDownloaded != -1)    //
            {
                int tempi = -1;
                do
                {
                    tempi = outLine.Data.IndexOf("https://", tempi + 1);
                    downloadinfo.NotDownloaded += 1;
                }
                while (tempi != -1);
                downloadinfo.NotDownloaded -= 1;  // THe above loop always counts one extra and I'm too lazy to figure out a good alternative method.
            }
            else if (outLine.Data.Contains("Done!"))  // Shared output.
            {
                if (downloadinfo.DLError)
                    downloadinfo.currentfile -= 1;
                downloadinfo.currentprogress = downloadinfo.currentfile / (double)downloadinfo.currentfiletotal * 100d;  // Let's ensure we're at the correct progress.
                if (process.StartInfo.FileName.Contains("polidown.exe") || downloadinfo.currentfiletotalS == 0)
                {
                    // Either we've finished polidown, or there's no msstream links to download.
                    downloadinfo.CurrentSpeed = StartupForm.IsItalian ? "Finito." : "Finished.";

                    if (downloadinfo.NotDownloaded != -1 | downloadinfo.NotDownloadedW != -1)
                    {
                        downloadinfo.Failed(segmented);
                    }

                    downloadinfo.EndedSuccessfully(StartupForm.IsItalian);
                    return;

                    //LogsStream.Close();
                }
                else
                {
                    downloadinfo.WebexProgress = downloadinfo.currentprogress;
                    if (downloadinfo.NotDownloaded != -1)
                    {
                        downloadinfo.NotDownloadedW = downloadinfo.NotDownloaded;
                    }
                    // We have some polidown links to download.
                    //RunCommandH(StartupForm.RootFolder + @"\Poli-pkg\dist\polidown.exe", StreamArgs);
                }
            }
            else if (outLine.Data.Contains("Going to the next one"))  // Shared output
            {
                downloadinfo.DLError = true;
            }
            else if (outLine.Data.Contains("This video is password protected") | outLine.Data.Contains("Wrong password!"))   // Never occurs for polidown
            {
                if (outLine.Data.Contains("Wrong password!"))
                    MessageBox.Show("Previous password was incorrect. Please try again.");
                string Password = StartupForm.IsItalian
                    ? Conversions.ToString(
                        InputForm.AskForInput(
                            "Inserisci la password per questo video: " +
                            Constants.vbCrLf + outLine.Data.Substring(outLine.Data.LastIndexOf("/") + 1), this.downloadForm.Location)
                        )
                    : Conversions.ToString(
                        InputForm.AskForInput(
                            "Please input the password for this video: " +
                            Constants.vbCrLf + outLine.Data.Substring(outLine.Data.LastIndexOf("/") + 1), this.downloadForm.Location)
                        );
                process.StandardInput.WriteLine(Password);
                // MessageBox.Show("Input Sent!")
            }
            else if (outLine.Data.Contains("ffmpeg version") & downloadinfo.CurrentSpeed != "Setting up..." & downloadinfo.CurrentSpeed != "Sto avviando...")   // ffmpeg output
            {
                downloadinfo.CurrentSpeed = StartupForm.IsItalian ? "Sto elaborando il file..." : "Processing file...";
            }
            else if (outLine.Data.Contains("Try in non-headless mode") | outLine.Data.Contains("this is not an exact science"))   // shared output
            {
                if (process.StartInfo.FileName.Contains("poliwebex.exe"))
                    downloadinfo.WebexProgress = 0d;
                downloadForm.RunCommandH(
                    process.StartInfo.FileName,
                    process.StartInfo.Arguments.Replace("-i 3", "-i 10") + " -l false",
                    downloadinfo.currentfiletotalS, downloadinfo.currentfiletotal,
                    downloadinfo.uri
                );
                try
                {
                    process.Close();
                    process.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return;
            }
            else if (outLine.Data.Contains("You need aria2c in $PATH for this to work"))  // Shared output
            {
                // This is a weird bug that just kinda...popped up. I'm not sure if it's an issue with my multiple desktops program, but juuuuuust to be on the safe side
                // If this happens, let's just ask the user to try again.
                // I can't really fix this as it doesn't really make any sense? And I really don't have enough info.

                MessageBox.Show(StartupForm.IsItalian ?
                    "Qualcosa è andato storto. Per favore riprova." :
                    "Something went wrong. Please try again.");

                downloadinfo.EndedSuccessfully(StartupForm.IsItalian);
            }
            else if (outLine.Data.Contains("We're already in non-headless mode")) // Shared output
            {
                MessageBox.Show(StartupForm.IsItalian ?
                    "Qualcosa è andato storto! Per favore crea un issue su github, e allega il file PoliDL-Logs.txt che puoi trovare in " + StartupForm.RootFolder :
                    "Something went wrong! Please file a github issue, and attach the PoliDL-Logs.txt file you can find in " + StartupForm.RootFolder);

                Application.Exit();
            }

            try
            {
                DownloadForm.LogsStream.Write(outLine.Data + Constants.vbCrLf);
            }
            catch (Exception ex)
            {
                // Error writing to the log file.
                Console.WriteLine(ex);
            }
        }
    }
}