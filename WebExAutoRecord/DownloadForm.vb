Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions
Imports Microsoft.WindowsAPICodePack.Dialogs
Imports PoliwebexGUI.StartupForm

Public Class DownloadForm

    Public Shared currentsegmenttotal As Integer
    Public Shared currentfiletotalS As Integer
    Public Shared currentfiletotal As Integer
    Public Shared StreamIsVideo As Boolean
    Public Shared currentfile As Integer
    Public Shared currentprogress As Double = 0
    Public Shared CurrentSpeed As String = ""
    Public Shared GlobalProcess As Process
    Public Shared DLError As Boolean
    Public Shared WebexProgress As Double = 0
    Public Shared NotDownloaded As Integer = -1
    Public Shared StreamArgs As String  'Why? Because I need to access it from outside where it was declared. Sue me. I'm tired of working on this godforsaken program.
    Private Shared Debug As Boolean = True
    Private Sub Browse_Click(sender As Object, e As EventArgs) Handles Browse.Click

        Dim COPF As New CommonOpenFileDialog
        COPF.InitialDirectory = "C:\\Users"
        If ModeSelect.SelectedIndex = 0 Then
            COPF.IsFolderPicker = False
            COPF.EnsureFileExists = True
            If IsItalian Then
                COPF.Filters.Add(New CommonFileDialogFilter("File HTML", "html,htm"))
                COPF.Filters.Add(New CommonFileDialogFilter("File Excel", "xlsx"))
                COPF.Filters.Add(New CommonFileDialogFilter("File Word", "docx"))
                COPF.Filters.Add(New CommonFileDialogFilter("Zip (di file docx/xlsx/html)", "zip"))
            Else
                COPF.Filters.Add(New CommonFileDialogFilter("HTML file", "html,htm"))
                COPF.Filters.Add(New CommonFileDialogFilter("Excel file", "xlsx"))
                COPF.Filters.Add(New CommonFileDialogFilter("Word file", "docx"))
                COPF.Filters.Add(New CommonFileDialogFilter("Zip (of docx/xlsx/html files)", "zip"))
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
            ModeSelect.Items.Add("Testo")
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

        Dim WebexURLs, StreamURLs As New List(Of String)
        If ModeSelect.SelectedIndex = 1 Then
            GetAllRecordingLinks(URLlist.Text, WebexURLs, StreamURLs)
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
                    GetAllRecordingLinks(File.ReadAllText(FilePath.Text), WebexURLs, StreamURLs)

                Case "xlsx", "docx", "zip"
                    'We're going to treat them as zip archives, and just read the xml files directly. It's simpler that way.
                    Dim XFile As ZipArchive

                    Try
                        XFile = ZipFile.OpenRead(FilePath.Text)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                        Return
                    End Try

                    GetAllLinksFromZip(XFile, WebexURLs, StreamURLs)
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

        If WebexURLs.Count = 0 And StreamURLs.Count = 0 Then
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


        Dim WebexArgs As String = "-t -i 3 -o """ & FolderPath.Text & """"
        StreamArgs = "-t -q 5 -i 3 -o """ & FolderPath.Text & """"
        Dim TempString As String

        If File.Exists(StartupForm.RootFolder & "\Poli-pkg\dist\config.json") Then
            Dim Config As String = File.ReadAllText(StartupForm.RootFolder & "\Poli-pkg\dist\config.json")

            If Not Config.Contains("codicePersona") Then
                If IsItalian Then
                    TempString = InputForm.AskForInput("Inserisci il tuo codice persona")
                Else
                    TempString = InputForm.AskForInput("Please input your person code")
                End If
                WebexArgs &= " -u " & TempString
                StreamArgs &= " -u " & TempString
            End If

            If Not Config.Contains("email") Then
                If IsItalian Then
                    WebexArgs &= " -e " & InputForm.AskForInput("Inserisci la tua email (nome.cognome@mail.polimi.it)")
                Else
                    WebexArgs &= " -e " & InputForm.AskForInput("Please input your email (name.surname@mail.polimi.it)")
                End If
            End If


            If Not Config.Contains("passwordSaved") OrElse
            Not (Config.IndexOf("true", Config.IndexOf("passwordSaved")) = Config.IndexOf("passwordSaved") + "passwordSaved"": ".Length) Then
                'Does the passwordsaved value exist?
                'Is the true right after the passwordSaved keyword?
                'Checking the position in this way also checks wheter or not it's set to true.
                If IsItalian Then
                    TempString = InputForm.AskForInput("Inserisci la tua password")
                Else
                    TempString = InputForm.AskForInput("Please input your password")
                End If
                WebexArgs &= " -p " & TempString
                StreamArgs &= " -p " & TempString
            End If

        Else    'Nothing is saved, ask everything to make sure.
            If IsItalian Then
                TempString = InputForm.AskForInput("Inserisci il tuo codice persona")
            Else
                TempString = InputForm.AskForInput("Please input your person code")
            End If
            WebexArgs &= " -u " & TempString
            StreamArgs &= " -u " & TempString

            If IsItalian Then
                TempString = InputForm.AskForInput("Inserisci la tua password")
            Else
                TempString = InputForm.AskForInput("Please input your password")
            End If
            WebexArgs &= " -p " & TempString
            StreamArgs &= " -p " & TempString

            If IsItalian Then
                TempString = InputForm.AskForInput("Inserisci la tua email (nome.cognome@mail.polimi.it)")
            Else
                TempString = InputForm.AskForInput("Please input your email (name.surname@mail.polimi.it)")
            End If
            WebexArgs &= " -e " & TempString
        End If

        WebexArgs &= " -v"
        StreamArgs &= " -v"
        For Each URL As String In WebexURLs
            WebexArgs &= " """ & URL & """"
        Next

        For Each URL As String In StreamURLs
            StreamArgs &= " """ & URL & """"
        Next

        'Time to boot up poliwebex.

        If Not CheckSegmented.Checked Then WebexArgs &= " -s"

        currentfile = 0
        currentfiletotalS = StreamURLs.Count
        currentfiletotal = WebexURLs.Count + StreamURLs.Count
        ProgressTracker.OverallProgress.Value = 0
        ProgressTracker.FileNum.Text = "File 0/" & currentfiletotal

        If IsItalian Then
            CurrentSpeed = "Sto avviando..."
        Else
            CurrentSpeed = "Setting up..."
        End If

        File.Delete(Environment.CurrentDirectory & "\WBDLlogs.txt")
        StreamIsVideo = False
        WebexProgress = 0
        If WebexURLs.Count <> 0 Then
            RunCommandH(StartupForm.RootFolder & "\Poli-pkg\dist\poliwebex.exe", WebexArgs)
        Else
            RunCommandH(StartupForm.RootFolder & "\Poli-pkg\dist\polidown.exe", StreamArgs)
        End If
        ProgressTracker.ShowDialog()

    End Sub

    Sub GetAllLinksFromZip(AFile As ZipArchive, ByRef WebexURLs As List(Of String), ByRef StreamURLs As List(Of String))

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

                GetAllLinksFromZip(XFile, WebexURLs, StreamURLs)

                XFile.Dispose()
            Else
                GetAllRecordingLinks(File.ReadAllText(Entry.Name), WebexURLs, StreamURLs)
            End If

            File.Delete(FileName)
        Next
    End Sub

    Sub GetAllRecordingLinks(AllText As String, ByRef WebexURLs As List(Of String), ByRef StreamURLs As List(Of String)) 'This just takes a big ol string (file) as input and a list, and adds all links to the list.
        Dim i As Integer = AllText.IndexOf("politecnicomilano.webex.com/")
        Do Until i = -1
            Dim r As New Regex("([^a-zA-Z0-9\/.?=:]+)|$|\n")
            Dim NewURL As String = ""
            NewURL = AllText.Substring(i, r.Match(AllText, i).Index - i).Trim
            NewURL = "https://" & NewURL
            If Not WebexURLs.Contains(NewURL) Then WebexURLs.Add(NewURL)

            'CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
            i = AllText.IndexOf("politecnicomilano.webex.com/", i + 1)
        Loop

        'This was it should just keep working no matter the link format.
        'Why did I do this you ask? Because I've stumbled across a fourth goddamn URL format, and with the way I've been doing I would've had to add support for each fucking URL scheme
        'And I've just about had it with this thing

        'Also if you're actually reading these comments god bless your soul and I apologize for the profanity (not really, bugger off)
        'I've also been experiencing a bug which seems to be related to the virtual desktop program I'm using so whatever
        'I'm keeping the following parts (even if they're theoretically not necessary) JUST IN CASE SOMEONE IS BRIGHT ENOUGH TO FOLLOW A LINK UP WITH ONE OF THE ADDITIONAL SYMBOLS I EXCLUDED.
        'JUST IN CASE. Nothing could surprise me at this point. I saw a link that had https spelt wrong, which is why I'm no longer looking for "https://politecnico." 


        i = AllText.IndexOf("politecnicomilano.webex.com/recordingservice/")
        'It may seem like a waste of resources to just check every time, but we can't be sure if the links are hyperlinks or not, so we'll just grab everything and see
        Do Until i = -1
            'We're going to use regex to check for the index of the first non-alphanumerical after the /playback/ in the link
            'This (SHOULD) let us handle most if not all cases? Since I'm assuming there'll at least be a space or something.

            Dim r As New Regex("([^a-zA-Z0-9]+)|$")
            Dim NewURL As String = ""
            If AllText.IndexOf("/playback/", i) = -1 Or (AllText.IndexOf("/play/", i) < AllText.IndexOf("/playback/", i)) Then
                NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("/play/", i) + "/play/".Length).Index - i).Trim
            Else
                NewURL = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("/playback/", i) + "/playback/".Length).Index - i).Trim
            End If

            NewURL = "https://" & NewURL

            If Not WebexURLs.Contains(NewURL) Then WebexURLs.Add(NewURL)

            'CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
            i = AllText.IndexOf("politecnicomilano.webex.com/recordingservice/", i + 1)
        Loop

        'Second loop, for the RCID type links.
        i = AllText.IndexOf("politecnicomilano.webex.com/politecnicomilano/")

        Do Until i = -1
            Dim r As New Regex("([^a-zA-Z0-9]+)|$")
            Dim NewURL As String = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("RCID=", i) + "RCID=".Length).Index - i).Trim

            NewURL = "https://" & NewURL
            If Not WebexURLs.Contains(NewURL) Then WebexURLs.Add(NewURL)

            'CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
            i = AllText.IndexOf("politecnicomilano.webex.com/politecnicomilano/", i + 1)
        Loop


        'Another loop, this time for msstream links
        i = AllText.IndexOf("web.microsoftstream.com")

        Do Until i = -1
            Dim r As New Regex("([^a-zA-Z0-9-]+)|$")    'This one excludes the - character from the match
            Dim NewURL As String = AllText.Substring(i, r.Match(AllText, AllText.IndexOf("/video/", i) + "/video/".Length).Index - i).Trim

            NewURL = "https://" & NewURL
            If Not StreamURLs.Contains(NewURL) Then StreamURLs.Add(NewURL)

            'CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
            i = AllText.IndexOf("web.microsoftstream.com", i + 1)
        Loop
    End Sub



    Shared Sub RunCommandH(Command As String, Arguments As String)
        'Console.WriteLine(Command)
        'Console.ReadLine()

        Dim oProcess As New Process()
        Dim NoOutputRedirect As Boolean = False
        Dim oStartInfo As ProcessStartInfo
        DLError = False
        NotDownloaded = -1

        If NoOutputRedirect Then
            oStartInfo = New ProcessStartInfo(Command, Arguments) With {
            .RedirectStandardOutput = False,
            .RedirectStandardError = False,
            .RedirectStandardInput = False,
            .UseShellExecute = True,
            .WindowStyle = ProcessWindowStyle.Normal,
            .CreateNoWindow = False,
            .WorkingDirectory = Command.Substring(0, Command.LastIndexOf("\"))
            }
        Else
            oStartInfo = New ProcessStartInfo(Command, Arguments) With {
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .RedirectStandardInput = True,
            .UseShellExecute = False,
            .WindowStyle = ProcessWindowStyle.Normal,
            .CreateNoWindow = True,
            .WorkingDirectory = Command.Substring(0, Command.LastIndexOf("\", Command.Length - 3))
            }
        End If


        oProcess.EnableRaisingEvents = True
        oProcess.StartInfo = oStartInfo
        currentprogress = WebexProgress

        AddHandler oProcess.OutputDataReceived, AddressOf OutputHandler
        AddHandler oProcess.ErrorDataReceived, AddressOf OutputHandler

        Try
            oProcess.Start()
            If Not NoOutputRedirect Then oProcess.BeginOutputReadLine()
            If Not NoOutputRedirect Then oProcess.BeginErrorReadLine()
        Catch ex As Exception
            File.WriteAllText(StartupForm.RootFolder & "\crashreport.txt", ex.ToString)
            If IsItalian Then
                MessageBox.Show("Errore nell'avvio del processo. Informazioni sull'errore salvate in " & StartupForm.RootFolder & "\crashreport.txt")
            Else
                MessageBox.Show("Error starting the process. Exception info saved in crashreport.txt")
            End If
            If IsItalian Then
                CurrentSpeed = "Finito."
            Else
                CurrentSpeed = "Finished."
            End If
        End Try

        GlobalProcess = oProcess
    End Sub



    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)

        Dim process As Process = sendingProcess

        Dim segmented As Boolean = process.StartInfo.Arguments.Contains(" -s")
        If process.StartInfo.FileName.Contains("polidown.exe") Then segmented = True    'polidown is always in segmented mode


        If Not String.IsNullOrEmpty(outLine.Data) Then
            If outLine.Data.Contains("Bad credentials.") Then   'Output is same on both.
                Try
                    File.Delete(StartupForm.RootFolder & "\Poli-pkg\dist\config.json")
                    If IsItalian Then
                        MessageBox.Show("Credenziali errate. Riprova, ti verrà chiesto di reinserirle.")
                    Else
                        MessageBox.Show("Bad credentials. Please try again, you will be prompted to input them. If that didn't happen, please delete the file at %APPDATA%\WebExRec\Poli-pkg\dist\config.json")
                    End If
                Catch ex As Exception
                    If IsItalian Then
                        MessageBox.Show("Credenziali errate. Non è stato possibile cancellare il file %APPDATA%\WebExRec\Poli-pkg\dist\config.json, per favore fallo manualmente")
                    Else
                        MessageBox.Show("Bad credentials. We could not delete the file at %APPDATA%\WebExRec\Poli-pkg\dist\config.json, please do so manually")
                    End If
                End Try

                'Might as well stay on the safe side - if one of them is outdated, it's likely the other one is as well.

                If IsItalian Then
                    CurrentSpeed = "Finito."
                Else
                    CurrentSpeed = "Finished."
                End If

                'I'm just going to completely erase the config.json file and kick the user back to the form.
            End If


            If outLine.Data.Contains("Start downloading video") Then    'Output is same on both
                If DLError Then
                    DLError = False
                Else
                    currentprogress = currentfile / currentfiletotal * 100  'Let's ensure we're at the correct progress.
                    currentfile += 1
                End If
            End If

            'JANK IT UP


            If outLine.Data.Contains("Downloading") And outLine.Data.Contains("item(s)") And segmented Then 'aria2c output - differs slightly with polidown.
                Dim Temp As Integer = outLine.Data.Substring(
                outLine.Data.IndexOf("Downloading") + "Downloading".Length,
                outLine.Data.IndexOf("item(s)") - (outLine.Data.IndexOf("Downloading") + "Downloading".Length)).Trim()
                If process.StartInfo.FileName.Contains("polidown.exe") Then
                    StreamIsVideo = Not (StreamIsVideo)
                    If StreamIsVideo Then
                        currentsegmenttotal = Temp * 2 + 10 'polidown has to download audio and video separately. 
                    End If 'So I multiply it by two and add 10 for safety in case there's a mismatch between the two
                Else
                    currentsegmenttotal = Temp
                End If

            End If

            If outLine.Data.Contains("0B CN") And Not segmented Then    'aria2c output
                'Means it's an update. We can get the speed from here.
                CurrentSpeed = outLine.Data.Substring(outLine.Data.IndexOf("DL:") + "DL:".Length, outLine.Data.Length - 1 - (outLine.Data.IndexOf("DL:") + "DL:".Length)) & "/s"
            End If

            If outLine.Data.Contains("[DL:") And segmented Then     'aria2c output
                CurrentSpeed = outLine.Data.Substring("[DL:".Length, outLine.Data.IndexOf("]") - "[DL:".Length) & "/s"
                If IsItalian Then
                    If CurrentSpeed = "0B/s" Then CurrentSpeed = "Sto leggendo dal disco..."
                Else
                    If CurrentSpeed = "0B/s" Then CurrentSpeed = "Reading from disk..."
                End If

            End If

            If outLine.Data.Contains("Download complete:") Then 'aria2c output
                If segmented Then
                    'MessageBox.Show(1 & "/" & currentfiletotal & "/" & currentsegmenttotal & "=" & (currentfile / currentfiletotal) / currentsegmenttotal * 100)
                    currentprogress += (1 / currentfiletotal) / currentsegmenttotal * 100
                Else
                    'MessageBox.Show(ProgressTracker.OverallProgress.Value)
                    'MessageBox.Show(currentfile & "-" & currentfiletotal)
                    currentprogress += 1 / currentfiletotal * 100
                End If
            End If

            If outLine.Data.Contains("Download has already completed:") And segmented Then   'aria2c output
                'Hey, as long as it works.
                currentprogress -= (1 / currentfiletotal) / currentsegmenttotal * 100
            End If

            'MessageBox.Show(outLine.Data)

            If outLine.Data.Contains("These videos have not been downloaded:") Then
                NotDownloaded = 0
                If outLine.Data.Contains("https://") Then
                    NotDownloaded += 1
                End If
            End If

            If outLine.Data.Contains("https://") And NotDownloaded <> -1 Then    '
                Dim tempi As Integer = -1
                Do
                    tempi = outLine.Data.IndexOf("https://", tempi + 1)
                    NotDownloaded += 1
                Loop Until tempi = -1
                NotDownloaded -= 1  'THe above loop always counts one extra and I'm too lazy to figure out a good alternative method.
            End If

            If outLine.Data.Contains("Done!") Then  'Shared output. 
                If DLError Then currentfile -= 1

                currentprogress = currentfile / currentfiletotal * 100  'Let's ensure we're at the correct progress.
                If process.StartInfo.FileName.Contains("polidown.exe") Or currentfiletotalS = 0 Then
                    'Either we've finished polidown, or there's no msstream links to download.
                    If IsItalian Then
                        CurrentSpeed = "Finito."
                    Else
                        CurrentSpeed = "Finished."
                    End If

                    If NotDownloaded <> -1 Then
                        If segmented Then
                            If IsItalian Then
                                MessageBox.Show("È fallito il download di " & NotDownloaded & " video. Riprova più tardi, oppure prova in modalità unsegmented.")
                            Else
                                MessageBox.Show("Could not download " & NotDownloaded & " videos. Please try again later, or try unsegmented mode.")
                            End If
                        Else
                            If IsItalian Then
                                MessageBox.Show("È fallito il download di " & NotDownloaded & " video. Riprova più tardi.")
                            Else
                                MessageBox.Show("Could not download " & NotDownloaded & " videos. Please try again later.")
                            End If
                        End If

                    Else
                        If IsItalian Then
                            MessageBox.Show("Finito!")
                        Else
                            MessageBox.Show("All done!")
                        End If

                    End If

                Else
                    WebexProgress = currentprogress
                    If NotDownloaded <> -1 Then
                        If segmented Then
                            If IsItalian Then
                                MessageBox.Show("È fallito il download di " & NotDownloaded & " video da Webex. Riprova più tardi in modalità non-segmented. Ora scarichiamo i video da microsoft stream.")
                            Else
                                MessageBox.Show("Could not download " & NotDownloaded & " videos from Webex. Try again later in unsegmented mode. We will now download the microsoft stream videos.")
                            End If
                        Else
                            If IsItalian Then
                                MessageBox.Show("È fallito il download di " & NotDownloaded & " video da Webex. Riprova più tardi. Ora scarichiamo i video da microsoft stream.")
                            Else
                                MessageBox.Show("Could not download " & NotDownloaded & " videos from Webex. Please try again later. We will now download the microsoft stream videos.")
                            End If
                        End If

                    Else
                        If IsItalian Then
                            MessageBox.Show("Abbiamo scaricato tutti i video da Webex. Ora scarichiamo quelli da microsoft stream.")
                        Else
                            MessageBox.Show("We downloaded all videos from Webex. We will now download the ones from microsoft stream.")
                        End If
                    End If
                    'We have some polidown links to download.
                    RunCommandH(StartupForm.RootFolder & "\Poli-pkg\dist\polidown.exe", StreamArgs)
                End If





            End If

            If outLine.Data.Contains("Going to the next one") Then  'Shared output
                DLError = True
            End If


            If outLine.Data.Contains("This video is password protected") Or outLine.Data.Contains("Wrong password!") Then   'Never occurs for polidown
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


            If outLine.Data.Contains("ffmpeg version") And CurrentSpeed <> "Setting up..." And CurrentSpeed <> "Sto avviando..." Then   'ffmpeg output
                If IsItalian Then
                    CurrentSpeed = "Sto elaborando il file..."
                Else
                    CurrentSpeed = "Processing file..."
                End If

            End If

            If outLine.Data.Contains("Try in non-headless mode") Or outLine.Data.Contains("this is not an exact science") Then   'shared output
                If process.StartInfo.FileName.Contains("poliwebex.exe") Then WebexProgress = 0
                DownloadForm.RunCommandH(process.StartInfo.FileName, process.StartInfo.Arguments.Replace("-i 3", "-i 10") & " -l false")
                Try
                    process.Close()
                    process.Dispose()
                Catch ex As Exception

                End Try

            End If

            If outLine.Data.Contains("You need aria2c in $PATH for this to work") Then  'Shared output
                'This is a weird bug that just kinda...popped up. I'm not sure if it's an issue with my multiple desktops program, but juuuuuust to be on the safe side
                'If this happens, let's just ask the user to try again.
                'I can't really fix this as it doesn't really make any sense? And I really don't have enough info.
                If IsItalian Then
                    MessageBox.Show("Qualcosa è andato storto. Per favore riprova.")
                Else
                    MessageBox.Show("Something went wrong. Please try again.")
                End If
                If IsItalian Then
                    CurrentSpeed = "Finito."
                Else
                    CurrentSpeed = "Finished."
                End If

            End If

            If outLine.Data.Contains("We're already in non-headless mode") Then 'Shared output
                If IsItalian Then
                    MessageBox.Show("Qualcosa è andato storto! Per favore crea un issue su github, e allega il file WBDLlogs.txt che puoi trovare in " & StartupForm.RootFolder)
                    Application.Exit()
                Else
                    MessageBox.Show("Something went wrong! Please file a github issue, and attach the WBDLlogs.txt file you can find in " & StartupForm.RootFolder)
                    Application.Exit()
                End If
            End If

            File.AppendAllText(Environment.CurrentDirectory & "\WBDLlogs.txt", outLine.Data & vbCrLf)


        End If
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