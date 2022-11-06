using PoliDLGUI.Forms;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PoliDLGUI
{
    internal static class Program
    {
        public static string RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WebExRec";
        public static bool ForceLog = Environment.CommandLine.Contains("--verboselogging"); //Debug argument to log everything
        public static bool IsItalian = Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "it-IT";

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Check for required DLLs and extract them if they're missing:
            string CurrentFolder = Environment.CurrentDirectory;
            if (!File.Exists(CurrentFolder + @"\Microsoft.WindowsAPICodePack.dll"))
                File.WriteAllBytes(CurrentFolder + @"\Microsoft.WindowsAPICodePack.dll", My.Resources.Resources.Microsoft_WindowsAPICodePack);
            if (!File.Exists(CurrentFolder + @"\Microsoft.WindowsAPICodePack.Shell.dll"))
                File.WriteAllBytes(CurrentFolder + @"\Microsoft.WindowsAPICodePack.Shell.dll", My.Resources.Resources.Microsoft_WindowsAPICodePack_Shell);
            if (!File.Exists(CurrentFolder + @"\System.IO.Compression.ZipFile.dll"))
                File.WriteAllBytes(CurrentFolder + @"\System.IO.Compression.ZipFile.dll", My.Resources.Resources.System_IO_Compression_ZipFile);
            if (!File.Exists(CurrentFolder + @"\System.Json.dll"))
                File.WriteAllBytes(CurrentFolder + @"\System.Json.dll", My.Resources.Resources.System_Json);

            Application.Run(new DownloadForm());
        }
    }
}