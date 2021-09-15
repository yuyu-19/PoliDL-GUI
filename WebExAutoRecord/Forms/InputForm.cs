using System;
using System.Drawing;
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

        //private readonly ManualResetEvent WaitEvent = new ManualResetEvent(false);

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
            //Should I fix this so it uses anchors instead of this weird position jank? Yes.
            //Am I going to? No.

            Label1.Text = Query;
            if (Query.Contains("password") & !Query.Contains("video:"))
                InputText.PasswordChar = '*';
            else
                InputText.PasswordChar = default;

            InputText.Text = "";
            int OldHeight = InputText.Location.Y;
            if (Query.Contains("video:"))
            {
                var p = InputText.Location;
                p.Y = p.Y + InputText.Height + 5;
                InputText.Location = p;
                p.Y += 20;
                p.X = OK.Location.X;
                OK.Location = p;
                Skip.Visible = true;
                Height = Height + InputText.Height + 10 + 20;
                reUsePW.Visible = true;
            }

            while (string.IsNullOrEmpty(InputText.Text) | InputText.Text is null)
            {
                ShowDialog();
                Activate();
            }
               
            if (Query.Contains("video:"))
            {
                Skip.Visible = false;
                var p = default(Point);
                p.X = InputText.Location.X;
                p.Y = OldHeight;
                InputText.Location = p;
                p.X = OK.Location.X;
                OK.Location = p;
                Height = Height - InputText.Height - 10 - 20;
            }
            if (!reUsePW.Checked)
                return InputText.Text;
            else
                return InputText.Text + "_ReUseForAllVideos!";  //Is this jank? 
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
                reUsePW.Text = "Riutilizza password?";
            }
        }

        private void reUsePW_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}