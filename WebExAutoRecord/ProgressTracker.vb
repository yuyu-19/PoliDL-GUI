Public Class ProgressTracker
    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
        'I swear I tried for hours to do this properly with multithreading etc, but it just refused.
        'JANK IT UP!

        If DownloadForm.CurrentSpeed = "Finished." Or DownloadForm.CurrentSpeed = "Finito." Then Me.Close()

        OverallProgress.Value = DownloadForm.currentprogress
        FileNum.Text = "File " & DownloadForm.currentfile & "/" & DownloadForm.currentfiletotal

        If DLspeed.Text <> DownloadForm.CurrentSpeed Then
            DLspeed.Text = DownloadForm.CurrentSpeed
            Dim size As Size = TextRenderer.MeasureText(DLspeed.Text, DLspeed.Font)
            DLspeed.Width = size.Width
            Dim p As Point = DLspeed.Location
            p.X = Me.ClientSize.Width - DLspeed.Width - 5
            DLspeed.Location = p
        End If
    End Sub

    Private Sub ProgressTracker_Closing(sender As Object, e As FormClosingEventArgs) Handles Me.Closing

        If DownloadForm.CurrentSpeed = "Processing file..." Or DownloadForm.CurrentSpeed = "Sto elaborando il file..." Then
            'We don't want the user to stop a file whilst it's processing.
            'TODO: Add the existing file check to poliwebex.js
            MessageBox.Show("Please wait until the file is done processing.")
            e.Cancel = True
            Return
        End If

        If DownloadForm.CurrentSpeed <> "Finished." And DownloadForm.CurrentSpeed <> "Finito." Then
            Dim ans As Integer
            If StartupForm.IsItalian Then
                If Not DownloadForm.GlobalProcess.StartInfo.Arguments.Contains(" -s") Then
                    ans = MsgBox("Sei sicuro? Interromperà il download corrente e dovrai ricominciare da capo, dato che sei in modalità unsegmented.", MsgBoxStyle.YesNo, "Exit?")
                Else
                    ans = MsgBox("Sei sicuro? Interromperà il download corrente.", MsgBoxStyle.YesNo, "Exit?")
                End If
            Else
                If Not DownloadForm.GlobalProcess.StartInfo.Arguments.Contains(" -s") Then
                    ans = MsgBox("Are you sure? This will stop the current download and you will have to start from scratch, since you're in unsegmented mode.", MsgBoxStyle.YesNo, "Exit?")
                Else
                    ans = MsgBox("Are you sure? This will stop the current download.", MsgBoxStyle.YesNo, "Exit?")
                End If

            End If
            If ans = DialogResult.Yes Then
                DownloadForm.GlobalProcess.Kill()
                DownloadForm.GlobalProcess.Dispose()
            Else
                e.Cancel = True
            End If
        End If


    End Sub

    Private Sub ProgressTracker_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class