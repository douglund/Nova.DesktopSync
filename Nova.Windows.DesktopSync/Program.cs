using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nova.Windows.DesktopSync.Configuration;
using Nova.Windows.DesktopSync.Properties;

namespace Nova.Windows.DesktopSync
{
    internal class Program
    {
        private static SyncConfig Config => Settings.Default?.SyncConfig;

        //[STAThread]
        private static void Main(string[] args)
        {
            try
            {
                ConsoleColor.TextTitle.WriteLineCenter($"BEGIN: DesktopSync");
                ConsoleColor.TextStatus.WriteLine("Loading Configuration...");

                //Settings.Default?.Reset();
                //Settings.Default.Save();

                VerifySyncConfig();
                VerifyFolderConfig();
                foreach (var folderConfig in Config.SyncFolders)
                    FolderSync(folderConfig);
            }
            catch (Exception ex)
            {
                Console.Out.WriteException(ex);
            }
            finally
            {
                Settings.Default.Save();
                Console.WriteLine();
                ConsoleColor.TextStatus.WriteLine($"END: DesktopSync");
                Console.WriteLine();
                Console.Out.Wait();
            }
        }

        private static void FolderSync(SyncFolderConfig folderConfig)
        {
            try
            {
                ConsoleColor.TextTitle2.WriteLineCenter($"FOLDER SYNC: {folderConfig?.Name ?? "null"}", " - ", " ");
                Console.Out.WriteObject(folderConfig, nameof(SyncFolderConfig));
                if (folderConfig == null)
                    throw new ArgumentNullException(nameof(folderConfig));

                EnsureFolderExists(folderConfig.FolderPath);
                var statusQuery = Enum.GetValues(typeof(SyncStatusType)).Cast<SyncStatusType>();
                var fileResults = statusQuery.ToDictionary(st => st, (s) => new List<FileInfo>());
                var files = folderConfig.SearchPattern.Split(';', ',').SelectMany(pat => Directory.GetFiles(folderConfig.FolderPath, pat, folderConfig.SearchOption)).ToArray();
                ConsoleColor.TextStatus.WriteLine($"{files.Length} file(s) found");
                if (files.Length == 0)
                    return;

                // copy each file found in source folder
                ConsoleColor.TextStatus.Write("Copy Progress: ");
                var pinned = ConsoleState.GetCurrentState();
                for (var i = 0; i < files.Length; i++)
                {
                    var fileInfo = new FileInfo(files[i]);
                    var syncResult = FileSync(folderConfig, fileInfo);
                    fileResults[syncResult].Add(fileInfo);

                    // progress
                    if ((i + 1) % 10 == 0)
                    {
                        var percent = Math.Round((i + 1M) / files.Length * 100, 2);
                        pinned.RecallState();
                        ConsoleColor.TextValue.Write($"{percent:0.00,6}%");
                        // Thread.Sleep(900);
                    }
                }

                pinned.RecallState();
                ConsoleColor.TextValue.WriteLine("100.00% \r\n");

                // Write sync status summary to console 
                foreach (var kv in fileResults)
                {
                    if (!kv.Value.Any())
                        continue;

                    var status = Regex.Replace(kv.Key.ToString(), "(\\B[A-Z])", " $1").ToLower();
                    ConsoleColor.TextStatus.WriteLine($"{kv.Value.Count} file(s) {status}...");
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteException(ex, false);
            }
            finally
            {
                ConsoleColor.TextStatus.WriteLine($"END: FOLDER SYNC");
            }
        }

        private static SyncStatusType FileSync(SyncFolderConfig folderConfig, FileInfo sourceFile)
        {
            try
            {
                if (folderConfig.MinimumKb > 0 && folderConfig.MinimumKb > sourceFile.Length / 1000)
                    return SyncStatusType.BelowSizeLimit;
                if (folderConfig.MaximumKb > 0 && folderConfig.MaximumKb < sourceFile.Length / 1000)
                    return SyncStatusType.ExceedSizeLimit;

                var changeExtension = !string.IsNullOrEmpty(folderConfig.FixFileExtension) && string.IsNullOrWhiteSpace(sourceFile.Extension);
                var targetFilename = changeExtension
                    ? Path.ChangeExtension(sourceFile.Name, folderConfig.FixFileExtension)
                    : sourceFile.Name;
                var targetFilePath = Path.Combine(Config.TargetFolder, targetFilename);
                var exists = File.Exists(targetFilePath);
                if (exists && !Config.Overwrite)
                    return SyncStatusType.AlreadyExist;

                sourceFile.CopyTo(targetFilePath, true);
                return SyncStatusType.Copied;
            }
            catch (Exception ex)
            {
                var fileEx = new IOException($"Error synchronizing file: '{sourceFile.Name}'", ex);
                Console.Out.WriteException(fileEx, false);
                return SyncStatusType.Failed;
            }
        }

        private static void EnsureFolderExists(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentNullException(nameof(path));

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw new DirectoryNotFoundException($"Error creating folder: '{path ?? "null"}'", ex);
            }
        }

        private static void VerifySyncConfig()
        {
            if (Config == null)
            {
                Settings.Default.SyncConfig = Settings.CreateDefaultSyncConfig();
                Settings.Default.Save();
            }

            EnsureFolderExists(Config.TargetFolder);

            // Config.Overwrite = !Config.Overwrite;
            Console.Out.WriteObject(Config, nameof(Config));
        }

        private static void VerifyFolderConfig()
        {
            var pinned = ConsoleState.GetCurrentState();
            if (Config.SyncFolders == null || !Config.SyncFolders.Any())
            {
                var ex = new ConfigurationErrorsException($"SyncFolders configuration setting.");
                Console.Out.WriteException(ex, false);
                var key = Console.Out.Wait("PLEASE SELECT AN OPTION: \r\n\t[L]oad Default Settings\r\n\t[B]rowse for Folder\r\n\tE[x]it");
                switch (key.Key)
                {
                    case ConsoleKey.X:
                        throw ex;
                    case ConsoleKey.B:
                    case ConsoleKey.L:
                        var defaultSync = Settings.CreateDefaultSyncConfig();
                        Config.SyncFolders = defaultSync.SyncFolders;
                        // Settings.Default.Save();
                        break;
                    default:
                        ConsoleColor.Yellow.Write("INVALID KEY! ");
                        ConsoleColor.TextLabel.WriteLine("Please choose from the available options!");
                        pinned.RecallState();
                        VerifyFolderConfig();
                        break;
                }
            }
            //else
            //{
            //    var folder = Config.SyncFolders.First();
            //    folder.PropertyChanged += (o,e) => Console.Beep();
            //    folder.MinimumKb++;
            //}
        }
    }
}