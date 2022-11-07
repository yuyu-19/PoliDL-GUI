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
            bool segmented = false;
            bool shouldLog = true;  //Removes unnecessary/spammy logs
            if (process.StartInfo.FileName.Contains("polidown.exe"))
                segmented = true;    // polidown is always in segmented mode

            if (string.IsNullOrEmpty(outLine.Data))
            {
                return;
            }

            DownloadInfo downloadinfo = null;

            try
            {
                downloadinfo = this.downloadForm.downloadPool.Find(process);
            }
            catch
            {
                ;
            }

            if (downloadinfo == null)
                return;

            string outLineData = outLine.Data.Trim();

            switch (outLineData)
            {
                case "You are not authorized to access this video.":
                    downloadinfo.Failed(segmented, false);
                    break;

                case "Video already downloaded. Skipping...":
                case "All requested videos have been downloaded!":
                case "Done!":
                    downloadinfo.EndedSuccessfully(Program.IsItalian);
                    break;

                case var s when s.Contains("Video title is:"):
                    var title = outLineData.Substring(("Video title is:").Length).Trim();
                    downloadinfo.title = title;
                    break;

                case var s when s.Contains("Bad credentials"):
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
                        File.Delete(Program.RootFolder + @"\Poli-pkg\dist\config.json");
                        MessageBox.Show(
                            Program.IsItalian ?
                            "Credenziali errate. Riprova, ti verrà chiesto di reinserirle." :
                            @"Bad credentials. Please try again, you will be prompted to input them. If that didn't happen, please delete the file at %APPDATA%\WebExRec\Poli-pkg\dist\config.json");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Program.IsItalian ?
                            @"Credenziali errate. Non è stato possibile cancellare il file %APPDATA%\WebExRec\Poli-pkg\dist\config.json, per favore fallo manualmente" :
                            @"Bad credentials. We could not delete the file at %APPDATA%\WebExRec\Poli-pkg\dist\config.json, please do so manually");

                        Console.WriteLine(ex);
                    }

                    // Might as well stay on the safe side - if one of them is outdated, it's likely the other one is as well.

                    downloadinfo.CurrentSpeed = Program.IsItalian ? "Finito." : "Finished.";
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
                    break;

                case var s when s.Contains("Start downloading video"):
                    if (downloadinfo.DLError)
                    {
                        downloadinfo.DLError = false;
                    }
                    else
                    {
                        downloadinfo.currentprogress = downloadinfo.currentfile / (double)downloadinfo.currentfiletotal * 100d;  // Let's ensure we're at the correct progress.
                        downloadinfo.currentfile += 1;
                    }
                    break;

                case var s when (s.Contains("Bad credentials") & s.Contains("item(s)") & segmented):
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
                    break;

                case var s when s.Contains("0B CN") & !segmented:
                    // Means it's an update. We can get the speed from here.
                    downloadinfo.CurrentSpeed = outLine.Data.Substring(outLine.Data.IndexOf("DL:") + "DL:".Length, outLine.Data.Length - 1 - (outLine.Data.IndexOf("DL:") + "DL:".Length)) + "/s";
                    shouldLog = false;
                    break;

                case var s when s.Contains("[DL:") & segmented:
                    try
                    {
                        downloadinfo.CurrentSpeed = outLine.Data.Substring("[DL:".Length, outLine.Data.IndexOf("]") - "[DL:".Length) + "/s";
                        if (downloadinfo.CurrentSpeed == "0B/s")
                        {
                            downloadinfo.CurrentSpeed = Program.IsItalian ? "Sto leggendo dal disco..." : "Reading from disk...";
                        }
                        shouldLog = false;
                    }
                    catch
                    {
                        ;
                    }
                    break;

                case var s when s.Contains("Download complete:"):
                    downloadinfo.currentprogress += segmented ?
                        (1d / downloadinfo.currentfiletotal / downloadinfo.currentsegmenttotal * 100d) :
                        (1d / downloadinfo.currentfiletotal * 100d);

                    if (segmented) shouldLog = false;   //Segmented gets very spammy with this specific line.
                    break;

                case var s when s.Contains("Download has already completed:") & segmented:
                    // Hey, as long as it works.
                    downloadinfo.currentprogress -= 1d / downloadinfo.currentfiletotal / downloadinfo.currentsegmenttotal * 100d;
                    shouldLog = false;
                    break;

                case var s when s.Contains("These videos have not been downloaded:"):
                    downloadinfo.NotDownloaded = 0;
                    if (outLine.Data.Contains("https://"))
                    {
                        downloadinfo.NotDownloaded += 1;
                    }
                    break;

                case var s when s.Contains("https://") & downloadinfo.NotDownloaded != -1:
                    int tempi = -1;
                    do
                    {
                        tempi = outLine.Data.IndexOf("https://", tempi + 1);
                        downloadinfo.NotDownloaded += 1;
                    }
                    while (tempi != -1);
                    downloadinfo.NotDownloaded -= 1;  // THe above loop always counts one extra and I'm too lazy to figure out a good alternative method.
                    break;

                case var s when s.Contains("Done!"):
                    if (downloadinfo.DLError)
                        downloadinfo.currentfile -= 1;
                    downloadinfo.currentprogress = downloadinfo.currentfile / (double)downloadinfo.currentfiletotal * 100d;  // Let's ensure we're at the correct progress.
                    if (process.StartInfo.FileName.Contains("polidown.exe") || downloadinfo.currentfiletotalS == 0)
                    {
                        // Either we've finished polidown, or there's no msstream links to download.
                        downloadinfo.CurrentSpeed = Program.IsItalian ? "Finito." : "Finished.";

                        if (downloadinfo.NotDownloaded != -1 | downloadinfo.NotDownloadedW != -1)
                        {
                            downloadinfo.Failed(segmented, false);
                        }

                        downloadinfo.EndedSuccessfully(Program.IsItalian);

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
                    break;

                case var s when s.Contains("Going to the next one") | s.Contains("TimeoutError"):
                    if (downloadinfo.Arguments.IndexOf("-l false") != -1)
                    {
                        downloadinfo.DLError = true;
                        downloadinfo.Failed(segmented, retry: false);
                    }
                    else { 
                        downloadForm.RunCommandH(
                            process.StartInfo.FileName,
                            process.StartInfo.Arguments.Replace("-i 3", "-i 10") + " -l false",
                            downloadinfo.currentfiletotalS, downloadinfo.currentfiletotal,
                            downloadinfo.uri
                        );

                        downloadinfo.Failed(segmented, retry: false);
                    }

                    break;

                case var s when s.Contains("This video is password protected") | s.Contains("Wrong password!"):
                    if (outLine.Data.Contains("Wrong password!"))
                    {
                        MessageBox.Show("Previous password was incorrect. Please try again.");
                        if (this.downloadForm.lastUsedVideoPW != "")
                            this.downloadForm.lastUsedVideoPW = "";
                    }
                    
                    if (this.downloadForm.lastUsedVideoPW != "")
                    {
                        process.StandardInput.WriteLine(this.downloadForm.lastUsedVideoPW);
                        break;  //Re-use the password.
                    }

                    string Password = Program.IsItalian
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

                    if (Password.Contains("_ReUseForAllVideos!"))
                    {
                        this.downloadForm.lastUsedVideoPW = Password.Substring(0, Password.Length - "_ReUseForAllVideos!".Length);
                        Password = this.downloadForm.lastUsedVideoPW;
                    }

                    process.StandardInput.WriteLine(Password);
                    // MessageBox.Show("Input Sent!")
                    break;

                case var s when s.Contains("ffmpeg version") & downloadinfo.CurrentSpeed != "Setting up..." & downloadinfo.CurrentSpeed != "Sto avviando...":
                    downloadinfo.CurrentSpeed = Program.IsItalian ? "Sto elaborando il file..." : "Processing file...";
                    break;

                case var s when s.Contains("Try in non-headless mode") | s.Contains("this is not an exact science"):

                    if (process.StartInfo.FileName.Contains("poliwebex.exe"))
                        downloadinfo.WebexProgress = 0d;
                    downloadForm.RunCommandH(
                        process.StartInfo.FileName,
                        process.StartInfo.Arguments.Replace("-i 3", "-i 10") + " -l false",
                        downloadinfo.currentfiletotalS, downloadinfo.currentfiletotal,
                        downloadinfo.uri
                    );

                    downloadinfo.Failed(segmented, retry: false);

                    break;

                case var s when s.Contains("You need aria2c in $PATH for this to work"):
                    // This is a weird bug that just kinda...popped up. I'm not sure if it's an issue with my multiple desktops program, but juuuuuust to be on the safe side
                    // If this happens, let's just ask the user to try again.
                    // I can't really fix this as it doesn't really make any sense? And I really don't have enough info.

                    MessageBox.Show(Program.IsItalian ?
                        "Qualcosa è andato storto. Per favore riprova." :
                        "Something went wrong. Please try again.");

                    downloadinfo.EndedSuccessfully(Program.IsItalian);
                    break;

                case var s when s.Contains("We're already in non-headless mode"):
                    MessageBox.Show(Program.IsItalian ?
                    "Qualcosa è andato storto! Per favore crea un issue su github, e allega il file PoliDL-Logs.txt che puoi trovare in " + Program.RootFolder :
                    "Something went wrong! Please file a github issue, and attach the PoliDL-Logs.txt file you can find in " + Program.RootFolder);

                    Application.Exit();
                    break;

                case var s when s.Contains("|OK") | s.Contains("for reading") | s.Contains("CN:1 DL:"):
                    shouldLog = false;  //Just to make the logs cleaner still
                    break;
            }


            try
            {
                if ((Program.ForceLog | shouldLog) & (outLine.Data.Trim() != ""))
                { //Really likes printing empty lines for some reason.
                    this.downloadForm.LogsStream.Write(outLine.Data + Constants.vbCrLf);
                    downloadinfo.AppendLog(outLine.Data);
                }
            }
            catch (Exception ex)
            {
                // Error writing to the log file.
                Console.WriteLine(ex);
            }
        }
    }
}