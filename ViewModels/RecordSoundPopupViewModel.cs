using Acr.UserDialogs;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using MediaManager;
using Plugin.Maui.Audio;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using Drastic.AudioRecorder;

namespace GrayWolf.ViewModels
{
    public class RecordSoundPopupViewModel : BasePopupViewModel
    {
        #region Properties
        private const string IconDelete = "delete";
        private const string IconPlay = "ic_play";
        private const string IconPlayDisabled = "ic_play_disabled";
        private const string IconStop = "ic_stop";
        private const string IconStopDisabled = "ic_stop_disabled";
        private const string IconRecord = "ic_record";
        private const string IconRecordDisabled = "ic_record_disabled";

        private readonly AudioRecorderService _recorder;
        private IPreparePlayback PreparePlayback { get; }
        private IPrepareRecording PrepareRecording { get; }

        private SoundRecordState _state;
        public SoundRecordState State
        {
            get => _state;
            private set => SetProperty(ref _state, value, onChanged: OnStateChanged);
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get => _canDelete;
            private set => SetProperty(ref _canDelete, value);
        }

        private string _deleteButtonSource;
        public string DeleteButtonSource
        {
            get => _deleteButtonSource;
            private set => SetProperty(ref _deleteButtonSource, value);
        }

        private string _playButtonSource;
        public string PlayButtonSource
        {
            get => _playButtonSource;
            private set => SetProperty(ref _playButtonSource, value);
        }

        private string _stopButtonSource;
        public string StopButtonSource
        {
            get => _stopButtonSource;
            private set => SetProperty(ref _stopButtonSource, value);
        }

        private string _recordButtonSource;
        public string RecordButtonSource
        {
            get => _recordButtonSource;
            private set => SetProperty(ref _recordButtonSource, value);
        }

        private AudioAttachment _audioAttachment;
        public AudioAttachment AudioAttachment
        {
            get => _audioAttachment;
            private set => SetProperty(ref _audioAttachment, value);
        }

        private Interfaces.INavigationService NavigationService { get; }
        #endregion

        #region Commands
        public ICommand RecordCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand StopCommand { get; }
        #endregion

        public RecordSoundPopupViewModel()
        {
            RecordCommand = new AsyncRelayCommand(OnRecord);
            PlayCommand = new AsyncRelayCommand(OnPlay);
            DeleteCommand = new Command(DeleteRecord);
            StopCommand = new Command(() => OnStopAsync());
            NavigationService = Services.NavigationService.Instance;
            _recorder = new  AudioRecorderService
            {

                StopRecordingOnSilence = false, //will stop recording after 2 seconds (default)
                StopRecordingAfterTimeout = true,  //stop recording after a max timeout (defined below)
                TotalAudioTimeout = TimeSpan.FromSeconds(600) //audio will stop recording after 15 seconds 
            };

            if (SimpleIoc.Default.IsRegistered<IPreparePlayback>())
            {
                PreparePlayback = SimpleIoc.Default.GetInstance<IPreparePlayback>();
            }
            if (SimpleIoc.Default.IsRegistered<IPrepareRecording>())
            {
                PrepareRecording = SimpleIoc.Default.GetInstance<IPrepareRecording>();
            }
            State = SoundRecordState.CanRecord;
            Initialize();
        }

        #region override
        public override async Task OnBacksAsync()
        {
            await OnStopAsync();
            await base.OnBacksAsync();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            CrossMediaManager.Current.MediaItemFinished += Current_MediaItemFinished;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            CrossMediaManager.Current.MediaItemFinished -= Current_MediaItemFinished;
        }
        #endregion

        private async void Initialize()
        {
            try 
            {
                var attachments = await LogService.GetLogAttachmentFilesAsync(LogService.CurrentFile.Id);
                AudioAttachment = attachments.FirstOrDefault(x => x is AudioAttachment) as AudioAttachment;
                State = AudioAttachment == null ? SoundRecordState.CanRecord : SoundRecordState.CanPlay;
            }
            catch(Exception ex)
            {
                await Alert.DisplayError(ex);
                AnalyticsService.TrackError(ex);
            }
        }

        private Task OnPlay()
        {
            return PlayAudio();
        }

        private async Task OnRecord()
        {
            if(State == SoundRecordState.Recording || State == SoundRecordState.Playing)
            {
                return;
            }
            try
            {
                var isPermissionsGranted = await LogService.GetStoragePermissionsAsync();
                if(!isPermissionsGranted)
                {
                    await Alert.ShowAlert(Localization.Localization.Log_StoragePermissionsMessage);
                }
                else
                {
                    State = SoundRecordState.Recording;
                    PrepareRecording?.PrepareRecording();
                    await _recorder.StartRecording();
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                if (ex.Message.ToLowerInvariant().Contains("devicenotavailable"))
                {
                    await AlertService.Instance.ShowAlert(Localization.Localization.Log_RecordDeviceNotAvailable);
                }
                else
                {
                    await AlertService.Instance.DisplayError(ex);
                }
                State = SoundRecordState.CanRecord;
            }
        }

        private Task OnStopAsync()
        {
            if(State == SoundRecordState.Playing)
            {
                return OnPlayStoppedAsync(true);
            }
            else if(State == SoundRecordState.Recording)
            {
                return OnRecordStoppedAsync();
            }
            return Task.CompletedTask;
        }

        private async Task OnRecordStoppedAsync()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                await _recorder.StopRecording();
                State = SoundRecordState.CanPlay;

                await SaveRecordAsync();

                var tst = new ToastConfig(Localization.Localization.RecordSound_AttachmentAdded)
                {
                    Duration = new TimeSpan(0, 0, 4),
                    Message = Localization.Localization.RecordSound_AttachmentAdded,
                    MessageTextColor =System.Drawing.Color.White,
                    Position = ToastPosition.Bottom,
                };
                Ioc.Default.GetService<IUserDialogs>().Toast(tst);
                State = SoundRecordState.CanPlay;
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

        private async Task SaveRecordAsync()
        {
            //To Strore the file location for Zipping it...
            //var audioFilePath = recorder.GetAudioFilePath();
            //Helpers.Constants.FilesToZipList.Add(audioFilePath);
            //To Save Audio files in Local Database...
            var audio = new AudioAttachment()
            {
                Path = _recorder.FilePath
            };

            var bytes = _recorder.GetAudioFileStream()
                .ConverteStreamToByteArray();
            var attachments = await LogService.GetLogAttachmentFilesAsync(LogService.CurrentFile.Id);
            var audioAttachments = attachments
                .Where(x => x is AudioAttachment)
                .Cast<AudioAttachment>();
            await LogService.RemoveAttachmentsAsync(audioAttachments);
            
            AudioAttachment = await LogService.AddLogAttachmentFileAsync(audio, true, LogService.CurrentFile.Id, bytes);
        }

        private async Task OnPlayStoppedAsync(bool shouldStopMediaManager)
        {
            try
            {
                if (shouldStopMediaManager)
                {
                    await CrossMediaManager.Current.Stop();
                }
                State = SoundRecordState.CanPlay;
            }
            catch(Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await AlertService.Instance.DisplayError(ex);
            }
        }

        private async Task PlayAudio()
        {
            if(State != SoundRecordState.CanPlay)
            {
                return;
            }
            try
            {
                if (AudioAttachment == null) 
                    return;

                PreparePlayback?.PreparePlayback();
                await CrossMediaManager.Current.Play(AudioAttachment.Path);
                State = SoundRecordState.Playing;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await AlertService.Instance.DisplayError(ex);
            }
        }

        private async void DeleteRecord()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                if (!CrossMediaManager.Current.IsStopped())
                {
                    await OnStopAsync();//CrossMediaManager.Current.Stop();
                }
                if (AudioAttachment == null)
                    return;
                await LogService.RemoveAttachmentsAsync(new List<AudioAttachment> { AudioAttachment });
                AudioAttachment = null;
                State = SoundRecordState.CanRecord;

                var tst = new ToastConfig(Localization.Localization.RecordSound_AttachmentsDeleted)
                {
                    Duration = new TimeSpan(0, 0, 4),
                    Message = Localization.Localization.RecordSound_AttachmentsDeleted,
                    MessageTextColor = System.Drawing.Color.White,
                    Position = ToastPosition.Bottom
                };
                Ioc.Default.GetService<IUserDialogs>().Toast(tst);
                //await Database.DeleteItemAsync<AttachmentDBO>(_attachment.Id);
                //await NavigationService.Nav.PopPopupAsync();
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

        private void OnStateChanged()
        {
            switch (State)
            {
                case SoundRecordState.Playing:
                    OnPlayState();
                    break;
                case SoundRecordState.Recording:
                    OnRecordingState();
                    break;
                case SoundRecordState.CanPlay:
                    OnCanPlayState();
                    break;
                case SoundRecordState.CanRecord:
                    OnCanRecordState();
                    break;
            }
        }

        private void OnPlayState()
        {
            PlayButtonSource = IconPlayDisabled;
            StopButtonSource = IconStop;
            RecordButtonSource = IconRecordDisabled;
            DeleteButtonSource = IconDelete;
        }

        private void OnRecordingState()
        {
            PlayButtonSource = IconPlayDisabled;
            StopButtonSource = IconStop;
            RecordButtonSource = IconRecordDisabled;
            DeleteButtonSource = IconDelete;
        }

        private void OnCanPlayState()
        {
            PlayButtonSource = IconPlay;
            StopButtonSource = IconStopDisabled;
            RecordButtonSource = IconRecord;
            DeleteButtonSource = IconDelete;
        }

        private void OnCanRecordState()
        {
            PlayButtonSource = IconPlayDisabled;
            StopButtonSource = IconStopDisabled;
            RecordButtonSource = IconRecord;
            DeleteButtonSource = IconDelete;
        }

        private async void Current_MediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            await OnPlayStoppedAsync(false);
        }
    }
}
