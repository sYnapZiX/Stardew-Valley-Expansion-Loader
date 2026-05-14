Public Class GitHubUpdater
    Structure Properties
        '### THESE 3 PROPERTIES WILL FORM THE UPDATE URL ###
        Shared ReadOnly Property RepositoryOwnerName As String = "sYnapZiX"
        Shared ReadOnly Property RepositoryName As String = "Stardew-Valley-Expansion-Loader"
        Shared ReadOnly Property AssetFile As String = "Release.zip"

        '### THIS PROPERTY WILL TELL THE UPDATER THE SEPERATIONS OF THE VERSION STRING ###
        Shared ReadOnly Property VersionSeperations As Byte = 4

        '### THIS PROPERTY WILL TELL THE UPDATER TO RUN EITHER SYNCHRONOUSLY (THREAD-LOCKING) OR ASYNCHRONOUSLY ###
        Shared ReadOnly Property Asynchronous As Boolean = False

        '### THIS PROPERTY DEFINES THE TEMPORARY FOLDER ###
        Shared ReadOnly Property TemporaryFolder As String = IO.Path.GetTempPath & ".$" & My.Application.Info.AssemblyName & ".tmp"

        '### THIS PROPERTY, IF BIGGER THEN 1, ENABLES SPLIT ACRHIVES MODE (.s001) ### (SEE: 'https://github.com/sYnapZiX/ArchiveSplitter' FOR MORE INFORMATION)
        Shared ReadOnly Property AssetParts As Integer = 1

        '### THESE PROPERTIES CAN BE SET AT RUNTINE ###
        Shared Property Enabled As Boolean = True '         ENABLE/DISABLE UPDATE ROUTINE
        Shared Property Retries As Integer = 4 '            NUMBER OF RETRIES FOR CHECKING AND DOWNLOADING
        Shared Property Silent As Boolean = False '         SHOW/HIDE MESSAGES/PROMTS
        Shared Property Timeout As Integer = 250 '          WEBCLIENT TIMEOUT (IN MILLISECONDS)
    End Structure
    Structure UserInterface
        Public Shared Sub CheckStage1() '                                                 ### YOUR UI CODE THAT HAPPENS BEFORE CHECKING FOR UPDATES ###########################################################################################################################################################################################################

        End Sub
        Public Shared Sub CheckTrue() '                                                   ### YOUR UI CODE THAT HAPPENS IF AN UPDATE IS AVAILABLE #############################################################################################################################################################################################################

        End Sub
        Public Shared Sub CheckFalse() '                                                  ### YOUR UI CODE THAT HAPPENS IF NO UPDATE IS AVAILABLE #############################################################################################################################################################################################################

        End Sub
        Public Shared Sub CheckFailed() '                                                 ### YOUR UI CODE THAT HAPPENS IF THE UPDATE CHECK HAS FAILED ########################################################################################################################################################################################################

        End Sub
        Public Shared Sub DownloadProgress(Progress As Integer, AssetPart As Integer) '   ### YOUR UI CODE THAT HAPPENS DURING DOWNLOAD #######################################################################################################################################################################################################################

        End Sub
        Public Shared Sub UpdateStage1() '                                                ### YOUR UI CODE THAT HAPPENS BEFORE ARCHIVE EXTRACTION #############################################################################################################################################################################################################

        End Sub
        Public Shared Sub UpdateStage2() '                                                ### YOUR UI CODE THAT HAPPENS BEFORE ARCHIVE DELETION ###############################################################################################################################################################################################################

        End Sub
        Public Shared Sub UpdateStage3() '                                                ### YOUR UI CODE THAT HAPPENS BEFORE UPDATESCRIPT IS CREATED ########################################################################################################################################################################################################

        End Sub
        Public Shared Sub UpdateStage4() '                                                ### YOUR UI CODE THAT HAPPENS BEFORE UPDATESCRIPT IS LAUNCHED #######################################################################################################################################################################################################

        End Sub
        Public Shared Sub DownloadFailed() '                                              ### YOUR UI CODE THAT HAPPENS WHEN THE DOWNLOAD HAS FAILED ##########################################################################################################################################################################################################

        End Sub
    End Structure

    '####################################################################################################################################################################################################################################################################################################################################
    '####################################################################################################################################################################################################################################################################################################################################
    '###### NO USER CHANGABLE CODE BEYOND THIS POINT ### NO USER CHANGABLE CODE BEYOND THIS POINT ### NO USER CHANGABLE CODE BEYOND THIS POINT ### NO USER CHANGABLE CODE BEYOND THIS POINT ### NO USER CHANGABLE CODE BEYOND THIS POINT ### NO USER CHANGABLE CODE BEYOND THIS POINT ### NO USER CHANGABLE CODE BEYOND THIS POINT ######
    '####################################################################################################################################################################################################################################################################################################################################
    '####################################################################################################################################################################################################################################################################################################################################
    Structure Handler
        Public Shared Sub DownloadProgressAsynchronous(sender As Object, e As Net.DownloadProgressChangedEventArgs)
            UserInterface.DownloadProgress(e.ProgressPercentage, TemporaryBuffer.FileCount)
        End Sub
        Public Shared Sub DownloadFinishedAsynchronous(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
            Application.DoEvents()
            UserInterface.DownloadProgress(0, TemporaryBuffer.FileCount)
            Application.DoEvents()

            If Not e.Cancelled Then
                If Properties.AssetParts = 1 Then
                    Dim DownloadTarget As String = Properties.TemporaryFolder & "\" & Properties.AssetFile
                    Dim ExtractionTarget As String = DownloadTarget.Replace(".zip", "")

                    Application.DoEvents()
                    UserInterface.UpdateStage1()
                    Application.DoEvents()

                    IO.Compression.ZipFile.ExtractToDirectory(DownloadTarget, ExtractionTarget)

                    Application.DoEvents()
                    UserInterface.UpdateStage2()
                    Application.DoEvents()

                    IO.File.Delete(DownloadTarget)

                    Application.DoEvents()
                    UserInterface.UpdateStage3()
                    Application.DoEvents()

                    UpdateScript.Create(TemporaryBuffer.ExecutableToMove, ExtractionTarget, TemporaryBuffer.LaunchExecutable, TemporaryBuffer.TargetFolder)

                    Application.DoEvents()
                    UserInterface.UpdateStage4()
                    Application.DoEvents()

                    UpdateScript.Run()
                    End
                ElseIf Properties.AssetParts > 1 Then
                    If TemporaryBuffer.FileCount = Properties.AssetParts Then
                        Dim DownloadTarget As String = Properties.TemporaryFolder & "\" & Properties.AssetFile & ".s" & TemporaryBuffer.FileCount.ToString("000")
                        Dim ExtractionTarget As String = Properties.TemporaryFolder & "\" & Properties.AssetFile.Replace(".zip", "")
                        Try
                            For i = 1 To Properties.AssetParts
                                Dim CurrentPart As String = Properties.TemporaryFolder & "\" & Properties.AssetFile & ".s" & i.ToString("000")
                                If IO.File.Exists(CurrentPart) Then
                                    Dim FileLength As Long = New IO.FileInfo(CurrentPart).Length
                                    Dim Append As Boolean = False
                                    If i = 1 Then
                                        Append = False
                                    Else
                                        Append = True
                                    End If
                                    Using FileWriter As New IO.StreamWriter(Properties.TemporaryFolder & "\" & Properties.AssetFile, Append, Text.Encoding.Default)
                                        FileWriter.Write(IO.File.ReadAllText(CurrentPart, Text.Encoding.Default))
                                        FileWriter.Flush()
                                        FileWriter.Close()
                                    End Using
                                    IO.File.Delete(CurrentPart)
                                Else
                                    Exit For
                                End If
                            Next
                        Catch
                            TemporaryBuffer.ExecutableToMove = ""

                            Application.DoEvents()
                            UserInterface.DownloadFailed()
                            Application.DoEvents()
                        End Try

                        Application.DoEvents()
                        UserInterface.UpdateStage1()
                        Application.DoEvents()

                        IO.Compression.ZipFile.ExtractToDirectory(Properties.TemporaryFolder & "\" & Properties.AssetFile, Properties.TemporaryFolder & "\" & Properties.AssetFile.Replace(".zip", ""))

                        Application.DoEvents()
                        UserInterface.UpdateStage2()
                        Application.DoEvents()

                        IO.File.Delete(Properties.TemporaryFolder & "\" & Properties.AssetFile)

                        Application.DoEvents()
                        UserInterface.UpdateStage3()
                        Application.DoEvents()

                        UpdateScript.Create(TemporaryBuffer.ExecutableToMove, ExtractionTarget, TemporaryBuffer.LaunchExecutable, TemporaryBuffer.TargetFolder)

                        Application.DoEvents()
                        UserInterface.UpdateStage4()
                        Application.DoEvents()

                        UpdateScript.Run()
                        End
                    Else
                        RemoveHandler TemporaryBuffer.AsynchronousWebClient.DownloadProgressChanged, AddressOf DownloadProgressAsynchronous
                        RemoveHandler TemporaryBuffer.AsynchronousWebClient.DownloadFileCompleted, AddressOf DownloadFinishedAsynchronous
                        TemporaryBuffer.FileCount += 1
                        Download(TemporaryBuffer.ExecutableToMove, "https://github.com/" & Properties.RepositoryOwnerName & "/" & Properties.RepositoryName & "/releases/latest/download/" & Properties.AssetFile & ".s" & TemporaryBuffer.FileCount.ToString("000"), Properties.TemporaryFolder & "\" & Properties.AssetFile & ".s" & TemporaryBuffer.FileCount.ToString("000"))
                    End If
                End If
            Else
                For i = 1 To Properties.Retries
                    If i = Properties.Retries Then
                        TemporaryBuffer.ExecutableToMove = ""
                        Application.DoEvents()
                        UserInterface.DownloadFailed()
                        Application.DoEvents()
                    Else
                        If Properties.AssetParts > 1 AndAlso Not TemporaryBuffer.FileCount = Properties.AssetParts Then
                            Download(TemporaryBuffer.ExecutableToMove, "https://github.com/" & Properties.RepositoryOwnerName & "/" & Properties.RepositoryName & "/releases/latest/download/" & Properties.TemporaryFolder & "\" & Properties.AssetFile & ".s" & TemporaryBuffer.FileCount.ToString("000"), Properties.TemporaryFolder & "\" & Properties.AssetFile & ".s" & TemporaryBuffer.FileCount.ToString("000"))
                        Else
                            Download()
                        End If
                    End If
                Next
            End If
        End Sub
    End Structure
    Structure Strings
        Public Shared Function NoInternetConnection() As DialogResult
            Dim Result As New DialogResult
            If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "de" Then '     GERMAN
                Result = MessageBox.Show("Es kann keine Internetverbindung hergestellt werden." & vbNewLine & "Möchten Sie es erneut versuchen?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "es" Then ' SPANISH
                Result = MessageBox.Show("No se puede establecer una conexión a Internet." & vbNewLine & "¿Quieres volver a intentarlo?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "fr" Then ' FRANCE
                Result = MessageBox.Show("Impossible d'établir une connexion Internet." & vbNewLine & "Voulez-vous réessayer ?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "hu" Then ' HUNGARIAN
                Result = MessageBox.Show("Nem sikerült internetkapcsolatot létesíteni." & vbNewLine & "Megpróbálja újra?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "it" Then ' ITALIAN
                Result = MessageBox.Show("Impossibile stabilire una connessione a Internet." & vbNewLine & "Vuoi riprovare?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ja" Then ' JAPANESE
                Result = MessageBox.Show("インターネットに接続できません。" & vbNewLine & "もう一度試しますか？", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ko" Then ' KOREAN
                Result = MessageBox.Show("인터넷 연결을 할 수 없습니다." & vbNewLine & "다시 시도하시겠습니까?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "pt" Then ' PORTUGUESE (BRASILIAN)
                Result = MessageBox.Show("Não foi possível estabelecer uma conexão com a Internet." & vbNewLine & "Deseja tentar novamente?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ru" Then ' RUSSIAN
                Result = MessageBox.Show("Не удалось установить подключение к Интернету." & vbNewLine & "Попробовать ещё раз?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "tr" Then ' TURKISH
                Result = MessageBox.Show("İnternet bağlantısı kurulamıyor." & vbNewLine & "Tekrar denemek ister misiniz?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "zh" Then ' CHINESE (SIMPLIFIED)
                Result = MessageBox.Show("无法建立互联网连接。" & vbNewLine & "要重试吗？", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            Else '                                                                                      ENGLISH
                Result = MessageBox.Show("Unable to establish an internet connection." & vbNewLine & "Try again?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            End If
            Return Result
        End Function
        Public Shared Function ServerUnreachable() As DialogResult
            Dim Result As New DialogResult
            If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "de" Then '     GERMAN
                Result = MessageBox.Show("Es kann keine Verbindung zu GitHub hergestellt werden." & vbNewLine & "Möchten Sie es erneut versuchen?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "es" Then ' SPANISH
                Result = MessageBox.Show("No se ha podido establecer una conexión con GitHub." & vbNewLine & "¿Quieres volver a intentarlo?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "fr" Then ' FRANCE
                Result = MessageBox.Show("Impossible d'établir une connexion à GitHub." & vbNewLine & "Voulez-vous réessayer ?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "hu" Then ' HUNGARIAN
                Result = MessageBox.Show("Nem sikerült kapcsolatot létesíteni a GitHubbal." & vbNewLine & "Megpróbálja újra?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "it" Then ' ITALIAN
                Result = MessageBox.Show("Impossibile stabilire una connessione con GitHub." & vbNewLine & "Vuoi riprovare?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ja" Then ' JAPANESE
                Result = MessageBox.Show("GitHubへの接続が確立できませんでした。" & vbNewLine & "もう一度試しますか？", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ko" Then ' KOREAN
                Result = MessageBox.Show("GitHub에 연결할 수 없습니다." & vbNewLine & "다시 시도하시겠습니까?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "pt" Then ' PORTUGUESE (BRASILIAN)
                Result = MessageBox.Show("Não foi possível estabelecer uma conexão com o GitHub." & vbNewLine & "Deseja tentar novamente?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "ru" Then ' RUSSIAN
                Result = MessageBox.Show("Не удалось установить соединение с GitHub." & vbNewLine & "Попробовать ещё раз?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "tr" Then ' TURKISH
                Result = MessageBox.Show("GitHub'a bağlantı kurulamıyor." & vbNewLine & "Tekrar denemek ister misiniz?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            ElseIf Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName = "zh" Then ' CHINESE (SIMPLIFIED)
                Result = MessageBox.Show("无法连接到 GitHub。" & vbNewLine & "要重试吗？", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            Else '                                                                                      ENGLISH
                Result = MessageBox.Show("Unable to establish a connection to GitHub." & vbNewLine & "Try again?", "GitHub Updater", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation)
            End If
            Return Result
        End Function
        Public Shared Function UpdateAvailable() As DialogResult
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
            Return Result
        End Function
        Public Shared Sub UpdateCheckFailed()
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
        End Sub
    End Structure
    Structure TemporaryBuffer
        Shared AsynchronousWebClient As Net.WebClient
        Shared DownloadTarget As String = ""
        Shared ExecutableToMove As String = ""
        Shared FileCount As Integer = 1
        Shared UpdateURL As String = ""
        Shared ReadOnly TargetFolder As String = My.Application.Info.DirectoryPath
        Shared ReadOnly LaunchExecutable As String = TargetFolder & "\" & IO.Path.GetFileName(Application.ExecutablePath)
    End Structure
    Structure UpdateScript
        Public Shared Sub Create(ExecutableToMove As String, ExtractionTarget As String, LaunchExecutable As String, TargetFolder As String)
            Using UpdateScriptWriter As New IO.StreamWriter(Properties.TemporaryFolder & "\UpdateScript.cmd", False)
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
                    UpdateScriptWriter.WriteLine("robocopy " & """" & ExtractionTarget & """ """ & TargetFolder & """ /E /MOVE")
                Else
                    UpdateScriptWriter.WriteLine("del " & """" & LaunchExecutable & """")
                    UpdateScriptWriter.WriteLine("move " & """" & Properties.TemporaryFolder & "\" & ExecutableToMove & """ """ & LaunchExecutable & """")
                End If
                UpdateScriptWriter.WriteLine("cls")
                UpdateScriptWriter.WriteLine("start " & """"" " & """" & LaunchExecutable & """")
                UpdateScriptWriter.Write("exit")
                UpdateScriptWriter.Flush()
                UpdateScriptWriter.Close()
            End Using
        End Sub
        Public Shared Sub Run()
            Process.Start("cmd", "/c " & """" & Properties.TemporaryFolder & "\UpdateScript.cmd" & """")
        End Sub
    End Structure
    Public Shared Sub CleanupTemporaryFiles()
        If IO.Directory.Exists(Properties.TemporaryFolder) Then
            For Each TemporaryFile As String In IO.Directory.GetFiles(Properties.TemporaryFolder, "*.*", IO.SearchOption.AllDirectories)
                Try
                    IO.File.Delete(TemporaryFile)
                Catch
                End Try
            Next
            For Each TemporaryFolder As String In IO.Directory.GetDirectories(Properties.TemporaryFolder, "*.*", IO.SearchOption.AllDirectories)
                Try
                    IO.Directory.Delete(TemporaryFolder, True)
                Catch
                End Try
            Next
        End If
    End Sub
    Public Shared Sub RemoveTemporaryFile(Filename As String)
        If IO.File.Exists(Properties.TemporaryFolder & Filename) Then IO.File.Delete(Properties.TemporaryFolder & Filename)
    End Sub
    Public Shared Sub RemoveTemporaryFolder()
        If IO.Directory.Exists(Properties.TemporaryFolder) Then IO.Directory.Delete(Properties.TemporaryFolder, True)
    End Sub
    Public Shared Function Check() As Boolean
        If Properties.Enabled Then
            Try
                RemoveTemporaryFolder()
                If My.Computer.Network.IsAvailable Then
                    If My.Computer.Network.Ping("www.github.com", Properties.Timeout) Then
                        Using UpdateClient As New Net.WebClient
                            TemporaryBuffer.UpdateURL = "https://github.com/" & Properties.RepositoryOwnerName & "/" & Properties.RepositoryName & "/releases/latest"

                            Application.DoEvents()
                            UserInterface.CheckStage1()
                            Application.DoEvents()

                            Dim GitHubPage As String = String.Empty

                            For i = 1 To Properties.Retries
                                If i = Properties.Retries Then
                                    GitHubPage = UpdateClient.DownloadString(TemporaryBuffer.UpdateURL)
                                Else
                                    Try
                                        GitHubPage = UpdateClient.DownloadString(TemporaryBuffer.UpdateURL)
                                    Catch
                                    End Try
                                End If
                                If Not GitHubPage = String.Empty Then Exit For
                            Next

                            Dim StartIndex As Integer = GitHubPage.IndexOf("<title>")
                            Dim EndIndex As Integer = GitHubPage.IndexOf("Â·")

                            If StartIndex <> -1 AndAlso EndIndex <> -1 Then
                                Dim CurrentVersion As String = String.Empty
                                Select Case Properties.VersionSeperations
                                    Case 1
                                        CurrentVersion = My.Application.Info.Version.Major.ToString
                                    Case 2
                                        CurrentVersion = My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString
                                    Case 3
                                        CurrentVersion = My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString & "." & My.Application.Info.Version.Build.ToString
                                    Case Else
                                        CurrentVersion = My.Application.Info.Version.Major.ToString & "." & My.Application.Info.Version.Minor.ToString & "." & My.Application.Info.Version.Build.ToString & "." & My.Application.Info.Version.MinorRevision.ToString
                                End Select

                                Dim UpdateVersion As String = GitHubPage.Substring(StartIndex + 7, EndIndex - StartIndex - 8).Remove(0, 8)
                                GitHubPage = String.Empty
                                If UpdateVersion = "Releases" Then UpdateVersion = CurrentVersion

                                If Not Properties.Silent AndAlso CurrentVersion <> UpdateVersion Then
                                    If Strings.UpdateAvailable = DialogResult.Yes Then
                                        Application.DoEvents()
                                        UserInterface.CheckTrue()
                                        Application.DoEvents()
                                        Return True
                                    Else
                                        Application.DoEvents()
                                        UserInterface.CheckFalse()
                                        Application.DoEvents()
                                        Return False
                                    End If
                                ElseIf CurrentVersion <> UpdateVersion Then
                                    Application.DoEvents()
                                    UserInterface.CheckTrue()
                                    Application.DoEvents()
                                    Return True
                                Else
                                    Application.DoEvents()
                                    UserInterface.CheckFalse()
                                    Application.DoEvents()
                                End If
                            End If
                        End Using
                    Else
                        If Strings.ServerUnreachable = DialogResult.Retry Then
                            Check()
                        Else
                            Application.DoEvents()
                            UserInterface.CheckFailed()
                            Application.DoEvents()
                        End If
                    End If
                Else
                    If Strings.NoInternetConnection = DialogResult.Retry Then
                        Check()
                    Else
                        Application.DoEvents()
                        UserInterface.CheckFailed()
                        Application.DoEvents()
                    End If
                End If
            Catch
                Application.DoEvents()
                UserInterface.CheckFailed()
                Application.DoEvents()
                If Not Properties.Silent Then Strings.UpdateCheckFailed()
            End Try
        Else
            Try
                RemoveTemporaryFolder()
            Catch
            End Try
        End If
        Return False
    End Function
    Public Shared Sub Download(Optional ExecutableToMove As String = "", Optional UpdateURLOverride As String = "", Optional DownloadTargetOverride As String = "")
        Try
            If My.Computer.Network.IsAvailable Then
                If My.Computer.Network.Ping("www.github.com", Properties.Timeout) Then
                    If UpdateURLOverride = "" AndAlso DownloadTargetOverride = "" Then
                        If Not IO.Directory.Exists(Properties.TemporaryFolder) Then
                            IO.Directory.CreateDirectory(Properties.TemporaryFolder)
                        Else
                            IO.Directory.Delete(Properties.TemporaryFolder, True)
                            IO.Directory.CreateDirectory(Properties.TemporaryFolder)
                        End If
                    End If
                    If Properties.Asynchronous Then
                        If UpdateURLOverride = "" Then
                            TemporaryBuffer.UpdateURL = "https://github.com/" & Properties.RepositoryOwnerName & "/" & Properties.RepositoryName & "/releases/latest/download/" & Properties.AssetFile
                        Else
                            TemporaryBuffer.UpdateURL = UpdateURLOverride
                        End If
                        If DownloadTargetOverride = "" Then
                            TemporaryBuffer.DownloadTarget = Properties.TemporaryFolder & "\" & Properties.AssetFile
                        Else
                            TemporaryBuffer.DownloadTarget = DownloadTargetOverride
                        End If
                        TemporaryBuffer.AsynchronousWebClient = New Net.WebClient
                        AddHandler TemporaryBuffer.AsynchronousWebClient.DownloadProgressChanged, AddressOf Handler.DownloadProgressAsynchronous
                        AddHandler TemporaryBuffer.AsynchronousWebClient.DownloadFileCompleted, AddressOf Handler.DownloadFinishedAsynchronous
                        TemporaryBuffer.ExecutableToMove = ExecutableToMove
                        TemporaryBuffer.AsynchronousWebClient.DownloadFileAsync(New Uri(TemporaryBuffer.UpdateURL), TemporaryBuffer.DownloadTarget)
                    Else
                        Using UpdateClient As New Net.WebClient
                            If Properties.AssetParts = 1 Then
                                If UpdateURLOverride = "" Then
                                    TemporaryBuffer.UpdateURL = "https://github.com/" & Properties.RepositoryOwnerName & "/" & Properties.RepositoryName & "/releases/latest/download/" & Properties.AssetFile
                                Else
                                    TemporaryBuffer.UpdateURL = UpdateURLOverride
                                End If
                                If DownloadTargetOverride = "" Then
                                    TemporaryBuffer.DownloadTarget = Properties.TemporaryFolder & "\" & Properties.AssetFile
                                Else
                                    TemporaryBuffer.DownloadTarget = DownloadTargetOverride
                                End If
                                For i = 1 To Properties.Retries
                                    If i = Properties.Retries Then
                                        UpdateClient.DownloadFile(TemporaryBuffer.UpdateURL, TemporaryBuffer.DownloadTarget)
                                    Else
                                        Try
                                            UpdateClient.DownloadFile(TemporaryBuffer.UpdateURL, TemporaryBuffer.DownloadTarget)
                                        Catch
                                        End Try
                                    End If
                                    If IO.File.Exists(TemporaryBuffer.DownloadTarget) Then Exit For
                                Next
                                If IO.File.Exists(TemporaryBuffer.DownloadTarget) Then
                                    Dim ExtractionTarget As String = TemporaryBuffer.DownloadTarget.Replace(".zip", "")

                                    Application.DoEvents()
                                    UserInterface.UpdateStage1()
                                    Application.DoEvents()

                                    IO.Compression.ZipFile.ExtractToDirectory(TemporaryBuffer.DownloadTarget, ExtractionTarget)

                                    Application.DoEvents()
                                    UserInterface.UpdateStage2()
                                    Application.DoEvents()

                                    IO.File.Delete(TemporaryBuffer.DownloadTarget)

                                    Application.DoEvents()
                                    UserInterface.UpdateStage3()
                                    Application.DoEvents()

                                    UpdateScript.Create(ExecutableToMove, ExtractionTarget, TemporaryBuffer.LaunchExecutable, TemporaryBuffer.TargetFolder)

                                    Application.DoEvents()
                                    UserInterface.UpdateStage4()
                                    Application.DoEvents()

                                    UpdateScript.Run()
                                    End
                                Else
                                    Application.DoEvents()
                                    UserInterface.DownloadFailed()
                                    Application.DoEvents()
                                End If
                            ElseIf Properties.AssetParts > 1 Then
                                For i = 1 To Properties.AssetParts
                                    If UpdateURLOverride = "" Then
                                        TemporaryBuffer.UpdateURL = "https://github.com/" & Properties.RepositoryOwnerName & "/" & Properties.RepositoryName & "/releases/latest/download/" & Properties.AssetFile & ".s" & i.ToString("000")
                                    Else
                                        TemporaryBuffer.UpdateURL = UpdateURLOverride
                                    End If
                                    If DownloadTargetOverride = "" Then
                                        TemporaryBuffer.DownloadTarget = Properties.TemporaryFolder & "\" & Properties.AssetFile & ".s" & i.ToString("000")
                                    Else
                                        TemporaryBuffer.DownloadTarget = DownloadTargetOverride
                                    End If
                                    For i2 = 1 To Properties.Retries
                                        Try
                                            UpdateClient.DownloadFile(TemporaryBuffer.UpdateURL, TemporaryBuffer.DownloadTarget)
                                        Catch
                                        End Try
                                        If IO.File.Exists(TemporaryBuffer.DownloadTarget) Then Exit For
                                    Next
                                    If i = Properties.AssetParts Then Exit For
                                Next
                                Try
                                    For i = 1 To Properties.AssetParts
                                        TemporaryBuffer.DownloadTarget = Properties.TemporaryFolder & "\" & Properties.AssetFile & ".s" & i.ToString("000")
                                        If IO.File.Exists(TemporaryBuffer.DownloadTarget) Then
                                            Dim FileLength As Long = New IO.FileInfo(TemporaryBuffer.DownloadTarget).Length
                                            Dim Append As Boolean = False
                                            If i = 1 Then
                                                Append = False
                                            Else
                                                Append = True
                                            End If
                                            Using FileWriter As New IO.StreamWriter(Properties.TemporaryFolder & "\" & Properties.AssetFile, Append, Text.Encoding.Default)
                                                FileWriter.Write(IO.File.ReadAllText(TemporaryBuffer.DownloadTarget, Text.Encoding.Default))
                                                FileWriter.Flush()
                                                FileWriter.Close()
                                            End Using
                                            IO.File.Delete(TemporaryBuffer.DownloadTarget)
                                        Else
                                            Exit For
                                        End If
                                    Next
                                    If IO.File.Exists(Properties.TemporaryFolder & "\" & Properties.AssetFile) Then
                                        Dim ExtractionTarget As String = Properties.TemporaryFolder & "\" & Properties.AssetFile.Replace(".zip", "")

                                        Application.DoEvents()
                                        UserInterface.UpdateStage1()
                                        Application.DoEvents()

                                        IO.Compression.ZipFile.ExtractToDirectory(Properties.TemporaryFolder & "\" & Properties.AssetFile, ExtractionTarget)

                                        Application.DoEvents()
                                        UserInterface.UpdateStage2()
                                        Application.DoEvents()

                                        IO.File.Delete(Properties.TemporaryFolder & "\" & Properties.AssetFile)

                                        Application.DoEvents()
                                        UserInterface.UpdateStage3()
                                        Application.DoEvents()

                                        UpdateScript.Create(ExecutableToMove, ExtractionTarget, TemporaryBuffer.LaunchExecutable, TemporaryBuffer.TargetFolder)

                                        Application.DoEvents()
                                        UserInterface.UpdateStage4()
                                        Application.DoEvents()

                                        UpdateScript.Run()
                                        End
                                    Else
                                        Application.DoEvents()
                                        UserInterface.DownloadFailed()
                                        Application.DoEvents()
                                    End If
                                Catch
                                    Application.DoEvents()
                                    UserInterface.DownloadFailed()
                                    Application.DoEvents()
                                End Try
                            End If
                        End Using
                    End If
                Else
                    If Strings.ServerUnreachable = DialogResult.Retry Then
                        Try
                            If IO.File.Exists(TemporaryBuffer.DownloadTarget) Then IO.File.Delete(TemporaryBuffer.DownloadTarget)
                            Download(ExecutableToMove, UpdateURLOverride, DownloadTargetOverride)
                        Catch
                            Application.DoEvents()
                            UserInterface.DownloadFailed()
                            Application.DoEvents()
                        End Try
                    Else
                        Application.DoEvents()
                        UserInterface.DownloadFailed()
                        Application.DoEvents()
                    End If
                End If
            Else
                If Strings.NoInternetConnection = DialogResult.Retry Then
                    If IO.File.Exists(TemporaryBuffer.DownloadTarget) Then IO.File.Delete(TemporaryBuffer.DownloadTarget)
                    Download(ExecutableToMove, UpdateURLOverride, DownloadTargetOverride)
                Else
                    Application.DoEvents()
                    UserInterface.DownloadFailed()
                    Application.DoEvents()
                End If
            End If
        Catch
            Application.DoEvents()
            UserInterface.DownloadFailed()
            Application.DoEvents()
        End Try
    End Sub
End Class
