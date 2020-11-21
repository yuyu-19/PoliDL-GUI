Imports System.Threading
Public Class InputForm
    Private ReadOnly WaitEvent As New ManualResetEvent(False)
    Private Sub OK_Click(sender As Object, e As EventArgs) Handles OK.Click
        If InputText.Text Is Nothing Or InputText.Text = "" Then
            MessageBox.Show("Please input something.")
        Else
            Me.Close()
        End If
    End Sub

    Public Function AskForInput(Query As String)
        Label1.Text = Query
        If Query.Contains("password") Then
            InputText.PasswordChar = "*"
        Else
            InputText.PasswordChar = Nothing
        End If
        InputText.Text = ""
        Dim OldHeight As Integer = InputText.Location.Y


        If Query.Contains("video:") Then

            Dim p As Point = InputText.Location

            p.Y = p.Y + InputText.Height + 5

            InputText.Location = p
            p.X = OK.Location.X
            OK.Location = p

            Skip.Visible = True
            Me.Height = Me.Height + InputText.Height + 10
        End If

        Do While InputText.Text = "" Or InputText.Text Is Nothing
            Me.ShowDialog()
        Loop

        If Query.Contains("video:") Then
            Skip.Visible = False
            Dim p As Point
            p.X = InputText.Location.X
            p.Y = OldHeight
            InputText.Location = p
            p.X = OK.Location.X
            OK.Location = p
            Me.Height = Me.Height - InputText.Height - 10
        End If

        Return InputText.Text
    End Function

    Private Sub Skip_Click(sender As Object, e As EventArgs) Handles Skip.Click
        InputText.Text = "0"
        Me.Close()
    End Sub

    Private Sub InputForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If StartupForm.IsItalian Then
            Skip.Text = "Salta"
        End If
    End Sub
End Class