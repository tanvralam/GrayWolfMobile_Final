using GrayWolf.Enums;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public interface ISettingsService
    {
        string BaseUrlPrefix { set; get; }
        string BaseUrl { get; }
        string Email { set; get; }
        string Password { set; get; }
        int LoggerIntervalMs { get; set; }
        int LogFileId { get; set; }
        GraphTimeOption GraphRange { get; set; }
        string SelectedGraphParameterId { get; set; }
        ZipProtectionMode ProtectionMode { get; set; }
        bool IncludeCsvIntoExport { get; set; }
        Task SetCustomZipPasswordAsync(string password);
        Task<string> GetCustomZipPasswordAsync();
        bool IsDemoMode { get; set; }
        bool IsFirstDemoRun { get; set; }
        ParameterNameDisplayOption ParameterNameDisplayMode { get; set; }
        bool AreGalleryAttachmentsEnabled { get; set; }
        bool ShowWelcomeMessage { get; set; }
        bool EnableSnapshotFromDSIILogButton { get; set; }
    }
}