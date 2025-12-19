using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using GalaSoft.MvvmLight.Messaging;
using GrayWolf.Interfaces;
using GrayWolf.Messages;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public class AuthService : IAuthService
    {
        private ISettingsService Settings { get; }

        public string CurrentUserLogin { get; private set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(CurrentUserLogin);

        public AuthService()
        {
            Settings = Ioc.Default.GetService<ISettingsService>();
            CurrentUserLogin = Settings.Email;
        }

        public Task LoginAsync(string login, string password)
        {
            CurrentUserLogin = login;
            Settings.Email = login;
            Settings.Password = password;
            Messenger.Default.Send(new AuthStatusMessage(true, login));
            return Task.CompletedTask;
        }

        public Task LogoutAsync()
        {
            CurrentUserLogin = null;
            SettingsService.Instance.Email = null;
            SettingsService.Instance.Password = null;
            Messenger.Default.Send(new AuthStatusMessage(false));
            return Task.CompletedTask;
        }
    }
}
