using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace PoliDLGUI.Forms
{
    public partial class InputForm
    {
        public InputForm(Point location)
        {
            this.Location = location;
            InitializeComponent();
            _OK.Name = "OK";
            _Skip.Name = "Skip";
        }

        private readonly ManualResetEvent WaitEvent = new ManualResetEvent(false);

        private void OK_Click(object sender, EventArgs e)
        {
            if (InputText.Text is null | string.IsNullOrEmpty(InputText.Text))
            {
                MessageBox.Show("Please input something.");
            }
            else
            {
                Close();
            }
        }

        public static object AskForInput(string Query, Point location)
        {
            InputForm inputForm = new InputForm(location);
            return inputForm.AskForInputInternal(Query);
        }

        public object AskForInputInternal(string Query)
        {
            Label1.Text = Query;
            if (Query.Contains("password"))
            {
                InputText.PasswordChar = '*';
            }
            else
            {
                InputText.PasswordChar = default;
            }

            InputText.Text = "";
            int OldHeight = InputText.Location.Y;
            if (Query.Contains("video:"))
            {
                var p = InputText.Location;
                p.Y = p.Y + InputText.Height + 5;
                InputText.Location = p;
                p.X = OK.Location.X;
                OK.Location = p;
                Skip.Visible = true;
                Height = Height + InputText.Height + 10;
            }

            while (string.IsNullOrEmpty(InputText.Text) | InputText.Text is null)
                ShowDialog();
            if (Query.Contains("video:"))
            {
                Skip.Visible = false;
                var p = default(Point);
                p.X = InputText.Location.X;
                p.Y = OldHeight;
                InputText.Location = p;
                p.X = OK.Location.X;
                OK.Location = p;
                Height = Height - InputText.Height - 10;
            }

            return InputText.Text;
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            InputText.Text = "0";
            Close();
        }

        private void InputForm_Load(object sender, EventArgs e)
        {
            if (StartupForm.IsItalian)
            {
                Skip.Text = "Salta";
            }
        }
    }
}