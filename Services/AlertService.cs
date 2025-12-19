using Acr.UserDialogs;
using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Extensions;
using System;
using System.Threading.Tasks;


namespace GrayWolf.Services
{
    public class AlertService : IAlertService
    {
        public static IAlertService Instance => Ioc.Default.GetService<IAlertService>();
        public AlertService()
        {

        }

        public Task ShowAlert(string message, string title = "", string cancel = "")
        {
            title = title.IsNullOrEmpty() ? Localization.Localization.WolfSenseAlert_Title : title;
            cancel = cancel.IsNullOrEmpty() ? Localization.Localization.Button_Ok : cancel;
            return  Ioc.Default.GetService<IUserDialogs>().AlertAsync(message, title, cancel);
        }


        public Task ShowMessage(string message, string title = "", string cancel = "")
        {
            title = title.IsNullOrEmpty() ? Localization.Localization.WolfSenseMessage_Title : title;
            cancel = cancel.IsNullOrEmpty() ? Localization.Localization.Button_Ok : cancel;
            return App.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public Task<bool> ShowMessageConfirmation(string message, string title = "", string accept = "", string cancel = "")
        {
            title = title.IsNullOrEmpty() ? Localization.Localization.WolfSenseMessage_Title : title;
            accept = accept.IsNullOrEmpty() ? Localization.Localization.Button_Ok : accept;
            cancel = cancel.IsNullOrEmpty() ? Localization.Localization.Button_Cancel : cancel;

            return App.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task DisplayError(Exception exception, string friendlyMessage = null)
        {
#if DEBUG
            Console.WriteLine($"{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            await App.Current.MainPage.DisplayAlert(exception.Message, exception.StackTrace, Localization.Localization.Button_Ok);

#else
            await App.Current.MainPage.DisplayAlert("Error", friendlyMessage, "OK");
#endif
        }

        public async Task<string> DisplayActionSheet(string Title, string cancel, string o, string[] items)
        {
            return await App.Current.MainPage.DisplayActionSheet(Title, cancel, o, items);
        }


        public void Toast(string message)
        {
            var tst = new ToastConfig(message)
            {
                Duration = new TimeSpan(0, 0, 4),
                MessageTextColor = System.Drawing.Color.White,
                Position = ToastPosition.Bottom,
            };
            Ioc.Default.GetService<IUserDialogs>().Toast(tst);
        }

    }
}
