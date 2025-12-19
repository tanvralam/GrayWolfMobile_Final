using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SelectAttachmentFilesPopupViewModel : BasePopupViewModel
    {
        private List<ArchiveEntry> _entries;
        public List<ArchiveEntry> Entries
        {
            get => _entries;
            private set => SetProperty(ref _entries, value);
        }

        public List<ArchiveEntry> MediaEntries => Entries?.Where(IsLargeFile)?.ToList() ?? new List<ArchiveEntry>();

        private int LogFileId { get; }

        private ExportLogOptions Option { get; }

        private ICommand _confirmCommand;
        public ICommand ConfirmCommand =>
          _confirmCommand = _confirmCommand ?? new Command(Confirm);

        private ICommand _entryTappedCommand;
        public ICommand EntryTappedCommand =>
          _entryTappedCommand = _entryTappedCommand ?? new Command<ArchiveEntry>((entry) => entry.IsSelected = !entry.IsSelected);

        public SelectAttachmentFilesPopupViewModel(List<ArchiveEntry> entries, int logFileId, ExportLogOptions option) : base()
        {
            Entries = entries;
            LogFileId = logFileId;
            Option = option;
        }

        private bool IsLargeFile(ArchiveEntry entry)
        {
            var extension = Path.GetExtension(entry.FilePath);
            return extension == Services.LogService.PhotoExtension || extension == Services.LogService.VideoExtension;
        }

        private bool NotLargeFile(ArchiveEntry entry) => !IsLargeFile(entry);

        private async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                var selectedFiles = Entries
                    .Where(NotLargeFile)
                    .ToList();
                selectedFiles.AddRange(MediaEntries.Where(x => x.IsSelected));
                await OnBacksAsync();
                await LogService.OnArchiveEntriesSelectedAsync(selectedFiles, LogFileId, Option);
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }

            IsBusy = false;
        }
    }
}
