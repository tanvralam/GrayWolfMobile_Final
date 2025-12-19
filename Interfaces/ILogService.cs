using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GrayWolf.Interfaces
{
    public interface ILogService
    {
        LogStatus Status { get; }

        bool IsLogging { get; }

        int LogFileId { get; }

        LogFile CurrentFile { get; }

        Task<List<LogFile>> GetLogFilesAsync();

        Task StartLog(LogFile file, LogFileWriteMode writeMode = LogFileWriteMode.Default);

        Task StopLog();

        Task ExportLogAsync(int logFileID, ExportLogOptions option = ExportLogOptions.Share);

        Task SetFileAsync(int selectedLogId);

        Task<bool> IsLogFileExistAsync(int? id);

        Task DeleteLocationAsync(int logFileId);

        Task ClearLocationAsync(int logFileId);

        Task RecordSound();

        Task AddNote();

        Task TakePhoto();

        Task AddVideo();

        Task<TAttachment> AddLogAttachmentFileAsync<TAttachment>(TAttachment logAttachmentFile, bool isNew, int fileId, byte[] bytes = null)
            where TAttachment : IAttachment;

        Task<bool> AddLogFileAsync(string fileName, int intervalMS);

        Task<TextAttachment> GetNoteAsync();

        Task<DrawableAttachment> GetCurrentDrawingNoteAsync();

        Task RemoveAttachmentsAsync(IEnumerable<IAttachment> attachments);

        Task<List<IAttachment>> GetLogAttachmentFilesAsync(int logFileId);

        Task<bool> GetStoragePermissionsAsync();

        Task<LogFileWriteMode> GetLogWriteModeAsync();
        
        Task<bool> CreateSnapshotAsync(LogFile file, bool isLogButtonClicked = false);

        Task<EventAttachment> GetEventAttachmentAsync();

        Task OnArchiveEntriesSelectedAsync(List<ArchiveEntry> entries, int logFileId, ExportLogOptions option);
        
        Task<LogFile> GetLogFileAsync(int id);
        Task SetFileAsync(LogFile file);
    }
}
