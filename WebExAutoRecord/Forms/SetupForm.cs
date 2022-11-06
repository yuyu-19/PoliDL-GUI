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
    public partial class SetupForm
    {
        public SetupForm()
        {
            InitializeComponent();
        }
        Boolean setupDone = false;
        private void SetupForm_Load(object sender, EventArgs e)
        {
            Question.Text = Program.IsItalian ? "Aggiornamento in corso, potrebbe richiedere qualche minuto..." : "Updating, could take a couple minutes...";
            Question.Location = new Point((this.Width - Question.Width) / 2, Question.Location.Y);
        }

        private void SetupForm_Shown(object sender, EventArgs e)
        {
            //Ensure that the application is FULLY RENDERED first.
            Application.DoEvents();
            // Unzip it all to a folder And use that as the root directory for everything else
            string appData = Program.RootFolder;

            // We don't have a version number, so that means the version unpacked must've been v1
            if (Directory.Exists(appData) && !File.Exists(appData + @"\version.txt"))
                File.WriteAllText(appData + @"\version.txt", "1");  

            //If the folder in appData exists and the version is out of date
            if (Directory.Exists(appData) && My.Resources.Resources.Version != (File.ReadAllText(appData + @"\version.txt") ?? "1"))
            {
                // Kill every process that is currently using the folder.
                foreach (var proc in Process.GetProcessesByName("chrome"))
                {
                    // MessageBox.Show("Process: " & proc.MainModule.FileName)
                    if (proc.MainModule.FileName.Contains(Program.RootFolder))
                    {
                        // MessageBox.Show("Killing process: " & proc.ProcessName)
                        proc.Kill();
                        proc.Dispose();
                    }
                }

                foreach (var proc in Process.GetProcessesByName("poliwebex"))
                {
                    // MessageBox.Show("Process: " & proc.MainModule.FileName)
                    if (proc.MainModule.FileName.Contains(Program.RootFolder))
                    {
                        // MessageBox.Show("Killing process: " & proc.ProcessName)
                        proc.Kill();
                        proc.Dispose();
                    }
                }

                foreach (var proc in Process.GetProcessesByName("polidown"))
                {
                    // MessageBox.Show("Process: " & proc.MainModule.FileName)
                    if (proc.MainModule.FileName.Contains(Program.RootFolder))
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
                    MessageBox.Show(Program.IsItalian ? @"Per favore elimina la cartella in %appdata%\WebExRec manualmente." : @"Please manually delete the %appdata%\WebExRec folder.");
                    //We were unable to update the data. Exit the entire application.
                    Application.Exit();
                }
            }

            //If the appData directory doesn't exist, if there was an update in progress or if poliwebex is missing
            if (!Directory.Exists(appData) | File.Exists(appData + @"\temp.zip") | !File.Exists(appData + @"\Poli-pkg\dist\poliwebex.exe"))
            {
                Cursor = Cursors.WaitCursor;
                if (File.Exists(appData + @"\temp.zip"))
                {
                    File.Delete(appData + @"\temp.zip");   // We don't know if it was done saving it to disk. Play it safe, start over.
                    Directory.Delete(appData, true);
                }

                try
                {
                    Directory.CreateDirectory(appData);
                    File.WriteAllBytes(appData + @"\temp.zip", My.Resources.Resources.Data);
                    var ZFile = ZipFile.OpenRead(appData + @"\temp.zip");
                    ZFile.ExtractToDirectory(appData);
                    ZFile.Dispose();
                    File.Delete(appData + @"\temp.zip");
                    File.WriteAllText(appData + @"\version.txt", My.Resources.Resources.Version);
                } catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Exit();
                }

                Cursor = Cursors.Default;
            }
            setupDone = true;
            Close();
        }
        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // disable user closing the form, but no one else
            if (!setupDone)
            {
                MessageBox.Show("Please wait until the program has finished updating.");
                e.Cancel = true;
            }
        }
    }
}