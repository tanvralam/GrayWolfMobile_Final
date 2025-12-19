using Acr.UserDialogs;
using GrayWolf.Extensions;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using RGPopup.Maui.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


using GrayWolf.Utility;


namespace GrayWolf.ViewModels
{
    public class AddCaptionPopupViewModel : BasePopupViewModel
    {
        #region variables

        AttachmentDBO _attachment;
        private Interfaces.INavigationService NavigationService { get; }
        private TextAttachment Attachment { get; set; }
        #endregion
        #region Properties
        private string _caption;
        private DateTime _timestamp;

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                RaisePropertyChanged();
            }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                RaisePropertyChanged();
            }
        }

        #endregion
        #region commands
        public ICommand AddCaptionCommand { get; }
        #endregion

        public AddCaptionPopupViewModel(AttachmentDBO attachment)
        {
            AddCaptionCommand = new Command<string>(OnAddCaption);
            _attachment = attachment;
            Load();
        }

        private async void Load()
        {
            PhotoVideoCaptionData data= await GetPhotoCaptionAsync(_attachment.CaptionPath);
            
            Caption = data.Label;
            Timestamp = data.Timestamp;

        }

        private async void OnAddCaption(string caption)
        {
            if (!SetBusy())
            {
                return;
            }
            try
            {
                if (Caption.IsNullOrWhiteSpace())
                {
                    return;
                }
                PhotoVideoCaptionData data = new PhotoVideoCaptionData();
                data.Label = Caption;
                data.Timestamp = Timestamp;

               await FileSystem.WriteAllTextAsync(_attachment.CaptionPath, data.Encode() );
                //_attachment.Caption = Caption;
                //await Database.UpdateAsync<AttachmentDBO>(_attachment);
                await Navigation.PopPopupAsync();
                DataGridPageViewModel.getAttachmentsLog(DataGridPageViewModel._logFile);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<PhotoVideoCaptionData> GetPhotoCaptionAsync(string captionpath)
        {
            try
            {
               captionpath = _attachment.CaptionPath;

                if (File.Exists(captionpath))
                {
                    string s = await FileSystem.ReadAllTextAsync(captionpath);
                    PhotoVideoCaptionData data = new PhotoVideoCaptionData(s);
                    return data; 
                }
            }
            catch(Exception ex)
            {
                
            }
            return new PhotoVideoCaptionData();
        }
        public async Task<bool> WritePhotoCaptionAsync(string captionpath, PhotoVideoCaptionData data)
        {
            try
            {
                if (!captionpath.ToLower().Contains("_caption"))
                {
                    // change it if it needs changing, otherwise, leave it be
                    captionpath = _attachment.CaptionPath;
                }
                FileInfo fi = new FileInfo(captionpath);
                if (fi.Exists) fi.Delete();

                await FileSystem.WriteAllTextAsync(captionpath, data.Encode() );
                return true;
            }
            catch
            {
            }
            return false;
        }

    }
}
