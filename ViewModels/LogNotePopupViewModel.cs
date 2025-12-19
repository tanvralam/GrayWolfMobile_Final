using CommunityToolkit.Mvvm.Input;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using RGPopup.Maui.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class LogNotePopupViewModel : BasePopupViewModel
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
        public ICommand DeleteNoteCommand { get; }
        private AttachmentDBO _attachment;
        private Interfaces.INavigationService NavigationService { get; }
        public bool IsDeleteVisible { get; set; }
        #endregion

        #region Commands

        public IAsyncRelayCommand AddNoteCommand { get; }
        #endregion

        public LogNotePopupViewModel(AttachmentDBO attachment)
        {
            if (attachment != null)
            {
                IsDeleteVisible = true;
                _attachment = attachment;
            }
            else
                IsDeleteVisible = false;
            NavigationService = Services.NavigationService.Instance;
            AddNoteCommand = new AsyncRelayCommand(OnAddNote);
            DeleteNoteCommand = new Command(DeleteNote);
            Load();
        }

        private async void DeleteNote()
        {
            try
            {
                await Database.DeleteItemAsync<AttachmentDBO>(_attachment.Id);
                await NavigationService.Nav.PopPopupAsync();
                DataGridPageViewModel.getAttachmentsLog(DataGridPageViewModel._logFile);
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

        private async Task OnAddNote()
        {
            try
            {
                var isNew = Attachment == null;
                var textNote = Attachment ?? new TextAttachment();
                textNote.TextContent = _text;

                await LogService.AddLogAttachmentFileAsync(textNote, isNew, LogService.CurrentFile.Id);
                AlertService.Instance.Toast("Text Note logged successfully.");
                await Navigation.PopPopupAsync();
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
        }
    }
}
