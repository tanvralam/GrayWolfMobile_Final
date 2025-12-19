using GrayWolf.ViewModels;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ZipPasswordPopup
    {
        public ZipPasswordPopup()
        {
            InitializeComponent();
            BindingContext = new ZipPasswordPopupViewModel();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Content.WidthRequest = width * 0.8;
        }
    }
}