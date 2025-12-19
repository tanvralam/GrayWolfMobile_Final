using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;


namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataGridPage : ContentPage
    {
        LogFile logFile;
        DataGridPageViewModel viewModel;
        public DataGridPage(LogFile logFile)
        {
            InitializeComponent();
            this.logFile = logFile;
            BindingContext = viewModel = new DataGridPageViewModel(logFile);
            
        }

        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((CollectionView)sender).SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //viewModel.getAttachmentsLog(logFile);
        }
    }
}