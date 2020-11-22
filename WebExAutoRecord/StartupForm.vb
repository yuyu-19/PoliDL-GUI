Imports System.Threading
Imports System.Globalization
Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions
Imports Microsoft.Win32.TaskScheduler
Imports Microsoft.WindowsAPICodePack.Dialogs
Imports System.Net

Public Class StartupForm

    Public RootFolder As String = Environment.CurrentDirectory

    Dim Courses As New List(Of CourseData)
    Public Shared IsItalian As Boolean = (Thread.CurrentThread.CurrentCulture.IetfLanguageTag = "it-IT")

    Private Sub StartupForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'It was originally built with code, so I'm still going to adjust the size of everything with it, rather than re-doing it in the designer.

        Dim CFont As New Font(Question.Font.FontFamily, 10, Question.Font.Style)
        Question.Font = CFont
        CreditLabel.Font = CFont
        localmode.Name = "recbutton"
        downloadmode.Name = "dlbutton"
        localmode.AutoSize = True
        downloadmode.AutoSize = True

        AddHandler localmode.Click, AddressOf LocalMode_Click
        AddHandler downloadmode.Click, AddressOf DownloadMode_Click



        If IsItalian Then
            Question.Text = "Vuoi gestire le registrazioni locali o scaricare delle registrazioni da webex?"
            localmode.Text = "Locale"
        Else
            Question.Text = "Would you like to manage the local recordings or download them from webex?"
            localmode.Text = "Local"
        End If
        downloadmode.Text = "Download"

        Dim size As Size = TextRenderer.MeasureText(Question.Text, CFont)
        Question.Width = size.Width
        Question.Height = size.Height
        Dim p As Point = Question.Location
        p.X = 10
        p.Y = 5
        Question.Location = p

        Me.Height = Question.Height + localmode.Height + 60
        Me.Width = Question.Width + 30

        p = localmode.Location
        p.Y = Question.Height + 10
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

        If Directory.Exists(appData) AndAlso Not File.Exists(appData & "\version.txt") Then
            File.WriteAllText(appData & "\version.txt", "1")  'We don't have a version number, so that means the version unpacked must've been v1
        End If

        If My.Resources.Version <> File.ReadAllText(appData & "\version.txt") Then
            Directory.Delete(appData, True) 'Version mismatch - the internal data is newer than the stored one
        End If

        If Not Directory.Exists(appData) Or File.Exists(appData & "\temp.zip") Or Not File.Exists(appData & "\StartRec.exe") Then

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

            Directory.CreateDirectory(appData)

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
            File.Delete(appData & "\temp.zip")
            Me.Cursor = Cursors.Default
        End If

        Question.Text = "Would you like to manage the local recordings or download them from webex?"
        If IsItalian Then
            Question.Text = "Vuoi gestire le registrazioni locali o scaricare delle registrazioni da webex?"
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

        DownloadForm.ShowDialog()
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