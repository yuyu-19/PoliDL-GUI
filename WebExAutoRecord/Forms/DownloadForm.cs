using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PoliDLGUI.Forms
{
    public partial class DownloadForm
    {
        public DownloadForm()
        {
            this.progressTracker = new ProgressTracker(this);
            InitializeComponent();
            _ModeSelect.Name = "ModeSelect";
            _Browse.Name = "Browse";
            _DLButton.Name = "DLButton";
            _BrowseFolder.Name = "BrowseFolder";
            _CheckSegmented.Name = "CheckSegmented";
        }

        public int currentsegmenttotal;
        public int currentfiletotalS;
        public int currentfiletotal;
        public bool StreamIsVideo;
        public int NotDownloadedW;
        public int currentfile;
        public double currentprogress = 0d;
        public string CurrentSpeed = "";
        public Process GlobalProcess;
        public bool DLError;
        public double WebexProgress = 0d;
        public int NotDownloaded = -1;
        public string StreamArgs;  // Why? Because I need to access it from outside where it was declared. Sue me. I'm tired of working on this godforsaken program.
        public StreamWriter LogsStream;
        private readonly ProgressTracker progressTracker;

        private void Browse_Click(object sender, EventArgs e)
        {
            var COPF = new CommonOpenFileDialog() { InitialDirectory = @"C:\\Users" };
            if (ModeSelect.SelectedIndex == 0)
            {
                COPF.IsFolderPicker = false;
                COPF.EnsureFileExists = true;
                if (StartupForm.IsItalian)
                {
                    COPF.Filters.Add(new CommonFileDialogFilter("File HTML", "html,htm"));
                    COPF.Filters.Add(new CommonFileDialogFilter("File Excel", "xlsx"));
                    COPF.Filters.Add(new CommonFileDialogFilter("File Word", "docx"));
                    COPF.Filters.Add(new CommonFileDialogFilter("Zip (di file docx/xlsx/html)", "zip"));
                }
                else
                {
                    COPF.Filters.Add(new CommonFileDialogFilter("HTML file", "html,htm"));
                    COPF.Filters.Add(new CommonFileDialogFilter("Excel file", "xlsx"));
                    COPF.Filters.Add(new CommonFileDialogFilter("Word file", "docx"));
                    COPF.Filters.Add(new CommonFileDialogFilter("Zip (of docx/xlsx/html files)", "zip"));
                }
            }
            else
            {
                COPF.IsFolderPicker = true;
                COPF.EnsurePathExists = true;
            }

            if (COPF.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FilePath.Text = COPF.FileName;
            }
        }

        private void BrowseFolder_Click(object sender, EventArgs e)
        {
            var COPF = new CommonOpenFileDialog()
            {
                InitialDirectory = @"C:\\Users",
                IsFolderPicker = true,
                EnsurePathExists = true
            };
            if (COPF.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FolderPath.Text = COPF.FileName;
            }
        }

        private void DownloadForm_Load(object sender, EventArgs e)
        {
            if (StartupForm.IsItalian)
            {
                BrowseFolder.Text = "Esplora";
                Browse.Text = "Esplora";
                ModeLbl.Text = "Modalità:";
                var CFont = new Font(ModeLbl.Font.FontFamily, 12f, ModeLbl.Font.Style);
                ModeLbl.Font = CFont;
                CheckSegmented.Text = "Usa unsegmented" + Constants.vbCrLf + "(Compatibilità)";
                DLfolderlabel.Text = "Cartella Download";
                var p = ModeLbl.Location;
                p.Y += 5;
                ModeLbl.Location = p;
                ExtensionInfo.Text = "Tipi di file supportati: html, xlsx, docx, zip (degli altri file)";
                ModeSelect.Items.Clear();
                ModeSelect.Items.Add("File");
                ModeSelect.Items.Add("Testo");
                ModeSelect.Items.Add("Cartella");
            }

            ModeSelect.SelectedIndex = 0;
        }

        private void ModeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Index = ModeSelect.SelectedIndex;
            if (Index == 2)
            {
                Index = 0; // Keep the same setup as the file if the user picked the "Folder" option
                if (StartupForm.IsItalian)
                {
                    MessageBox.Show("Assicurati che la cartella e le sue sottocartelle contengano solo i tipi di file supportati (xlsx/docx/html/zip)");
                }
                else
                {
                    MessageBox.Show("Please make sure the folder and its subfolders only contain the supported filetypes (xlsx/docx/html/zip)");
                }
            }

            // I just didn't want to do a pointless conversion to boolean when I can just do it implicitly, sue me
            FilePath.Visible = Conversions.ToBoolean(Math.Abs(Index - 1));
            Browse.Visible = Conversions.ToBoolean(Math.Abs(Index - 1));
            ExtensionInfo.Visible = Conversions.ToBoolean(Math.Abs(Index - 1));
            URLlist.Visible = Conversions.ToBoolean(Index);
            Height = 135 + 320 * Index;
            var p = DLButton.Location;
            p.Y = 67 + 322 * Index;
            DLButton.Location = p;
            p = CheckSegmented.Location;
            p.Y = 64 + 318 * Index;
            p.X = 361 * Math.Abs(Index - 1) + 16 * Index;
            CheckSegmented.Location = p;
        }

        private void DLButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FolderPath.Text))
            {
                if (StartupForm.IsItalian)
                {
                    MessageBox.Show("Per favore inserisci la cartella di download");
                }
                else
                {
                    MessageBox.Show("Please input the download folder.");
                }

                return;
            }

            List<string> WebexURLs = new List<string>(), StreamURLs = new List<string>();
            if (ModeSelect.SelectedIndex == 1)
            {
                GetAllRecordingLinks(URLlist.Text, ref WebexURLs, ref StreamURLs);
            }
            else
            {
                if (string.IsNullOrEmpty(FilePath.Text) & ModeSelect.SelectedIndex == 0)
                {
                    if (StartupForm.IsItalian)
                    {
                        MessageBox.Show("Per favore seleziona un file.");
                    }
                    else
                    {
                        MessageBox.Show("Please input the file's location.");
                    }

                    return;
                }
                else if (string.IsNullOrEmpty(FilePath.Text) & ModeSelect.SelectedIndex == 2)
                {
                    if (StartupForm.IsItalian)
                    {
                        MessageBox.Show("Per favore inserisci la cartella.");
                    }
                    else
                    {
                        MessageBox.Show("Please input the folder's location.");
                    }

                    return;
                }

                // File/Folder mode. Check the extension, and continue from there.
                if (ModeSelect.SelectedIndex == 2)
                {
                    ZipFile.CreateFromDirectory(FilePath.Text, FilePath.Text.Substring(FilePath.Text.LastIndexOf(@"\") + 1) + ".zip");
                    FilePath.Text += ".zip";
                }
                // Add support for folders with files by just making them into a zip archive.
                // That is peak laziness, I know, but I just want to be done with this goddamn thing.

                string extension = FilePath.Text.Substring(FilePath.Text.LastIndexOf(".") + 1).ToLower().Trim();
                switch (extension ?? "")
                {
                    case "html":
                    case "htm":
                        {
                            GetAllRecordingLinks(File.ReadAllText(FilePath.Text), ref WebexURLs, ref StreamURLs);
                            break;
                        }

                    case "xlsx":
                    case "docx":
                    case "zip":
                        {
                            // We're going to treat them as zip archives, and just read the xml files directly. It's simpler that way.
                            ZipArchive XFile;
                            try
                            {
                                XFile = ZipFile.OpenRead(FilePath.Text);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                return;
                            }

                            GetAllLinksFromZip(XFile, ref WebexURLs, ref StreamURLs);
                            XFile.Dispose();
                            break;
                        }

                    default:
                        {
                            if (StartupForm.IsItalian)
                            {
                                MessageBox.Show("Tipo di file non supportato. Riprova.");
                            }
                            else
                            {
                                MessageBox.Show("Unsupported file type. Please retry.");
                            }

                            return;
                        }
                }
            }

            if (ModeSelect.SelectedIndex == 2)
            {
                File.Delete(FilePath.Text);
            }

            if (WebexURLs.Count == 0 & StreamURLs.Count == 0)
            {
                if (StartupForm.IsItalian)
                {
                    MessageBox.Show("Nessun URL trovato.");
                }
                else
                {
                    MessageBox.Show("No URLs found.");
                }

                return;
            }

            // poliwebex.exe -v [URL ARRAY] -o [OUTPUT DIR] -s
            // We're calling it with the -v [URL ARRAY] option, so let's build the string.

            // Check if config.json exists. If it does, get the email and ID from it, as well as if the password is saved or not.

            string WebexArgs = "-t -i 3 -o \"" + FolderPath.Text + "\"";
            StreamArgs = "-t -q 5 -i 3 -o \"" + FolderPath.Text + "\"";
            string TempString;
            if (File.Exists(StartupForm.RootFolder + @"\Poli-pkg\dist\config.json"))
            {
                string Config = File.ReadAllText(StartupForm.RootFolder + @"\Poli-pkg\dist\config.json");
                if (!Config.Contains("codicePersona"))
                {
                    if (StartupForm.IsItalian)
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Inserisci il tuo codice persona", this.Location));
                    }
                    else
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Please input your person code", this.Location));
                    }

                    WebexArgs += " -u " + TempString;
                    StreamArgs += " -u " + TempString;
                }

                if (!Config.Contains("email") & WebexURLs.Count > 0)    // If it's webex
                {
                    if (StartupForm.IsItalian)
                    {
                        WebexArgs = Conversions.ToString(WebexArgs + Operators.ConcatenateObject(
                                " -e ",
                                InputForm.AskForInput("Inserisci la tua email (nome.cognome@mail.polimi.it)", this.Location))
                            );
                    }
                    else
                    {
                        WebexArgs = Conversions.ToString(WebexArgs + Operators.ConcatenateObject(
                                " -e ",
                                InputForm.AskForInput("Please input your email (name.surname@mail.polimi.it)", this.Location))
                            );
                    }
                }

                if (!Config.Contains("passwordSaved") || !(Config.IndexOf("true", Config.IndexOf("passwordSaved")) == Config.IndexOf("passwordSaved") + "passwordSaved\": ".Length))
                {
                    // Does the passwordsaved value exist?
                    // Is the true right after the passwordSaved keyword?
                    // Checking the position in this way also checks wheter or not it's set to true.
                    if (StartupForm.IsItalian)
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Inserisci la tua password", this.Location));
                    }
                    else
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Please input your password", this.Location));
                    }

                    WebexArgs += " -p " + TempString;
                    StreamArgs += " -p " + TempString;
                }
            }
            else    // Nothing is saved, ask everything to make sure.
            {
                if (StartupForm.IsItalian)
                {
                    TempString = Conversions.ToString(InputForm.AskForInput("Inserisci il tuo codice persona", this.Location));
                }
                else
                {
                    TempString = Conversions.ToString(InputForm.AskForInput("Please input your person code", this.Location));
                }

                WebexArgs += " -u " + TempString;
                StreamArgs += " -u " + TempString;
                if (StartupForm.IsItalian)
                {
                    TempString = Conversions.ToString(InputForm.AskForInput("Inserisci la tua password", this.Location));
                }
                else
                {
                    TempString = Conversions.ToString(InputForm.AskForInput("Please input your password", this.Location));
                }

                WebexArgs += " -p " + TempString;
                StreamArgs += " -p " + TempString;
                if (WebexURLs.Count > 0)
                {
                    if (StartupForm.IsItalian)
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Inserisci la tua email (nome.cognome@mail.polimi.it)", this.Location));
                    }
                    else
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Please input your email (name.surname@mail.polimi.it)", this.Location));
                    }
                }

                WebexArgs += " -e " + TempString;
            }

            WebexArgs += " -v";
            StreamArgs += " -v";
            foreach (string URL in WebexURLs)
                WebexArgs += " \"" + URL + "\"";
            foreach (string URL in StreamURLs)
                StreamArgs += " \"" + URL + "\"";

            // Time to boot up poliwebex.

            if (!CheckSegmented.Checked)
                WebexArgs += " -s";
            currentfile = 0;
            currentfiletotalS = StreamURLs.Count;
            currentfiletotal = WebexURLs.Count + StreamURLs.Count;
            progressTracker.OverallProgress.Value = 0;
            progressTracker.FileNum.Text = "File 0/" + currentfiletotal;
            if (StartupForm.IsItalian)
            {
                CurrentSpeed = "Sto avviando...";
            }
            else
            {
                CurrentSpeed = "Setting up...";
            }

            if (!Directory.Exists(StartupForm.RootFolder + @"\Logs"))
                Directory.CreateDirectory(StartupForm.RootFolder + @"\Logs");
            LogsStream = new StreamWriter(StartupForm.RootFolder + @"\Logs\" + @"\PoliDL-Logs_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt", false)
            {
                AutoFlush = true
            };
            StreamIsVideo = false;
            WebexProgress = 0d;
            NotDownloadedW = -1;
            if (WebexURLs.Count != 0)
            {
                RunCommandH(StartupForm.RootFolder + @"\Poli-pkg\dist\poliwebex.exe", WebexArgs);
            }
            else
            {
                RunCommandH(StartupForm.RootFolder + @"\Poli-pkg\dist\polidown.exe", StreamArgs);
            }

            this.Hide();
            progressTracker.ShowDialog();
            this.Show();
        }

        public void GetAllLinksFromZip(ZipArchive AFile, ref List<string> WebexURLs, ref List<string> StreamURLs)
        {
            foreach (var Entry in AFile.Entries)
            {
                string FileName = Entry.Name;
                string FileExtension = FileName.Substring(FileName.LastIndexOf("."));
                while (File.Exists(FileName))
                    FileName = FileName.Replace(FileExtension, "_1" + FileExtension);
                Entry.ExtractToFile(FileName, true);
                if (FileExtension == ".xlsx" | FileExtension == ".docx" | FileExtension == ".zip")
                {
                    ZipArchive XFile;
                    try
                    {
                        XFile = ZipFile.OpenRead(FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    GetAllLinksFromZip(XFile, ref WebexURLs, ref StreamURLs);
                    XFile.Dispose();
                }
                else
                {
                    GetAllRecordingLinks(File.ReadAllText(Entry.Name), ref WebexURLs, ref StreamURLs);
                }

                File.Delete(FileName);
            }
        }

        public void GetAllRecordingLinks(string AllText, ref List<string> WebexURLs, ref List<string> StreamURLs) // This just takes a big ol string (file) as input and a list, and adds all links to the list.
        {
            AllText = System.Net.WebUtility.UrlDecode(AllText);  // Fixes the URL encoding weirdness
            int i = AllText.IndexOf("politecnicomilano.webex.com/");
            while (i != -1)
            {
                var r = new Regex(@"([^a-zA-Z0-9\/.?=:]+)|$|\n");
                string NewURL = AllText.Substring(i, r.Match(AllText, i).Index - i).Trim();
                NewURL = "https://" + NewURL;
                if (!WebexURLs.Contains(NewURL))
                    WebexURLs.Add(NewURL);

                // CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
                i = AllText.IndexOf("politecnicomilano.webex.com/", i + 1);
            }

            // This was it should just keep working no matter the link format.
            // Why did I do this you ask? Because I've stumbled across a fourth goddamn URL format, and with the way I've been doing I would've had to add support for each fucking URL scheme
            // And I've just about had it with this thing

            // Also if you're actually reading these comments god bless your soul and I apologize for the profanity (not really, bugger off)
            // I've also been experiencing a bug which seems to be related to the virtual desktop program I'm using so whatever
            // I'm keeping the following parts (even if they're theoretically not necessary) JUST IN CASE SOMEONE IS BRIGHT ENOUGH TO FOLLOW A LINK UP WITH ONE OF THE ADDITIONAL SYMBOLS I EXCLUDED.
            // JUST IN CASE. Nothing could surprise me at this point. I saw a link that had https spelt wrong, which is why I'm no longer looking for "https://politecnico."

            i = AllText.IndexOf("politecnicomilano.webex.com/recordingservice/");
            // It may seem like a waste of resources to just check every time, but we can't be sure if the links are hyperlinks or not, so we'll just grab everything and see
            while (i != -1)
            {
                // We're going to use regex to check for the index of the first non-alphanumerical after the /playback/ in the link
                // This (SHOULD) let us handle most if not all cases? Since I'm assuming there'll at least be a space or something.

                var r = new Regex("([^a-zA-Z0-9]+)|$");
                string NewURL;
                if (AllText.IndexOf("/playback/", i) == -1 | AllText.IndexOf("/play/", i) < AllText.IndexOf("/playback/", i))
                {
                    NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("/play/", i) + "/play/".Length).Index - i).Trim();
                }
                else
                {
                    NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("/playback/", i) + "/playback/".Length).Index - i).Trim();
                }

                NewURL = "https://" + NewURL;
                if (!WebexURLs.Contains(NewURL))
                    WebexURLs.Add(NewURL);

                // CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
                i = AllText.IndexOf("politecnicomilano.webex.com/recordingservice/", i + 1);
            }

            // Second loop, for the RCID type links.
            i = AllText.IndexOf("politecnicomilano.webex.com/politecnicomilano/");
            while (i != -1)
            {
                var r = new Regex("([^a-zA-Z0-9]+)|$");
                string NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("RCID=", i) + "RCID=".Length).Index - i).Trim();
                NewURL = "https://" + NewURL;
                if (!WebexURLs.Contains(NewURL))
                    WebexURLs.Add(NewURL);

                // CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
                i = AllText.IndexOf("politecnicomilano.webex.com/politecnicomilano/", i + 1);
            }

            // Another loop, this time for msstream links
            i = AllText.IndexOf("web.microsoftstream.com");
            while (i != -1)
            {
                var r = new Regex("([^a-zA-Z0-9-]+)|$");    // This one excludes the - character from the match
                string NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("/video/", i) + "/video/".Length).Index - i).Trim();
                NewURL = "https://" + NewURL;
                if (!StreamURLs.Contains(NewURL))
                    StreamURLs.Add(NewURL);

                // CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
                i = AllText.IndexOf("web.microsoftstream.com", i + 1);
            }

            // And another one, for sharepoint links.
            i = AllText.IndexOf("polimi365-my.sharepoint.com");
            while (i != -1)
            {
                var r = new Regex("([^a-zA-Z0-9-_]+)|$");    // This one excludes the - and _ characters from the match
                string NewURL;
                if (AllText.IndexOf("_layouts/", i) == AllText.IndexOf("_polimi_it/", i) + "_polimi_it/".Length)
                {
                    // It's the onedrive style of link
                    NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("&originalPath=", i) + "&originalPath=".Length).Index - i).Trim();
                }
                else
                {
                    // It's the other type
                    NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("_polimi_it/", i) + "_polimi_it/".Length).Index - i).Trim();
                }

                NewURL = "https://" + NewURL;
                if (!StreamURLs.Contains(NewURL))
                    StreamURLs.Add(NewURL);

                // CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
                i = AllText.IndexOf("polimi365-my.sharepoint.com", i + 1);
            }
        }

        public void RunCommandH(string Command, string Arguments)
        {
            // Console.WriteLine(Command)
            // Console.ReadLine()

            var oProcess = new Process();
            bool NoOutputRedirect = false;
            ProcessStartInfo oStartInfo;
            DLError = false;
            if (NoOutputRedirect)
            {
                oStartInfo = new ProcessStartInfo(Command, Arguments)
                {
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    CreateNoWindow = false,
                    WorkingDirectory = Command.Substring(0, Command.LastIndexOf(@"\"))
                };
            }
            else
            {
                oStartInfo = new ProcessStartInfo(Command, Arguments)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    CreateNoWindow = true,
                    WorkingDirectory = Command.Substring(0, Command.LastIndexOf(@"\", Command.Length - 3))
                };
            }

            oProcess.EnableRaisingEvents = true;
            oProcess.StartInfo = oStartInfo;
            currentprogress = WebexProgress;
            NotDownloaded = -1;
            oProcess.OutputDataReceived += OutputHandler;
            oProcess.ErrorDataReceived += OutputHandler;
            try
            {
                oProcess.Start();
                if (!NoOutputRedirect)
                    oProcess.BeginOutputReadLine();
                if (!NoOutputRedirect)
                    oProcess.BeginErrorReadLine();
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
                    CurrentSpeed = "Finito.";
                }
                else
                {
                    CurrentSpeed = "Finished.";
                }
            }

            GlobalProcess = oProcess;
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Process process = (Process)sendingProcess;
            bool segmented = process.StartInfo.Arguments.Contains(" -s");
            if (process.StartInfo.FileName.Contains("polidown.exe"))
                segmented = true;    // polidown is always in segmented mode
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                if (outLine.Data.Contains("Bad credentials."))   // Output is same on both.
                {
                    try
                    {
                        File.Delete(StartupForm.RootFolder + @"\Poli-pkg\dist\config.json");
                        if (StartupForm.IsItalian)
                        {
                            MessageBox.Show("Credenziali errate. Riprova, ti verrà chiesto di reinserirle.");
                        }
                        else
                        {
                            MessageBox.Show(@"Bad credentials. Please try again, you will be prompted to input them. If that didn't happen, please delete the file at %APPDATA%\WebExRec\Poli-pkg\dist\config.json");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (StartupForm.IsItalian)
                        {
                            MessageBox.Show(@"Credenziali errate. Non è stato possibile cancellare il file %APPDATA%\WebExRec\Poli-pkg\dist\config.json, per favore fallo manualmente");
                        }
                        else
                        {
                            MessageBox.Show(@"Bad credentials. We could not delete the file at %APPDATA%\WebExRec\Poli-pkg\dist\config.json, please do so manually");
                        }

                        Console.WriteLine(ex);
                    }

                    // Might as well stay on the safe side - if one of them is outdated, it's likely the other one is as well.

                    if (StartupForm.IsItalian)
                    {
                        CurrentSpeed = "Finito.";
                    }
                    else
                    {
                        CurrentSpeed = "Finished.";
                    }

                    // I'm just going to completely erase the config.json file and kick the user back to the form.
                }

                if (outLine.Data.Contains("Start downloading video"))    // Output is same on both
                {
                    if (DLError)
                    {
                        DLError = false;
                    }
                    else
                    {
                        currentprogress = currentfile / (double)currentfiletotal * 100d;  // Let's ensure we're at the correct progress.
                        currentfile += 1;
                    }
                }

                // JANK IT UP

                if (outLine.Data.Contains("Downloading") & outLine.Data.Contains("item(s)") & segmented) // aria2c output - differs slightly with polidown.
                {
                    int Temp = Conversions.ToInteger(outLine.Data.Substring(outLine.Data.IndexOf("Downloading") + "Downloading".Length, outLine.Data.IndexOf("item(s)") - (outLine.Data.IndexOf("Downloading") + "Downloading".Length)).Trim());
                    if (process.StartInfo.FileName.Contains("polidown.exe"))
                    {
                        StreamIsVideo = !StreamIsVideo;
                        if (StreamIsVideo)
                        {
                            currentsegmenttotal = Temp * 2 + 10; // polidown has to download audio and video separately.
                        } // So I multiply it by two and add 10 for safety in case there's a mismatch between the two
                    }
                    else
                    {
                        currentsegmenttotal = Temp;
                    }
                }

                if (outLine.Data.Contains("0B CN") & !segmented)    // aria2c output
                {
                    // Means it's an update. We can get the speed from here.
                    CurrentSpeed = outLine.Data.Substring(outLine.Data.IndexOf("DL:") + "DL:".Length, outLine.Data.Length - 1 - (outLine.Data.IndexOf("DL:") + "DL:".Length)) + "/s";
                }

                if (outLine.Data.Contains("[DL:") & segmented)     // aria2c output
                {
                    CurrentSpeed = outLine.Data.Substring("[DL:".Length, outLine.Data.IndexOf("]") - "[DL:".Length) + "/s";
                    if (StartupForm.IsItalian)
                    {
                        if (CurrentSpeed == "0B/s")
                            CurrentSpeed = "Sto leggendo dal disco...";
                    }
                    else if (CurrentSpeed == "0B/s")
                        CurrentSpeed = "Reading from disk...";
                }

                if (outLine.Data.Contains("Download complete:")) // aria2c output
                {
                    if (segmented)
                    {
                        // MessageBox.Show(1 & "/" & currentfiletotal & "/" & currentsegmenttotal & "=" & (currentfile / currentfiletotal) / currentsegmenttotal * 100)
                        currentprogress += 1d / currentfiletotal / currentsegmenttotal * 100d;
                    }
                    else
                    {
                        // MessageBox.Show(ProgressTracker.OverallProgress.Value)
                        // MessageBox.Show(currentfile & "-" & currentfiletotal)
                        currentprogress += 1d / currentfiletotal * 100d;
                    }
                }

                if (outLine.Data.Contains("Download has already completed:") & segmented)   // aria2c output
                {
                    // Hey, as long as it works.
                    currentprogress -= 1d / currentfiletotal / currentsegmenttotal * 100d;
                }

                // MessageBox.Show(outLine.Data)

                if (outLine.Data.Contains("These videos have not been downloaded:"))
                {
                    NotDownloaded = 0;
                    if (outLine.Data.Contains("https://"))
                    {
                        NotDownloaded += 1;
                    }
                }

                if (outLine.Data.Contains("https://") & NotDownloaded != -1)    //
                {
                    int tempi = -1;
                    do
                    {
                        tempi = outLine.Data.IndexOf("https://", tempi + 1);
                        NotDownloaded += 1;
                    }
                    while (tempi != -1);
                    NotDownloaded -= 1;  // THe above loop always counts one extra and I'm too lazy to figure out a good alternative method.
                }

                if (outLine.Data.Contains("Done!"))  // Shared output.
                {
                    if (DLError)
                        currentfile -= 1;
                    currentprogress = currentfile / (double)currentfiletotal * 100d;  // Let's ensure we're at the correct progress.
                    if (process.StartInfo.FileName.Contains("polidown.exe") | currentfiletotalS == 0)
                    {
                        // Either we've finished polidown, or there's no msstream links to download.
                        if (StartupForm.IsItalian)
                        {
                            CurrentSpeed = "Finito.";
                        }
                        else
                        {
                            CurrentSpeed = "Finished.";
                        }

                        if (NotDownloaded != -1 | NotDownloadedW != -1)
                        {
                            NotDownloaded = Math.Max(NotDownloaded, 0) + Math.Max(NotDownloadedW, 0);
                            if (segmented)
                            {
                                if (StartupForm.IsItalian)
                                {
                                    MessageBox.Show("È fallito il download di " + NotDownloaded + " video. Riprova più tardi, oppure prova in modalità unsegmented.");
                                }
                                else
                                {
                                    MessageBox.Show("Could not download " + NotDownloaded + " videos. Please try again later, or try unsegmented mode.");
                                }
                            }
                            else if (StartupForm.IsItalian)
                            {
                                MessageBox.Show("È fallito il download di " + NotDownloaded + " video. Riprova più tardi.");
                            }
                            else
                            {
                                MessageBox.Show("Could not download " + NotDownloaded + " videos. Please try again later.");
                            }
                        }
                        else if (StartupForm.IsItalian)
                        {
                            MessageBox.Show("Finito!");
                        }
                        else
                        {
                            MessageBox.Show("All done!");
                        }

                        LogsStream.Close();
                    }
                    else
                    {
                        WebexProgress = currentprogress;
                        if (NotDownloaded != -1)
                        {
                            NotDownloadedW = NotDownloaded;
                        }
                        // We have some polidown links to download.
                        RunCommandH(StartupForm.RootFolder + @"\Poli-pkg\dist\polidown.exe", StreamArgs);
                    }
                }

                if (outLine.Data.Contains("Going to the next one"))  // Shared output
                {
                    DLError = true;
                }

                if (outLine.Data.Contains("This video is password protected") | outLine.Data.Contains("Wrong password!"))   // Never occurs for polidown
                {
                    if (outLine.Data.Contains("Wrong password!"))
                        MessageBox.Show("Previous password was incorrect. Please try again.");
                    string Password;
                    if (StartupForm.IsItalian)
                    {
                        Password = Conversions.ToString(
                            InputForm.AskForInput(
                                "Inserisci la password per questo video: " +
                                Constants.vbCrLf + outLine.Data.Substring(outLine.Data.LastIndexOf("/") + 1), this.Location)
                            );
                    }
                    else
                    {
                        Password = Conversions.ToString(
                            InputForm.AskForInput(
                                "Please input the password for this video: " +
                                Constants.vbCrLf + outLine.Data.Substring(outLine.Data.LastIndexOf("/") + 1), this.Location)
                            );
                    }

                    process.StandardInput.WriteLine(Password);
                    // MessageBox.Show("Input Sent!")
                }

                if (outLine.Data.Contains("ffmpeg version") & CurrentSpeed != "Setting up..." & CurrentSpeed != "Sto avviando...")   // ffmpeg output
                {
                    if (StartupForm.IsItalian)
                    {
                        CurrentSpeed = "Sto elaborando il file...";
                    }
                    else
                    {
                        CurrentSpeed = "Processing file...";
                    }
                }

                if (outLine.Data.Contains("Try in non-headless mode") | outLine.Data.Contains("this is not an exact science"))   // shared output
                {
                    if (process.StartInfo.FileName.Contains("poliwebex.exe"))
                        WebexProgress = 0d;
                    RunCommandH(process.StartInfo.FileName, process.StartInfo.Arguments.Replace("-i 3", "-i 10") + " -l false");
                    try
                    {
                        process.Close();
                        process.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                if (outLine.Data.Contains("You need aria2c in $PATH for this to work"))  // Shared output
                {
                    // This is a weird bug that just kinda...popped up. I'm not sure if it's an issue with my multiple desktops program, but juuuuuust to be on the safe side
                    // If this happens, let's just ask the user to try again.
                    // I can't really fix this as it doesn't really make any sense? And I really don't have enough info.
                    if (StartupForm.IsItalian)
                    {
                        MessageBox.Show("Qualcosa è andato storto. Per favore riprova.");
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong. Please try again.");
                    }

                    if (StartupForm.IsItalian)
                    {
                        CurrentSpeed = "Finito.";
                    }
                    else
                    {
                        CurrentSpeed = "Finished.";
                    }
                }

                if (outLine.Data.Contains("We're already in non-headless mode")) // Shared output
                {
                    if (StartupForm.IsItalian)
                    {
                        MessageBox.Show("Qualcosa è andato storto! Per favore crea un issue su github, e allega il file PoliDL-Logs.txt che puoi trovare in " + StartupForm.RootFolder);
                        Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong! Please file a github issue, and attach the PoliDL-Logs.txt file you can find in " + StartupForm.RootFolder);
                        Application.Exit();
                    }
                }

                try
                {
                    LogsStream.Write(outLine.Data + Constants.vbCrLf);
                }
                catch (Exception ex)
                {
                    // Error writing to the log file.
                    Console.WriteLine(ex);
                }
            }
        }

        private void CheckSegmented_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckSegmented.Checked == true)
            {
                int ans;
                if (StartupForm.IsItalian)
                {
                    ans = (int)Interaction.MsgBox("Sei sicuro? Questo renderà il download più lento e la barra di download meno precisa." + " È consigliato solo se stai avendo problemi.", MsgBoxStyle.YesNo, "Download non segmentato?");
                }
                else
                {
                    ans = (int)Interaction.MsgBox("Are you sure? This will make the download slower and the progress bar less accurate." + " It's only recommended if you're experiencing issues.", MsgBoxStyle.YesNo, "Unsegmented download?");
                }

                if (ans != (int)DialogResult.Yes)
                {
                    CheckSegmented.Checked = false;
                }
            }
        }
    }
}