using GrayWolf.ViewModels;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CsvExportInfoPopupPage
    {
        public CsvExportInfoPopupPage()
        {
            InitializeComponent();
            BindingContext = new CsvExportInfoPopupPageViewModel();
        }
    }
}