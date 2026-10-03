using GrayWolf.Enums;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using IFileSystem = GrayWolf.Interfaces.IFileSystem;

namespace GrayWolf.Converters
{
    public static class LogFileConverters
    {
        public static LogFile ToLogFile(this LogFileDBO dbo, IFileSystem fileSystem)
        {
            if (dbo == null)
                return null;
            var folderPath = ResolveFolderPath(dbo.Name, fileSystem);
            return new LogFile
            {
                Id = dbo.Id,
                Name = dbo.Name,
                TrendLoggingActive = dbo.TrendLoggingActive,
                FolderPath = folderPath,
                LcvFilePath = Path.Combine(folderPath, $"{dbo.Name}.lcv"),
                LjhFilePath = Path.Combine(folderPath, $"{dbo.Name}.ljh"),
                HasContent = dbo.HasContent,
                LoggingInterval = dbo.LoggingInterval,
                IsGraphAvailable = dbo.IsGraphAvailable,
                IsSelected = dbo.IsSelected,
                ParameterNameDisplayMode = (ParameterNameDisplayOption)dbo.ParameterNamesDisplayMode
            };
        }

        private static string ResolveFolderPath(string logName, IFileSystem fileSystem)
        {
            var currentFolderPath = Path.Combine(fileSystem.GetAppDocumentsFolderPath(), logName);

#if ANDROID
            // Xamarin.Android mapped MyDocuments to the app's files directory.
            // .NET MAUI maps it to files/Documents, so an in-place upgrade can
            // still have its log files one directory above the new location.
            var legacyFolderPath = Path.Combine(
                Microsoft.Maui.Storage.FileSystem.AppDataDirectory,
                logName);

            if (!PathsReferToSameFolder(currentFolderPath, legacyFolderPath) &&
                CountLogFiles(legacyFolderPath, logName) > CountLogFiles(currentFolderPath, logName))
            {
                // Use the legacy folder in place. Do not move, overwrite, or delete
                // customer data while recovering an existing Xamarin installation.
                return legacyFolderPath;
            }
#endif

            return currentFolderPath;
        }

        private static int CountLogFiles(string folderPath, string logName)
        {
            var count = 0;
            if (File.Exists(Path.Combine(folderPath, $"{logName}.lcv")))
            {
                count++;
            }

            if (File.Exists(Path.Combine(folderPath, $"{logName}.ljh")))
            {
                count++;
            }

            return count;
        }

        private static bool PathsReferToSameFolder(string firstPath, string secondPath)
        {
            return string.Equals(
                Path.GetFullPath(firstPath).TrimEnd(Path.DirectorySeparatorChar),
                Path.GetFullPath(secondPath).TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase);
        }

        public static LogFileDBO ToLogFileDbo(this LogFile domain)
        {
            return new LogFileDBO
            {
                Id = domain.Id,
                Name = domain.Name,
                TrendLoggingActive = domain.TrendLoggingActive,
                HasContent = domain.HasContent,
                IsGraphAvailable = domain.IsGraphAvailable,
                LoggingInterval = domain.LoggingInterval,
                IsSelected = domain.IsSelected,
                ParameterNamesDisplayMode = (int)domain.ParameterNameDisplayMode
            };
        }
    }
}
