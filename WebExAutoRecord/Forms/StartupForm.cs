using Microsoft.VisualBasic;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;

namespace PoliDLGUI.Forms
{
    public partial class StartupForm
    {
        public StartupForm()
        {
            InitializeComponent();
        }

        public static string RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WebExRec";
        //private readonly List<CourseData> Courses = new List<CourseData>();
        public static bool IsItalian = Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "it-IT";

        private void StartupForm_Load(object sender, EventArgs e)
        {
            // It was originally built with code, so I'm still going to adjust the size of everything with it, rather than re-doing it in the designer.

            var CFont = new Font(Question.Font.FontFamily, 10f, Question.Font.Style);
            Question.Font = CFont;
            CreditLabel.Font = CFont;
            CreditLabel.Text = "PoliWebex and PoliDown by @sup3rgiu " + Constants.vbCrLf + "GUI by @yuyu-19";
            CreditLabel.TextAlign = ContentAlignment.MiddleCenter;
            localmode.Name = "recbutton";
            downloadmode.Name = "dlbutton";
            localmode.AutoSize = true;
            downloadmode.AutoSize = true;

            if (IsItalian)
            {
                Question.Text = "Vuoi gestire le registrazioni locali o scaricare delle registrazioni?";
                localmode.Text = "Locale";
            }
            else
            {
                Question.Text = "Would you like to manage the local recordings or download some?";
                localmode.Text = "Local";
            }

            downloadmode.Text = "Download";
            var size = TextRenderer.MeasureText(Question.Text, CFont);
            Question.Width = size.Width;
            Question.Height = size.Height;
            Height = Question.Height + localmode.Height + 80;
            Width = Question.Width + 80;
            var p = Question.Location;
            p.X = (int)Math.Round(ClientSize.Width / 2d - Question.Width / 2d);
            p.Y = 5;
            Question.Location = p;
            p = localmode.Location;
            p.Y = Question.Height + 30;
            p.X = 10;
            localmode.Location = p;
            p.X = ClientSize.Width - downloadmode.Width - 10;
            downloadmode.Location = p;
            downloadmode.Enabled = false;
            localmode.Enabled = false;
            p.X = (int)Math.Round(ClientSize.Width / 2d - CreditLabel.Width / 2d);
            p.Y = ClientSize.Height - CreditLabel.Height - 2;
            CreditLabel.Location = p;
            // Unzip it all to a folder And use that as the root directory for everything else
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WebExRec";
            RootFolder = appData;
            string CurrentFolder = Environment.CurrentDirectory;

            // I spent hours trying to figure out a better way to do this, but MSBuild was having NONE OF IT.
            // So fuck it.

            if (!File.Exists(CurrentFolder + @"\Microsoft.Win32.TaskScheduler.dll"))
            {
                File.WriteAllBytes(CurrentFolder + @"\Microsoft.Win32.TaskScheduler.dll", My.Resources.Resources.Microsoft_Win32_TaskScheduler);
            }

            if (!File.Exists(CurrentFolder + @"\Microsoft.WindowsAPICodePack.dll"))
            {
                File.WriteAllBytes(CurrentFolder + @"\Microsoft.WindowsAPICodePack.dll", My.Resources.Resources.Microsoft_WindowsAPICodePack);
            }

            if (!File.Exists(CurrentFolder + @"\Microsoft.WindowsAPICodePack.Shell.dll"))
            {
                File.WriteAllBytes(CurrentFolder + @"\Microsoft.WindowsAPICodePack.Shell.dll", My.Resources.Resources.Microsoft_WindowsAPICodePack_Shell);
            }

            if (!File.Exists(CurrentFolder + @"\System.IO.Compression.ZipFile.dll"))
            {
                File.WriteAllBytes(CurrentFolder + @"\System.IO.Compression.ZipFile.dll", My.Resources.Resources.System_IO_Compression_ZipFile);
            }

            if (Directory.Exists(appData) && !File.Exists(appData + @"\version.txt"))
            {
                File.WriteAllText(appData + @"\version.txt", "1");  // We don't have a version number, so that means the version unpacked must've been v1
            }

            // Kill every process that is currently using the folder.
            if (Directory.Exists(appData) && (My.Resources.Resources.Version ?? "") != (File.ReadAllText(appData + @"\version.txt") ?? ""))
            {
                foreach (var proc in Process.GetProcessesByName("chrome"))
                {
                    // MessageBox.Show("Process: " & proc.MainModule.FileName)
                    if (proc.MainModule.FileName.Contains(RootFolder))
                    {
                        // MessageBox.Show("Killing process: " & proc.ProcessName)
                        proc.Kill();
                        proc.Dispose();
                    }
                }

                foreach (var proc in Process.GetProcessesByName("poliwebex"))
                {
                    // MessageBox.Show("Process: " & proc.MainModule.FileName)
                    if (proc.MainModule.FileName.Contains(RootFolder))
                    {
                        // MessageBox.Show("Killing process: " & proc.ProcessName)
                        proc.Kill();
                        proc.Dispose();
                    }
                }

                foreach (var proc in Process.GetProcessesByName("polidown"))
                {
                    // MessageBox.Show("Process: " & proc.MainModule.FileName)
                    if (proc.MainModule.FileName.Contains(RootFolder))
                    {
                        // MessageBox.Show("Killing process: " & proc.ProcessName)
                        proc.Kill();
                        proc.Dispose();
                    }
                }

                try
                {
                    Directory.Delete(appData, true); // Version mismatch - the internal data is newer than the stored one
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    if (IsItalian)
                    {
                        MessageBox.Show(@"Per favore elimina la cartella in %appdata%\WebExRec manualmente.");
                    }
                    else
                    {
                        MessageBox.Show(@"Please manually delete the %appdata%\WebExRec folder.");
                    }

                    Application.Exit();
                }
            }

            if (!Directory.Exists(appData) | File.Exists(appData + @"\temp.zip") | !File.Exists(appData + @"\StartRec.exe"))
            {
                string OldQuestion = Question.Text;
                if (IsItalian)
                {
                    Question.Text = "Setup iniziale, potrebbe richiedere qualche minuto...";
                }
                else
                {
                    Question.Text = "Running first time setup, could take a couple minutes...";
                }

                Cursor = Cursors.WaitCursor;
                if (File.Exists(appData + @"\temp.zip"))
                {
                    File.Delete(appData + @"\temp.zip");   // We don't know if it was done saving it to disk. Play it safe.
                    Directory.Delete(appData, true);
                }

                try
                {
                    Directory.CreateDirectory(appData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Exit();
                }

                try
                {
                    File.WriteAllBytes(appData + @"\temp.zip", My.Resources.Resources.Data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Exit();
                }

                var ZFile = ZipFile.OpenRead(appData + @"\temp.zip");
                try
                {
                    ZFile.ExtractToDirectory(appData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Exit();
                }

                ZFile.Dispose();
                try
                {
                    File.Delete(appData + @"\temp.zip");
                    File.WriteAllText(appData + @"\version.txt", My.Resources.Resources.Version);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Cursor = Cursors.Default;
                Question.Text = OldQuestion;
            }

            downloadmode.Enabled = true;
            localmode.Enabled = true;
        }

        public object IsWebEx(Task t)
        {
            return t.Name.Contains("WebExRec-") & !t.Name.Contains("WebExRec-OS-");
        }

        public object IsWebExOS(Task t)
        {
            return t.Name.Contains("WebExRec-OS-");
        }

        private void LocalMode_Click(object sender, EventArgs e)
        {
            GenerateDataForm generateDataForm = new GenerateDataForm();
            generateDataForm.ShowDialog();
        }

        private void DownloadMode_Click(object sender, EventArgs e)
        {
            DownloadForm downloadForm = new DownloadForm();
            downloadForm.Show();
        }

        public class CourseData
        {
            public Dictionary<string, string> Professors = new Dictionary<string, string>();
            public string Name;
            public int ID;
            public string StartDate;
            public string EndDate;
            public List<DayData> Days = new List<DayData>();
            public List<DayData> OneShots = new List<DayData>();
        }

        public class DayData
        {
            public string DayName;
            public string StartTime;
            public string EndTime;
            public string WebExLink;
            public bool TempDisabled;
        }

        private void Downloadmode_Click_1(object sender, EventArgs e)
        {
            DownloadMode_Click(sender, e);
        }

        private void Localmode_Click_1(object sender, EventArgs e)
        {
            LocalMode_Click(sender, e);
        }

        private void StartupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch
            {
                ;
            }
        }
    }
}