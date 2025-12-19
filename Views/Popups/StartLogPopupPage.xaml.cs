using GrayWolf.ViewModels;
using RGPopup.Maui.Pages;



namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartLogPopupPage
    {
        private readonly StartLogPopupPageViewModel _startLogPopupPageVm;

        public StartLogPopupPage()
        {
            InitializeComponent();
            _startLogPopupPageVm = new StartLogPopupPageViewModel();
            _startLogPopupPageVm.SelectedLogInitialized += VM_SelectedLogInitialized;
            _startLogPopupPageVm.LogFileAdded += VM_LogFileAdded;
            this.BindingContext = _startLogPopupPageVm;
        }
    }
}