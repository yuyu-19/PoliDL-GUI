Imports System.Threading
Imports System.Globalization
Imports System.Drawing
Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.Win32.TaskScheduler
Imports Microsoft.WindowsAPICodePack.Dialogs
Imports System.Net
Imports WebExAutoRecord.StartupForm

Public Class GenerateDataForm
    Dim Courses As New List(Of CourseData)
    Dim IsItalian As Boolean = (Thread.CurrentThread.CurrentCulture.IetfLanguageTag = "it-IT")

    Private Sub GenerateDataForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Using ts As New TaskService()

            Dim ToBeDeleted As New List(Of Object)

            Dim pred As Predicate(Of Task) = AddressOf IsWebExOS

            Dim OneShotsFound As New Dictionary(Of String, List(Of DayData))

            For Each t As Task In ts.RootFolder.EnumerateTasks(pred, True)
                Dim tempstring As String = t.Name.Replace("WebExRec-OS-", "")
                Dim courseID As String = tempstring.Substring(0, tempstring.IndexOf("-"))


                Dim ListOfOneShots As List(Of DayData) = Nothing

                If Not OneShotsFound.TryGetValue(courseID, ListOfOneShots) Then
                    ListOfOneShots = New List(Of DayData)
                    OneShotsFound.Add(courseID, ListOfOneShots)
                End If

                Dim NewOneShot As New DayData With {
                    .TempDisabled = False
                }

                Dim trigger As Trigger = t.Definition.Triggers(0)
                If Date.Now > trigger.EndBoundary Then
                    ToBeDeleted.Add(t)
                Else
                    NewOneShot.StartTime = trigger.StartBoundary.ToString("HH:mm")
                    NewOneShot.EndTime = trigger.EndBoundary.ToString("HH:mm")
                    NewOneShot.DayName = trigger.StartBoundary.ToString("dd/MM/yyyy")
                    Dim x As ExecAction = t.Definition.Actions(0)
                    NewOneShot.WebExLink = x.Arguments.Substring(x.Arguments.IndexOf("https://"))

                    OneShotsFound(courseID).Add(NewOneShot)
                End If

            Next

            For Each t As Task In ToBeDeleted
                ts.RootFolder.DeleteTask(t.Name)    'Delete all tasks that have no triggers left
            Next

            ToBeDeleted.Clear()

            'If sender.name = "StartTimePicker" Then
            '    If DTP.Value.TimeOfDay > max Then DTP.Value = DTP.Value.Date + max
            '    Courses(CourseIndex).OneShots.Last.StartTime = DTP.Value.ToString("HH:mm")
            'ElseIf sender.name = "EndTimePicker" Then
            '    If DTP.Value.TimeOfDay > max Then DTP.Value = DTP.Value.Date + max
            '    Courses(CourseIndex).OneShots.Last.EndTime = DTP.Value.ToString("HH:mm")
            'ElseIf sender.name = "DayPicker" Then
            '    Courses(CourseIndex).OneShots.Last.DayName = DTP.Value.ToString("dd/MM/yyyy")
            'End If


            pred = AddressOf IsWebEx


            For Each t As Task In ts.RootFolder.EnumerateTasks(pred, True)
                'We're going to fill up the Courses list here, and empty it later if there's actually no tasks. I'd like to avoid iterating through them all twice.

                Dim NewCourse As New CourseData


                NewCourse.ID = t.Name.Replace("WebExRec-", "")

                If OneShotsFound.ContainsKey(NewCourse.ID) Then
                    NewCourse.OneShots = OneShotsFound(NewCourse.ID)
                End If

                NewCourse.Name = t.Definition.RegistrationInfo.Description.Replace("REC: ", "")

                Dim lines As String() = File.ReadAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & NewCourse.ID & "\LinkSchedule.txt")


                If t.Definition.Triggers.Count <> 0 Then

                End If
                For Each trigger In t.Definition.Triggers
                    If Date.Now > trigger.EndBoundary Then
                        ToBeDeleted.Add(trigger)
                    Else
                        If NewCourse.EndDate Is Nothing Then NewCourse.EndDate = trigger.EndBoundary.ToString("dd/MM/yyyy")
                        If NewCourse.StartDate Is Nothing Then NewCourse.StartDate = trigger.StartBoundary.ToString("dd/MM/yyyy")

                        Dim WT As WeeklyTrigger = TryCast(trigger, WeeklyTrigger)
                        If WT Is Nothing Then Continue For           'Not a wek
                        Dim NewDay As New DayData

                        NewDay.DayName = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName([Enum].Parse(GetType(DayOfWeek), WT.DaysOfWeek.ToString))
                        'This looks long, complicated and stupid, but it's necessary because WT.DaysOfWeek returns a "DaysOfTheWeek" enum, whereas GetDayName requires a DayOfWeek enum. Simple, amirite?

                        NewDay.DayName = NewDay.DayName.Replace(NewDay.DayName(0), Char.ToUpper(NewDay.DayName(0))) 'Complicated? Yes. But functional.

                        NewDay.StartTime = trigger.StartBoundary.ToString("HH:mm")
                        NewDay.EndTime = trigger.EndBoundary.ToString("HH:mm")

                        'I could've probably just gotten all the day data from the linkschedule.txt file. 
                        'I'm dumb. Whatever, I would've had to do this anyways to check for and remove outdated triggers.

                        For Each line In lines
                            Dim data As String() = line.Split(",")
                            If data(0) = [Enum].Parse(GetType(DayOfWeek), WT.DaysOfWeek.ToString) And data(1) = NewDay.StartTime And data(2) = NewDay.EndTime Then
                                NewDay.WebExLink = data(3)
                                NewDay.TempDisabled = data(4)
                            End If
                        Next


                        NewCourse.Days.Add(NewDay)
                    End If
                Next


                For Each tr As Trigger In ToBeDeleted
                    t.Definition.Triggers.Remove(tr)
                Next



                ToBeDeleted.Clear()

                Dim NewProfessors As New Dictionary(Of String, String)
                For Each line In File.ReadAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & NewCourse.ID & "\ProfessorLinks.txt")
                    NewProfessors.Add(line.Substring(0, line.IndexOf(",")), line.Substring(line.IndexOf(",") + 1))
                Next

                NewCourse.Professors = NewProfessors

                If t.Definition.Triggers.Count = 0 Then
                    ToBeDeleted.Add(t)
                Else
                    Courses.Add(NewCourse)  'If this task SHOULDN'T be deleted, add its course to the list
                End If

                ToBeDeleted.Clear()
            Next

            For Each t As Task In ToBeDeleted
                ts.RootFolder.DeleteTask(t.Name)    'Delete all tasks that have no triggers left
            Next

            If ts.RootFolder.EnumerateTasks(pred, True).Count = 0 Then 'A webexrec task is present (we're checking this AFTER we've cleared out all the old ones)
                Courses.Clear() 'There's no tasks present, so let's reset and keep going.
            Else
                Me.Tag = "edit"
                Dim f As Form = CreateCourseForm()
                Me.Hide()
                f.ShowDialog(Me)
                Me.Close()
            End If
        End Using



        If IsItalian Then
            info.Text = "Per favore inserire la locazione del file HTML."
            Generate.Text = "Genera"
            SavePathTitle.Text = "Cartella registazioni"
            Browse.Text = "Esplora"
            BrowseFile.Text = "Esplora"
        End If
    End Sub

    Public Function IsWebEx(ByVal t As Task)
        Return t.Name.Contains("WebExRec-") And Not t.Name.Contains("WebExRec-OS-")
    End Function

    Public Function IsWebExOS(ByVal t As Task)
        Return t.Name.Contains("WebExRec-OS-")
    End Function


    Private Sub Generate_Click(sender As Object, e As EventArgs) Handles Generate.Click
        If IsItalian Then
            If SavePath.Text = "" Then
                MessageBox.Show("Per favore seleziona la cartella in cui salvare le registrazioni.")
                Return
            ElseIf HtmlPath.Text = "" Then
                MessageBox.Show("Per favore inserisci la posizione del file HTML.")
                Return
            End If

        Else
            If SavePath.Text = "" Then
                MessageBox.Show("Please select the folder to save the recordings in.")
                Return
            ElseIf HtmlPath.Text = "" Then
                MessageBox.Show("Please input the path to the HTML file.")
                Return
            End If
        End If



        Dim teachermark, semestermark, lessonsstartmark, lessonsendmark, frommark, tomark, virtualclassroommark As String
        Dim DayNames As New List(Of String)
        Dim DayNamesEN() As String = {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"} 'Yeah, making a duplicate sounds stupid, but I always need the english name

        If File.ReadAllText(HtmlPath.Text).Contains("Docente:") Then    'We don't care about the OS language here, we only care about the HTML file's.
            teachermark = "Docente:"
            semestermark = "Semestre:"
            lessonsstartmark = "Inizio lezioni:"
            lessonsendmark = "Fine lezioni:"
            frommark = "dalle"
            tomark = " alle"
            DayNames = {"Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato"}.ToList
            virtualclassroommark = "Aula virtuale - "
            Dim obsconfig As List(Of String) = File.ReadAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\global.ini").ToList 'Set the OBS language to italian
            For Each line As String In obsconfig
                If line.Contains("[BASIC]") Then
                    obsconfig.Insert(obsconfig.IndexOf(line) - 1, "Language=it-IT")
                    Exit For
                ElseIf line.Contains("Language=") Then
                    Exit For
                End If
            Next

            File.WriteAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\global.ini", obsconfig.ToArray)
        Else
            teachermark = "Professor:"
            semestermark = "Semester:"
            lessonsstartmark = "Start of lessons:"
            lessonsendmark = "End of lesson:"
            frommark = "DALLE"
            tomark = " ALLE"
            DayNames = {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"}.ToList
            virtualclassroommark = "Virtual Classroom - "
        End If

        Dim Lines As List(Of String) = File.ReadAllLines(HtmlPath.Text, System.Text.Encoding.Default).ToList()

        Dim startindex As Integer
        Dim LastCourseIndex As Integer = 0

        For Each Line In Lines

            If Line.Contains(teachermark) Then
                Courses.Add(New CourseData)
                Dim CourseLine As String = Lines(Lines.IndexOf(Line, LastCourseIndex) - 2)
                LastCourseIndex = Lines.IndexOf(Line, LastCourseIndex) + 1
                startindex = CourseLine.IndexOf("<b>") + 3
                Courses.Last.ID = CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim()
                startindex = CourseLine.IndexOf("-", startindex) + 1
                Courses.Last.Name = CourseLine.Substring(startindex, CourseLine.IndexOf("</b>", startindex) - startindex).Trim()
                Dim r As New Regex("[^a-zA-Z0-9 _\-]")
                Courses.Last.Name = r.Replace(Courses.Last.Name, "")

            ElseIf Line.Contains(lessonsstartmark) Then
                startindex = Line.IndexOf("</strong>") + "</strong>".Length
                Courses.Last.StartDate = Line.Substring(startindex).Trim
            ElseIf Line.Contains(lessonsendmark) Then
                startindex = Line.IndexOf("</strong>") + "</strong>".Length
                Courses.Last.EndDate = Line.Substring(startindex).Trim

            ElseIf Line.Contains(virtualclassroommark) Then

                If Not Line.Contains("https://politecnicomilano.webex.com") Then
                    Continue For 'It's a duplicate. Ignore it, since it doens't even contain the link.
                End If

                startindex = Line.IndexOf(virtualclassroommark) + virtualclassroommark.Length

                Dim profname As String = Line.Substring(startindex).Replace("</a>", "").Trim()

                startindex = Line.IndexOf("<a href=""") + "<a href=""".Length
                Dim webexlink As String = Line.Substring(startindex, Line.IndexOf("?") - startindex)

                Courses.Last.Professors.Add(profname, webexlink)

            ElseIf Line.Contains("<ul style=""margin-top: 0px; margin-bottom: 0px;") Then     'Assume that line contains day data, since that's the only unhandled type
                Dim i As Integer = 0
                Do Until Line.IndexOf(":5px;"">", i) = -1
                    i = Line.IndexOf(":5px;"">", i) + 7
                    Dim NewDay As New DayData
                    NewDay.TempDisabled = False
                    NewDay.DayName = Line.Substring(i, Line.IndexOf(frommark, i) - i).Trim()
                    i = Line.IndexOf(frommark, i) + frommark.Length
                    NewDay.StartTime = Line.Substring(i, Line.IndexOf(tomark, i) - i).Trim()

                    i = Line.IndexOf(tomark, i) + tomark.Length
                    NewDay.EndTime = Line.Substring(i, Line.IndexOf(",", i) - i).Trim()

                    If Courses.Last.Days.Count = 0 Then
                        Courses.Last.Days.Add(NewDay)
                    Else
                        For Each day In Courses.Last.Days       'This checks if the day is a duplicate. If it is, don't add it. This is to handle the case with multiple groups on the same date.
                            If NewDay.DayName = day.DayName And NewDay.StartTime = day.StartTime And NewDay.EndTime = day.EndTime Then
                                Exit For
                            ElseIf day Is Courses.Last.Days.Last Then
                                Courses.Last.Days.Add(NewDay)
                                Exit For
                            End If
                        Next
                    End If

                Loop

            End If


        Next


        'Debug coursedata printout.

        'Dim asda As String

        'For Each course In Courses
        '    asda = course.Name & "-" & course.ID
        '    For Each professor In course.Professors
        '        asda = asda & vbCrLf & professor.Key & " - " & professor.Value
        '    Next

        '    For Each day In course.Days
        '        asda = asda & vbCrLf & day.DayName & "-" & day.StartTime & "-" & day.EndTime
        '    Next
        '    MessageBox.Show(asda)
        'Next



        'Now we ask the user to assign a name to every day, of every course.

        Dim f As Form = CreateCourseForm()

        Do
            f.ShowDialog(Me)
            For Each Course In Courses
                For Each day In Course.Days
                    If day.WebExLink = "" Then
                        If IsItalian Then
                            MessageBox.Show("Per favore riempi tutte le date.")
                        Else
                            MessageBox.Show("Please fill in all the dates.")
                        End If
                        Continue Do  'If there's an empty link, restart
                    End If
                Next
            Next
            Exit Do
        Loop




        'Now we have all the CourseData objects in Courses, and they're all set up correctly. We just iterate through them and set up the tasks

        Using ts As New TaskService()

            For Each Course In Courses
                Dim CourseTD As TaskDefinition = ts.NewTask
                CourseTD.RegistrationInfo.Description = "REC: " & Course.Name

                If File.Exists(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\LinkSchedule.txt") Then File.Delete(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\LinkSchedule.txt")
                If File.Exists(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\ProfessorLinks.txt") Then File.Delete(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\ProfessorLinks.txt")

                Dim fourAMbodgebelike As String = ""

                For Each Professor In Course.Professors
                    fourAMbodgebelike = fourAMbodgebelike & Professor.Key & "," & Professor.Value & vbCrLf
                Next
                fourAMbodgebelike = fourAMbodgebelike.Substring(0, fourAMbodgebelike.Length - vbCrLf.Length)
                Directory.CreateDirectory(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID)

                File.WriteAllText(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\ProfessorLinks.txt", fourAMbodgebelike)

                fourAMbodgebelike = ""
                For Each day In Course.Days
                    fourAMbodgebelike = fourAMbodgebelike & DayNames.IndexOf(day.DayName) & "," & day.StartTime & "," & day.EndTime & "," & day.WebExLink & ",0" & vbCrLf
                    'Write all the links with the day/time in the corresponding folder

                    Dim WT As New WeeklyTrigger
                    WT.DaysOfWeek = [Enum].Parse(GetType(DaysOfTheWeek), DayNamesEN(DayNames.IndexOf(day.DayName))) 'This looks stupid, but it at least works regardless of langauge
                    'There's probably a billion better ways to do this but it's 3am and I'm tired

                    'ERROR HERE???

                    WT.StartBoundary = DateTime.ParseExact(Course.StartDate & " " & day.StartTime & ":59", "dd/MM/yyyy HH:mm:ss", Thread.CurrentThread.CurrentCulture)
                    WT.EndBoundary = DateTime.ParseExact(Course.EndDate & " " & day.EndTime, "dd/MM/yyyy HH:mm", Thread.CurrentThread.CurrentCulture)
                    WT.WeeksInterval = 1

                    CourseTD.Triggers.Add(WT)
                Next

                File.WriteAllText(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\LinkSchedule.txt", fourAMbodgebelike)

                CourseTD.Actions.Add(New ExecAction(StartupForm.RootFolder & "StartRec.exe", Course.ID, StartupForm.RootFolder))

                'REMOVE THE URL ARGUMENT. COMPLETELY USELESS. WRITE THEM ALL TO A FILE WITH THE TIMES AND DATES, AND HAVE THE AHK SCRIPT FIGURE OUT WHICH ONE IT NEEDS
                'NEED TO IMPLEMENT BEING ABLE TO READ THEM AGAIN AS WELL

                ts.RootFolder.RegisterTaskDefinition("WebExRec-" & Course.ID, CourseTD)

                'Generate OBS profile

                Directory.CreateDirectory(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID)
                File.Copy(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\template\streamEncoder.json", StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\streamEncoder.json", True)
                Dim text As String = File.ReadAllText(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\template\basic.ini")
                text = text.Replace("[SCREENRESX]", Screen.PrimaryScreen.Bounds.Width)
                text = text.Replace("[SCREENRESY]", Screen.PrimaryScreen.Bounds.Height)
                text = text.Replace("[PATH]", SavePath.Text & "\" & Course.Name & "-" & Course.ID)
                text = text.Replace("[SERVERPORT]", Course.ID Mod 65534)
                text = text.Replace("template", Course.ID)

                Directory.CreateDirectory(SavePath.Text & "\" & Course.Name & "-" & Course.ID)
                'This saves the current template file, for comparing later.


                File.WriteAllText(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\basic.ini", text)
                'File.WriteAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\links.txt", Course.WebExLinks)
                'OBS profile generation complete
            Next

            File.Copy(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\template\basic.ini", StartupForm.RootFolder & "\OBS\config\obs-studio\currenttemplate.ini", True)

        End Using

        If IsItalian Then
            MessageBox.Show("Tutte le registrazioni sono state impostate.")
        Else
            MessageBox.Show("All recordings have been scheduled.")
        End If
        Application.Exit()

    End Sub

    Function CreateCourseForm() As Form

        Dim f As New Form
        Dim P As New Point
        f.Width = 0
        For Each course In Courses
            P.X = 15
            P.Y = 35 * Courses.IndexOf(course) + 10
            Dim RTB As New RichTextBox
            RTB.ReadOnly = True
            RTB.AutoSize = True
            RTB.BorderStyle = BorderStyle.None
            RTB.Multiline = False
            RTB.Text = course.Name
            Dim CFont As New Font(RTB.Font.FontFamily, 9, RTB.Font.Style)
            RTB.Font = CFont
            Dim size As Size = TextRenderer.MeasureText(course.Name, CFont)
            RTB.Width = size.Width
            RTB.Height = size.Height

            RTB.Location = P
            f.Controls.Add(RTB)
            Dim EB As New Button
            If IsItalian Then
                EB.Text = "Modifica"
            Else
                EB.Text = "edit"
            End If

            EB.Name = "edit"
            EB.Tag = Courses.IndexOf(course)
            EB.AutoSize = True
            f.Controls.Add(EB)

            P.X = P.X + RTB.Width + 10
            EB.Location = P

            If f.Width < P.X + EB.Width + 30 Then f.Width = P.X + EB.Width + 30

            AddHandler EB.Click, AddressOf Edit_Click
        Next


        Dim max As Integer = 0

        For Each CTRL As Control In f.Controls
            If TypeOf CTRL Is RichTextBox AndAlso CTRL.Width > max Then max = CTRL.Width       'Get the max width
        Next

        Dim Done As New Button
        If IsItalian Then
            Done.Text = "Fine"
        Else
            Done.Text = "Done"
        End If

        Done.Name = "Done"
        Done.Tag = "Done"

        For Each CTRL As Control In f.Controls
            If TypeOf CTRL Is RichTextBox Then CTRL.Width = max

            If TypeOf CTRL Is Button Then
                P = CTRL.Location
                P.X = 15 + max + 10
                CTRL.Location = P
                Done.Size = CTRL.Size
            End If
        Next

        f.Height = 35 * Courses.Count + 45 + Done.Height + 10

        P.X = f.ClientSize.Width - 15 - Done.Width
        P.Y = f.ClientSize.Height - 5 - Done.Height
        Done.Location = P
        f.Controls.Add(Done)
        AddHandler Done.Click, AddressOf f.Close



        Return f

    End Function

    Private Sub Edit_Click(sender As Object, e As EventArgs)
        Dim CurrentCourse As CourseData = Courses(sender.tag)
        Dim b As Button = sender
        Dim IsEdit As Boolean = (Me.Tag = "edit")
        Dim p As Point

        Dim HolyShitImSoFuckingDoneWithThis As Integer = 0

        Dim f As New Form
        f.Width = 0
        f.Text = "Schedule for " & CurrentCourse.Name
        If IsItalian Then f.Text = "Programma di " & CurrentCourse.Name

        f.Tag = Courses.IndexOf(CurrentCourse)
        For Each Day In CurrentCourse.Days
            p.X = 15
            p.Y = 35 * CurrentCourse.Days.IndexOf(Day) + 10
            Dim RTB As New RichTextBox
            RTB.ReadOnly = True
            RTB.AutoSize = True
            RTB.BorderStyle = BorderStyle.None
            RTB.Multiline = False
            RTB.Text = Day.DayName & ":" & Day.StartTime & "-" & Day.EndTime
            Dim CFont As New Font(RTB.Font.FontFamily, 9, RTB.Font.Style)
            RTB.Font = CFont
            Dim size As Size = TextRenderer.MeasureText(RTB.Text, CFont)
            RTB.Width = size.Width
            RTB.Height = size.Height
            RTB.Tag = CurrentCourse.Days.IndexOf(Day)

            RTB.Location = p
            f.Controls.Add(RTB)
            Dim CB As New ComboBox
            Dim LongestItem As String = ""
            For Each Professor In CurrentCourse.Professors
                CB.Items.Add(Professor.Key)
                If Professor.Key.Length > LongestItem.Length Then LongestItem = Professor.Key
            Next
            CB.Font = CFont
            CB.Text = ""
            size = TextRenderer.MeasureText(LongestItem, CFont)
            CB.Width = size.Width
            CB.Height = size.Height
            CB.AllowDrop = False
            CB.ImeMode = ImeMode.NoControl
            CB.Tag = CurrentCourse.Days.IndexOf(Day)
            Dim res As String = Nothing
            CB.Text = ""

            If Day.WebExLink <> "" Then
                For Each pair In CurrentCourse.Professors
                    If pair.Value = Day.WebExLink Then CB.Text = pair.Key
                Next
            End If
            f.Controls.Add(CB)

            p.X = p.X + RTB.Width + 10
            CB.Location = p

            AddHandler CB.TextChanged, AddressOf ComboBox_ChangeElement


            Dim DB As New Button

            DB.Text = "Delete"
            If IsItalian Then DB.Text = "Elimina"

            DB.Name = "Delete"
            DB.Tag = CurrentCourse.Days.IndexOf(Day)
            DB.AutoSize = True
            f.Controls.Add(DB)

            p.X = p.X + CB.Width + 10
            DB.Location = p


            AddHandler DB.Click, AddressOf Delete_Click

            If IsEdit Then
                Dim CkBox As New CheckBox
                CkBox.Checked = Day.TempDisabled
                CkBox.Enabled = True
                CkBox.Name = "TempDisable"
                CkBox.AutoSize = True
                CkBox.Tag = CurrentCourse.Days.IndexOf(Day)
                If IsItalian Then
                    CkBox.Text = "Disabilita una volta"
                Else
                    CkBox.Text = "Disable once"
                End If

                f.Controls.Add(CkBox)
                HolyShitImSoFuckingDoneWithThis = DB.Width
                p.X = p.X + DB.Width + 10
                p.Y = p.Y + (CB.Height - CkBox.Height) / 2
                CkBox.Location = p
                p.Y = p.Y - (CB.Height - CkBox.Height) / 2
                If f.Width < p.X + CkBox.Width + 30 Then f.Width = p.X + CkBox.Width + 30

                AddHandler CkBox.CheckedChanged, AddressOf Checkbox_Changed
            Else
                If f.Width < p.X + DB.Width + 30 Then f.Width = p.X + DB.Width + 30
            End If


        Next



        f.Height = 35 * CurrentCourse.Days.Count + 45




        Dim max1, max2 As Integer                        'Probably not the best solution to make everything the same size, and probably wasteful, but I'm just lazy and bad


        For Each CTRL As Control In f.Controls
            If TypeOf CTRL Is RichTextBox AndAlso CTRL.Width > max1 Then max1 = CTRL.Width

            If TypeOf CTRL Is ComboBox AndAlso CTRL.Width > max2 Then max2 = CTRL.Width

        Next

        For Each CTRL As Control In f.Controls
            If TypeOf CTRL Is RichTextBox Then CTRL.Width = max1

            If TypeOf CTRL Is ComboBox Then
                CTRL.Width = max2 + 17            'Compensating for the arrow
                p = CTRL.Location
                p.X = 15 + max1 + 10
                CTRL.Location = p
            End If
        Next

        Dim Iamahackandjustwantthistobeover As Point

        For Each CTRL As Control In f.Controls
            If TypeOf CTRL Is Button Then
                p = CTRL.Location
                p.X = 15 + max1 + 10 + max2 + 17 + 10
                CTRL.Location = p
                f.Width = 15 + max1 + 10 + max2 + 10 + CTRL.Width + 47
                Iamahackandjustwantthistobeover = p
            ElseIf TypeOf CTRL Is CheckBox Then
                p = CTRL.Location
                p.X = 15 + max1 + 10 + max2 + 17 + HolyShitImSoFuckingDoneWithThis + 20
                CTRL.Location = p
                f.Width = 15 + max1 + 10 + max2 + 10 + HolyShitImSoFuckingDoneWithThis + CTRL.Width + 47
            End If
        Next




        'Add a OneShots As List(of DayData) property to courseData.
        'Fill that in by checking for tasks with WebExRec-OS- (additional check in the existing sub)
        'If a oneshot recording is marked as completed (the current date is beyond the endboundary) then delete it
        'Oneshot's dayname contains the full date, instead of just the weekday

        If IsEdit Then

            Dim AddOneShot As New Button
            AddOneShot.AutoSize = True

            If IsItalian Then
                AddOneShot.Text = "Aggiungi registrazione temp."
            Else
                AddOneShot.Text = "Add one-time recording"
            End If
            f.Height = f.Height + AddOneShot.Height + 5
            f.Controls.Add(AddOneShot)
            p.X = 15
            p.Y = f.ClientSize.Height - AddOneShot.Height - 5
            AddOneShot.Location = p

            AddHandler AddOneShot.Click, AddressOf OneShot_Click

            'To create one, add an element to the oneshots list, and fill it all in
            'Then create the task (figuring out the endboundary is gonna be a pain in the ass)
            'I don't need to touch the link file. Just do it via tasks.
        End If

        Dim Done As New Button
        If IsItalian Then
            Done.Text = "Fine"
        Else
            Done.Text = "Done"
        End If

        Done.Name = "Done"
        Done.Tag = "Done"
        Done.AutoSize = True

        If Not IsEdit Then
            f.Height = f.Height + Done.Height + 5
        End If


        Iamahackandjustwantthistobeover.Y = f.ClientSize.Height - 5 - Done.Height
        Done.Location = Iamahackandjustwantthistobeover
        f.Controls.Add(Done)
        AddHandler Done.Click, AddressOf f.Close

        If IsEdit Then
            For Each OneShot In CurrentCourse.OneShots
                f.Height = f.Height + 35

                p = New Point
                p.X = 15
                Dim RTB, RTB2 As New RichTextBox With {
                            .ReadOnly = True,
                            .AutoSize = True,
                            .BorderStyle = BorderStyle.None,
                            .Multiline = False
                        }
                RTB.Text = OneShot.DayName & ":" & OneShot.StartTime & "-" & OneShot.EndTime
                Dim CFont As New Font(RTB.Font.FontFamily, 9, RTB.Font.Style)
                RTB.Font = CFont
                RTB.Size = TextRenderer.MeasureText(RTB.Text, CFont)
                p.Y = f.ClientSize.Height - 10 - RTB.Height
                RTB.Location = p
                f.Controls.Add(RTB)
                'Gotta add the oneshots/whatever
                'BOOKMARK
                RTB2.Font = CFont

                For Each professor In CurrentCourse.Professors
                    If professor.Value = OneShot.WebExLink Then
                        RTB2.Text = professor.Key
                    End If
                Next

                RTB2.Size = TextRenderer.MeasureText(RTB2.Text, CFont)
                p.X = Done.Location.X + Done.Width - RTB2.Width + 4
                f.Controls.Add(RTB2)
                RTB2.Location = p
            Next
        End If



        f.ShowDialog(b.Parent)



    End Sub

    Private Sub OneShot_Click(sender As Object, e As EventArgs)
        Dim b As Button = sender
        Dim courseindex As Integer = b.Parent.Tag
        Dim currentcourse As CourseData = Courses(courseindex)
        Dim NewOneShot As New DayData With {
            .TempDisabled = False
        }
        Courses(courseindex).OneShots.Add(NewOneShot)
        Dim f As New Form
        f.Tag = courseindex

        Dim DayPicker As New DateTimePicker
        Dim CFont As New Font(DayPicker.Font.FontFamily, 9, DayPicker.Font.Style)
        DayPicker.Format = DateTimePickerFormat.Short
        DayPicker.Font = CFont
        Dim p As New Point(5, 5)
        f.Controls.Add(DayPicker)
        DayPicker.Size = TextRenderer.MeasureText("99/99/9999", CFont)
        DayPicker.Width += 15
        DayPicker.Location = p
        DayPicker.Name = "DayPicker"

        AddHandler DayPicker.ValueChanged, AddressOf DTP_ChangeElement
        DTP_ChangeElement(DayPicker, Nothing)

        Dim StartTimePicker As New DateTimePicker
        StartTimePicker.CustomFormat = "HH:mm"
        StartTimePicker.Format = DateTimePickerFormat.Custom
        StartTimePicker.ShowUpDown = True
        StartTimePicker.Font = CFont
        p.X = p.X + DayPicker.Width + 5
        f.Controls.Add(StartTimePicker)
        StartTimePicker.Size = TextRenderer.MeasureText("99:99", CFont)
        StartTimePicker.Width += 15
        StartTimePicker.Location = p
        StartTimePicker.Name = "StartTimePicker"

        AddHandler StartTimePicker.ValueChanged, AddressOf DTP_ChangeElement
        DTP_ChangeElement(StartTimePicker, Nothing)

        Dim EndTimePicker As New DateTimePicker
        EndTimePicker.CustomFormat = "HH:mm"
        EndTimePicker.Format = DateTimePickerFormat.Custom
        EndTimePicker.ShowUpDown = True
        EndTimePicker.Font = CFont
        p.X = p.X + StartTimePicker.Width + 5
        f.Controls.Add(EndTimePicker)
        EndTimePicker.Size = TextRenderer.MeasureText("99:99", CFont)
        EndTimePicker.Width += 15
        EndTimePicker.Location = p
        EndTimePicker.Name = "EndTimePicker"

        AddHandler EndTimePicker.ValueChanged, AddressOf DTP_ChangeElement
        DTP_ChangeElement(EndTimePicker, Nothing)

        Dim CBprof As New ComboBox
        CBprof.Name = "CBProf"
        Dim LongestItem As String = ""
        For Each Professor In Courses(courseindex).Professors
            CBprof.Items.Add(Professor.Key)
            If Professor.Key.Length > LongestItem.Length Then LongestItem = Professor.Key
        Next

        CBprof.Font = CFont
        CBprof.Text = ""
        CBprof.Width = Size.Width
        CBprof.Height = Size.Height
        CBprof.AllowDrop = False
        CBprof.ImeMode = ImeMode.NoControl
        p.X = p.X + StartTimePicker.Width + 5
        f.Controls.Add(CBprof)
        CBprof.Size = TextRenderer.MeasureText(LongestItem, CFont)
        CBprof.Width += 17
        CBprof.Location = p

        f.Width = 5 + DayPicker.Width + 5 + StartTimePicker.Width + 5 + EndTimePicker.Width + 5 + CBprof.Width + 5 + 17


        AddHandler CBprof.TextChanged, AddressOf CBprofOS_ChangeElement

        Dim Done As New Button
        If IsItalian Then
            Done.Text = "Fine"
        Else
            Done.Text = "Done"
        End If

        Done.Name = "Done"
        Done.Tag = "Done"
        Done.AutoSize = True
        f.Controls.Add(Done)

        f.Height = 5 + CBprof.Height + 5 + 35
        p.X = f.ClientSize.Width - 5 - Done.Width
        p.Y = CBprof.Location.Y + CBprof.Height + 5
        Done.Location = p

        f.Height = 5 + CBprof.Height + 5 + Done.Height + 5 + 35
        AddHandler Done.Click, AddressOf OSDone_Click


        f.ShowDialog(b.Parent)

        'UHHHH. Need to figure out how to add/display the oneshots in the previous window.


    End Sub

    Private Sub OSDone_Click(sender As Object, e As EventArgs)
        Dim OSForm As Form = sender.parent
        Dim CBprof As ComboBox = OSForm.Controls.Find("CBProf", False)(0)
        Dim ConfigForm As Form = OSForm.Owner
        Dim courseindex As Integer = ConfigForm.Tag
        Dim currentcourse As CourseData = Courses(courseindex)

        If CBprof.Text = "" Then
            If IsItalian Then
                MessageBox.Show("Per favore seleziona il professore")
            Else
                MessageBox.Show("Please choose the professor")
            End If
        Else
            Try
                Using ts As New TaskService()
                    Dim td As TaskDefinition = ts.NewTask
                    td.RegistrationInfo.Description = "Oneshot recording for " & currentcourse.ID
                    Dim T As New TimeTrigger
                    T.StartBoundary = DateTime.ParseExact(currentcourse.OneShots.Last.DayName & " " & currentcourse.OneShots.Last.StartTime & ":59", "dd/MM/yyyy HH:mm:ss", Thread.CurrentThread.CurrentCulture)
                    T.EndBoundary = DateTime.ParseExact(currentcourse.OneShots.Last.DayName & " " & currentcourse.OneShots.Last.EndTime, "dd/MM/yyyy HH:mm", Thread.CurrentThread.CurrentCulture)
                    If T.EndBoundary <= T.StartBoundary Then
                        If IsItalian Then
                            MessageBox.Show("L'orario di fine deve essere dopo quello d'inizio.")
                        Else
                            MessageBox.Show("The end time must be after the start time.")
                        End If
                        Return
                    End If
                    td.Triggers.Add(T)
                    td.Actions.Add(New ExecAction(StartupForm.RootFolder & "\StartRec.exe", currentcourse.ID & " " & currentcourse.OneShots.Last.WebExLink, StartupForm.RootFolder))

                    Dim pred As Predicate(Of Task) = AddressOf IsWebExOS
                    Dim taskcount As Integer = ts.RootFolder.EnumerateTasks(pred, True).Count
                    ts.RootFolder.RegisterTaskDefinition("WebExRec-OS-" & currentcourse.ID & "-" & taskcount, td)

                    ConfigForm.Height = OSForm.Owner.Height + 35

                    Dim p As Point
                    p.X = 15
                    Dim RTB, RTB2 As New RichTextBox With {
                        .ReadOnly = True,
                        .AutoSize = True,
                        .BorderStyle = BorderStyle.None,
                        .Multiline = False
                    }
                    RTB.Text = currentcourse.OneShots.Last.DayName & ":" & currentcourse.OneShots.Last.StartTime & "-" & currentcourse.OneShots.Last.EndTime
                    Dim CFont As New Font(RTB.Font.FontFamily, 9, RTB.Font.Style)
                    RTB.Font = CFont
                    RTB.Size = TextRenderer.MeasureText(RTB.Text, CFont)
                    p.Y = ConfigForm.ClientSize.Height - 10 - RTB.Height
                    RTB.Location = p
                    ConfigForm.Controls.Add(RTB)
                    'Gotta add the oneshots/whatever
                    'BOOKMARK
                    RTB2.Font = CFont
                    RTB2.Text = CBprof.Text
                    RTB2.Size = TextRenderer.MeasureText(CBprof.Text, CFont)
                    p.X = ConfigForm.ClientSize.Width - 5 - RTB2.Width
                    ConfigForm.Controls.Add(RTB2)
                    RTB2.Location = p
                End Using
                OSForm.Close()
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try

        End If

    End Sub


    Private Sub DTP_ChangeElement(sender As Object, e As EventArgs)
        Dim DTP As DateTimePicker = sender
        Dim CourseIndex As Integer = DTP.Parent.Tag
        Dim max As New TimeSpan(23, 59, 59)
        If sender.name = "StartTimePicker" Then
            If DTP.Value.TimeOfDay > max Then DTP.Value = DTP.Value.Date + max
            Courses(CourseIndex).OneShots.Last.StartTime = DTP.Value.ToString("HH:mm")
        ElseIf sender.name = "EndTimePicker" Then
            If DTP.Value.TimeOfDay > max Then DTP.Value = DTP.Value.Date + max
            Courses(CourseIndex).OneShots.Last.EndTime = DTP.Value.ToString("HH:mm")
        ElseIf sender.name = "DayPicker" Then
            Courses(CourseIndex).OneShots.Last.DayName = DTP.Value.ToString("dd/MM/yyyy")
        End If
    End Sub

    Private Sub CBprofOS_ChangeElement(sender As Object, e As EventArgs)
        Dim CB As ComboBox = sender
        Dim CourseIndex As Integer = CB.Parent.Tag
        Courses(CourseIndex).OneShots.Last.WebExLink = Courses(CourseIndex).Professors.Item(CB.Text)
    End Sub
    Private Sub ComboBox_ChangeElement(sender As Object, e As EventArgs)
        Dim CB As ComboBox = sender
        Dim IsEdit As Boolean = (Me.Tag = "edit")
        Dim CourseIndex As Integer = CB.Parent.Tag
        Dim DayIndex As Integer = sender.tag

        Dim CurrentCourse As CourseData = Courses(CourseIndex)
        Dim CurrentDay As DayData = CurrentCourse.Days(DayIndex)

        If IsEdit Then  'Update the actual file if we're in edit mode
            Dim DayNames As List(Of String) = {"Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato"}.ToList
            'I'm sure all of this could be consensed down to like a single line of code if I knew how the fuck the datetime stuff works
            'but I'm too retarded/don't care enough about the efficency of this function that's used once in a blue moon

            If Not DayNames.Contains(CurrentDay.DayName) Then
                DayNames = {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"}.ToList
            End If

            Dim DayNamesEN As String() = {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"}

            Dim lines As List(Of String) = File.ReadAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Courses(CourseIndex).ID & "\LinkSchedule.txt").ToList

            For Each line In lines
                Dim data As String() = line.Split(",")
                If [Enum].Parse(GetType(DayOfWeek), DayNamesEN(DayNames.IndexOf(CurrentDay.DayName))) = data(0) And data(1) = CurrentDay.StartTime And data(2) = CurrentDay.EndTime And CurrentDay.WebExLink = data(3) Then
                    lines(lines.IndexOf(line)) = line.Replace(data(2) & "," & data(3), data(2) & "," & CurrentCourse.Professors.Item(CB.Text))
                    Exit For
                End If
            Next

            File.Delete(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Courses(CourseIndex).ID & "\LinkSchedule.txt")
            File.WriteAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Courses(CourseIndex).ID & "\LinkSchedule.txt", lines)

        End If


        'MessageBox.Show("Set URL for course " & CurrentCourse.ID & " on " & CurrentDay.DayName & " from " & CurrentDay.StartTime & " to " & CurrentDay.EndTime & " to " & CurrentCourse.Professors.Item(CB.Text))
        Courses(CourseIndex).Days(DayIndex).WebExLink = CurrentCourse.Professors.Item(CB.Text)
    End Sub

    Private Sub Checkbox_Changed(sender As Object, e As EventArgs)
        Dim CkBox As CheckBox = sender
        Dim CourseIndex As Integer = CkBox.Parent.Tag
        Dim DayIndex As Integer = sender.tag
        Courses(CourseIndex).Days(DayIndex).TempDisabled = CkBox.CheckState
        Dim CurrentDay As DayData = Courses(CourseIndex).Days(DayIndex)


        Dim DayNames As List(Of String) = {"Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato"}.ToList
        'I'm sure all of this could be consensed down to like a single line of code if I knew how the fuck the datetime stuff works
        'but I'm too retarded/don't care enough about the efficency of this function that's used once in a blue moon

        If Not DayNames.Contains(CurrentDay.DayName) Then
            DayNames = {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"}.ToList
        End If

        Dim DayNamesEN As String() = {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"}

        Dim lines As List(Of String) = File.ReadAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Courses(CourseIndex).ID & "\LinkSchedule.txt").ToList

        For Each line In lines
            Dim data As String() = line.Split(",")
            If [Enum].Parse(GetType(DayOfWeek), DayNamesEN(DayNames.IndexOf(CurrentDay.DayName))) = data(0) And data(1) = CurrentDay.StartTime And data(2) = CurrentDay.EndTime And CurrentDay.WebExLink = data(3) Then
                lines(lines.IndexOf(line)) = line.Replace(data(3) & "," & data(4), data(3) & "," & Convert.ToInt32(CurrentDay.TempDisabled))
                Exit For
            End If
        Next

        File.Delete(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Courses(CourseIndex).ID & "\LinkSchedule.txt")
        File.WriteAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Courses(CourseIndex).ID & "\LinkSchedule.txt", lines)

    End Sub


    Private Sub Delete_Click(sender As Object, e As EventArgs)
        Dim b As Button = sender
        Dim f As Form = b.Parent
        Dim DB As Button = Nothing
        Dim IsEdit As Boolean = False

        For Each control In f.Controls
            Dim CK As CheckBox = TryCast(control, CheckBox)
            If CK IsNot Nothing Then IsEdit = True
        Next

        Dim ToDelete As New List(Of Control)
        Dim ToBeLowered As New List(Of Control)

        For Each CTRL As Control In f.Controls

            If IsNumeric(CTRL.Tag) AndAlso CTRL.Tag > b.Tag Then    'If it's a day after the one getting delete'd
                Dim p As Point = CTRL.Location
                p.Y = p.Y - 35          'Move it up one slot
                CTRL.Location = p

                ToBeLowered.Add(CTRL)
            ElseIf IsNumeric(CTRL.Tag) AndAlso CTRL.Tag = b.Tag Then

                Dim CB As ComboBox = TryCast(CTRL, ComboBox)
                If CB IsNot Nothing Then RemoveHandler CB.TextChanged, AddressOf ComboBox_ChangeElement

                DB = TryCast(CTRL, Button)
                If DB IsNot Nothing Then RemoveHandler DB.Click, AddressOf Delete_Click

                ToDelete.Add(CTRL)
            End If

            DB = TryCast(CTRL, Button)
            If DB IsNot Nothing AndAlso Not IsNumeric(DB.Tag) Then
                Dim p As Point = DB.Location
                p.Y -= 35
                DB.Location = p
            End If

        Next

        For Each ctrl In ToBeLowered
            ctrl.Tag = ctrl.Tag - 1
        Next

        If IsEdit Then
            'INSERT CODE TO REMOVE THE TRIGGER FROM THE TASK HEREEEEEEEEEEEEE

            Using ts As New TaskService()
                Dim t As Task = ts.GetTask("WebExRec-" & Courses(f.Tag).ID)
                Dim Currentday As DayData = Courses(f.Tag).Days(b.Tag)

                Dim toberemoved As Trigger
                For Each trigger In t.Definition.Triggers
                    Dim WT As WeeklyTrigger = TryCast(trigger, WeeklyTrigger)
                    If WT Is Nothing Then Continue For           'Not a wek
                    If Currentday.StartTime = trigger.StartBoundary.ToString("HH:mm") And Currentday.EndTime = trigger.EndBoundary.ToString("HH:mm") Then
                        If Currentday.DayName.ToLower = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName([Enum].Parse(GetType(DayOfWeek), WT.DaysOfWeek.ToString)) Then
                            MessageBox.Show("All trigger data matches")
                            toberemoved = trigger
                        End If
                    End If
                Next
                If toberemoved IsNot Nothing Then
                    t.Definition.Triggers.Remove(toberemoved)
                    t.RegisterChanges()
                End If

            End Using
        End If


        Courses(f.Tag).Days.RemoveAt(b.Tag)

        For Each CTRL In ToDelete
            f.Controls.Remove(CTRL)
            CTRL.Dispose()
            CTRL = Nothing
        Next


        f.Height -= 35
    End Sub

    Private Sub Browse_Click(sender As Object, e As EventArgs) Handles Browse.Click
        Dim COPF As New CommonOpenFileDialog
        COPF.InitialDirectory = "C:\\Users"
        COPF.IsFolderPicker = True
        If COPF.ShowDialog = CommonFileDialogResult.Ok Then
            SavePath.Text = COPF.FileName
        End If
    End Sub

    Private Sub BrowseFile_Click(sender As Object, e As EventArgs) Handles BrowseFile.Click
        Dim COPF As New CommonOpenFileDialog
        COPF.InitialDirectory = "C:\\Users"
        COPF.IsFolderPicker = False
        COPF.EnsureFileExists = True
        COPF.Filters.Add(New CommonFileDialogFilter("HTML file", ".html"))
        If COPF.ShowDialog = CommonFileDialogResult.Ok Then
            HtmlPath.Text = COPF.FileName
        End If

    End Sub


End Class

