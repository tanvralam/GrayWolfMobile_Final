using GrayWolf.Interfaces;
using SQLite;
using System;
using System.IO;
using Microsoft.Maui.Storage;

namespace GrayWolf.Services
{
    public class SQLiteConn : ISQLite
    {
        private const string DatabaseFileName = "mydatabase.sqlite";

        public SQLiteAsyncConnection GetConnection()
        {
            return new SQLiteAsyncConnection(ResolveDatabasePath());
        }

        private static string ResolveDatabasePath()
        {
            var currentPath = Path.Combine(
                Microsoft.Maui.Storage.FileSystem.AppDataDirectory,
                DatabaseFileName);

#if ANDROID
            // Xamarin.Android stored the database under SpecialFolder.ApplicationData.
            // Keep using that database when it exists so an in-place MAUI upgrade does
            // not present a new, empty database. Neither database is moved or deleted.
            var legacyDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData,
                Environment.SpecialFolderOption.DoNotVerify);
            var legacyPath = Path.Combine(legacyDirectory, DatabaseFileName);

            if (!PathsReferToSameFile(currentPath, legacyPath) && IsGrayWolfDatabase(legacyPath))
            {
                if (IsGrayWolfDatabase(currentPath))
                {
                    DatabaseRecovery.MergeMauiDatabaseIntoLegacyDatabase(
                        legacyPath,
                        currentPath);
                }

                return legacyPath;
            }
#endif

            return currentPath;
        }

        private static bool PathsReferToSameFile(string firstPath, string secondPath)
        {
            return string.Equals(
                Path.GetFullPath(firstPath),
                Path.GetFullPath(secondPath),
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsGrayWolfDatabase(string path)
        {
            if (!File.Exists(path) || new FileInfo(path).Length == 0)
            {
                return false;
            }

            try
            {
                using var connection = new SQLiteConnection(path, SQLiteOpenFlags.ReadOnly);
                return connection.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM sqlite_master " +
                    "WHERE type = 'table' AND name IN " +
                    "('LogFileDBO', 'AttachmentDBO', 'GrayWolfDeviceDBO', " +
                    "'LogRowDBO', 'ReadingDBO', 'UnitConversionDBO')") > 0;
            }
            catch
            {
                // Leave an unreadable legacy file untouched and use the MAUI database.
                return false;
            }
        }
    }
}
