using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GrayWolf.ViewModels
{
    public class ShowImagePopupViewModel : BasePopupViewModel
    {
        public string ImageSource { get; }
        public ICommand DeleteImageCommand { get; }
        private AttachmentDBO _attachment;
        private INavigationService NavigationService { get; }
        public ShowImagePopupViewModel(AttachmentDBO attachment)
        {
            try
            {
                _attachment = attachment;
                if (attachment != null)
                    ImageSource = attachment.Path;
                NavigationService = Services.NavigationService.Instance;
                DeleteImageCommand = new Command(DeleteImage);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
            }
        }

        private async void DeleteImage()
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
