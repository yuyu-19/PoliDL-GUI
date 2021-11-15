using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.WindowsAPICodePack.Dialogs;
using PoliDLGUI.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Json;
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
            _CheckSegmented.Checked = true;
        }
        public string lastUsedVideoPW = "";
        public DownloadPool downloadPool = null;

        public StreamWriter LogsStream = null;
        private readonly ProgressTracker progressTracker;

        //private readonly ProgressTracker progressTracker;

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
                    COPF.Filters.Add(new CommonFileDialogFilter("TXT", "txt"));
                }
                else
                {
                    COPF.Filters.Add(new CommonFileDialogFilter("HTML file", "html,htm"));
                    COPF.Filters.Add(new CommonFileDialogFilter("Excel file", "xlsx"));
                    COPF.Filters.Add(new CommonFileDialogFilter("Word file", "docx"));
                    COPF.Filters.Add(new CommonFileDialogFilter("Zip (of docx/xlsx/html files)", "zip"));
                    COPF.Filters.Add(new CommonFileDialogFilter("TXT", "txt"));
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
            this.BringToFront();
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
            this.BringToFront();
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
                CheckSegmented.Text = "Unsegmented" + Constants.vbCrLf + "(Più affidabile)";
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

        public const int maxDownloadInParallel = 7;

        private void DLButton_Click(object sender, EventArgs e)
        {
            if (downloadPool == null)
            {
                downloadPool = new DownloadPool(maxDownloadInParallel, this, this.progressTracker);
            }

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
                try
                {
                    GetAllRecordingLinks(URLlist.Text, ref WebexURLs, ref StreamURLs);
                } catch (InvalidOperationException ex)
                {
                    if (StartupForm.IsItalian)
                    {
                        MessageBox.Show("Rilevato link recman. Per scaricare video dall'archivio registrazioni, devi andare nella pagina webeep del tuo corso, e copiare il link \"Archivio registrazioni\" cliccando con il tasto destro.");
                    }
                    else
                    {
                        MessageBox.Show("Detected a recman link. In order to download videos from the recording archive, you must go on the webeep page for your course, and copy the \"Recordings archive\" link by right clicking.");
                    }
                    WebexURLs.Clear();
                    StreamURLs.Clear();
                }
                
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
                ZipArchive XFile = null;
                try
                {
                    switch (extension ?? "")
                    {
                        case "txt":
                            {
                                GetAllRecordingLinks(File.ReadAllText(FilePath.Text), ref WebexURLs, ref StreamURLs);
                                break;
                            }
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
                catch (InvalidOperationException ex)
                {
                    if (StartupForm.IsItalian)
                    {
                        MessageBox.Show("Rilevato link recman. Per scaricare video dall'archivio registrazioni, devi andare nella pagina webeep del tuo corso, e copiare il link \"Archivio registrazioni\" cliccando con il tasto destro.");
                    }
                    else
                    {
                        MessageBox.Show("Detected a recman link. In order to download videos from the recording archive, you must go on the webeep page for your course, and copy the \"Recordings archive\" link by right clicking.");
                    }
                    WebexURLs.Clear();
                    StreamURLs.Clear();
                } 
                finally
                {
                    if (XFile != null)
                    {
                        XFile.Dispose();
                    }
                }
            }
            
            

            if (ModeSelect.SelectedIndex == 2)
            {
                File.Delete(FilePath.Text);
            }

            // poliwebex.exe -v [URL ARRAY] -o [OUTPUT DIR] -s
            // We're calling it with the -v [URL ARRAY] option, so let's build the string.

            // Check if config.json exists. If it does, get the email and ID from it, as well as if the password is saved or not.

            string WebexArgs = "-t -i 3 -l false -o \"" + FolderPath.Text + "\"";
            
            string StreamArgs = "-t -q 5 -i 3 -l false -o \"" + FolderPath.Text + "\"";

            string TempString;
            
            try
            {
                var tmpObj = JsonValue.Parse(File.ReadAllText(StartupForm.RootFolder + @"\Poli-pkg\dist\config.json"));
            } catch
            {
                //Config is fucked, delete it.
                File.Delete(StartupForm.RootFolder + @"\Poli-pkg\dist\config.json");
            }
            
            if (File.Exists(StartupForm.RootFolder + @"\Poli-pkg\dist\config.json"))
            {
                string Config = File.ReadAllText(StartupForm.RootFolder + @"\Poli-pkg\dist\config.json");
                if (!Config.Contains("SPID")) {
                    Config = ""; //Erase the config.
                }
                if (!Config.Contains("codicePersona"))
                {
                    if (StartupForm.IsItalian)
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Inserisci il tuo nome utente per SPID", this.Location));
                    }
                    else
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Please input your SPID username", this.Location));
                    }

                    WebexArgs += " -u " + TempString;
                    StreamArgs += " -u " + TempString;
                }

                if (!Config.Contains("email") && WebexURLs.Count > 0)
                {
                    if (StartupForm.IsItalian)
                    {
                        WebexArgs += " -e " + Conversions.ToString(InputForm.AskForInput("Inserisci la tua email (nome.cognome@mail.polimi.it)", this.Location));
                    }
                    else
                    {
                        WebexArgs += " -e " + Conversions.ToString(InputForm.AskForInput("Please input your email (name.surname@mail.polimi.it)", this.Location));
                    }
                }

                if (!Config.Contains("passwordSaved") || !(Config.IndexOf("true", Config.IndexOf("passwordSaved")) == Config.IndexOf("passwordSaved") + "passwordSaved\": ".Length))
                {
                    // Does the passwordsaved value exist?
                    // Is the true right after the passwordSaved keyword?
                    // Checking the position in this way also checks wheter or not it's set to true.
                    if (StartupForm.IsItalian)
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Inserisci la tua password SPID", this.Location));
                    }
                    else
                    {
                        TempString = Conversions.ToString(InputForm.AskForInput("Please input your SPID password", this.Location));
                    }

                    WebexArgs += " -p " + TempString;
                    StreamArgs += " -p " + TempString;
                }
            }
            else    // Nothing is saved, ask everything to make sure.
            {
                if (StartupForm.IsItalian)
                {
                    TempString = Conversions.ToString(InputForm.AskForInput("Inserisci il tuo nome utente SPID", this.Location));
                }
                else
                {
                    TempString = Conversions.ToString(InputForm.AskForInput("Please input your SPID username", this.Location));
                }

                WebexArgs += " -u " + TempString;
                StreamArgs += " -u " + TempString;
                if (StartupForm.IsItalian)
                {
                    TempString = Conversions.ToString(InputForm.AskForInput("Inserisci la tua password SPID", this.Location));
                }
                else
                {
                    TempString = Conversions.ToString(InputForm.AskForInput("Please input your SPID password", this.Location));
                }

                WebexArgs += " -p " + TempString;
                StreamArgs += " -p " + TempString;
                if (WebexURLs.Count > 0)
                {
                    if (StartupForm.IsItalian)
                    {
                        WebexArgs += " -e " + Conversions.ToString(InputForm.AskForInput("Inserisci la tua email (nome.cognome@mail.polimi.it)", this.Location));
                    }
                    else
                    {
                        WebexArgs += " -e " + Conversions.ToString(InputForm.AskForInput("Please input your email (name.surname@mail.polimi.it)", this.Location));
                    }
                }
            }


            WebexArgs += " -v";
            StreamArgs += " -v";

            /*
            foreach (string URL in WebexURLs)
                WebexArgs += " \"" + URL + "\"";
            foreach (string URL in StreamURLs)
                StreamArgs += " \"" + URL + "\"";
            */

            // Time to boot up poliwebex.

            if (!Directory.Exists(StartupForm.RootFolder + @"\Logs"))
                Directory.CreateDirectory(StartupForm.RootFolder + @"\Logs");

            if (LogsStream == null)
            {
                LogsStream = new StreamWriter(StartupForm.RootFolder + @"\Logs\" + @"\PoliDL-Logs_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt", append: false)
                {
                    AutoFlush = true
                };
            }

            //We need to ensure the recman links are properly converted, and for that we need the username/password, so we will run the extraction here.
            List <string> tempURLs = new List<string>();
            foreach (var x in WebexURLs)
            {
                if (x.Contains("aunicalogin.polimi.it/aunicalogin/getservizio.xml"))
                {
                    //Extract links from every URL.
                    var extrInfo = new ProcessStartInfo(StartupForm.RootFolder + @"\Poli-pkg\dist\poliwebex.exe", WebexArgs + " \"" + x + "\" -x true");
                    LogsStream.WriteLine(WebexArgs);

                    bool debugOut = false;
                    extrInfo.RedirectStandardOutput = !debugOut;
                    extrInfo.UseShellExecute = false;
                    extrInfo.CreateNoWindow = !debugOut;
                    extrInfo.WorkingDirectory = StartupForm.RootFolder + @"\Poli-pkg\dist\";

                    var extract = new Process();
                    extract.StartInfo = extrInfo;

                    if (StartupForm.IsItalian)
                    {
                        MessageBox.Show("Rilevato link recman. Premi OK per avviare l'estrazione dei link (potrebbe richiedere qualche minuto).");
                    }
                    else
                    {
                        MessageBox.Show("Detected a recman link. Press OK to begin the link extraction (it could take a couple minutes).");
                    }
                    extract.Start();

                    extract.WaitForExit();
                    StreamReader stdOut = extract.StandardOutput;
                    string tmp = "";
                    int readLinks = 0;
                    while(!stdOut.EndOfStream)
                    {
                        tmp = stdOut.ReadLine().Trim();
                        //MessageBox.Show(tmp);
                        if (tmp == "]")
                            readLinks = 0;

                        if (readLinks == 2)
                            tempURLs.Add(tmp.Replace("'", "").Replace(",", ""));
                            
                        if (tmp == "Extracted links:" || tmp == "[")
                            readLinks++; //Make sure we're pulling from the correct list.
                        LogsStream.WriteLine(tmp);
                    }
                }
            }

            //Cleanup time. Remove all recman links.
            WebexURLs.RemoveAll(x => x.Contains("aunicalogin.polimi.it/aunicalogin/getservizio.xml"));

            WebexURLs.AddRange(tempURLs);

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

            int total = 0;
            if (StreamURLs != null)
            {
                total += StreamURLs.Count;
            }
            if (WebexURLs != null)
            {
                total += WebexURLs.Count;
            }

            progressTracker.OverallProgressCurrent.Value = 0;
            progressTracker.OverallProgressCurrent.Minimum = 0;
            progressTracker.OverallProgressTotal.Value = 0;
            progressTracker.OverallProgressTotal.Minimum = 0;
            progressTracker.OverallProgressTotal.Maximum = total;
            downloadPool.total = total;

            if (StreamURLs != null && StreamURLs.Count > 0)
            {
                foreach (var x in StreamURLs)
                {
                    string s2 = StreamArgs;
                    s2 += " \"" + x + "\"";
                    RunCommandH(StartupForm.RootFolder + @"\Poli-pkg\dist\polidown.exe", s2, StreamURLs.Count, WebexURLs.Count, new Uri(x));
                }
            }

            foreach (var x in WebexURLs)
                LogsStream.WriteLine(x);

            if (WebexURLs != null && WebexURLs.Count > 0)
            {
                foreach (var x in WebexURLs)
                {
                    string s2 = WebexArgs;
                    s2 += " \"" + x + "\"";
                    RunCommandH(StartupForm.RootFolder + @"\Poli-pkg\dist\poliwebex.exe", s2, StreamURLs.Count, WebexURLs.Count, new Uri(x));
                }
            }

            this.Hide();
            progressTracker.Show();
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
            //Check if there's a recman link, if there is clear the list, pop up an explenation and return.
            if (AllText.Contains("recman_frontend"))
            {
                throw new InvalidOperationException("Recman link detected. Warn user.");
            }

            if (string.IsNullOrEmpty(AllText))
                return;

            AllText = System.Net.WebUtility.UrlDecode(AllText);  // Fixes the URL encoding weirdness

            AllText = AllText.Trim();

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

            // This way it should just keep working no matter the link format.
            // Why did I do this you ask? Because I've stumbled across a fourth goddamn URL format, and with the way I've been doing I would've had to add support for each fucking URL scheme
            // And I've just about had it with this thing

            // Also if you're actually reading these comments god bless your soul and I apologize for the profanity (not really, bugger off)
            // I've also been experiencing a bug which seems to be related to the virtual desktop program I'm using so whatever
            
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

            i = AllText.IndexOf("polimi365.sharepoint.com");
            while (i != -1)
            {
                var r = new Regex("([^a-zA-Z0-9-_]+)|$");    // This one excludes the - and _ characters from the match
                string NewURL;
                NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("?uniqueId=", i) + "?uniqueId=".Length).Index - i).Trim();
                NewURL = "https://" + NewURL;
                if (!StreamURLs.Contains(NewURL))
                    StreamURLs.Add(NewURL);

                // CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
                i = AllText.IndexOf("polimi365.sharepoint.com", i + 1);
            }

            //One more loop, for CORRECT recman links - turn them immediately into the corresponding webex links and save them.
            i = AllText.IndexOf("aunicalogin.polimi.it/aunicalogin/getservizio.xml");
            while (i != -1)
            {
                var r = new Regex("([^a-zA-Z0-9-_]+)|$");    // This one excludes the - and _ characters from the match
                string NewURL;
                NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("c_classe_webeep=", i) + "?c_classe_webeep=".Length).Index - i).Trim();
                NewURL = "https://" + NewURL;

                if (!WebexURLs.Contains(NewURL))
                    WebexURLs.Add(NewURL);

                // CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
                i = AllText.IndexOf("aunicalogin.polimi.it/aunicalogin/getservizio.xml", i + 1);
            }

        }

        public void RunCommandH(string Command, string Arguments, int StreamURLs, int WebexURLs, Uri uri)
        {
            // Console.WriteLine(Command)
            // Console.ReadLine()
            //if (Command.Contains("poliwebex") & Arguments.Contains("-l false")) MessageBox.Show("Running in non-headless: " + Command + Arguments);
            if (badCredentials)
            {
                this.Close();
                return;
            }

            var oProcess = new Process();
            DownloadInfo downloadInfo = new DownloadInfo(progressTracker, downloadPool, uri)
            {
                process = oProcess,
                currentfile = 0,
                currentfiletotalS = StreamURLs,
                currentfiletotal = WebexURLs + StreamURLs
            };

            if (StartupForm.IsItalian)
            {
                downloadInfo.CurrentSpeed = "Sto avviando...";
            }
            else
            {
                downloadInfo.CurrentSpeed = "Setting up...";
            }

            downloadInfo.Command = Command;
            downloadInfo.Arguments = Arguments;
            downloadPool.Add(downloadInfo);
        }

        public bool badCredentials = false;

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
                try
                {
                    this.progressTracker.CloseThis(null);
                }
                catch
                {
                    ;
                }

                this.Close();
            }
        }

        private void CheckSegmented_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckSegmented.Checked == false)
            {
                int ans;
                if (StartupForm.IsItalian)
                {
                    ans = (int)Interaction.MsgBox("Sei sicuro? Questo renderà il download più veloce su PC sufficientemente potenti, ma meno affidabile.", MsgBoxStyle.YesNo, "Download segmentato?");
                }
                else
                {
                    ans = (int)Interaction.MsgBox("Are you sure? This will make the download faster on good enough computers, but less reliable.", MsgBoxStyle.YesNo, "Segmented download?");
                }

                if (ans != (int)DialogResult.Yes)
                {
                    CheckSegmented.Checked = true;
                }
            }
        }

        private void URLlist_TextChanged(object sender, EventArgs e)
        {
        }

        private void DownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProgressTracker.KillAllProcesses(this.downloadPool);
        }
    }
}