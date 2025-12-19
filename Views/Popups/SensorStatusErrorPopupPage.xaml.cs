using GrayWolf.Models.DBO;
using GrayWolf.ViewModels;
using System.IO;



namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorStatusErrorPopupPage
    {
        public SensorStatusErrorPopupPage(object param)
        {
            InitializeComponent();
            BindingContext = new SensorStatusErrorPopupViewModel(param);
        }
    }
}