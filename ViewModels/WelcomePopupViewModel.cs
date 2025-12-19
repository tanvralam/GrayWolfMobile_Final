using System.Threading.Tasks;

namespace GrayWolf.ViewModels
{
    public class WelcomePopupViewModel : BaseDoNotShowAgainPopupViewModel
    {
        public WelcomePopupViewModel() : base()
        {
        }

        public override async Task OnBacksAsync()
        {
            if (!SetBusy())
            {
                return;
            }

            Settings.ShowWelcomeMessage = !DoNotShowAgain;
            await base.OnBacksAsync();

            IsBusy = false;
        }

        protected override async void Confirm()
        {
            await OnBacksAsync();
        }
    }
}
