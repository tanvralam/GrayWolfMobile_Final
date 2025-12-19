using Acr.UserDialogs;
using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Extensions;
using GrayWolf.Models.Domain;
using System;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class AddEventPopupViewModel : BasePopupViewModel
    {
        #region variables
        private EventAttachment _attachment;
        public EventAttachment Attachment
        {
            get => _attachment;
            private set => SetProperty(ref _attachment, value);
        }

        private Event _event;
        public Event Event
        {
            get => _event;
            private set => SetProperty(ref _event, value);
        }
        #endregion

        #region commands
        public ICommand ConfirmCommand { get; }
        #endregion

        public AddEventPopupViewModel(DateTime dateTime)
        {
            ConfirmCommand = new Command(Confirm);
            Event = new Event
            {
                DateTime = dateTime.ToString("o")
            };
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                Attachment = await LogService.GetEventAttachmentAsync();
            }
            catch(Exception ex)
            {
                await Alert.DisplayError(ex);
                AnalyticsService.TrackError(ex);
                await OnBacksAsync();
            }
        }

        private async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }
            try
            {
                if (Event.Label.IsNullOrWhiteSpace())
                {
                    return;
                }
                Attachment.Events.Add(Event);
                await LogService.AddLogAttachmentFileAsync(Attachment, !Attachment.IsCreated, LogService.CurrentFile.Id);

                var format = Localization.Localization.AddEvent_SuccessMessage_Format;
                var message = string.Format(format, LogService.CurrentFile.Name);
                var tst = new ToastConfig(Localization.Localization.RecordSound_AttachmentAdded)
                {
                    Duration = new TimeSpan(0, 0, 4),
                    Message = message,
                    MessageTextColor =System.Drawing.Color.White,
                    Position = ToastPosition.Bottom,
                };
                Ioc.Default.GetService<IUserDialogs>().Toast(tst);
                await OnBacksAsync();
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
    }
}
