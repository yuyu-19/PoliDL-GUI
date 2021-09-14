using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32.TaskScheduler;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace PoliDLGUI.Forms
{
    public partial class GenerateDataForm
    {
        public GenerateDataForm()
        {
            InitializeComponent();
            _Generate.Name = "Generate";
            _Browse.Name = "Browse";
            _BrowseFile.Name = "BrowseFile";
        }

        private readonly List<StartupForm.CourseData> Courses = new List<StartupForm.CourseData>();
        private readonly bool IsItalian = Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "it-IT";

        private void GenerateDataForm_Load(object sender, EventArgs e)
        {
            using (var ts = new TaskService())
            {
                var ToBeDeleted = new List<object>();
                var OneShotsFound = new Dictionary<string, List<StartupForm.DayData>>();
                Predicate<Task> pred = (Task t) => this.IsWebExOS(t);
                foreach (Task t in ts.RootFolder.EnumerateTasks(pred, true))
                {
                    string tempstring = t.Name.Replace("WebExRec-OS-", "");
                    string courseID = tempstring.Substring(0, tempstring.IndexOf("-"));
                    if (!OneShotsFound.TryGetValue(courseID, out List<StartupForm.DayData> ListOfOneShots))
                    {
                        ListOfOneShots = new List<StartupForm.DayData>();
                        OneShotsFound.Add(courseID, ListOfOneShots);
                    }

                    var NewOneShot = new StartupForm.DayData() { TempDisabled = false };
                    var trigger = t.Definition.Triggers[0];
                    if (DateTime.Now > trigger.EndBoundary)
                    {
                        ToBeDeleted.Add(t);
                    }
                    else
                    {
                        NewOneShot.StartTime = trigger.StartBoundary.ToString("HH:mm");
                        NewOneShot.EndTime = trigger.EndBoundary.ToString("HH:mm");
                        NewOneShot.DayName = trigger.StartBoundary.ToString("dd/MM/yyyy");
                        ExecAction x = (ExecAction)t.Definition.Actions[0];
                        NewOneShot.WebExLink = x.Arguments.Substring(x.Arguments.IndexOf("https://"));
                        OneShotsFound[courseID].Add(NewOneShot);
                    }
                }

                foreach (Task t in ToBeDeleted)
                    ts.RootFolder.DeleteTask(t.Name);    // Delete all tasks that have no triggers left
                ToBeDeleted.Clear();

                // If sender.name = "StartTimePicker" Then
                // If DTP.Value.TimeOfDay > max Then DTP.Value = DTP.Value.Date + max
                // Courses(CourseIndex).OneShots.Last.StartTime = DTP.Value.ToString("HH:mm")
                // ElseIf sender.name = "EndTimePicker" Then
                // If DTP.Value.TimeOfDay > max Then DTP.Value = DTP.Value.Date + max
                // Courses(CourseIndex).OneShots.Last.EndTime = DTP.Value.ToString("HH:mm")
                // ElseIf sender.name = "DayPicker" Then
                // Courses(CourseIndex).OneShots.Last.DayName = DTP.Value.ToString("dd/MM/yyyy")
                // End If

                pred = (Task t) => this.IsWebEx(t);
                foreach (Task t in ts.RootFolder.EnumerateTasks(pred, true))
                {
                    // We're going to fill up the Courses list here, and empty it later if there's actually no tasks. I'd like to avoid iterating through them all twice.

                    var NewCourse = new StartupForm.CourseData() { ID = Conversions.ToInteger(t.Name.Replace("WebExRec-", "")) };
                    if (OneShotsFound.ContainsKey(NewCourse.ID.ToString()))
                    {
                        NewCourse.OneShots = OneShotsFound[NewCourse.ID.ToString()];
                    }

                    NewCourse.Name = t.Definition.RegistrationInfo.Description.Replace("REC: ", "");
                    var lines = File.ReadAllLines(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + NewCourse.ID + @"\LinkSchedule.txt");
                    if (t.Definition.Triggers.Count != 0)
                    {
                    }

                    foreach (var trigger in t.Definition.Triggers)
                    {
                        if (DateTime.Now > trigger.EndBoundary)
                        {
                            ToBeDeleted.Add(trigger);
                        }
                        else
                        {
                            if (NewCourse.EndDate is null)
                                NewCourse.EndDate = trigger.EndBoundary.ToString("dd/MM/yyyy");
                            if (NewCourse.StartDate is null)
                                NewCourse.StartDate = trigger.StartBoundary.ToString("dd/MM/yyyy");
                            if (trigger is not WeeklyTrigger WT)
                                continue;           // Not a wek
                            var NewDay = new StartupForm.DayData() { DayName = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), WT.DaysOfWeek.ToString())) };
                            // This looks long, complicated and stupid, but it's necessary because WT.DaysOfWeek returns a "DaysOfTheWeek" enum, whereas GetDayName requires a DayOfWeek enum. Simple, amirite?

                            NewDay.DayName = NewDay.DayName.Replace(NewDay.DayName[0], char.ToUpper(NewDay.DayName[0])); // Complicated? Yes. But functional.
                            NewDay.StartTime = trigger.StartBoundary.ToString("HH:mm");
                            NewDay.EndTime = trigger.EndBoundary.ToString("HH:mm");

                            // I could've probably just gotten all the day data from the linkschedule.txt file.
                            // I'm dumb. Whatever, I would've had to do this anyways to check for and remove outdated triggers.

                            foreach (var line in lines)
                            {
                                var data = line.Split(',');
                                if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(Operators.ConditionalCompareObjectEqual(data[0], Enum.Parse(typeof(DayOfWeek), WT.DaysOfWeek.ToString()), false), (data[1] ?? "") == (NewDay.StartTime ?? "")), (data[2] ?? "") == (NewDay.EndTime ?? ""))))
                                {
                                    NewDay.WebExLink = data[3];
                                    NewDay.TempDisabled = Conversions.ToBoolean(data[4]);
                                }
                            }

                            NewCourse.Days.Add(NewDay);
                        }
                    }

                    foreach (Trigger tr in ToBeDeleted)
                        t.Definition.Triggers.Remove(tr);
                    ToBeDeleted.Clear();
                    var NewProfessors = new Dictionary<string, string>();
                    foreach (var line in File.ReadAllLines(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + NewCourse.ID + @"\ProfessorLinks.txt"))
                        NewProfessors.Add(line.Substring(0, line.IndexOf(",")), line.Substring(line.IndexOf(",") + 1));
                    NewCourse.Professors = NewProfessors;
                    if (t.Definition.Triggers.Count == 0)
                    {
                        ToBeDeleted.Add(t);
                    }
                    else
                    {
                        Courses.Add(NewCourse);
                    }  // If this task SHOULDN'T be deleted, add its course to the list

                    ToBeDeleted.Clear();
                }

                foreach (Task t in ToBeDeleted)
                    ts.RootFolder.DeleteTask(t.Name);    // Delete all tasks that have no triggers left
                if (ts.RootFolder.EnumerateTasks(pred, true).Count() == 0) // A webexrec task is present (we're checking this AFTER we've cleared out all the old ones)
                {
                    Courses.Clear(); // There's no tasks present, so let's reset and keep going.
                }
                else
                {
                    Tag = "edit";
                    var f = CreateCourseForm();
                    Hide();
                    f.ShowDialog(this);
                    Close();
                }
            }

            if (IsItalian)
            {
                info.Text = "Per favore inserire la locazione del file HTML.";
                Generate.Text = "Genera";
                SavePathTitle.Text = "Cartella registazioni";
                Browse.Text = "Esplora";
                BrowseFile.Text = "Esplora";
            }
        }

        public bool IsWebEx(Task t)
        {
            return t.Name.Contains("WebExRec-") & !t.Name.Contains("WebExRec-OS-");
        }

        public bool IsWebExOS(Task t)
        {
            return t.Name.Contains("WebExRec-OS-");
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            if (IsItalian)
            {
                if (string.IsNullOrEmpty(SavePath.Text))
                {
                    MessageBox.Show("Per favore seleziona la cartella in cui salvare le registrazioni.");
                    return;
                }
                else if (string.IsNullOrEmpty(HtmlPath.Text))
                {
                    MessageBox.Show("Per favore inserisci la posizione del file HTML.");
                    return;
                }
            }
            else if (string.IsNullOrEmpty(SavePath.Text))
            {
                MessageBox.Show("Please select the folder to save the recordings in.");
                return;
            }
            else if (string.IsNullOrEmpty(HtmlPath.Text))
            {
                MessageBox.Show("Please input the path to the HTML file.");
                return;
            }

            string teachermark, semestermark, lessonsstartmark, lessonsendmark, frommark, tomark, virtualclassroommark;
            var DayNames = new List<string>();
            var DayNamesEN = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }; // Yeah, making a duplicate sounds stupid, but I always need the english name
            if (File.ReadAllText(HtmlPath.Text).Contains("Docente:"))    // We don't care about the OS language here, we only care about the HTML file's.
            {
                teachermark = "Docente:";
                semestermark = "Semestre:";
                lessonsstartmark = "Inizio lezioni:";
                lessonsendmark = "Fine lezioni:";
                frommark = "dalle";
                tomark = " alle";
                DayNames = new[] { "Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato" }.ToList();
                virtualclassroommark = "Aula virtuale - ";
                var obsconfig = File.ReadAllLines(StartupForm.RootFolder + @"\OBS\config\obs-studio\global.ini").ToList(); // Set the OBS language to italian
                foreach (string line in obsconfig)
                {
                    if (line.Contains("[BASIC]"))
                    {
                        obsconfig.Insert(obsconfig.IndexOf(line) - 1, "Language=it-IT");
                        break;
                    }
                    else if (line.Contains("Language="))
                    {
                        break;
                    }
                }

                File.WriteAllLines(StartupForm.RootFolder + @"\OBS\config\obs-studio\global.ini", obsconfig.ToArray());
            }
            else
            {
                teachermark = "Professor:";
                semestermark = "Semester:";
                lessonsstartmark = "Start of lessons:";
                lessonsendmark = "End of lesson:";
                frommark = "DALLE";
                tomark = " ALLE";
                DayNames = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }.ToList();
                virtualclassroommark = "Virtual Classroom - ";
            }

            var Lines = File.ReadAllLines(HtmlPath.Text, System.Text.Encoding.Default).ToList();
            int startindex;
            int LastCourseIndex = 0;
            foreach (var Line in Lines)
            {
                if (Line.Contains(teachermark))
                {
                    Courses.Add(new StartupForm.CourseData());
                    string CourseLine = Lines[Lines.IndexOf(Line, LastCourseIndex) - 2];
                    LastCourseIndex = Lines.IndexOf(Line, LastCourseIndex) + 1;
                    startindex = CourseLine.IndexOf("<b>") + 3;
                    Courses.Last().ID = Conversions.ToInteger(CourseLine.Substring(startindex, CourseLine.IndexOf("-", startindex) - startindex).Trim());
                    startindex = CourseLine.IndexOf("-", startindex) + 1;
                    Courses.Last().Name = CourseLine.Substring(startindex, CourseLine.IndexOf("</b>", startindex) - startindex).Trim();
                    var r = new Regex(@"[^a-zA-Z0-9 _\-]");
                    Courses.Last().Name = r.Replace(Courses.Last().Name, "");
                }
                else if (Line.Contains(lessonsstartmark))
                {
                    startindex = Line.IndexOf("</strong>") + "</strong>".Length;
                    Courses.Last().StartDate = Line.Substring(startindex).Trim();
                }
                else if (Line.Contains(lessonsendmark))
                {
                    startindex = Line.IndexOf("</strong>") + "</strong>".Length;
                    Courses.Last().EndDate = Line.Substring(startindex).Trim();
                }
                else if (Line.Contains(virtualclassroommark))
                {
                    if (!Line.Contains("https://politecnicomilano.webex.com"))
                    {
                        continue; // It's a duplicate. Ignore it, since it doens't even contain the link.
                    }

                    startindex = Line.IndexOf(virtualclassroommark) + virtualclassroommark.Length;
                    string profname = Line.Substring(startindex).Replace("</a>", "").Trim();
                    startindex = Line.IndexOf("<a href=\"") + "<a href=\"".Length;
                    string webexlink = Line.Substring(startindex, Line.IndexOf("?") - startindex);
                    Courses.Last().Professors.Add(profname, webexlink);
                }
                else if (Line.Contains("<ul style=\"margin-top: 0px; margin-bottom: 0px;"))     // Assume that line contains day data, since that's the only unhandled type
                {
                    int i = 0;
                    while (Line.IndexOf(":5px;\">", i) != -1)
                    {
                        i = Line.IndexOf(":5px;\">", i) + 7;
                        var NewDay = new StartupForm.DayData()
                        {
                            TempDisabled = false,
                            DayName = Line.Substring(i, Line.IndexOf(frommark, i) - i).Trim()
                        };
                        i = Line.IndexOf(frommark, i) + frommark.Length;
                        NewDay.StartTime = Line.Substring(i, Line.IndexOf(tomark, i) - i).Trim();
                        i = Line.IndexOf(tomark, i) + tomark.Length;
                        NewDay.EndTime = Line.Substring(i, Line.IndexOf(",", i) - i).Trim();
                        if (Courses.Last().Days.Count == 0)
                        {
                            Courses.Last().Days.Add(NewDay);
                        }
                        else
                        {
                            foreach (var day in Courses.Last().Days)       // This checks if the day is a duplicate. If it is, don't add it. This is to handle the case with multiple groups on the same date.
                            {
                                if ((NewDay.DayName ?? "") == (day.DayName ?? "") & (NewDay.StartTime ?? "") == (day.StartTime ?? "") & (NewDay.EndTime ?? "") == (day.EndTime ?? ""))
                                {
                                    break;
                                }
                                else if (ReferenceEquals(day, Courses.Last().Days.Last()))
                                {
                                    Courses.Last().Days.Add(NewDay);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Debug coursedata printout.

            // Dim asda As String

            // For Each course In Courses
            // asda = course.Name & "-" & course.ID
            // For Each professor In course.Professors
            // asda = asda & vbCrLf & professor.Key & " - " & professor.Value
            // Next

            // For Each day In course.Days
            // asda = asda & vbCrLf & day.DayName & "-" & day.StartTime & "-" & day.EndTime
            // Next
            // MessageBox.Show(asda)
            // Next

            // Now we ask the user to assign a name to every day, of every course.

            var f = CreateCourseForm();
            do
            {
                f.ShowDialog(this);
                foreach (var Course in Courses)
                {
                    foreach (var day in Course.Days)
                    {
                        if (string.IsNullOrEmpty(day.WebExLink))
                        {
                            if (IsItalian)
                            {
                                MessageBox.Show("Per favore riempi tutte le date.");
                            }
                            else
                            {
                                MessageBox.Show("Please fill in all the dates.");
                            }

                            continue;  // If there's an empty link, restart
                        }
                    }
                }

                break;
            }
            while (true);

            // Now we have all the CourseData objects in Courses, and they're all set up correctly. We just iterate through them and set up the tasks

            using (var ts = new TaskService())
            {
                foreach (var Course in Courses)
                {
                    var CourseTD = ts.NewTask();
                    CourseTD.RegistrationInfo.Description = "REC: " + Course.Name;
                    if (File.Exists(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID + @"\LinkSchedule.txt"))
                        File.Delete(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID + @"\LinkSchedule.txt");
                    if (File.Exists(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID + @"\ProfessorLinks.txt"))
                        File.Delete(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID + @"\ProfessorLinks.txt");
                    string fourAMbodgebelike = "";
                    foreach (var Professor in Course.Professors)
                        fourAMbodgebelike = fourAMbodgebelike + Professor.Key + "," + Professor.Value + Constants.vbCrLf;
                    fourAMbodgebelike = fourAMbodgebelike.Substring(0, fourAMbodgebelike.Length - Constants.vbCrLf.Length);
                    Directory.CreateDirectory(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID);
                    File.WriteAllText(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID + @"\ProfessorLinks.txt", fourAMbodgebelike);
                    fourAMbodgebelike = "";
                    foreach (var day in Course.Days)
                    {
                        fourAMbodgebelike = fourAMbodgebelike + new List<string>().IndexOf(day.DayName) + "," + day.StartTime + "," + day.EndTime + "," + day.WebExLink + ",0" + Constants.vbCrLf;
                        // Write all the links with the day/time in the corresponding folder

                        // There's probably a billion better ways to do this but it's 3am and I'm tired
                        // ERROR HERE???
                        var WT = new WeeklyTrigger()
                        {
                            DaysOfWeek = (DaysOfTheWeek)Enum.Parse(typeof(DaysOfTheWeek), DayNamesEN[new List<string>().IndexOf(day.DayName)]), // This looks stupid, but it at least works regardless of langauge
                            StartBoundary = DateTime.ParseExact(Course.StartDate + " " + day.StartTime + ":59", "dd/MM/yyyy HH:mm:ss", Thread.CurrentThread.CurrentCulture),
                            EndBoundary = DateTime.ParseExact(Course.EndDate + " " + day.EndTime, "dd/MM/yyyy HH:mm", Thread.CurrentThread.CurrentCulture),
                            WeeksInterval = 1
                        };
                        CourseTD.Triggers.Add(WT);
                    }

                    File.WriteAllText(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID + @"\LinkSchedule.txt", fourAMbodgebelike);
                    CourseTD.Actions.Add(new ExecAction(StartupForm.RootFolder + "StartRec.exe", Course.ID.ToString(), StartupForm.RootFolder));

                    // REMOVE THE URL ARGUMENT. COMPLETELY USELESS. WRITE THEM ALL TO A FILE WITH THE TIMES AND DATES, AND HAVE THE AHK SCRIPT FIGURE OUT WHICH ONE IT NEEDS
                    // NEED TO IMPLEMENT BEING ABLE TO READ THEM AGAIN AS WELL

                    ts.RootFolder.RegisterTaskDefinition("WebExRec-" + Course.ID, CourseTD);

                    // Generate OBS profile

                    Directory.CreateDirectory(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID);
                    File.Copy(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\template\streamEncoder.json", StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID + @"\streamEncoder.json", true);
                    string text = File.ReadAllText(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\template\basic.ini");
                    text = text.Replace("[SCREENRESX]", Screen.PrimaryScreen.Bounds.Width.ToString());
                    text = text.Replace("[SCREENRESY]", Screen.PrimaryScreen.Bounds.Height.ToString());
                    text = text.Replace("[PATH]", SavePath.Text + @"\" + Course.Name + "-" + Course.ID);
                    text = text.Replace("[SERVERPORT]", (Course.ID % 65534).ToString());
                    text = text.Replace("template", Course.ID.ToString());
                    Directory.CreateDirectory(SavePath.Text + @"\" + Course.Name + "-" + Course.ID);
                    // This saves the current template file, for comparing later.

                    File.WriteAllText(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Course.ID + @"\basic.ini", text);
                    // File.WriteAllLines(StartupForm.RootFolder & "\OBS\config\obs-studio\basic\profiles\" & Course.ID & "\links.txt", Course.WebExLinks)
                    // OBS profile generation complete
                }

                File.Copy(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\template\basic.ini", StartupForm.RootFolder + @"\OBS\config\obs-studio\currenttemplate.ini", true);
            }

            if (IsItalian)
            {
                MessageBox.Show("Tutte le registrazioni sono state impostate.");
            }
            else
            {
                MessageBox.Show("All recordings have been scheduled.");
            }

            Application.Exit();
        }

        public Form CreateCourseForm()
        {
            var f = new Form();
            var P = new Point();
            f.Width = 0;
            foreach (var course in Courses)
            {
                P.X = 15;
                P.Y = 35 * Courses.IndexOf(course) + 10;
                var RTB = new RichTextBox()
                {
                    ReadOnly = true,
                    AutoSize = true,
                    BorderStyle = BorderStyle.None,
                    Multiline = false,
                    Text = course.Name
                };
                var CFont = new Font(RTB.Font.FontFamily, 9f, RTB.Font.Style);
                RTB.Font = CFont;
                var size = TextRenderer.MeasureText(course.Name, CFont);
                RTB.Width = size.Width;
                RTB.Height = size.Height;
                RTB.Location = P;
                f.Controls.Add(RTB);
                var EB = new Button();
                if (IsItalian)
                {
                    EB.Text = "Modifica";
                }
                else
                {
                    EB.Text = "edit";
                }

                EB.Name = "edit";
                EB.Tag = Courses.IndexOf(course);
                EB.AutoSize = true;
                f.Controls.Add(EB);
                P.X = P.X + RTB.Width + 10;
                EB.Location = P;
                if (f.Width < P.X + EB.Width + 30)
                    f.Width = P.X + EB.Width + 30;
                EB.Click += Edit_Click;
            }

            int max = 0;
            foreach (Control CTRL in f.Controls)
            {
                if (CTRL is RichTextBox && CTRL.Width > max)
                    max = CTRL.Width;       // Get the max width
            }

            var Done = new Button();
            if (IsItalian)
            {
                Done.Text = "Fine";
            }
            else
            {
                Done.Text = "Done";
            }

            Done.Name = "Done";
            Done.Tag = "Done";
            foreach (Control CTRL in f.Controls)
            {
                if (CTRL is RichTextBox)
                    CTRL.Width = max;
                if (CTRL is Button)
                {
                    P = CTRL.Location;
                    P.X = 15 + max + 10;
                    CTRL.Location = P;
                    Done.Size = CTRL.Size;
                }
            }

            f.Height = 35 * Courses.Count + 45 + Done.Height + 10;
            P.X = f.ClientSize.Width - 15 - Done.Width;
            P.Y = f.ClientSize.Height - 5 - Done.Height;
            Done.Location = P;
            f.Controls.Add(Done);
            Done.Click += (_, __) => f.Close();
            return f;
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            StartupForm.CourseData CurrentCourse = null;
            if (sender is not Button button)
            {
                return;
            }

            CurrentCourse = Courses[Convert.ToInt32(button.Tag)];

            Button b = (Button)sender;
            bool IsEdit = Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Tag, "edit", false));
            var p = default(Point);
            int HolyShitImSoFuckingDoneWithThis = 0;
            var f = new Form()
            {
                Width = 0,
                Text = "Schedule for " + CurrentCourse.Name
            };
            if (IsItalian)
                f.Text = "Programma di " + CurrentCourse.Name;
            f.Tag = Courses.IndexOf(CurrentCourse);
            foreach (var Day in CurrentCourse.Days)
            {
                p.X = 15;
                p.Y = 35 * CurrentCourse.Days.IndexOf(Day) + 10;
                var RTB = new RichTextBox()
                {
                    ReadOnly = true,
                    AutoSize = true,
                    BorderStyle = BorderStyle.None,
                    Multiline = false,
                    Text = Day.DayName + ":" + Day.StartTime + "-" + Day.EndTime
                };
                var CFont = new Font(RTB.Font.FontFamily, 9f, RTB.Font.Style);
                RTB.Font = CFont;
                var size = TextRenderer.MeasureText(RTB.Text, CFont);
                RTB.Width = size.Width;
                RTB.Height = size.Height;
                RTB.Tag = CurrentCourse.Days.IndexOf(Day);
                RTB.Location = p;
                f.Controls.Add(RTB);
                var CB = new ComboBox();
                string LongestItem = "";
                foreach (var Professor in CurrentCourse.Professors)
                {
                    CB.Items.Add(Professor.Key);
                    if (Professor.Key.Length > LongestItem.Length)
                        LongestItem = Professor.Key;
                }

                CB.Font = CFont;
                CB.Text = "";
                size = TextRenderer.MeasureText(LongestItem, CFont);
                CB.Width = size.Width;
                CB.Height = size.Height;
                CB.AllowDrop = false;
                CB.ImeMode = ImeMode.NoControl;
                CB.Tag = CurrentCourse.Days.IndexOf(Day);
                CB.Text = "";
                if (!string.IsNullOrEmpty(Day.WebExLink))
                {
                    foreach (var pair in CurrentCourse.Professors)
                    {
                        if ((pair.Value ?? "") == (Day.WebExLink ?? ""))
                            CB.Text = pair.Key;
                    }
                }

                f.Controls.Add(CB);
                p.X = p.X + RTB.Width + 10;
                CB.Location = p;
                CB.TextChanged += ComboBox_ChangeElement;
                var DB = new Button() { Text = "Delete" };
                if (IsItalian)
                    DB.Text = "Elimina";
                DB.Name = "Delete";
                DB.Tag = CurrentCourse.Days.IndexOf(Day);
                DB.AutoSize = true;
                f.Controls.Add(DB);
                p.X = p.X + CB.Width + 10;
                DB.Location = p;
                DB.Click += Delete_Click;
                if (IsEdit)
                {
                    var CkBox = new CheckBox()
                    {
                        Checked = Day.TempDisabled,
                        Enabled = true,
                        Name = "TempDisable",
                        AutoSize = true,
                        Tag = CurrentCourse.Days.IndexOf(Day)
                    };
                    if (IsItalian)
                    {
                        CkBox.Text = "Disabilita una volta";
                    }
                    else
                    {
                        CkBox.Text = "Disable once";
                    }

                    f.Controls.Add(CkBox);
                    HolyShitImSoFuckingDoneWithThis = DB.Width;
                    p.X = p.X + DB.Width + 10;
                    p.Y = (int)Math.Round(p.Y + (CB.Height - CkBox.Height) / 2d);
                    CkBox.Location = p;
                    p.Y = (int)Math.Round(p.Y - (CB.Height - CkBox.Height) / 2d);
                    if (f.Width < p.X + CkBox.Width + 30)
                        f.Width = p.X + CkBox.Width + 30;
                    CkBox.CheckedChanged += Checkbox_Changed;
                }
                else if (f.Width < p.X + DB.Width + 30)
                    f.Width = p.X + DB.Width + 30;
            }

            f.Height = 35 * CurrentCourse.Days.Count + 45;
            int max1 = default, max2 = default;                        // Probably not the best solution to make everything the same size, and probably wasteful, but I'm just lazy and bad
            foreach (Control CTRL in f.Controls)
            {
                if (CTRL is RichTextBox && CTRL.Width > max1)
                    max1 = CTRL.Width;
                if (CTRL is ComboBox && CTRL.Width > max2)
                    max2 = CTRL.Width;
            }

            foreach (Control CTRL in f.Controls)
            {
                if (CTRL is RichTextBox)
                    CTRL.Width = max1;
                if (CTRL is ComboBox)
                {
                    CTRL.Width = max2 + 17;            // Compensating for the arrow
                    p = CTRL.Location;
                    p.X = 15 + max1 + 10;
                    CTRL.Location = p;
                }
            }

            var Iamahackandjustwantthistobeover = default(Point);
            foreach (Control CTRL in f.Controls)
            {
                if (CTRL is Button)
                {
                    p = CTRL.Location;
                    p.X = 15 + max1 + 10 + max2 + 17 + 10;
                    CTRL.Location = p;
                    f.Width = 15 + max1 + 10 + max2 + 10 + CTRL.Width + 47;
                    Iamahackandjustwantthistobeover = p;
                }
                else if (CTRL is CheckBox)
                {
                    p = CTRL.Location;
                    p.X = 15 + max1 + 10 + max2 + 17 + HolyShitImSoFuckingDoneWithThis + 20;
                    CTRL.Location = p;
                    f.Width = 15 + max1 + 10 + max2 + 10 + HolyShitImSoFuckingDoneWithThis + CTRL.Width + 47;
                }
            }

            // Add a OneShots As List(of DayData) property to courseData.
            // Fill that in by checking for tasks with WebExRec-OS- (additional check in the existing sub)
            // If a oneshot recording is marked as completed (the current date is beyond the endboundary) then delete it
            // Oneshot's dayname contains the full date, instead of just the weekday

            if (IsEdit)
            {
                var AddOneShot = new Button() { AutoSize = true };
                if (IsItalian)
                {
                    AddOneShot.Text = "Aggiungi registrazione temp.";
                }
                else
                {
                    AddOneShot.Text = "Add one-time recording";
                }

                f.Height = f.Height + AddOneShot.Height + 5;
                f.Controls.Add(AddOneShot);
                p.X = 15;
                p.Y = f.ClientSize.Height - AddOneShot.Height - 5;
                AddOneShot.Location = p;
                AddOneShot.Click += OneShot_Click;

                // To create one, add an element to the oneshots list, and fill it all in
                // Then create the task (figuring out the endboundary is gonna be a pain in the ass)
                // I don't need to touch the link file. Just do it via tasks.
            }

            var Done = new Button();
            if (IsItalian)
            {
                Done.Text = "Fine";
            }
            else
            {
                Done.Text = "Done";
            }

            Done.Name = "Done";
            Done.Tag = "Done";
            Done.AutoSize = true;
            if (!IsEdit)
            {
                f.Height = f.Height + Done.Height + 5;
            }

            Iamahackandjustwantthistobeover.Y = f.ClientSize.Height - 5 - Done.Height;
            Done.Location = Iamahackandjustwantthistobeover;
            f.Controls.Add(Done);
            Done.Click += (_, __) => f.Close();
            if (IsEdit)
            {
                foreach (var OneShot in CurrentCourse.OneShots)
                {
                    f.Height += 35;
                    p = new Point() { X = 15 };
                    RichTextBox RTB = new RichTextBox()
                    {
                        ReadOnly = true,
                        AutoSize = true,
                        BorderStyle = BorderStyle.None,
                        Multiline = false
                    }, RTB2 = new RichTextBox()
                    {
                        ReadOnly = true,
                        AutoSize = true,
                        BorderStyle = BorderStyle.None,
                        Multiline = false
                    };
                    RTB.Text = OneShot.DayName + ":" + OneShot.StartTime + "-" + OneShot.EndTime;
                    var CFont = new Font(RTB.Font.FontFamily, 9f, RTB.Font.Style);
                    RTB.Font = CFont;
                    RTB.Size = TextRenderer.MeasureText(RTB.Text, CFont);
                    p.Y = f.ClientSize.Height - 10 - RTB.Height;
                    RTB.Location = p;
                    f.Controls.Add(RTB);
                    // Gotta add the oneshots/whatever
                    // BOOKMARK
                    RTB2.Font = CFont;
                    foreach (var professor in CurrentCourse.Professors)
                    {
                        if ((professor.Value ?? "") == (OneShot.WebExLink ?? ""))
                        {
                            RTB2.Text = professor.Key;
                        }
                    }

                    RTB2.Size = TextRenderer.MeasureText(RTB2.Text, CFont);
                    p.X = Done.Location.X + Done.Width - RTB2.Width + 4;
                    f.Controls.Add(RTB2);
                    RTB2.Location = p;
                }
            }

            f.ShowDialog(b.Parent);
        }

        private void OneShot_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            int courseindex = Conversions.ToInteger(b.Parent.Tag);
            var currentcourse = Courses[courseindex];
            var NewOneShot = new StartupForm.DayData() { TempDisabled = false };
            Courses[courseindex].OneShots.Add(NewOneShot);
            var f = new Form() { Tag = courseindex };
            var DayPicker = new DateTimePicker();
            var CFont = new Font(DayPicker.Font.FontFamily, 9f, DayPicker.Font.Style);
            DayPicker.Format = DateTimePickerFormat.Short;
            DayPicker.Font = CFont;
            var p = new Point(5, 5);
            f.Controls.Add(DayPicker);
            DayPicker.Size = TextRenderer.MeasureText("99/99/9999", CFont);
            DayPicker.Width += 15;
            DayPicker.Location = p;
            DayPicker.Name = "DayPicker";
            DayPicker.ValueChanged += DTP_ChangeElement;
            DTP_ChangeElement(DayPicker, null);
            var StartTimePicker = new DateTimePicker()
            {
                CustomFormat = "HH:mm",
                Format = DateTimePickerFormat.Custom,
                ShowUpDown = true,
                Font = CFont
            };
            p.X = p.X + DayPicker.Width + 5;
            f.Controls.Add(StartTimePicker);
            StartTimePicker.Size = TextRenderer.MeasureText("99:99", CFont);
            StartTimePicker.Width += 15;
            StartTimePicker.Location = p;
            StartTimePicker.Name = "StartTimePicker";
            StartTimePicker.ValueChanged += DTP_ChangeElement;
            DTP_ChangeElement(StartTimePicker, null);
            var EndTimePicker = new DateTimePicker()
            {
                CustomFormat = "HH:mm",
                Format = DateTimePickerFormat.Custom,
                ShowUpDown = true,
                Font = CFont
            };
            p.X = p.X + StartTimePicker.Width + 5;
            f.Controls.Add(EndTimePicker);
            EndTimePicker.Size = TextRenderer.MeasureText("99:99", CFont);
            EndTimePicker.Width += 15;
            EndTimePicker.Location = p;
            EndTimePicker.Name = "EndTimePicker";
            EndTimePicker.ValueChanged += DTP_ChangeElement;
            DTP_ChangeElement(EndTimePicker, null);
            var CBprof = new ComboBox() { Name = "CBProf" };
            string LongestItem = "";
            foreach (var Professor in Courses[courseindex].Professors)
            {
                CBprof.Items.Add(Professor.Key);
                if (Professor.Key.Length > LongestItem.Length)
                    LongestItem = Professor.Key;
            }

            CBprof.Font = CFont;
            CBprof.Text = "";
            CBprof.Width = Size.Width;
            CBprof.Height = Size.Height;
            CBprof.AllowDrop = false;
            CBprof.ImeMode = ImeMode.NoControl;
            p.X = p.X + StartTimePicker.Width + 5;
            f.Controls.Add(CBprof);
            CBprof.Size = TextRenderer.MeasureText(LongestItem, CFont);
            CBprof.Width += 17;
            CBprof.Location = p;
            f.Width = 5 + DayPicker.Width + 5 + StartTimePicker.Width + 5 + EndTimePicker.Width + 5 + CBprof.Width + 5 + 17;
            CBprof.TextChanged += CBprofOS_ChangeElement;
            var Done = new Button();
            if (IsItalian)
            {
                Done.Text = "Fine";
            }
            else
            {
                Done.Text = "Done";
            }

            Done.Name = "Done";
            Done.Tag = "Done";
            Done.AutoSize = true;
            f.Controls.Add(Done);
            f.Height = 5 + CBprof.Height + 5 + 35;
            p.X = f.ClientSize.Width - 5 - Done.Width;
            p.Y = CBprof.Location.Y + CBprof.Height + 5;
            Done.Location = p;
            f.Height = 5 + CBprof.Height + 5 + Done.Height + 5 + 35;
            Done.Click += OSDone_Click;
            f.ShowDialog(b.Parent);

            // UHHHH. Need to figure out how to add/display the oneshots in the previous window.
        }

        private void OSDone_Click(object sender, EventArgs e)
        {
            Form OSForm = ((Form)sender).ParentForm;
            ComboBox CBprof = (ComboBox)OSForm.Controls.Find("CBProf", false)[0];
            var ConfigForm = OSForm.Owner;
            int courseindex = Conversions.ToInteger(ConfigForm.Tag);
            var currentcourse = Courses[courseindex];
            if (string.IsNullOrEmpty(CBprof.Text))
            {
                if (IsItalian)
                {
                    MessageBox.Show("Per favore seleziona il professore");
                }
                else
                {
                    MessageBox.Show("Please choose the professor");
                }
            }
            else
            {
                try
                {
                    using (var ts = new TaskService())
                    {
                        var td = ts.NewTask();
                        td.RegistrationInfo.Description = "Oneshot recording for " + currentcourse.ID;
                        var T = new TimeTrigger()
                        {
                            StartBoundary = DateTime.ParseExact(currentcourse.OneShots.Last().DayName + " " + currentcourse.OneShots.Last().StartTime + ":59", "dd/MM/yyyy HH:mm:ss", Thread.CurrentThread.CurrentCulture),
                            EndBoundary = DateTime.ParseExact(currentcourse.OneShots.Last().DayName + " " + currentcourse.OneShots.Last().EndTime, "dd/MM/yyyy HH:mm", Thread.CurrentThread.CurrentCulture)
                        };
                        if (T.EndBoundary <= T.StartBoundary)
                        {
                            if (IsItalian)
                            {
                                MessageBox.Show("L'orario di fine deve essere dopo quello d'inizio.");
                            }
                            else
                            {
                                MessageBox.Show("The end time must be after the start time.");
                            }

                            return;
                        }

                        td.Triggers.Add(T);
                        td.Actions.Add(new ExecAction(StartupForm.RootFolder + @"\StartRec.exe", currentcourse.ID + " " + currentcourse.OneShots.Last().WebExLink, StartupForm.RootFolder));
                        bool pred(Task t) => this.IsWebExOS(t);
                        int taskcount = ts.RootFolder.EnumerateTasks(pred, true).Count();
                        ts.RootFolder.RegisterTaskDefinition("WebExRec-OS-" + currentcourse.ID + "-" + taskcount, td);
                        ConfigForm.Height = OSForm.Owner.Height + 35;
                        var p = default(Point);
                        p.X = 15;
                        RichTextBox RTB = new RichTextBox()
                        {
                            ReadOnly = true,
                            AutoSize = true,
                            BorderStyle = BorderStyle.None,
                            Multiline = false
                        }, RTB2 = new RichTextBox()
                        {
                            ReadOnly = true,
                            AutoSize = true,
                            BorderStyle = BorderStyle.None,
                            Multiline = false
                        };
                        RTB.Text = currentcourse.OneShots.Last().DayName + ":" + currentcourse.OneShots.Last().StartTime + "-" + currentcourse.OneShots.Last().EndTime;
                        var CFont = new Font(RTB.Font.FontFamily, 9f, RTB.Font.Style);
                        RTB.Font = CFont;
                        RTB.Size = TextRenderer.MeasureText(RTB.Text, CFont);
                        p.Y = ConfigForm.ClientSize.Height - 10 - RTB.Height;
                        RTB.Location = p;
                        ConfigForm.Controls.Add(RTB);
                        // Gotta add the oneshots/whatever
                        // BOOKMARK
                        RTB2.Font = CFont;
                        RTB2.Text = CBprof.Text;
                        RTB2.Size = TextRenderer.MeasureText(CBprof.Text, CFont);
                        p.X = ConfigForm.ClientSize.Width - 5 - RTB2.Width;
                        ConfigForm.Controls.Add(RTB2);
                        RTB2.Location = p;
                    }

                    OSForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void DTP_ChangeElement(object sender, EventArgs e)
        {
            DateTimePicker DTP = (DateTimePicker)sender;
            int CourseIndex = Conversions.ToInteger(DTP.Parent.Tag);
            var max = new TimeSpan(23, 59, 59);
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(DTP.Name, "StartTimePicker", false)))
            {
                if (DTP.Value.TimeOfDay > max)
                    DTP.Value = DTP.Value.Date + max;
                Courses[CourseIndex].OneShots.Last().StartTime = DTP.Value.ToString("HH:mm");
            }
            else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(DTP.Name, "EndTimePicker", false)))
            {
                if (DTP.Value.TimeOfDay > max)
                    DTP.Value = DTP.Value.Date + max;
                Courses[CourseIndex].OneShots.Last().EndTime = DTP.Value.ToString("HH:mm");
            }
            else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(DTP.Name, "DayPicker", false)))
            {
                Courses[CourseIndex].OneShots.Last().DayName = DTP.Value.ToString("dd/MM/yyyy");
            }
        }

        private void CBprofOS_ChangeElement(object sender, EventArgs e)
        {
            ComboBox CB = (ComboBox)sender;
            int CourseIndex = Conversions.ToInteger(CB.Parent.Tag);
            Courses[CourseIndex].OneShots.Last().WebExLink = Courses[CourseIndex].Professors[CB.Text];
        }

        private void ComboBox_ChangeElement(object sender, EventArgs e)
        {
            ComboBox CB = (ComboBox)sender;
            bool IsEdit = Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Tag, "edit", false));
            int CourseIndex = Conversions.ToInteger(CB.Parent.Tag);
            int DayIndex = Conversions.ToInteger(CB.Tag);
            var CurrentCourse = Courses[CourseIndex];
            var CurrentDay = CurrentCourse.Days[DayIndex];
            if (IsEdit)  // Update the actual file if we're in edit mode
            {
                var DayNames = new[] { "Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato" }.ToList();
                // I'm sure all of this could be consensed down to like a single line of code if I knew how the fuck the datetime stuff works
                // but I'm too retarded/don't care enough about the efficency of this function that's used once in a blue moon

                if (!DayNames.Contains(CurrentDay.DayName))
                {
                    DayNames = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }.ToList();
                }

                var DayNamesEN = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                var lines = File.ReadAllLines(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Courses[CourseIndex].ID + @"\LinkSchedule.txt").ToList();
                foreach (var line in lines)
                {
                    var data = line.Split(',');
                    if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(Operators.AndObject(Operators.ConditionalCompareObjectEqual(Enum.Parse(typeof(DayOfWeek), DayNamesEN[DayNames.IndexOf(CurrentDay.DayName)]), data[0], false), (data[1] ?? "") == (CurrentDay.StartTime ?? "")), (data[2] ?? "") == (CurrentDay.EndTime ?? "")), (CurrentDay.WebExLink ?? "") == (data[3] ?? ""))))
                    {
                        lines[lines.IndexOf(line)] = line.Replace(data[2] + "," + data[3], data[2] + "," + CurrentCourse.Professors[CB.Text]);
                        break;
                    }
                }

                File.Delete(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Courses[CourseIndex].ID + @"\LinkSchedule.txt");
                File.WriteAllLines(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Courses[CourseIndex].ID + @"\LinkSchedule.txt", lines);
            }

            // MessageBox.Show("Set URL for course " & CurrentCourse.ID & " on " & CurrentDay.DayName & " from " & CurrentDay.StartTime & " to " & CurrentDay.EndTime & " to " & CurrentCourse.Professors.Item(CB.Text))
            Courses[CourseIndex].Days[DayIndex].WebExLink = CurrentCourse.Professors[CB.Text];
        }

        private void Checkbox_Changed(object sender, EventArgs e)
        {
            CheckBox CkBox = (CheckBox)sender;
            int CourseIndex = Conversions.ToInteger(CkBox.Parent.Tag);
            int DayIndex = Conversions.ToInteger(CkBox.Tag);
            Courses[CourseIndex].Days[DayIndex].TempDisabled = Conversions.ToBoolean(CkBox.CheckState);
            var CurrentDay = Courses[CourseIndex].Days[DayIndex];
            var DayNames = new[] { "Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato" }.ToList();
            // I'm sure all of this could be consensed down to like a single line of code if I knew how the fuck the datetime stuff works
            // but I'm too retarded/don't care enough about the efficency of this function that's used once in a blue moon

            if (!DayNames.Contains(CurrentDay.DayName))
            {
                DayNames = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }.ToList();
            }

            var DayNamesEN = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            var lines = File.ReadAllLines(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Courses[CourseIndex].ID + @"\LinkSchedule.txt").ToList();
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(Operators.AndObject(Operators.ConditionalCompareObjectEqual(Enum.Parse(typeof(DayOfWeek), DayNamesEN[DayNames.IndexOf(CurrentDay.DayName)]), data[0], false), (data[1] ?? "") == (CurrentDay.StartTime ?? "")), (data[2] ?? "") == (CurrentDay.EndTime ?? "")), (CurrentDay.WebExLink ?? "") == (data[3] ?? ""))))
                {
                    lines[lines.IndexOf(line)] = line.Replace(data[3] + "," + data[4], data[3] + "," + Convert.ToInt32(CurrentDay.TempDisabled));
                    break;
                }
            }

            File.Delete(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Courses[CourseIndex].ID + @"\LinkSchedule.txt");
            File.WriteAllLines(StartupForm.RootFolder + @"\OBS\config\obs-studio\basic\profiles\" + Courses[CourseIndex].ID + @"\LinkSchedule.txt", lines);
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Form f = (Form)b.Parent;
            Button DB = null;
            bool IsEdit = false;
            foreach (var control in f.Controls)
            {
                if (control is CheckBox CK)
                    IsEdit = true;
            }

            var ToDelete = new List<Control>();
            var ToBeLowered = new List<Control>();
            foreach (Control CTRL in f.Controls)
            {
                if (Information.IsNumeric(CTRL.Tag) && Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(CTRL.Tag, b.Tag, false)))    // If it's a day after the one getting delete'd
                {
                    var p = CTRL.Location;
                    p.Y -= 35;          // Move it up one slot
                    CTRL.Location = p;
                    ToBeLowered.Add(CTRL);
                }
                else if (Information.IsNumeric(CTRL.Tag) && Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(CTRL.Tag, b.Tag, false)))
                {
                    if (CTRL is ComboBox CB)
                        CB.TextChanged -= ComboBox_ChangeElement;
                    DB = CTRL as Button;
                    if (DB is object)
                        DB.Click -= Delete_Click;
                    ToDelete.Add(CTRL);
                }

                DB = CTRL as Button;
                if (DB is object && !Information.IsNumeric(DB.Tag))
                {
                    var p = DB.Location;
                    p.Y -= 35;
                    DB.Location = p;
                }
            }

            foreach (var ctrl in ToBeLowered)
            {
                if (ctrl.Tag is int @int)
                {
                    ctrl.Tag = @int - 1;
                }
            }
            if (IsEdit)
            {
                // INSERT CODE TO REMOVE THE TRIGGER FROM THE TASK HEREEEEEEEEEEEEE

                using var ts = new TaskService();
                var t = ts.GetTask("WebExRec-" + Courses[Conversions.ToInteger(f.Tag)].ID);
                var Currentday = Courses[Conversions.ToInteger(f.Tag)].Days[Conversions.ToInteger(b.Tag)];
                Trigger toberemoved = null;
                foreach (var trigger in t.Definition.Triggers)
                {
                    if (trigger is not WeeklyTrigger WT)
                        continue;           // Not a wek
                    if ((Currentday.StartTime ?? "") == (trigger.StartBoundary.ToString("HH:mm") ?? "") & (Currentday.EndTime ?? "") == (trigger.EndBoundary.ToString("HH:mm") ?? ""))
                    {
                        if ((Currentday.DayName.ToLower() ?? "") == (CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), WT.DaysOfWeek.ToString())) ?? ""))
                        {
                            MessageBox.Show("All trigger data matches");
                            toberemoved = trigger;
                        }
                    }
                }

                if (toberemoved is object)
                {
                    t.Definition.Triggers.Remove(toberemoved);
                    t.RegisterChanges();
                }
            }

            Courses[Conversions.ToInteger(f.Tag)].Days.RemoveAt(Conversions.ToInteger(b.Tag));
            foreach (var CTRL in ToDelete)
            {
                f.Controls.Remove(CTRL);
                CTRL.Dispose();
            }

            f.Height -= 35;
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            var COPF = new CommonOpenFileDialog()
            {
                InitialDirectory = @"C:\\Users",
                IsFolderPicker = true
            };
            if (COPF.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SavePath.Text = COPF.FileName;
            }
        }

        private void BrowseFile_Click(object sender, EventArgs e)
        {
            var COPF = new CommonOpenFileDialog()
            {
                InitialDirectory = @"C:\\Users",
                IsFolderPicker = false,
                EnsureFileExists = true
            };
            COPF.Filters.Add(new CommonFileDialogFilter("HTML file", "html,htm"));
            if (COPF.ShowDialog() == CommonFileDialogResult.Ok)
            {
                HtmlPath.Text = COPF.FileName;
            }
        }
    }
}