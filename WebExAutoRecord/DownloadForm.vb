Imports System.Threading
Imports System.Globalization
Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions
Imports Microsoft.Win32.TaskScheduler
Imports Microsoft.WindowsAPICodePack.Dialogs
Imports System.Net
Imports System.Text
Imports WebExAutoRecord.StartupForm

Public Class DownloadForm

    Public Shared currentsegmenttotal As Integer
    Public Shared currentfiletotal As Integer
    Public Shared currentfile As Integer
    Public Shared currentprogress As Double = 0
    Public Shared CurrentSpeed As String = ""
    Public Shared GlobalProcess As Process
    Public Shared DLError As Boolean
    Private Sub Browse_Click(sender As Object, e As EventArgs) Handles Browse.Click

        Dim COPF As New CommonOpenFileDialog
        COPF.InitialDirectory = "C:\\Users"
        If ModeSelect.SelectedIndex = 0 Then
            COPF.IsFolderPicker = False
            COPF.EnsureFileExists = True
            If IsItalian Then
                COPF.Filters.Add(New CommonFileDialogFilter("File HTML", ".html,.htm"))
                COPF.Filters.Add(New CommonFileDialogFilter("File Excel", ".xlsx"))
                COPF.Filters.Add(New CommonFileDialogFilter("File Word", ".docx"))
                COPF.Filters.Add(New CommonFileDialogFilter("Zip (di file docx/xlsx/html)", ".zip"))
            Else
                COPF.Filters.Add(New CommonFileDialogFilter("HTML file", ".html,.htm"))
                COPF.Filters.Add(New CommonFileDialogFilter("Excel file", ".xlsx"))
                COPF.Filters.Add(New CommonFileDialogFilter("Word file", ".docx"))
                COPF.Filters.Add(New CommonFileDialogFilter("Zip (of docx/xlsx/html files)", ".zip"))
            End If

        Else
            COPF.IsFolderPicker = True
            COPF.EnsurePathExists = True
        End If

        If COPF.ShowDialog = CommonFileDialogResult.Ok Then
            FilePath.Text = COPF.FileName
        End If

    End Sub

    Private Sub BrowseFolder_Click(sender As Object, e As EventArgs) Handles BrowseFolder.Click
        Dim COPF As New CommonOpenFileDialog
        COPF.InitialDirectory = "C:\\Users"
        COPF.IsFolderPicker = True
        COPF.EnsurePathExists = True
        If COPF.ShowDialog = CommonFileDialogResult.Ok Then
            FolderPath.Text = COPF.FileName
        End If
    End Sub

    Private Sub DownloadForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        If IsItalian Then
            BrowseFolder.Text = "Esplora"
            Browse.Text = "Esplora"
            ModeLbl.Text = "Modalità:"
            Dim CFont As New Font(ModeLbl.Font.FontFamily, 12, ModeLbl.Font.Style)
            ModeLbl.Font = CFont
            CheckSegmented.Text = "Usa unsegmented" & vbCrLf & "(Compatibilità)"
            DLfolderlabel.Text = "Cartella Download"
            Dim p As Point = ModeLbl.Location
            p.Y += 5
            ModeLbl.Location = p
            ExtensionInfo.Text = "Tipi di file supportati: html, xlsx, docx, zip (degli altri file)"
            ModeSelect.Items.Clear()
            ModeSelect.Items.Add("File")
            ModeSelect.Items.Add("Elenco di URL")
            ModeSelect.Items.Add("Cartella")
        End If

        ModeSelect.SelectedIndex = 0
    End Sub

    Private Sub ModeSelect_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ModeSelect.SelectedIndexChanged

        Dim Index As Integer = ModeSelect.SelectedIndex
        If Index = 2 Then
            Index = 0 'Keep the same setup as the file if the user picked the "Folder" option
            If IsItalian Then
                MessageBox.Show("Assicurati che la cartella e le sue sottocartelle contengano solo i tipi di file supportati (xlsx/docx/html/zip)")
            Else
                MessageBox.Show("Please make sure the folder and its subfolders only contain the supported filetypes (xlsx/docx/html/zip)")
            End If
        End If

        'I just didn't want to do a pointless conversion to boolean when I can just do it implicitly, sue me
        FilePath.Visible = Math.Abs(Index - 1)
        Browse.Visible = Math.Abs(Index - 1)
        ExtensionInfo.Visible = Math.Abs(Index - 1)
        URLlist.Visible = Index
        Me.Height = 135 + 320 * Index
        Dim p As Point = DLButton.Location
        p.Y = 67 + 322 * Index
        DLButton.Location = p

        p = CheckSegmented.Location
        p.Y = 64 + 318 * Index
        p.X = 361 * Math.Abs(Index - 1) + 16 * Index
        CheckSegmented.Location = p
    End Sub

    Private Sub DLButton_Click(sender As Object, e As EventArgs) Handles DLButton.Click

        InputForm.Location = Me.Location

        If FolderPath.Text = "" Then
            If IsItalian Then
                MessageBox.Show("Per favore inserisci la cartella di download")
            Else
                MessageBox.Show("Please input the download folder.")
            End If

            Return
        End If

        Dim URLs As New List(Of String)
        If ModeSelect.SelectedIndex = 1 Then
            URLs = URLlist.Lines.ToList
        Else

            If FilePath.Text = "" And ModeSelect.SelectedIndex = 0 Then
                If IsItalian Then
                    MessageBox.Show("Per favore seleziona un file.")
                Else
                    MessageBox.Show("Please input the file's location.")
                End If

                Return
            ElseIf FilePath.Text = "" And ModeSelect.SelectedIndex = 2 Then
                If IsItalian Then
                    MessageBox.Show("Per favore inserisci la cartella.")
                Else
                    MessageBox.Show("Please input the folder's location.")
                End If

                Return
            End If

            'File/Folder mode. Check the extension, and continue from there.
            If ModeSelect.SelectedIndex = 2 Then
                ZipFile.CreateFromDirectory(FilePath.Text, FilePath.Text.Substring(FilePath.Text.LastIndexOf("\") + 1) & ".zip")
                FilePath.Text = FilePath.Text & ".zip"
            End If
            'Add support for folders with files by just making them into a zip archive. 
            'That is peak laziness, I know, but I just want to be done with this goddamn thing. 

            Dim extension As String = FilePath.Text.Substring(FilePath.Text.LastIndexOf(".") + 1).ToLower.Trim

            Select Case extension
                Case "html", "htm"
                    GetAllRecordingLinks(File.ReadAllText(FilePath.Text), URLs)

                Case "xlsx", "docx", "zip"
                    'We're going to treat them as zip archives, and just read the xml files directly. It's simpler that way.
                    Dim XFile As ZipArchive

                    Try
                        XFile = ZipFile.OpenRead(FilePath.Text)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                        Return
                    End Try

                    GetAllLinksFromZip(XFile, URLs)
                    XFile.Dispose()

                Case Else
                    If IsItalian Then
                        MessageBox.Show("Tipo di file non supportato. Riprova.")
                    Else
                        MessageBox.Show("Unsupported file type. Please retry.")
                    End If

                    Return
            End Select
        End If

        If ModeSelect.SelectedIndex = 2 Then
            File.Delete(FilePath.Text)
        End If

        If URLs.Count = 0 Then
            If IsItalian Then
                MessageBox.Show("Nessun URL trovato.")
            Else
                MessageBox.Show("No URLs found.")
            End If

            Return
        End If



        'poliwebex.exe -v [URL ARRAY] -o [OUTPUT DIR] -s
        'We're calling it with the -v [URL ARRAY] option, so let's build the string.

        'Check if config.json exists. If it does, get the email and ID from it, as well as if the password is saved or not.


        Dim arguments As String = "-t -i 3 -o """ & FolderPath.Text & """"
        If File.Exists(StartupForm.RootFolder & "\PoliWebex-pkg\dist\config.json") Then
            Dim AllText As String = File.ReadAllText(StartupForm.RootFolder & "\PoliWebex-pkg\dist\config.json")
            If IsItalian Then
                If Not AllText.Contains("codicePersona") Then arguments &= " -u " & InputForm.AskForInput("Inserisci il tuo codice persona")
                If Not AllText.Contains("email") Then arguments &= " -e " & InputForm.AskForInput("Inserisci la tua email (nome.cognome@mail.polimi.it)")
            Else
                If Not AllText.Contains("codicePersona") Then arguments &= " -u " & InputForm.AskForInput("Please input your person code")
                If Not AllText.Contains("email") Then arguments &= " -e " & InputForm.AskForInput("Please input your email (name.surname@mail.polimi.it)")
            End If


            If Not AllText.Contains("passwordSaved") OrElse Not (AllText.IndexOf("true", AllText.IndexOf("passwordSaved")) = AllText.IndexOf("passwordSaved") + "passwordSaved"": ".Length) Then
                'Does the passwordsaved value exist?
                'Is the true right after the passwordSaved keyword?
                '(Checking the position in this way also checks wheter or not it's set to true.
                If IsItalian Then
                    arguments &= " -p " & InputForm.AskForInput("Inserisci la tua password")
                Else
                    arguments &= " -p " & InputForm.AskForInput("Please input your password")
                End If
            End If

        Else    'Nothing is saved, ask everything
            If IsItalian Then
                arguments &= " -u " & InputForm.AskForInput("Inserisci il tuo codice persona") &
                " -e " & InputForm.AskForInput("Inserisci la tua email (name.surname@mail.polimi.it)") &
                " -p " & InputForm.AskForInput("Inserisci la tua password")
            Else
                arguments &= " -u " & InputForm.AskForInput("Please input your person code") &
                " -e " & InputForm.AskForInput("Please input your email (name.surname@mail.polimi.it)") &
                " -p " & InputForm.AskForInput("Please input your password")
            End If

        End If

        arguments &= " -v"
        For Each URL As String In URLs
            arguments &= " """ & URL & """"
        Next

        'Time to boot up poliwebex.
        'Need to implement the italian translation for all this UI shit.

        If Not CheckSegmented.Checked Then arguments &= " -s"


        ProgressTracker.OverallProgress.Value = 0
        ProgressTracker.FileNum.Text = "File 0/" & URLs.Count
        currentfile = 0
        currentfiletotal = URLs.Count
        If IsItalian Then
            CurrentSpeed = "Sto avviando..."
        Else
            CurrentSpeed = "Setting up..."
        End If

        File.Delete(StartupForm.RootFolder & "\WBDLlogs.txt")
        RunCommandH(StartupForm.RootFolder & "\PoliWebex-pkg\dist\poliwebex.exe", arguments)
        ProgressTracker.ShowDialog()

    End Sub

    Sub GetAllLinksFromZip(AFile As ZipArchive, ByRef URLs As List(Of String))

        For Each Entry In AFile.Entries
            Dim FileName As String = Entry.Name
            Dim FileExtension As String = FileName.Substring(FileName.LastIndexOf("."))

            Do While File.Exists(FileName)
                FileName = FileName.Replace(FileExtension, "_1" & FileExtension)
            Loop

            Entry.ExtractToFile(FileName, True)

            If FileExtension = ".xlsx" Or FileExtension = ".docx" Or FileExtension = ".zip" Then
                Dim XFile As ZipArchive
                Try
                    XFile = ZipFile.OpenRead(FileName)
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                    Return
                End Try

                GetAllLinksFromZip(XFile, URLs)

                XFile.Dispose()
            Else
                GetAllRecordingLinks(File.ReadAllText(Entry.Name), URLs)
            End If

            File.Delete(FileName)
        Next
    End Sub

    Sub GetAllRecordingLinks(AllText As String, ByRef URLs As List(Of String)) 'This just takes a big ol string (file) as input and a list, and adds all links to the list.
        Dim i As Integer = AllText.IndexOf("politecnicomilano.webex.com/recordingservice/")
        'It may seem like a waste of resources to just check every time, but we can't be sure if the links are hyperlinks or not, so we'll just grab everything and see
        Do Until i = -1
            'We're going to use regex to check for the index of the first non-alphanumerical after the /playback/ in the link
            'This (SHOULD) let us handle most if not all cases? Since I'm assuming there'll at least be a space or something.

            Dim r As New Regex("([^a-zA-Z0-9]+)|$")
            Dim NewURL As String = ""
            If AllText.IndexOf("/playback/", i) = -1 Then
                NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("/play/", i) + "/play/".Length).Index - i).Trim
            Else
                NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("/playback/", i) + "/playback/".Length).Index - i).Trim
            End If

            NewURL = "https://" & NewURL

            If Not URLs.Contains(NewURL) Then URLs.Add(NewURL)

            'CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
            i = AllText.IndexOf("politecnicomilano.webex.com/recordingservice/", i + 1)
        Loop

        'Second loop, for the RCID type links.
        i = AllText.IndexOf("politecnicomilano.webex.com/politecnicomilano/")

        Do Until i = -1
            Dim r As New Regex("([^a-zA-Z0-9]+)|$")
            Dim NewURL As String = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("RCID=", i) + "RCID=".Length).Index - i).Trim
            If Not URLs.Contains(NewURL) Then URLs.Add(NewURL)

            'CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
            i = AllText.IndexOf("politecnicomilano.webex.com/politecnicomilano/", i + 1)
        Loop
    End Sub



    Sub RunCommandH(Command As String, Arguments As String)
        'Console.WriteLine(Command)
        'Console.ReadLine()

        Dim oProcess As New Process()
        Dim Debug As Boolean = False
        Dim oStartInfo As ProcessStartInfo


        If Debug Then
            oStartInfo = New ProcessStartInfo(Command, Arguments) With {
            .RedirectStandardOutput = False,
            .RedirectStandardError = False,
            .RedirectStandardInput = False,
            .UseShellExecute = True,
            .WindowStyle = ProcessWindowStyle.Normal,
            .CreateNoWindow = False,
            .WorkingDirectory = StartupForm.RootFolder & "\PoliWebex-pkg\dist\"
            }
        Else
            oStartInfo = New ProcessStartInfo(Command, Arguments) With {
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .RedirectStandardInput = True,
            .UseShellExecute = False,
            .WindowStyle = ProcessWindowStyle.Normal,
            .CreateNoWindow = True,
            .WorkingDirectory = StartupForm.RootFolder & "\PoliWebex-pkg\dist\"
            }
        End If
        oProcess.EnableRaisingEvents = True
        oProcess.StartInfo = oStartInfo

        AddHandler oProcess.OutputDataReceived, AddressOf OutputHandler
        AddHandler oProcess.Exited, AddressOf HandleEnd
        AddHandler oProcess.ErrorDataReceived, AddressOf OutputHandler

        Try
            oProcess.Start()
            If Not Debug Then oProcess.BeginOutputReadLine()
            If Not Debug Then oProcess.BeginErrorReadLine()
        Catch ex As Exception
            File.WriteAllText(StartupForm.RootFolder & "\crashreport.txt", ex.ToString)
            If IsItalian Then
                MessageBox.Show("Errore nell'avvio del processo. Informazioni sull'errore salvate in " & StartupForm.RootFolder & "\crashreport.txt")
            Else
                MessageBox.Show("Error starting the process. Exception info saved in crashreport.txt")
            End If

        End Try

        GlobalProcess = oProcess
    End Sub



    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)

        Dim process As Process = sendingProcess

        Dim segmented As Boolean = process.StartInfo.Arguments.Contains(" -s")

        'IMPORTANT: Handle closing the program properly if the form closes (kill poliwebex)
        'Add a warning using form.close


        'Need to properly implement the "Done!" prompt etc. in process.exited

        If Not String.IsNullOrEmpty(outLine.Data) Then
            If outLine.Data.Contains("Bad credentials.") Then
                File.Delete(StartupForm.RootFolder & "\PoliWebex-pkg\dist\config.json")
                If IsItalian Then
                    MessageBox.Show("Credenziali errate. Riprova, ti verrà chiesto di reinserirle.")
                Else
                    MessageBox.Show("Bad credentials. Please try again, you will be prompted to input them.")
                End If
                If IsItalian Then
                    CurrentSpeed =
                        
                    "Finito."
                Else
                    CurrentSpeed = "Finished."
                End If

                'I'm just going to completely erase the config.json file and kick the user back to the form.
            End If
            If outLine.Data.Contains("non-headless mode") Then
                'This is just for testing purposes
                'MessageBox.Show("The bug happened. Running in non-headless mode now.")
            End If

            If outLine.Data.Contains("Start downloading video") Then
                If DLError Then
                    DLError = False
                Else
                    currentfile += 1
                End If
            End If
            'JANK IT UP


            If outLine.Data.Contains("Downloading") And outLine.Data.Contains("item(s)") And segmented Then
                currentsegmenttotal = outLine.Data.Substring(
                outLine.Data.IndexOf("Downloading") + "Downloading".Length,
                outLine.Data.IndexOf("item(s)") - (outLine.Data.IndexOf("Downloading") + "Downloading".Length)).Trim()
            End If

            If outLine.Data.Contains("0B CN") And Not segmented Then
                'Means it's an update. We can get the speed from here.
                CurrentSpeed = outLine.Data.Substring(outLine.Data.IndexOf("DL:") + "DL:".Length, outLine.Data.Length - 1 - (outLine.Data.IndexOf("DL:") + "DL:".Length)) & "/s"
            End If

            If outLine.Data.Contains("[DL:") And segmented Then
                CurrentSpeed = outLine.Data.Substring("[DL:".Length, outLine.Data.IndexOf("]") - "[DL:".Length) & "/s"
                If IsItalian Then
                    If CurrentSpeed = "0B/s" Then CurrentSpeed = "Sto leggendo dal disco..."
                Else
                    If CurrentSpeed = "0B/s" Then CurrentSpeed = "Reading from disk..."
                End If

            End If

            If outLine.Data.Contains("Download complete:") Then
                If segmented Then
                    'MessageBox.Show(1 & "/" & currentfiletotal & "/" & currentsegmenttotal & "=" & (currentfile / currentfiletotal) / currentsegmenttotal * 100)
                    currentprogress += (1 / currentfiletotal) / currentsegmenttotal * 100
                Else
                    'MessageBox.Show(ProgressTracker.OverallProgress.Value)
                    'MessageBox.Show(currentfile & "-" & currentfiletotal)
                    currentprogress += 1 / currentfiletotal * 100
                End If
            End If

            If outLine.Data.Contains("Download has already completed:") And segmented Then
                'Hey, as long as it works.
                currentprogress -= (1 / currentfiletotal) / currentsegmenttotal * 100
            End If
            'MessageBox.Show(outLine.Data)

            If outLine.Data.Contains("Done!") Then
                If IsItalian Then
                    CurrentSpeed = "Finito."
                Else
                    CurrentSpeed = "Finished."
                End If

                If Not DLError Then currentfile += 1
                If currentfile < currentfiletotal Then
                    If IsItalian Then
                        MessageBox.Show("È fallito il download di " & currentfiletotal - currentfile & " video. Riprova più tardi.")
                    Else
                        MessageBox.Show("Could not download " & currentfiletotal - currentfile & " videos. Please try again later.")
                    End If

                Else
                    If IsItalian Then
                        MessageBox.Show("Finito!")
                    Else
                        MessageBox.Show("All done!")
                    End If

                End If
            End If

            If outLine.Data.Contains("Persistent errors during the download") Then
                DLError = True
            End If

            'Add stuff for the password protected videos

            If outLine.Data.Contains("This video is password protected") Or outLine.Data.Contains("Wrong password!") Then
                If outLine.Data.Contains("Wrong password!") Then MessageBox.Show("Previous password was incorrect. Please try again.")
                Dim Password As String
                If IsItalian Then
                    Password = InputForm.AskForInput("Inserisci la password per questo video: " & vbCrLf & outLine.Data.Substring(outLine.Data.LastIndexOf("/") + 1))
                Else
                    Password = InputForm.AskForInput("Please input the password for this video: " & vbCrLf & outLine.Data.Substring(outLine.Data.LastIndexOf("/") + 1))
                End If
                process.StandardInput.WriteLine(Password)
                'MessageBox.Show("Input Sent!")
            End If


            If outLine.Data.Contains("ffmpeg version") And CurrentSpeed <> "Setting up..." And CurrentSpeed <> "Sto avviando..." Then
                If IsItalian Then
                    CurrentSpeed = "Sto elaborando il file..."
                Else
                    CurrentSpeed = "Processing file..."
                End If

            End If

            File.AppendAllText(StartupForm.RootFolder & "\WBDLlogs.txt", outLine.Data & vbCrLf)


            End If
    End Sub

    Sub HandleEnd(Sender As Object, e As EventArgs)
        Dim process As Process = Sender
        'MessageBox.Show(process.ExitCode)
        If process.ExitCode = 3 Then
            RunCommandH(process.StartInfo.FileName, process.StartInfo.Arguments & " -l false -i 10")
            'We're gonna try in non-headless mode, with the max timeout.
        ElseIf process.ExitCode = 4 Then
            MessageBox.Show("Something went wrong! Please file a github issue, and attach the WBDLlogs.txt file you can find in " & StartupForm.RootFolder)
        End If

        process.Close()

        process.Dispose()
    End Sub

    Private Sub CheckSegmented_CheckedChanged(sender As Object, e As EventArgs) Handles CheckSegmented.CheckedChanged

        If CheckSegmented.Checked = True Then
            Dim ans As Integer
            If IsItalian Then
                ans = MsgBox("Sei sicuro? Questo renderà il download più lento e la barra di download meno precisa." &
       " È consigliato solo se stai avendo problemi.", MsgBoxStyle.YesNo, "Download non segmentato?")
            Else
                ans = MsgBox("Are you sure? This will make the download slower and the progress bar less accurate." &
       " It's only recommended if you're experiencing issues.", MsgBoxStyle.YesNo, "Unsegmented download?")
            End If


            If ans <> DialogResult.Yes Then
                CheckSegmented.Checked = False
            End If
        End If

    End Sub
End Class