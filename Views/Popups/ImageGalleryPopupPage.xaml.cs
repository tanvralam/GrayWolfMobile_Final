using GrayWolf.Models.DBO;
using GrayWolf.ViewModels;
using System.Collections.ObjectModel;



namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageGalleryPopupPage
    {
        public ImageGalleryPopupPage(ObservableCollection<AttachmentDBO> attachment = null)
        {
            InitializeComponent();
            BindingContext = new ImageGalleryPopupViewModel(attachment, this);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Content.HeightRequest = height * 0.7;
        }

        private void lstCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}