Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Public Class Main
    Dim AssemblyName As String = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs(0))
    Dim SessionFile As String = My.Application.Info.DirectoryPath & "\" & AssemblyName & ".$se"
    Dim CrackCheck As Boolean = True

    Dim LooseMode As Boolean = False

    Dim SMAPIDirectories() As String = {"Mods\ConsoleCommands", "Mods\SaveBackup", "smapi-internal"}
    Dim SMAPIFiles() As String = {"StardewModdingAPI.deps.json", "StardewModdingAPI.dll", "StardewModdingAPI.exe", "StardewModdingAPI.exe.config", "StardewModdingAPI.runtimeconfig.json", "StardewModdingAPI.xml", "steam_appid.txt"}
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Hide()

        If GitHubUpdater.Check Then
            GitHubUpdater.Download("Stardew Valley Expansion Loader.exe")
        End If

        Dim Language As String = Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName

        Dim StardewValleyRoamingPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\StardewValley"
        Dim DefaultSavePath As String = StardewValleyRoamingPath & "\Saves"
        Dim VanillaSavePath As String = StardewValleyRoamingPath & "\Saves\.Vanilla"
        Dim CustomSavePath As String = ""

        Dim BinaryArchiveFilename As String = AssemblyName & ".bin"
        Dim ExpansionArchiveFilename As String = AssemblyName & ".xna"
        Dim LoadExpansions As Boolean = File.Exists(BinaryArchiveFilename) OrElse File.Exists(ExpansionArchiveFilename)

        Try
            ' # CHECK IF STARDEW VALLEY IS RUNNING #
            If Process.GetProcessesByName("Stardew Valley").Length > 0 Then
                If Language = "de" Then '     GERMAN
                    ErrorMessage("Stardew Valley läuft bereits.")
                ElseIf Language = "es" Then ' SPANISH
                    ErrorMessage("Stardew Valley ya se está ejecutando.")
                ElseIf Language = "fr" Then ' FRANCE
                    ErrorMessage("Stardew Valley est déjà lancé.")
                ElseIf Language = "hu" Then ' HUNGARIAN
                    ErrorMessage("A Stardew Valley már fut.")
                ElseIf Language = "it" Then ' ITALIAN
                    ErrorMessage("Stardew Valley è già in esecuzione.")
                ElseIf Language = "ja" Then ' JAPANESE
                    ErrorMessage("『スターデューバレー』はすでに起動しています")
                ElseIf Language = "ko" Then ' KOREAN
                    ErrorMessage("스타듀 밸리가 이미 실행 중입니다")
                ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                    ErrorMessage("O Stardew Valley já está em execução.")
                ElseIf Language = "ru" Then ' RUSSIAN
                    ErrorMessage("Stardew Valley уже запущена.")
                ElseIf Language = "tr" Then ' TURKISH
                    ErrorMessage("Stardew Valley zaten çalışıyor.")
                ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                    ErrorMessage("《星露谷物语》已经运行")
                Else '                        ENGLISH
                    ErrorMessage("Stardew Valley is already running.")
                End If
                End
            End If
        Catch
        End Try

        Try
            ' # COMMAND LINE ARGUMENTS #
            If Environment.GetCommandLineArgs(1) = "-c" OrElse
               Environment.GetCommandLineArgs(1) = "--cleanup" Then
                Cleanup()
            ElseIf Environment.GetCommandLineArgs(1) = "-cc" OrElse
                   Environment.GetCommandLineArgs(1) = "--createconfigs" Then
                Try
                    ZipFile.ExtractToDirectory(BinaryArchiveFilename, My.Application.Info.DirectoryPath)
                    File.Copy("Stardew Valley.deps.json", "StardewModdingAPI.deps.json")
                    Using StardewValley As New Process
                        StardewValley.StartInfo.FileName = "StardewModdingAPI.exe"
                        StardewValley.Start()
                        StardewValley.WaitForExit()
                    End Using
                    For Each Waste As String In SMAPIFiles
                        Try
                            File.Delete(Waste)
                        Catch
                        End Try
                    Next
                    Directory.Delete(My.Application.Info.DirectoryPath & "\Mods\ConsoleCommands", True)
                    Directory.Delete(My.Application.Info.DirectoryPath & "\Mods\SaveBackup", True)
                Catch
                    End
                End Try
            ElseIf Environment.GetCommandLineArgs(1) = "-e" OrElse
                   Environment.GetCommandLineArgs(1) = "--extract" Then
                Try
                    ZipFile.ExtractToDirectory(ExpansionArchiveFilename, My.Application.Info.DirectoryPath)
                Catch
                    End
                End Try
            ElseIf Environment.GetCommandLineArgs(1) = "-p" OrElse
                   Environment.GetCommandLineArgs(1) = "--pack" Then
                Try
                    If Directory.Exists(My.Application.Info.DirectoryPath & "\Mods\ConsoleCommands") Then Directory.Delete(My.Application.Info.DirectoryPath & "\Mods\ConsoleCommands", True)
                    If Directory.Exists(My.Application.Info.DirectoryPath & "\Mods\SaveBackup") Then Directory.Delete(My.Application.Info.DirectoryPath & "\Mods\SaveBackup", True)
                    ZipFile.CreateFromDirectory(My.Application.Info.DirectoryPath & "\Mods", AssemblyName & ".zip", CompressionLevel.Fastest, False)
                    If File.Exists(ExpansionArchiveFilename) Then File.Delete(ExpansionArchiveFilename)
                    File.Move(AssemblyName & ".zip", ExpansionArchiveFilename)
                    Directory.Delete(My.Application.Info.DirectoryPath & "\Mods", True)
                Catch
                    End
                End Try
            End If
            End
        Catch
        End Try

        ' # CHECK FOR STARDEW VALLEY EXECUTABLE #
        If Not File.Exists("Stardew Valley.exe") Then
            If Language = "de" Then '     GERMAN
                ErrorMessage("Kann Stardew Valley.exe nicht finden.")
            ElseIf Language = "es" Then ' SPANISH
                ErrorMessage("No se ha encontrado el archivo ejecutable de Stardew Valley.")
            ElseIf Language = "fr" Then ' FRANCE
                ErrorMessage("Impossible de trouver le fichier exécutable de Stardew Valley.")
            ElseIf Language = "hu" Then ' HUNGARIAN
                ErrorMessage("Nem sikerült megtalálni a Stardew Valley futtatható fájlt.")
            ElseIf Language = "it" Then ' ITALIAN
                ErrorMessage("Impossibile trovare il file eseguibile di Stardew Valley.")
            ElseIf Language = "ja" Then ' JAPANESE
                ErrorMessage("「スターデューバレー」の実行ファイルが見つかりません")
            ElseIf Language = "ko" Then ' KOREAN
                ErrorMessage("'스타듀 밸리' 실행 파일을 찾을 수 없습니다")
            ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                ErrorMessage("Não foi possível encontrar o arquivo executável do Stardew Valley.")
            ElseIf Language = "ru" Then ' RUSSIAN
                ErrorMessage("Не удалось найти исполняемый файл Stardew Valley.")
            ElseIf Language = "tr" Then ' TURKISH
                ErrorMessage("Stardew Valley çalıştırılabilir dosyası bulunamadı.")
            ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                ErrorMessage("找不到《星露谷物语》的可执行文件")
            Else '                        ENGLISH
                ErrorMessage("Unable to find Stardew Valley executable.")
            End If
            End
        End If

        If File.Exists(AssemblyName & ".ini") Then
            Try
                ' # READ CONFIG FILE #
                Dim ConfigHeader As String = String.Empty
                For Each Line As String In File.ReadAllLines(AssemblyName & ".ini")
                    If Line.StartsWith("[") AndAlso Line.EndsWith("]") Then ConfigHeader = Line
                    If ConfigHeader = "[Saving]" AndAlso Line.StartsWith("DirectoryName=") Then
                        Dim CustomSaveDirectory As String = Line.Replace("DirectoryName=", "")
                        If CustomSaveDirectory <> "" Then CustomSavePath = DefaultSavePath & "\." & CustomSaveDirectory
                    ElseIf ConfigHeader = S("W0NyYWNrQ2hlY2td") AndAlso Line = S("SmFja1NwYXJyb3c9VHJ1ZQ??") Then
                        CrackCheck = False
                    End If
                Next
            Catch
                If Language = "de" Then '     GERMAN
                    ErrorMessage("Konfigurationsdatei konnte nicht gelesen werden.")
                ElseIf Language = "es" Then ' SPANISH
                    ErrorMessage("No se puede leer el archivo de configuración.")
                ElseIf Language = "fr" Then ' FRANCE
                    ErrorMessage("Impossible de lire le fichier de configuration.")
                ElseIf Language = "hu" Then ' HUNGARIAN
                    ErrorMessage("A konfigurációs fájl nem olvasható.")
                ElseIf Language = "it" Then ' ITALIAN
                    ErrorMessage("Impossibile leggere il file di configurazione.")
                ElseIf Language = "ja" Then ' JAPANESE
                    ErrorMessage("設定ファイルを読み込めません")
                ElseIf Language = "ko" Then ' KOREAN
                    ErrorMessage("구성 파일을 읽을 수 없습니다")
                ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                    ErrorMessage("Não foi possível ler o arquivo de configuração.")
                ElseIf Language = "ru" Then ' RUSSIAN
                    ErrorMessage("Не удается прочитать файл конфигурации.")
                ElseIf Language = "tr" Then ' TURKISH
                    ErrorMessage("Yapılandırma dosyası okunamıyor.")
                ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                    ErrorMessage("无法读取配置文件")
                Else '                        ENGLISH
                    ErrorMessage("Unable to read configuration file.")
                End If
                End
            End Try
        Else
            Try
                ' # WRITE EXAMPLE CONFIG FILE #
                If Environment.GetCommandLineArgs(1) = "-ec" OrElse
                   Environment.GetCommandLineArgs(1) = "--exampleconfig" Then
                    Using ConfigWriter As New StreamWriter(AssemblyName & ".ini", False)
                        ConfigWriter.WriteLine("[Saving]")
                        ConfigWriter.Write("DirectoryName=" & AssemblyName.Replace("Stardew Valley ", ""))
                        ConfigWriter.Flush()
                        ConfigWriter.Close()
                    End Using
                End If
            Catch
            End Try
        End If

        If CrackCheck Then
            ' # CHECK IF PIRATED VERSION #
            If File.Exists("steam_api64.rne") OrElse File.Exists("steam_emu.ini") Then
                If Language = "de" Then '     GERMAN
                    ErrorMessage("Raubkopierte Versionen von Stardew Valley werden nicht unterstützt.")
                ElseIf Language = "es" Then ' SPANISH
                    ErrorMessage("No se admiten versiones piratas de Stardew Valley.")
                ElseIf Language = "fr" Then ' FRANCE
                    ErrorMessage("Les versions piratées de Stardew Valley ne sont pas prises en charge.")
                ElseIf Language = "hu" Then ' HUNGARIAN
                    ErrorMessage("A Stardew Valley kalózverziói nem támogatottak.")
                ElseIf Language = "it" Then ' ITALIAN
                    ErrorMessage("Le versioni pirata di Stardew Valley non sono supportate.")
                ElseIf Language = "ja" Then ' JAPANESE
                    ErrorMessage("『スターデューバレー』の海賊版はサポート対象外です")
                ElseIf Language = "ko" Then ' KOREAN
                    ErrorMessage("‘스타듀 밸리’의 불법 복제판은 지원되지 않습니다")
                ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                    ErrorMessage("Versões piratas do Stardew Valley não são compatíveis.")
                ElseIf Language = "ru" Then ' RUSSIAN
                    ErrorMessage("Пиратские версии игры Stardew Valley не поддерживаются.")
                ElseIf Language = "tr" Then ' TURKISH
                    ErrorMessage("Stardew Valley'in korsan sürümleri desteklenmemektedir.")
                ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                    ErrorMessage("不支持《星露谷物语》的盗版版本")
                Else '                        ENGLISH
                    ErrorMessage("Pirated versions of Stardew Valley are not supported.")
                End If
                End
            End If
        End If

        ' # CLEANUP CRASHED SESSION #
        If File.Exists(SessionFile) Then
            Cleanup()
            Try
                File.SetAttributes(SessionFile, FileAttributes.Normal)
                ' # MOVE CUSTOM SAVES #
                For Each CustomSave As String In File.ReadAllLines(SessionFile)
                    If Not CustomSave = "" Then Directory.Move(DefaultSavePath & "\" & CustomSave, CustomSavePath & "\" & CustomSave)
                Next
                File.SetAttributes(SessionFile, FileAttributes.Hidden)
                ' # RESTORE VANILLA SAVES #
                For Each VanillaSave As String In Directory.GetDirectories(VanillaSavePath)
                    Directory.Move(VanillaSave, DefaultSavePath & "\" & Path.GetFileName(VanillaSave))
                Next
                Directory.Delete(VanillaSavePath)
                File.Delete(SessionFile)
            Catch
                If Language = "de" Then '     GERMAN
                    ErrorMessage("Die abgestürzte Sitzung kann nicht bereinigt werden.")
                ElseIf Language = "es" Then ' SPANISH
                    ErrorMessage("No se puede limpiar la sesión que se ha bloqueado.")
                ElseIf Language = "fr" Then ' FRANCE
                    ErrorMessage("Impossible de nettoyer la session qui a planté.")
                ElseIf Language = "hu" Then ' HUNGARIAN
                    ErrorMessage("A leállt munkamenet nem tisztítható.")
                ElseIf Language = "it" Then ' ITALIAN
                    ErrorMessage("Impossibile ripulire la sessione interrotta.")
                ElseIf Language = "ja" Then ' JAPANESE
                    ErrorMessage("クラッシュしたセッションをクリーンアップできません")
                ElseIf Language = "ko" Then ' KOREAN
                    ErrorMessage("종료되지 않은 세션을 정리할 수 없습니다")
                ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                    ErrorMessage("Não foi possível limpar a sessão que travou.")
                ElseIf Language = "ru" Then ' RUSSIAN
                    ErrorMessage("Не удалось очистить сеанс, завершившийся сбоем.")
                ElseIf Language = "tr" Then ' TURKISH
                    ErrorMessage("Çöken oturum temizlenemiyor.")
                ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                    ErrorMessage("无法清理崩溃的会话")
                Else '                        ENGLISH
                    ErrorMessage("Unable to cleanup crashed session.")
                End If
                End
            End Try
        End If

        If LoadExpansions Then
            ' # CHECK FOR EXISTING MODS #
            If Directory.Exists("Mods") OrElse Directory.Exists("smapi-internal") OrElse File.Exists("StardewModdingAPI.exe") Then
                If Language = "de" Then '     GERMAN
                    ErrorMessage("Kann nicht mit modifiziertem Spiel ausgeführt werden.")
                ElseIf Language = "es" Then ' SPANISH
                    ErrorMessage("No se puede ejecutar en una versión modificada del juego.")
                ElseIf Language = "fr" Then ' FRANCE
                    ErrorMessage("Impossible de lancer le jeu sur une version modifiée.")
                ElseIf Language = "hu" Then ' HUNGARIAN
                    ErrorMessage("A módosított játékon nem futtatható.")
                ElseIf Language = "it" Then ' ITALIAN
                    ErrorMessage("Impossibile eseguire il gioco su una versione modificata.")
                ElseIf Language = "ja" Then ' JAPANESE
                    ErrorMessage("改造されたゲームでは実行できません")
                ElseIf Language = "ko" Then ' KOREAN
                    ErrorMessage("수정된 게임에서는 실행할 수 없습니다")
                ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                    ErrorMessage("Não é possível executar em uma versão modificada do jogo.")
                ElseIf Language = "ru" Then ' RUSSIAN
                    ErrorMessage("Не удается запустить модифицированную игру.")
                ElseIf Language = "tr" Then ' TURKISH
                    ErrorMessage("Değiştirilmiş bir oyunda çalıştırılamıyor.")
                ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                    ErrorMessage("无法在修改过的游戏中运行")
                Else '                        ENGLISH
                    ErrorMessage("Unable to run on a modified game.")
                End If
                End
            Else
                If Directory.Exists(My.Application.Info.DirectoryPath & "\" & AssemblyName) Then LooseMode = True

                If LooseMode Then
                    ' # MOVE MOD DATA
                    Try
                        Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\Mods")
                        For Each Extension As String In Directory.GetDirectories(My.Application.Info.DirectoryPath & "\" & AssemblyName & "\Mods", "*.*", SearchOption.TopDirectoryOnly)
                            Directory.Move(Extension, My.Application.Info.DirectoryPath & "\Mods\" & Path.GetFileName(Extension))
                        Next
                        Directory.Delete(My.Application.Info.DirectoryPath & "\" & AssemblyName, True)
                    Catch
                        If Language = "de" Then '     GERMAN
                            ErrorMessage("Erweiterungen konnten nicht verschoben werden.")
                        ElseIf Language = "es" Then ' SPANISH
                            ErrorMessage("No se pueden mover las extensiones.")
                        ElseIf Language = "fr" Then ' FRANCE
                            ErrorMessage("Impossible de déplacer les extensions.")
                        ElseIf Language = "hu" Then ' HUNGARIAN
                            ErrorMessage("A kiterjesztések áthelyezése nem lehetséges.")
                        ElseIf Language = "it" Then ' ITALIAN
                            ErrorMessage("Impossibile spostare le estensioni.")
                        ElseIf Language = "ja" Then ' JAPANESE
                            ErrorMessage("拡張機能を移動できない")
                        ElseIf Language = "ko" Then ' KOREAN
                            ErrorMessage("확장 프로그램을 이동할 수 없음")
                        ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                            ErrorMessage("Não é possível mover extensões.")
                        ElseIf Language = "ru" Then ' RUSSIAN
                            ErrorMessage("Невозможно переместить расширения.")
                        ElseIf Language = "tr" Then ' TURKISH
                            ErrorMessage("Uzantıları taşıyamıyorum.")
                        ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                            ErrorMessage("无法移动扩展")
                        Else '                        ENGLISH
                            ErrorMessage("Unable move extensions.")
                        End If
                        GoTo Cleanup
                    End Try
                End If

                Try
                    ' # EXTRACT SMAPI BINARIES #
                    ZipFile.ExtractToDirectory(BinaryArchiveFilename, My.Application.Info.DirectoryPath)
                    File.Create(SessionFile).Close()
                    File.SetAttributes(SessionFile, FileAttributes.Hidden)
                Catch
                    If Language = "de" Then '     GERMAN
                        ErrorMessage("Binärdateien konnten nicht extrahiert werden.")
                    ElseIf Language = "es" Then ' SPANISH
                        ErrorMessage("No se pueden extraer los archivos binarios.")
                    ElseIf Language = "fr" Then ' FRANCE
                        ErrorMessage("Impossible d'extraire les fichiers binaires.")
                    ElseIf Language = "hu" Then ' HUNGARIAN
                        ErrorMessage("A bináris fájlok kibontása nem sikerült.")
                    ElseIf Language = "it" Then ' ITALIAN
                        ErrorMessage("Impossibile estrarre i file binari.")
                    ElseIf Language = "ja" Then ' JAPANESE
                        ErrorMessage("バイナリを抽出できません")
                    ElseIf Language = "ko" Then ' KOREAN
                        ErrorMessage("바이너리 파일을 추출할 수 없음")
                    ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                        ErrorMessage("Não é possível extrair os arquivos binários.")
                    ElseIf Language = "ru" Then ' RUSSIAN
                        ErrorMessage("Не удается извлечь двоичные файлы.")
                    ElseIf Language = "tr" Then ' TURKISH
                        ErrorMessage("İkili dosyalar çıkarılamıyor.")
                    ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                        ErrorMessage("无法提取二进制文件")
                    Else '                        ENGLISH
                        ErrorMessage("Unable extract binaries.")
                    End If
                    GoTo Cleanup
                End Try

                If Not LooseMode Then
                    Try
                        ' # EXTRACT MOD DATA #
                        ZipFile.ExtractToDirectory(ExpansionArchiveFilename, My.Application.Info.DirectoryPath)
                    Catch
                        If Language = "de" Then '     GERMAN
                            ErrorMessage("Erweiterungen konnten nicht extrahiert werden.")
                        ElseIf Language = "es" Then ' SPANISH
                            ErrorMessage("No se pueden extraer las extensiones.")
                        ElseIf Language = "fr" Then ' FRANCE
                            ErrorMessage("Impossible d'extraire les extensions.")
                        ElseIf Language = "hu" Then ' HUNGARIAN
                            ErrorMessage("A kiterjesztések nem tudók kinyerni.")
                        ElseIf Language = "it" Then ' ITALIAN
                            ErrorMessage("Impossibile estrarre le estensioni.")
                        ElseIf Language = "ja" Then ' JAPANESE
                            ErrorMessage("拡張子を抽出できません")
                        ElseIf Language = "ko" Then ' KOREAN
                            ErrorMessage("확장자를 추출할 수 없음")
                        ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                            ErrorMessage("Não é possível extrair extensões.")
                        ElseIf Language = "ru" Then ' RUSSIAN
                            ErrorMessage("Невозможно извлечь расширения.")
                        ElseIf Language = "tr" Then ' TURKISH
                            ErrorMessage("Uzantıları ayıklanamıyor.")
                        ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                            ErrorMessage("无法提取扩展名")
                        Else '                        ENGLISH
                            ErrorMessage("Unable extract extensions.")
                        End If
                        GoTo Cleanup
                    End Try
                Else

                End If

                Try
                    ' # COPY DEPENDENCIES #
                    File.Copy("Stardew Valley.deps.json", "StardewModdingAPI.deps.json")
                Catch
                    If Language = "de" Then '     GERMAN
                        ErrorMessage("Abhängigkeiten konnten nicht kopiert werden.")
                    ElseIf Language = "es" Then ' SPANISH
                        ErrorMessage("No se pueden copiar las dependencias.")
                    ElseIf Language = "fr" Then ' FRANCE
                        ErrorMessage("Impossible de copier les dépendances.")
                    ElseIf Language = "hu" Then ' HUNGARIAN
                        ErrorMessage("A függőségek másolása nem sikerült.")
                    ElseIf Language = "it" Then ' ITALIAN
                        ErrorMessage("Impossibile copiare le dipendenze.")
                    ElseIf Language = "ja" Then ' JAPANESE
                        ErrorMessage("依存関係をコピーできません")
                    ElseIf Language = "ko" Then ' KOREAN
                        ErrorMessage("종속성을 복사할 수 없습니다")
                    ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                        ErrorMessage("Não foi possível copiar as dependências.")
                    ElseIf Language = "ru" Then ' RUSSIAN
                        ErrorMessage("Не удалось скопировать зависимости.")
                    ElseIf Language = "tr" Then ' TURKISH
                        ErrorMessage("Bağımlılıklar kopyalanamıyor.")
                    ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                        ErrorMessage("无法复制依赖项")
                    Else '                        ENGLISH
                        ErrorMessage("Unable to copy dependencies.")
                    End If
                    GoTo Cleanup
                End Try
            End If
        End If

        If CustomSavePath <> "" Then
            Try
                ' # CREATE NECESSARY DIRECTORIES #
                If Not Directory.Exists(StardewValleyRoamingPath) Then Directory.CreateDirectory(StardewValleyRoamingPath)
                If Not Directory.Exists(DefaultSavePath) Then Directory.CreateDirectory(DefaultSavePath)
                If Not Directory.Exists(VanillaSavePath) Then Directory.CreateDirectory(VanillaSavePath)
                If Not Directory.Exists(CustomSavePath) Then Directory.CreateDirectory(CustomSavePath)
            Catch
                If Language = "de" Then '     GERMAN
                    ErrorMessage("Benötigte Benutzerverzeichnisse konnten nicht erstellt werden.")
                ElseIf Language = "es" Then ' SPANISH
                    ErrorMessage("No se pueden crear los directorios de usuario necesarios.")
                ElseIf Language = "fr" Then ' FRANCE
                    ErrorMessage("Impossible de créer les répertoires utilisateur requis.")
                ElseIf Language = "hu" Then ' HUNGARIAN
                    ErrorMessage("Nem sikerült létrehozni a szükséges felhasználói könyvtárakat.")
                ElseIf Language = "it" Then ' ITALIAN
                    ErrorMessage("Impossibile creare le directory utente necessarie.")
                ElseIf Language = "ja" Then ' JAPANESE
                    ErrorMessage("必要なユーザーディレクトリを作成できません")
                ElseIf Language = "ko" Then ' KOREAN
                    ErrorMessage("필요한 사용자 디렉터리를 생성할 수 없습니다")
                ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                    ErrorMessage("Não foi possível criar os diretórios de usuário necessários.")
                ElseIf Language = "ru" Then ' RUSSIAN
                    ErrorMessage("Не удалось создать необходимые каталоги пользователей.")
                ElseIf Language = "tr" Then ' TURKISH
                    ErrorMessage("Gerekli kullanıcı dizinleri oluşturulamıyor.")
                ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                    ErrorMessage("无法创建必要的用户目录")
                Else '                        ENGLISH
                    ErrorMessage("Unable to create neccesary user directories.")
                End If
                GoTo Cleanup
            End Try

            Try
                ' # MOVE VANILLA SAVES #
                For Each VanillaSave As String In Directory.GetDirectories(DefaultSavePath)
                    If Not Path.GetFileName(VanillaSave).StartsWith(".") Then
                        Directory.Move(VanillaSave, VanillaSavePath & "\" & Path.GetFileName(VanillaSave))
                    End If
                Next
                ' # RESTORE CUSTOM SAVES #
                If File.Exists(SessionFile) Then File.SetAttributes(SessionFile, FileAttributes.Normal)
                Using SafetyWriter As New StreamWriter(SessionFile, False)
                    For Each CustomSave As String In Directory.GetDirectories(CustomSavePath)
                        Directory.Move(CustomSave, DefaultSavePath & "\" & Path.GetFileName(CustomSave))
                        SafetyWriter.WriteLine(Path.GetFileName(CustomSave))
                    Next
                    SafetyWriter.Flush()
                    SafetyWriter.Close()
                End Using
                File.SetAttributes(SessionFile, FileAttributes.Hidden)
            Catch
                If Language = "de" Then '     GERMAN
                    ErrorMessage("Spielstände konnten nicht ausgetauscht werden.")
                ElseIf Language = "es" Then ' SPANISH
                    ErrorMessage("No se pueden intercambiar las partidas guardadas.")
                ElseIf Language = "fr" Then ' FRANCE
                    ErrorMessage("Impossible d'échanger les sauvegardes.")
                ElseIf Language = "hu" Then ' HUNGARIAN
                    ErrorMessage("A mentések cseréje nem lehetséges.")
                ElseIf Language = "it" Then ' ITALIAN
                    ErrorMessage("Impossibile scambiare i salvataggi.")
                ElseIf Language = "ja" Then ' JAPANESE
                    ErrorMessage("セーブデータの入れ替えができない")
                ElseIf Language = "ko" Then ' KOREAN
                    ErrorMessage("저장 데이터를 교체할 수 없습니다")
                ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                    ErrorMessage("Não é possível trocar os arquivos de salvamento.")
                ElseIf Language = "ru" Then ' RUSSIAN
                    ErrorMessage("Не удается обменять сохранения.")
                ElseIf Language = "tr" Then ' TURKISH
                    ErrorMessage("Kaydedilmiş oyunları değiştiremiyorum.")
                ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                    ErrorMessage("无法交换存档")
                Else '                        ENGLISH
                    ErrorMessage("Unable to swap savegames.")
                End If
                GoTo RestoreSaves
            End Try
        End If

        GC.Collect(GC.MaxGeneration)

        Try
            ' # START STARDEW VALLEY #
            Using StardewValley As New Process
                If LoadExpansions Then
                    StardewValley.StartInfo.FileName = "StardewModdingAPI.exe"
                Else
                    StardewValley.StartInfo.FileName = "Stardew Valley.exe"
                End If
                StardewValley.Start()
                StardewValley.WaitForExit()
            End Using
        Catch
            If Language = "de" Then '     GERMAN
                ErrorMessage("Stardew Valley konnte nicht gestartet werden.")
            ElseIf Language = "es" Then ' SPANISH
                ErrorMessage("No se puede iniciar Stardew Valley.")
            ElseIf Language = "fr" Then ' FRANCE
                ErrorMessage("Impossible de lancer Stardew Valley.")
            ElseIf Language = "hu" Then ' HUNGARIAN
                ErrorMessage("A Stardew Valley nem indítható el.")
            ElseIf Language = "it" Then ' ITALIAN
                ErrorMessage("Impossibile avviare Stardew Valley.")
            ElseIf Language = "ja" Then ' JAPANESE
                ErrorMessage("『スターデューバレー』を起動できません")
            ElseIf Language = "ko" Then ' KOREAN
                ErrorMessage("'스타듀 밸리'를 실행할 수 없습니다")
            ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                ErrorMessage("Não é possível iniciar o Stardew Valley.")
            ElseIf Language = "ru" Then ' RUSSIAN
                ErrorMessage("Не удается запустить Stardew Valley.")
            ElseIf Language = "tr" Then ' TURKISH
                ErrorMessage("Stardew Valley başlatılamıyor.")
            ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                ErrorMessage("无法启动《星露谷物语》")
            Else '                        ENGLISH
                ErrorMessage("Unable to launch Stardew Valley.")
            End If
        End Try
RestoreSaves:
        If CustomSavePath <> "" Then
            Try
                ' # MOVE CUSTOM SAVES #
                For Each CustomSave As String In Directory.GetDirectories(DefaultSavePath)
                    If Not Path.GetFileName(CustomSave).StartsWith(".") Then
                        Directory.Move(CustomSave, CustomSavePath & "\" & Path.GetFileName(CustomSave))
                    End If
                Next
                ' # RESTORE VANILLA SAVES #
                For Each VanillaSave As String In Directory.GetDirectories(VanillaSavePath)
                    Directory.Move(VanillaSave, DefaultSavePath & "\" & Path.GetFileName(VanillaSave))
                Next
                Directory.Delete(VanillaSavePath)
                If File.Exists(SessionFile) Then File.SetAttributes(SessionFile, FileAttributes.Normal)
                Using SafetyWriter As New StreamWriter(SessionFile, False)
                    SafetyWriter.Write("")
                    SafetyWriter.Flush()
                    SafetyWriter.Close()
                End Using
                File.SetAttributes(SessionFile, FileAttributes.Hidden)
            Catch
                If Language = "de" Then '     GERMAN
                    ErrorMessage("Spielstände konnten nicht wiederhergestellt werden.")
                ElseIf Language = "es" Then ' SPANISH
                    ErrorMessage("No se pueden restaurar las partidas guardadas.")
                ElseIf Language = "fr" Then ' FRANCE
                    ErrorMessage("Impossible de restaurer les sauvegardes.")
                ElseIf Language = "hu" Then ' HUNGARIAN
                    ErrorMessage("A mentések visszaállítása nem sikerült.")
                ElseIf Language = "it" Then ' ITALIAN
                    ErrorMessage("Impossibile ripristinare i salvataggi.")
                ElseIf Language = "ja" Then ' JAPANESE
                    ErrorMessage("セーブデータを復元できません")
                ElseIf Language = "ko" Then ' KOREAN
                    ErrorMessage("저장 데이터를 복원할 수 없습니다")
                ElseIf Language = "pt" Then ' PORTUGUESE (BRASILIAN)
                    ErrorMessage("Não é possível restaurar os jogos salvos.")
                ElseIf Language = "ru" Then ' RUSSIAN
                    ErrorMessage("Не удается восстановить сохраненные игры.")
                ElseIf Language = "tr" Then ' TURKISH
                    ErrorMessage("Kaydedilmiş oyunları geri yükleyemiyorum.")
                ElseIf Language = "zh" Then ' CHINESE (SIMPLIFIED)
                    ErrorMessage("无法恢复存档")
                Else '                        ENGLISH
                    ErrorMessage("Unable to restore savegames.")
                End If
                End
            End Try
        End If
Cleanup:
        ' # REMOVE LEFTOVER FILES #
        If LoadExpansions Then Cleanup(True)
        End
    End Sub
    Private Sub ErrorMessage(Message As String)
        If CrackCheck Then
            MessageBox.Show(Message, AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Dim RNG As New Random
            Select Case RNG.Next(0, 2)
                Case 0
                    MessageBox.Show("Yarrr!", AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Case 1
                    MessageBox.Show("Ahoi!", AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select
        End If
    End Sub
    Private Sub Cleanup(Optional EndApplication As Boolean = False)
        For Each Waste As String In SMAPIDirectories
            Try
                Directory.Delete(My.Application.Info.DirectoryPath & "\" & Waste, True)
            Catch
            End Try
        Next
        For Each Waste As String In SMAPIFiles
            Try
                File.Delete(My.Application.Info.DirectoryPath & "\" & Waste)
            Catch
            End Try
        Next
        If LooseMode Then
            Try
                Directory.CreateDirectory(My.Application.Info.DirectoryPath & "\" & AssemblyName)
                Directory.Move(My.Application.Info.DirectoryPath & "\Mods", My.Application.Info.DirectoryPath & "\" & AssemblyName & "\Mods")
            Catch
            End Try
        End If
        Try
            Directory.Delete(My.Application.Info.DirectoryPath & "\Mods", True)
        Catch
        End Try
        If EndApplication Then
            Try
                If File.ReadAllLines(SessionFile).Length = 0 Then File.Delete(SessionFile)
            Catch
            End Try
        End If
    End Sub
    Private Function S(Text As String) As String
        Try
            Dim Bytes() As Byte = System.Convert.FromBase64String(Text.Replace("?", "="))
            Return System.Text.Encoding.UTF8.GetString(Bytes)
        Catch
            Return "Error"
        End Try
    End Function
End Class
