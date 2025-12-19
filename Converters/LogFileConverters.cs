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
            var folderPath = Path.Combine(fileSystem.GetAppDocumentsFolderPath(), dbo.Name);
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
