using System;
using System.Windows.Input;



namespace GrayWolf.ViewModels
{
    public class AboutPageViewModel : BaseViewModel
    {
        #region properties
        public string Url => @"https://graywolfsensing.com/";

        public string SupportEmail => "tech_support@graywolfsensing.com";

        public string InstructionsLink => @"https://graywolfsensing.com/wolfsense-mobile-app-quick-start/";
        public string InstructionsLinkText => "Online Instructions";

        public string Version => VersionTracking.CurrentVersion;
        #endregion

        #region commands
        public ICommand WebCommand { get; }

        public ICommand SupportCommand { get; }

        public ICommand InstructionsCommand { get; }
        #endregion

        public AboutPageViewModel()
        {
            WebCommand = new Command(OpenWeb);
            SupportCommand = new Command(OpenEmailApp);
            InstructionsCommand = new Command(OpenInstructions);
        }

        #region methods
        private async void OpenWeb()
        {
            if (!SetBusy())
            {
                return;
            }

            await Browser.OpenAsync(Url);

            IsBusy = false;
        }

        private async void OpenEmailApp()
        {
            if (!SetBusy())
            {
                return;
            }

            await Launcher.OpenAsync(new Uri($"mailto:{SupportEmail}"));

            IsBusy = false;
        }

        private async void OpenInstructions()
        {
            if (!SetBusy())
            {
                return;
            }

            await Launcher.OpenAsync(InstructionsLink);

            IsBusy = false;
        }
        #endregion
    }
}
