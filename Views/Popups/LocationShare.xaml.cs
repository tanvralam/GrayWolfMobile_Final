using GrayWolf.ViewModels;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationShare
    {
        public LocationShare(bool exportButtonsOnly)
        {
            InitializeComponent();
            var vm = new LocationShareViewModel(exportButtonsOnly);
            BindingContext = vm;
            vm.LogFileAdded += VM_LogFileAdded;
            vm.SelectedLogInitialized += VM_SelectedLogInitialized;
        }
    }
}