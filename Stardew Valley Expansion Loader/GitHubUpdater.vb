Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Text
Public Class GitHubUpdater
    Structure Properties
        '### THESE 3 PROPERTIES WILL FORM THE UPDATE URL ###
        Shared ReadOnly Property RepositoryOwnerName As String = "sYnapZiX"
        Shared ReadOnly Property RepositoryName As String = "Stardew-Valley-Expansion-Loader"
        Shared ReadOnly Property AssetFile As String = "Release.zip"

        '### THIS PROPERTY WILL TELL THE UPDATER THE LENGTH OF THE VERSION STRING ###
        Shared ReadOnly Property VersionDigits As Byte = 4

        '### THIS PROPERTY DEFINES THE TEMPORARY FOLDER ###
        Shared ReadOnly Property TemporaryFolder As String = Path.GetTempPath & "GitHubUpdater"

        '### THESE PROPERTIES CAN BE SET AT RUNTINE ###
        Shared Property Silent As Boolean = False '     SHOW / HIDE MESSAGES
        Shared Property Timeout As Integer = 250 '      WEBCLIENT TIMEOUT
    End Structure
    Public Shared Function Check() As Boolean
        Try
            CleanupTemporaryFiles()

            If My.Computer.Network.Ping("www.github.com", Properties.Timeout) Then
                Application.DoEvents()
                Using UpdateClient As New WebClient
                    Dim UpdateURL As String = "https://github.com/" & Properties.RepositoryOwnerName & "/" & Properties.RepositoryName & "/releases/latest"
                    Dim GitHubPage As String = UpdateClient.DownloadString(UpdateURL)
                    Dim StartIndex As Integer = GitHubPage.IndexOf("<title>")
                    Dim EndIndex As Integer = GitHubPage.IndexOf("Â·")
                    If StartIndex <> -1 AndAlso EndIndex <> -1 Then
                        Dim CurrentVersion As String = String.Empty
                        Select Case Properties.VersionDigits
                            Case 1
                                CurrentVersion = My.Application.Info.Version.Major.ToString
                            Case 2
                                CurrentVersion = My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString
                            Case 3
                                CurrentVersion = My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString & "." & My.Application.Info.Version.Build.ToString
                            Case Else
                                CurrentVersion = My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString & "." & My.Application.Info.Version.Build.ToString & "." & My.Application.Info.Version.MinorRevision.ToString
                        End Select
                        Dim UpdateVersion As String = GitHubPage.Substring(StartIndex + 7, EndIndex - StartIndex - 8).Replace("Release ", "")
                        If UpdateVersion = "Releases" Then UpdateVersion = CurrentVersion
                        If Not Properties.Silent AndAlso CurrentVersion <> UpdateVersion Then
                            Dim Result As New DialogResult
                            If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "de" Then '     GERMAN
                                Result = MessageBox.Show("Eine neuere Version ist verfügbar." & vbNewLine & "Möchten Sie jetzt ein Update durchführen?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "es" Then ' SPANISH
                                Result = MessageBox.Show("Una versión más reciente está disponible." & vbNewLine & "¿Deseas actualizar ahora?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "fr" Then ' FRANCE
                                Result = MessageBox.Show("Une version plus récente est disponible." & vbNewLine & "Voulez-vous mettre à jour maintenant ?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "hu" Then ' HUNGARIAN
                                Result = MessageBox.Show("Újabb verzió érhető el." & vbNewLine & "Szeretné most frissíteni?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "it" Then ' ITALIAN
                                Result = MessageBox.Show("È disponibile una versione più recente." & vbNewLine & "Vuoi aggiornare ora?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ja" Then ' JAPANESE
                                Result = MessageBox.Show("新しいバージョンが利用可能です。" & vbNewLine & "今すぐ更新しますか？", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ko" Then ' KOREAN
                                Result = MessageBox.Show("새로운 버전이 있습니다." & vbNewLine & "지금 업데이트하시겠습니까?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "pt" Then ' PORTUGUESE (BRASILIAN)
                                Result = MessageBox.Show("Uma versão mais recente está disponível." & vbNewLine & "Deseja atualizar agora?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ru" Then ' RUSSIAN
                                Result = MessageBox.Show("Доступна новая версия." & vbNewLine & "Хотите обновить сейчас?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "tr" Then ' TURKISH
                                Result = MessageBox.Show("Daha yeni bir sürüm mevcut." & vbNewLine & "Şimdi güncellemek ister misiniz?", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "zh" Then ' CHINESE (SIMPLIFIED)
                                Result = MessageBox.Show("有新版本可用。" & vbNewLine & "现在要更新吗？", "GitHub Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            Else '                                                                                      ENGLISH
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
            If Not Properties.Silent Then
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "de" Then '     GERMAN
                    MessageBox.Show("Beim suchen nach Updates ist ein Fehler aufgetreten.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "es" Then ' SPANISH
                    MessageBox.Show("Se produjo un error al buscar actualizaciones.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "fr" Then ' FRANCE
                    MessageBox.Show("Une erreur s’est produite lors de la recherche de mises à jour.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "hu" Then ' HUNGARIAN
                    MessageBox.Show("Hiba történt a frissítések keresése közben.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "it" Then ' ITALIAN
                    MessageBox.Show("Si è verificato un errore durante la ricerca degli aggiornamenti.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ja" Then ' JAPANESE
                    MessageBox.Show("更新の確認中にエラーが発生しました。", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ko" Then ' KOREAN
                    MessageBox.Show("업데이트를 확인하는 중 오류가 발생했습니다.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "pt" Then ' PORTUGUESE (BRASILIAN)
                    MessageBox.Show("Ocorreu um erro ao procurar atualizações.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ru" Then ' RUSSIAN
                    MessageBox.Show("Произошла ошибка при поиске обновлений.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "tr" Then ' TURKISH
                    MessageBox.Show("Güncellemeler aranırken bir hata oluştu.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "zh" Then ' CHINESE (SIMPLIFIED)
                    MessageBox.Show("在搜索更新时发生错误。", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else '                                                                                      ENGLISH
                    MessageBox.Show("An error occured while searching for updates.", "GitHub Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        End Try
        Return False
    End Function
    Public Shared Sub Download(Optional ExecutableToMove As String = "")
        Try
            If My.Computer.Network.Ping("www.github.com", Properties.Timeout) Then
                If Not Directory.Exists(Properties.TemporaryFolder) Then
                    Directory.CreateDirectory(Properties.TemporaryFolder)
                Else
                    Directory.Delete(Properties.TemporaryFolder, True)
                    Directory.CreateDirectory(Properties.TemporaryFolder)
                End If
                Using UpdateClient As New WebClient
                    Dim UpdateURL As String = "https://github.com/" & Properties.RepositoryOwnerName & "/" & Properties.RepositoryName & "/releases/latest/download/" & Properties.AssetFile
                    Dim DownloadTarget As String = Properties.TemporaryFolder & "\" & Properties.AssetFile
                    Dim UpdateScript As String = Properties.TemporaryFolder & "\UpdateScript.cmd"
                    Dim TargetFolder As String = My.Application.Info.DirectoryPath
                    Dim LaunchExecutable As String = TargetFolder & "\" & Path.GetFileName(Application.ExecutablePath)
                    UpdateClient.DownloadFile(UpdateURL, DownloadTarget)
                    If Properties.AssetFile.EndsWith(".zip") Then
                        Dim ExtractionTarget As String = DownloadTarget.Replace(".zip", "")
                        ZipFile.ExtractToDirectory(DownloadTarget, ExtractionTarget)
                        File.Delete(DownloadTarget)
                        Using UpdateScriptWriter As New StreamWriter(UpdateScript, False, Encoding.UTF8)
                            UpdateScriptWriter.WriteLine("@echo off")
                            If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "de" Then '     GERMAN
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Aktualisierungsprogramm")
                                UpdateScriptWriter.WriteLine("echo Bitte schliessen Sie dieses Fenster nicht!")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "es" Then ' SPANISH
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Actualizando la utilidad")
                                UpdateScriptWriter.WriteLine("echo ¡No cierre esta ventana!")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "fr" Then ' FRANCE
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Mise à jour de l’utilitaire")
                                UpdateScriptWriter.WriteLine("echo Ne fermez pas cette fenêtre !")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "hu" Then ' HUNGARIAN
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Segédprogram frissítése")
                                UpdateScriptWriter.WriteLine("echo Ne zárja be ezt az ablakot!")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "it" Then ' ITALIAN
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Aggiornamento dell’utilità")
                                UpdateScriptWriter.WriteLine("echo Non chiudere questa finestra!")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ja" Then ' JAPANESE
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " ユーティリティを更新中")
                                UpdateScriptWriter.WriteLine("echo このウィンドウを閉じないでください！")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ko" Then ' KOREAN
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " 유틸리티 업데이트 중")
                                UpdateScriptWriter.WriteLine("echo 이 창을 닫지 마세요!")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "pt" Then ' PORTUGUESE (BRASILIAN)
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Atualizando o utilitário")
                                UpdateScriptWriter.WriteLine("echo Não feche esta janela!")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ru" Then ' RUSSIAN
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Обновление утилиты")
                                UpdateScriptWriter.WriteLine("echo Не закрывайте это окно!")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "tr" Then ' TURKISH
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Yardımcı program güncelleniyor")
                                UpdateScriptWriter.WriteLine("echo Bu pencereyi kapatmayın!")
                            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "zh" Then ' CHINESE (SIMPLIFIED)
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " 正在更新工具")
                                UpdateScriptWriter.WriteLine("echo 请勿关闭此窗口!")
                            Else '                                                                                      ENGLISH
                                UpdateScriptWriter.WriteLine("echo " & My.Application.Info.ProductName & " Updating Utility")
                                UpdateScriptWriter.WriteLine("echo Do not close this window!")
                            End If
                            UpdateScriptWriter.WriteLine("echo.")
                            UpdateScriptWriter.WriteLine("timeout 5 /NOBREAK")
                            UpdateScriptWriter.WriteLine("cls")
                            If ExecutableToMove = "" Then
                                UpdateScriptWriter.WriteLine("robocopy " & """" & DownloadTarget & """ """ & TargetFolder & """ /E /MOVE")
                            Else
                                UpdateScriptWriter.WriteLine("del " & """" & LaunchExecutable & """")
                                UpdateScriptWriter.WriteLine("move " & """" & Properties.TemporaryFolder & "\" & ExecutableToMove & """ """ & LaunchExecutable & """")
                            End If
                            UpdateScriptWriter.WriteLine("cls")
                            UpdateScriptWriter.WriteLine("start " & """" & LaunchExecutable & """")
                            UpdateScriptWriter.WriteLine("exit")
                            UpdateScriptWriter.Flush()
                            UpdateScriptWriter.Close()
                        End Using
                        Process.Start("cmd", "/c " & """" & UpdateScript & """")
                        End
                    End If
                End Using
            End If
        Catch
        End Try
    End Sub
    Public Shared Sub CleanupTemporaryFiles()
        Try
            If Directory.Exists(Properties.TemporaryFolder) Then
                Directory.Delete(Properties.TemporaryFolder, True)
            End If
        Catch
        End Try
    End Sub
End Class
