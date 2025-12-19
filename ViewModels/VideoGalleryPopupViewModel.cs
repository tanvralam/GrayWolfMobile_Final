using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using GrayWolf.Views.Popups;
using MvvmHelpers;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;



namespace GrayWolf.ViewModels
{
    public class VideoGalleryPopupViewModel : BasePopupViewModel
    {
        #region Properties

        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }

        private TextAttachment Attachment { get; set; }
        private ObservableCollection<AttachmentDBO> _logAttachment;

        public ObservableCollection<AttachmentDBO> LogAttachment
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
        private Interfaces.INavigationService NavigationService { get; }
        public bool IsDeleteVisible { get; set; }
        VideoGalleryPopupPage _videoGalleryPopup;
        #endregion

        #region Commands
        //public IAsyncDelegateCommand AddNoteCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand MagnifierCommand { get; }
        public ICommand AddCaptionCommand { get; }
        #endregion

        public VideoGalleryPopupViewModel(ObservableCollection<AttachmentDBO> attachment, VideoGalleryPopupPage videoGalleryPopup)
        {
            if (attachment != null)
            {
                IsDeleteVisible = true;
                LogAttachment = new ObservableCollection<AttachmentDBO>(attachment.Where(x => x.IsVideo).ToList());
            }
            else
                IsDeleteVisible = false;
            _videoGalleryPopup = videoGalleryPopup;
            NavigationService = Services.NavigationService.Instance;
            AddCaptionCommand = new Command(OnAddCaption);
            MagnifierCommand = new Command(OnMagnifierClick);
            DeleteCommand = new Command(DeleteImage);
            Load();
        }

        private async void OnAddCaption(object _attachment)
        {
            AttachmentDBO attachment = (AttachmentDBO)_attachment;
            var addCaptionPopup = new AddCaptionPopup(attachment);
            await NavigationService.Nav.PushPopupAsync(addCaptionPopup);
        }

        private async void OnMagnifierClick(object _attachment)
        {
            AttachmentDBO attachment = (AttachmentDBO)_attachment;
            var videoPopup = new ShowVideoPopup(attachment);
            await NavigationService.Nav.PushPopupAsync(videoPopup);
        }

        private async void DeleteImage(object _attachment)
        {
            try
            {
                bool val = await _videoGalleryPopup.DisplayAlert("", "Are you sure you want to delete?", "Yes", "No");
                if (val)
                {
                    AttachmentDBO attachment = (AttachmentDBO)_attachment;
                    await Database.DeleteItemAsync<AttachmentDBO>(attachment.Id);
                    LogAttachment.Remove(attachment);
                    DataGridPageViewModel.getAttachmentsLog(DataGridPageViewModel._logFile);
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
        }

        private async void Load()
        {
            Attachment = await LogService.GetNoteAsync();
            Text = Attachment?.TextContent;
        }
    }
}
