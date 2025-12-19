using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GrayWolf.ViewModels
{
    public class ShowVideoPopupViewModel : BasePopupViewModel
    {
        public string VideoSource { get; }
        public ICommand DeleteVideoCommand { get; }
        private AttachmentDBO _attachment;
        private INavigationService NavigationService { get; }
        public ShowVideoPopupViewModel(AttachmentDBO attachment)
        {
            try
            {
                _attachment = attachment;
                if (attachment != null)
                    VideoSource = attachment.Path;
                NavigationService = Services.NavigationService.Instance;
                DeleteVideoCommand = new Command(DeleteVideo);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
        }

        private async void DeleteVideo()
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
    }
}
