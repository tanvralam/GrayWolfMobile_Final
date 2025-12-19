using RGPopup.Maui.Extensions;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class AttachmentsSizeWarningPopupViewModel : BasePopupViewModel
    {
        private TaskCompletionSource<bool> TCS { get; }

        public ICommand ConfirmCommand { get; }

        public AttachmentsSizeWarningPopupViewModel(TaskCompletionSource<bool> tcs)
        {
            TCS = tcs;
            ConfirmCommand = new Command(Confirm);
        }

        public override async Task OnBacksAsync()
        {
            await base.OnBacksAsync();
            TCS.TrySetResult(false);
        }

        private async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }

            await Navigation.PopPopupAsync();
            
            TCS.TrySetResult(true);

            IsBusy = false;
        }
    }
}
