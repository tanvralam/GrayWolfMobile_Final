
using CommunityToolkit.Mvvm.Input;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using RGPopup.Maui.Extensions;
using Syncfusion.Maui.ImageEditor;
using System;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public partial class ImageNotePopupViewModel : BaseViewModel
    {
        private DrawableAttachment Attachment { get; set; }

        
        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged();
            }
        }

        #region Commands
        public ICommand CancelCommand { get; }

        #endregion

        public ImageNotePopupViewModel()
        {
            CancelCommand = new AsyncRelayCommand(ClosePopup);
        }
        private async Task ClosePopup()
        {
            await NavigationService.Instance.Nav.PopPopupAsync();
        }

        public async void OnImageSaved(byte[] bytes)
        {

            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var isNew = Attachment == null;
                await LogService.AddLogAttachmentFileAsync(new DrawableAttachment(), isNew, LogService.CurrentFile.Id, bytes);

                await ClosePopup();
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
                await Alert.DisplayError(e);
            }
            finally
            {
                IsBusy = false;
            }

        }
        public async Task Load()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                Attachment = await LogService.GetCurrentDrawingNoteAsync();
                if(Attachment != null)
                {
                    Image = ImageSource.FromFile(Attachment.Path);
                }
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
                await Alert.DisplayError(e);

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
