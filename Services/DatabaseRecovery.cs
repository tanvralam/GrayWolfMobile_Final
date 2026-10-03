using GrayWolf.Helpers;
using GrayWolf.Models.DBO;
using SQLite;
using System.Diagnostics;

namespace GrayWolf.Services
{
    internal static class DatabaseRecovery
    {
        private const string MigrationId = "xamarin-and-maui-database-merge-v1";

        public static void MergeMauiDatabaseIntoLegacyDatabase(
            string legacyDatabasePath,
            string mauiDatabasePath)
        {
            try
            {
                CreateBackupIfMissing(legacyDatabasePath, ".before-maui-merge.bak");
                CreateBackupIfMissing(mauiDatabasePath, ".before-xamarin-merge.bak");

                using var legacy = new SQLiteConnection(legacyDatabasePath);
                legacy.CreateTable<DatabaseMigrationRecord>();

                if (legacy.Find<DatabaseMigrationRecord>(MigrationId) != null)
                {
                    return;
                }

                using var maui = new SQLiteConnection(mauiDatabasePath);
                EnsureTables(legacy);
                EnsureTables(maui);

                var legacyLogs = legacy.Table<LogFileDBO>().ToList();
                var mauiLogs = maui.Table<LogFileDBO>().ToList();
                var mauiAttachments = maui.Table<AttachmentDBO>().ToList();
                var importedLogs = PrepareImportedLogs(legacyLogs, mauiLogs);

                legacy.RunInTransaction(() =>
                {
                    var loggerIdMap = new Dictionary<int, int>();

                    foreach (var imported in importedLogs)
                    {
                        var sourceId = imported.Log.Id;
                        imported.Log.Id = 0;
                        imported.Log.IsSelected = false;
                        legacy.Insert(imported.Log);
                        loggerIdMap[sourceId] = imported.Log.Id;
                    }

                    foreach (var attachment in mauiAttachments)
                    {
                        if (!loggerIdMap.TryGetValue(attachment.LoggerId, out var newLoggerId))
                        {
                            continue;
                        }

                        attachment.Id = 0;
                        attachment.LoggerId = newLoggerId;
                        legacy.Insert(attachment);
                    }

                    UpsertAll(legacy, maui.Table<GrayWolfDeviceDBO>().ToList());
                    UpsertAll(legacy, maui.Table<ReadingDBO>().ToList());
                    UpsertAll(legacy, maui.Table<UnitConversionDBO>().ToList());
                    UpsertAll(legacy, maui.Table<LogRowDBO>().ToList());

                    legacy.Insert(new DatabaseMigrationRecord
                    {
                        Id = MigrationId,
                        CompletedUtc = DateTime.UtcNow
                    });
                });
            }
            catch (Exception ex)
            {
                // Recovery must never prevent the app from opening the untouched
                // Xamarin database. Both original databases and their backups remain.
                Debug.WriteLine($"Database recovery merge failed: {ex}");
            }
        }

        private static List<ImportedLog> PrepareImportedLogs(
            List<LogFileDBO> legacyLogs,
            List<LogFileDBO> mauiLogs)
        {
            var imported = new List<ImportedLog>();
            var usedNames = new HashSet<string>(
                legacyLogs.Select(log => log.Name),
                StringComparer.OrdinalIgnoreCase);

            var currentDocumentsRoot = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments,
                Environment.SpecialFolderOption.Create);

            foreach (var mauiLog in mauiLogs)
            {
                var originalName = mauiLog.Name;
                var importedName = GetUniqueName(originalName, usedNames);

                if (!string.Equals(originalName, importedName, StringComparison.Ordinal))
                {
                    CopyLogFolder(
                        Path.Combine(currentDocumentsRoot, originalName),
                        Path.Combine(currentDocumentsRoot, importedName),
                        originalName,
                        importedName);
                    mauiLog.Name = importedName;
                }

                usedNames.Add(importedName);
                imported.Add(new ImportedLog(mauiLog));
            }

            return imported;
        }

        private static string GetUniqueName(string originalName, HashSet<string> usedNames)
        {
            if (!usedNames.Contains(originalName))
            {
                return originalName;
            }

            for (var index = 1; ; index++)
            {
                var suffix = index == 1 ? " (2.1)" : $" (2.1-{index})";
                var maximumBaseLength = Math.Max(
                    1,
                    Constants.MAX_FILE_LOG_NAME_LENGTH - suffix.Length);
                var baseName = originalName.Length > maximumBaseLength
                    ? originalName[..maximumBaseLength]
                    : originalName;
                var candidate = $"{baseName}{suffix}";

                if (!usedNames.Contains(candidate))
                {
                    return candidate;
                }
            }
        }

        private static void CopyLogFolder(
            string sourceFolder,
            string destinationFolder,
            string originalLogName,
            string destinationLogName)
        {
            if (!Directory.Exists(sourceFolder))
            {
                return;
            }

            Directory.CreateDirectory(destinationFolder);

            foreach (var sourceFile in Directory.EnumerateFiles(sourceFolder))
            {
                var fileName = Path.GetFileName(sourceFile);
                if (string.Equals(fileName, $"{originalLogName}.lcv", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = $"{destinationLogName}.lcv";
                }
                else if (string.Equals(fileName, $"{originalLogName}.ljh", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = $"{destinationLogName}.ljh";
                }

                var destinationFile = Path.Combine(destinationFolder, fileName);
                if (!File.Exists(destinationFile))
                {
                    File.Copy(sourceFile, destinationFile);
                }
            }

            foreach (var sourceDirectory in Directory.EnumerateDirectories(sourceFolder))
            {
                CopyDirectory(
                    sourceDirectory,
                    Path.Combine(destinationFolder, Path.GetFileName(sourceDirectory)));
            }
        }

        private static void CopyDirectory(string sourceFolder, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);

            foreach (var sourceFile in Directory.EnumerateFiles(sourceFolder))
            {
                var destinationFile = Path.Combine(destinationFolder, Path.GetFileName(sourceFile));
                if (!File.Exists(destinationFile))
                {
                    File.Copy(sourceFile, destinationFile);
                }
            }

            foreach (var sourceDirectory in Directory.EnumerateDirectories(sourceFolder))
            {
                CopyDirectory(
                    sourceDirectory,
                    Path.Combine(destinationFolder, Path.GetFileName(sourceDirectory)));
            }
        }

        private static void UpsertAll<T>(SQLiteConnection connection, IEnumerable<T> rows)
        {
            foreach (var row in rows)
            {
                connection.InsertOrReplace(row);
            }
        }

        private static void EnsureTables(SQLiteConnection connection)
        {
            connection.CreateTable<LogFileDBO>();
            connection.CreateTable<AttachmentDBO>();
            connection.CreateTable<GrayWolfDeviceDBO>();
            connection.CreateTable<LogRowDBO>();
            connection.CreateTable<ReadingDBO>();
            connection.CreateTable<UnitConversionDBO>();
        }

        private static void CreateBackupIfMissing(string databasePath, string suffix)
        {
            var backupPath = databasePath + suffix;
            if (!File.Exists(backupPath))
            {
                File.Copy(databasePath, backupPath);
            }
        }

        private sealed class DatabaseMigrationRecord
        {
            [PrimaryKey]
            public string Id { get; set; }

            public DateTime CompletedUtc { get; set; }
        }

        private sealed record ImportedLog(LogFileDBO Log);
    }
}
