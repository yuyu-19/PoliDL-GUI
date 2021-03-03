Imports System.IO
Imports System.IO.Compression
Imports System.Threading
Imports Microsoft.Win32.TaskScheduler

Public Class StartupForm

    Public RootFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\WebExRec"

    ReadOnly Courses As New List(Of CourseData)
    Public Shared IsItalian As Boolean = (Thread.CurrentThread.CurrentCulture.IetfLanguageTag = "it-IT")

    Private Sub StartupForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'It was originally built with code, so I'm still going to adjust the size of everything with it, rather than re-doing it in the designer.

        Dim CFont As New Font(Question.Font.FontFamily, 10, Question.Font.Style)
        Question.Font = CFont
        CreditLabel.Font = CFont
        CreditLabel.Text = "PoliWebex and PoliDown by @sup3rgiu " & vbCrLf & "GUI by @yuyu-19"
        CreditLabel.TextAlign = ContentAlignment.MiddleCenter
        localmode.Name = "recbutton"
        downloadmode.Name = "dlbutton"
        localmode.AutoSize = True
        downloadmode.AutoSize = True

        AddHandler localmode.Click, AddressOf LocalMode_Click
        AddHandler downloadmode.Click, AddressOf DownloadMode_Click

        If IsItalian Then
            Question.Text = "Vuoi gestire le registrazioni locali o scaricare delle registrazioni?"
            localmode.Text = "Locale"
        Else
            Question.Text = "Would you like to manage the local recordings or download some?"
            localmode.Text = "Local"
        End If
        downloadmode.Text = "Download"

        Dim size As Size = TextRenderer.MeasureText(Question.Text, CFont)
        Question.Width = size.Width
        Question.Height = size.Height

        Me.Height = Question.Height + localmode.Height + 80
        Me.Width = Question.Width + 80
        Dim p As Point = Question.Location
        p.X = Me.ClientSize.Width / 2 - Question.Width / 2
        p.Y = 5
        Question.Location = p

        p = localmode.Location
        p.Y = Question.Height + 30
        p.X = 10
        localmode.Location = p

        p.X = Me.ClientSize.Width - downloadmode.Width - 10
        downloadmode.Location = p

        downloadmode.Enabled = False
        localmode.Enabled = False
        p.X = Me.ClientSize.Width / 2 - CreditLabel.Width / 2
        p.Y = Me.ClientSize.Height - CreditLabel.Height - 2
        CreditLabel.Location = p
        'Unzip it all to a folder And use that as the root directory for everything else
        Dim appData As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\WebExRec"
        RootFolder = appData

        Dim CurrentFolder As String = Environment.CurrentDirectory

        'I spent hours trying to figure out a better way to do this, but MSBuild was having NONE OF IT.
        'So fuck it.

        If Not File.Exists(CurrentFolder & "\Microsoft.Win32.TaskScheduler.dll") Then
            File.WriteAllBytes(CurrentFolder & "\Microsoft.Win32.TaskScheduler.dll", My.Resources.Microsoft_Win32_TaskScheduler)
        End If

        If Not File.Exists(CurrentFolder & "\Microsoft.WindowsAPICodePack.dll") Then
            File.WriteAllBytes(CurrentFolder & "\Microsoft.WindowsAPICodePack.dll", My.Resources.Microsoft_WindowsAPICodePack)
        End If

        If Not File.Exists(CurrentFolder & "\Microsoft.WindowsAPICodePack.Shell.dll") Then
            File.WriteAllBytes(CurrentFolder & "\Microsoft.WindowsAPICodePack.Shell.dll", My.Resources.Microsoft_WindowsAPICodePack_Shell)
        End If

        If Not File.Exists(CurrentFolder & "\System.IO.Compression.ZipFile.dll") Then
            File.WriteAllBytes(CurrentFolder & "\System.IO.Compression.ZipFile.dll", My.Resources.System_IO_Compression_ZipFile)
        End If

        If Directory.Exists(appData) AndAlso Not File.Exists(appData & "\version.txt") Then
            File.WriteAllText(appData & "\version.txt", "1")  'We don't have a version number, so that means the version unpacked must've been v1
        End If

        'Kill every process that is currently using the folder.
        If Directory.Exists(appData) AndAlso My.Resources.Version <> File.ReadAllText(appData & "\version.txt") Then
            For Each proc In Process.GetProcessesByName("chrome")
                'MessageBox.Show("Process: " & proc.MainModule.FileName)
                If proc.MainModule.FileName.Contains(RootFolder) Then
                    'MessageBox.Show("Killing process: " & proc.ProcessName)
                    proc.Kill()
                    proc.Dispose()
                End If
            Next

            For Each proc In Process.GetProcessesByName("poliwebex")
                'MessageBox.Show("Process: " & proc.MainModule.FileName)
                If proc.MainModule.FileName.Contains(RootFolder) Then
                    'MessageBox.Show("Killing process: " & proc.ProcessName)
                    proc.Kill()
                    proc.Dispose()
                End If
            Next

            For Each proc In Process.GetProcessesByName("polidown")
                'MessageBox.Show("Process: " & proc.MainModule.FileName)
                If proc.MainModule.FileName.Contains(RootFolder) Then
                    'MessageBox.Show("Killing process: " & proc.ProcessName)
                    proc.Kill()
                    proc.Dispose()
                End If
            Next

            Try
                Directory.Delete(appData, True) 'Version mismatch - the internal data is newer than the stored one
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                If IsItalian Then
                    MessageBox.Show("Per favore elimina la cartella in %appdata%\WebExRec manualmente.")
                Else
                    MessageBox.Show("Please manually delete the %appdata%\WebExRec folder.")
                End If
                Application.Exit()
            End Try
        End If

        If Not Directory.Exists(appData) Or File.Exists(appData & "\temp.zip") Or Not File.Exists(appData & "\StartRec.exe") Then
            Dim OldQuestion As String = Question.Text
            If IsItalian Then
                Question.Text = "Setup iniziale, potrebbe richiedere qualche minuto..."
            Else
                Question.Text = "Running first time setup, could take a couple minutes..."
            End If
            Me.Cursor = Cursors.WaitCursor

            If File.Exists(appData & "\temp.zip") Then
                File.Delete(appData & "\temp.zip")   'We don't know if it was done saving it to disk. Play it safe.
                Directory.Delete(appData, True)
            End If

            Try
                Directory.CreateDirectory(appData)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Application.Exit()
            End Try

            Try
                System.IO.File.WriteAllBytes(appData & "\temp.zip", My.Resources.Data)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Application.Exit()
            End Try

            Dim ZFile As ZipArchive = ZipFile.OpenRead(appData & "\temp.zip")
            Try
                ZFile.ExtractToDirectory(appData)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Application.Exit()
            End Try
            ZFile.Dispose()
            Try
                File.Delete(appData & "\temp.zip")
                File.WriteAllText(appData & "\version.txt", My.Resources.Version)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
            Me.Cursor = Cursors.Default
            Question.Text = OldQuestion
        End If

        downloadmode.Enabled = True
        localmode.Enabled = True
    End Sub

    Public Function IsWebEx(ByVal t As Task)
        Return t.Name.Contains("WebExRec-") And Not t.Name.Contains("WebExRec-OS-")
    End Function

    Public Function IsWebExOS(ByVal t As Task)
        Return t.Name.Contains("WebExRec-OS-")
    End Function

    Private Sub LocalMode_Click(sender As Object, e As EventArgs)

        GenerateDataForm.ShowDialog()
    End Sub

    Private Sub DownloadMode_Click(sender As Object, e As EventArgs)

        DownloadForm.Show()
    End Sub

    Public Class CourseData
        Public Professors As New Dictionary(Of String, String)
        Public Name As String
        Public ID As Integer
        Public StartDate As String
        Public EndDate As String
        Public Days As New List(Of DayData)
        Public OneShots As New List(Of DayData)
    End Class

    Public Class DayData
        Public DayName As String
        Public StartTime As String
        Public EndTime As String
        Public WebExLink As String
        Public TempDisabled As Boolean
    End Class

End Class