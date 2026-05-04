Imports System.IO
Imports System.IO.Compression
Imports System.Net
Public Class GitHubUpdater
    Shared Property RepositoryOwnerName As String = "sYnapZiX"
    Shared Property RepositoryName As String = "Stardew-Valley-Expansion-Loader"
    Shared Property AssetFile As String = "Release.zip"
    Public Shared Function Check(Optional Silent As Boolean = False, Optional FourDigitVersionNumber As Boolean = True) As Boolean
        Try
            CleanupTemporaryFiles()

            If My.Computer.Network.Ping("www.github.com", 250) Then
                Application.DoEvents()
                Using UpdateClient As New WebClient
                    Dim UpdateString As String = UpdateClient.DownloadString("https://github.com/" & RepositoryOwnerName & "/" & RepositoryName & "/releases/latest")
                    Dim StartIndex As Integer = UpdateString.IndexOf("<title>")
                    Dim EndIndex As Integer = UpdateString.IndexOf("Â·")
                    If StartIndex <> -1 AndAlso EndIndex <> -1 Then
                        Dim CurrentVersion As String
                        If FourDigitVersionNumber Then
                            CurrentVersion = My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build & "." & My.Application.Info.Version.MinorRevision
                        Else
                            CurrentVersion = My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor
                        End If
                        Dim UpdateVersion As String = UpdateString.Substring(StartIndex + 7, EndIndex - StartIndex - 8).Replace("Release ", "")
                        If UpdateVersion = "Releases" Then UpdateVersion = CurrentVersion
                        If Not Silent AndAlso CurrentVersion <> UpdateVersion Then
                            Dim Result As New DialogResult
                            If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "de" Then
                                Result = MessageBox.Show("Eine neuere Version ist verfügbar." & vbNewLine & "Möchten Sie jetzt ein Update durchführen?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            Else
                                Result = MessageBox.Show("A newer version is available." & vbNewLine & "Do you want to update now?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            End If
                            If Result = DialogResult.Yes Then Return True
                        Else
                            Return CurrentVersion <> UpdateVersion
                        End If
                    End If
                End Using
            End If
        Catch
            If Not Silent Then
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "de" Then
                    MessageBox.Show("Beim suchen nach Updates ist ein Fehler aufgetreten.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    MessageBox.Show("An error occured while searching for updates.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        End Try
        Return False
    End Function
    Public Shared Sub Download()
        Try
            If My.Computer.Network.Ping("www.github.com", 250) Then
                If Not Directory.Exists(Path.GetTempPath & "GitHubUpdater") Then
                    Directory.CreateDirectory(Path.GetTempPath & "GitHubUpdater")
                Else
                    Directory.Delete(Path.GetTempPath & "GitHubUpdater", True)
                    Directory.CreateDirectory(Path.GetTempPath & "GitHubUpdater")
                End If
                Using UpdateClient As New WebClient
                    UpdateClient.DownloadFile("https://github.com/" & RepositoryOwnerName & "/" & RepositoryName & "/releases/latest/download/" & AssetFile, Path.GetTempPath & "GitHubUpdater\" & AssetFile)
                    If AssetFile.EndsWith(".zip") Then
                        ZipFile.ExtractToDirectory(Path.GetTempPath & "GitHubUpdater\" & AssetFile, Path.GetTempPath & "GitHubUpdater\" & AssetFile.Replace(".zip", ""))
                        File.Delete(Path.GetTempPath & "GitHubUpdater\" & AssetFile)
                        Using UpdateScript As New StreamWriter(Path.GetTempPath & "GitHubUpdater\UpdateScript.cmd", False)
                            UpdateScript.WriteLine("@echo off")
                            If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "de" Then
                                UpdateScript.WriteLine("echo " & My.Application.Info.ProductName & " Aktualisierungsprogramm")
                                UpdateScript.WriteLine("echo Bitte schliessen Sie dieses Fenster nicht!")
                            Else
                                UpdateScript.WriteLine("echo " & My.Application.Info.ProductName & " Updating Utility")
                                UpdateScript.WriteLine("echo Do not close this window!")
                            End If
                            UpdateScript.WriteLine("echo.")
                            UpdateScript.WriteLine("timeout 5 /NOBREAK")
                            UpdateScript.WriteLine("cls")
                            UpdateScript.WriteLine("del " & """" & My.Application.Info.DirectoryPath & "\" & My.Application.Info.AssemblyName & ".exe" & """")
                            UpdateScript.WriteLine("move " & """" & Path.GetTempPath & "GitHubUpdater\Stardew Valley Expansion Loader.exe" & """ """ & My.Application.Info.DirectoryPath & "\" & My.Application.Info.AssemblyName & ".exe" & """")
                            UpdateScript.WriteLine("cls")
                            UpdateScript.WriteLine("start " & """" & My.Application.Info.DirectoryPath & "\" & My.Application.Info.AssemblyName & ".exe" & """")
                            UpdateScript.WriteLine("exit")
                        End Using
                        Process.Start("cmd", "/c " & """" & Path.GetTempPath & "GitHubUpdater\UpdateScript.cmd" & """")
                        End
                    End If
                End Using
            End If
        Catch
        End Try
    End Sub
    Public Shared Sub CleanupTemporaryFiles()
        Try
            If Directory.Exists(Path.GetTempPath & "GitHubUpdater") Then
                Directory.Delete(Path.GetTempPath & "GitHubUpdater", True)
            End If
        Catch
        End Try
    End Sub
End Class
