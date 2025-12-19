using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public abstract class BaseDoNotShowAgainPopupViewModel : BasePopupViewModel
    {
        private bool _doNotShowAgain;
        public bool DoNotShowAgain
        {
            get => _doNotShowAgain;
            set => SetProperty(ref _doNotShowAgain, value);
        }

        public ICommand ConfirmCommand { get; protected set; }

        public ICommand SetDoNotShowAgainCommand { get; protected set; }

        public BaseDoNotShowAgainPopupViewModel()
        {
            ConfirmCommand = new Command(Confirm);
            SetDoNotShowAgainCommand = new Command<bool>(SetDoNotShowAgain);
        }

        protected abstract void Confirm();

        protected virtual void SetDoNotShowAgain(bool value)
        {
            DoNotShowAgain = value;
        }
    }
}
