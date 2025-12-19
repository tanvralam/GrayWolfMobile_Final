using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using GrayWolf.Services;
using GrayWolf.Views.Popups;
using MediaManager;
using MvvmHelpers;
using Newtonsoft.Json;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class DataGridPageViewModel : BaseViewModel
    {
        #region variables
        private DataTable _dataTable;
        public DataTable DataTable
        {
            get => _dataTable;
            set => SetProperty(ref _dataTable, value);
        }

        private List<DataGridSet> _sets;
        public List<DataGridSet> Sets
        {
            get => _sets;
            private set => SetProperty(ref _sets, value);
        }

        private DataGridSet _set;
        public DataGridSet Set
        {
            get => _set;
            private set => SetProperty(ref _set, value);
        }

        private bool _setSelectionVisible;
        public bool SetSelectionVisible
        {
            get => _setSelectionVisible;
            private set => SetProperty(ref _setSelectionVisible, value);
        }

        private ObservableRangeCollection<AttachmentDBO> _logAttachment; 

        public ObservableRangeCollection<AttachmentDBO> LogAttachment
        {
            get
            {
                return _logAttachment;
            }
            set
            {
                _logAttachment = value;
                OnPropertyChanged("LogAttachment");
            }
        }

        private List<AttachmentDBO> lstAttachments;

        private AttachmentDBO _attachment;

        public AttachmentDBO Attachment
        {
            get { return _attachment; }
            set 
            { 
                _attachment = value;
                OnPropertyChanged("Attachment");
                SelectedItemChanged();
            }
        }

        private INavigationService NavigationService { get; }
        #endregion

        #region commands
        public ICommand SelectSetCommand { get; }
        static DataGridPageViewModel viewModel;
        public static LogFile _logFile;
        #endregion

        public DataGridPageViewModel(LogFile logFile)
         {
            viewModel = this;
            _logFile = logFile;
            SelectSetCommand = new Command(SelectSet);
            NavigationService = Services.NavigationService.Instance;
            Init(logFile);
        }

        public static async void getAttachmentsLog(LogFile file)
        {
            try
            {
                if (viewModel == null)
                    return;
                viewModel.LogAttachment = new ObservableRangeCollection<AttachmentDBO>();
                List<AttachmentDBO> attachments = await viewModel.Database.GetItemsAsync<AttachmentDBO>();
                attachments = attachments.Where(x => x.Name.Split('_')[0].Equals(file.Name)).ToList();
                foreach (var attachment in attachments)
                {
                    if (attachment.IsAudio)
                    {
                        attachment.Icon = "microphone.png";
                        if(viewModel.LogAttachment?.Where(x => x.IsAudio)?.Count() <= 0)
                        {
                            viewModel.LogAttachment.Add(attachment);
                        }
                    }
                    else if (attachment.IsMedia)
                    {
                        attachment.Icon = "camera.png";
                        if (viewModel.LogAttachment.Where(x => x.IsMedia).Count() <= 0)
                        {
                            viewModel.LogAttachment.Add(attachment);
                        }
                    }
                    else if (attachment.IsVideo)
                    {
                        attachment.Icon = "ic_video.png";
                        if (viewModel.LogAttachment?.Where(x => x.IsVideo)?.Count() <= 0)
                        {
                            viewModel.LogAttachment.Add(attachment);
                        }
                    }
                    else if (attachment.IsEvent)
                    {
                        attachment.Icon = "events.png";
                        if (viewModel.LogAttachment?.Where(x => x.IsEvent)?.Count() <= 0)
                        {
                            viewModel.LogAttachment.Add(attachment);
                        }
                    }
                    else if (attachment.IsText)
                    {
                        attachment.Icon = "note.png";
                        if (viewModel.LogAttachment?.Where(x => x.IsText)?.Count() <= 0)
                        {
                            viewModel.LogAttachment.Add(attachment);
                        }
                    }
                }
                viewModel.lstAttachments = attachments;
                viewModel.LogAttachment = new ObservableRangeCollection<AttachmentDBO>(viewModel.LogAttachment);
            }
            catch (Exception ex)
            {
                viewModel.AnalyticsService.TrackError(ex);
                await viewModel.Alert.DisplayError(ex);
            }
        }

        protected async void Init(LogFile file)
        {
            try
            {
                getAttachmentsLog(file);
                var ljhText = await FileSystem.ReadAllTextAsync(file.LjhFilePath);
                var ljh = JsonConvert.DeserializeObject<LJH_Holder>(ljhText);
                var lines = (await FileSystem.ReadAllLinesAsync(file.LcvFilePath)).ToList();
                var lastLine = lines.LastOrDefault();
                var currentSetStrId = lastLine.Split(',').FirstOrDefault();
                var currentSetId = int.Parse(currentSetStrId);

                Sets = ljh.Sets
                    .Select(x => GetSetFromLjh(x, lines))
                    .ToList();
                SetSelectionVisible = Sets.Count > 1;
                Title = file.Name;

                var initialSet = Sets.FirstOrDefault(x => x.SetId == currentSetId);
                if (initialSet == null)
                {
                    await Alert.ShowAlert(Localization.Localization.Data_NoData);
                    await OnBacksAsync();
                }
                else
                {
                    await SelectSetAsync(initialSet);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
                await OnBacksAsync();
            }
        }

        private async void SelectedItemChanged() 
        {
            try
            {
                if (Attachment != null)
                {
                    if (Attachment.IsAudio)
                    {
                        var recordPopup = new RecordSoundPopupPage();
                        await NavigationService.Nav.PushPopupAsync(recordPopup);
                    }
                    //else if (Attachment.IsEvent)
                    //{
                    //    var addEventPopup = new AddEventPopupPage(DateTime.Now);
                    //    await NavigationService.Nav.PushPopupAsync(addEventPopup);
                    //}
                    else if (Attachment.IsMedia)
                    {
                        var imageGalleryPopup = new ImageGalleryPopupPage(new ObservableCollection<AttachmentDBO>(lstAttachments));
                       await NavigationService.Nav.PushPopupAsync(imageGalleryPopup);
                    }
                    else if (Attachment.IsText)
                    {
                        var notePopup = new LogNotePopupPage(Attachment);
                        await NavigationService.Nav.PushPopupAsync(notePopup);
                    }
                    else if (Attachment.IsVideo)
                    {
                        var videoGalleryPopup = new VideoGalleryPopupPage(new ObservableCollection<AttachmentDBO>(lstAttachments));
                        await NavigationService.Nav.PushPopupAsync(videoGalleryPopup);
                    }
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
        }

        private async Task SelectSetAsync(DataGridSet set)
        {
            try
            {
                DataTable = new DataTable()
                    .FillColumns(set.Columns)
                    .FillRows(set.Lines);
                Set = set;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex, $"Failed to read data for set {set?.SetId}");
            }
        }

        private async void SelectSet()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                var tcs = new TaskCompletionSource<DataGridSet>();
                var popup = new SelectSetPopupPage(Sets.ToList(), tcs, Set);
                await Navigation.PushPopupAsync(popup);
                var set = await tcs.Task;
                await SelectSetAsync(set);
            }
            catch(TaskCanceledException) { }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }

            IsBusy = false;
        }

        private DataGridSet GetSetFromLjh(LJH_Set ljhSet, List<string> lines)
        {
            var ignoredColumnIndexes = GetIgnoredColumnsIndexes(ljhSet.Columns);
            return new DataGridSet
            {
                SetId = ljhSet.SetID,
                Columns = GetColumnsFromSet(ljhSet, ignoredColumnIndexes),
                Lines = GetLinesBySet(ljhSet, lines, ignoredColumnIndexes),
            };
        }

        private List<string> GetLinesBySet(LJH_Set set, List<string> lines, List<int> ignoredColumnIndexes)
        {
            var result = lines
                .Where(x => x.Split(',').FirstOrDefault() == $"{set.SetID}")
                .Select(x =>
                {
                    var items = x.Split(',').Select(y => y.Trim()).ToList();
                    items.RemoveAt(0);

                    for (int i = ignoredColumnIndexes.Count - 1; i >= 0; i--)
                    {
                        items.RemoveAt(ignoredColumnIndexes[i] + 1);
                    }

                    return string.Join(",", items);
                })
                .ToList();

            return result;
        }

        private List<string> GetColumnsFromSet(LJH_Set ljhSet, List<int> ignoredColumnIndexes)
        {
            var allColumns = ljhSet?.Columns?.Select(x => $"{x.Sensor} {x.Unit} {x.SerialNumber}")?.ToList() ?? new List<string>();

            return FilterColumns(allColumns, ignoredColumnIndexes);
        }

        private List<string> FilterColumns(List<string> allColumns, List<int> ignoredColumnIndexes)
        {
            var list = allColumns.ToList();
            for (int i = ignoredColumnIndexes.Count - 1; i >= 0; i--)
            {
                list.RemoveAt(ignoredColumnIndexes[i]);
            }
            return list;
        }

        private List<int> GetIgnoredColumnsIndexes(List<LJH_Column> columns)
        {
            return columns
                .Where(x => x.Code.IsCoordinatesSensorCode() || x.Sensor.IsNullOrEmpty())
                .Select(x => columns.IndexOf(x))
                .ToList();
        }

        //private ImageSource icon;

        //public ImageSource Icon { get => icon; set => Set(ref icon, value); }
    }
}
