using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;

namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorTipsPage
    {
        public SensorTipsPage(Reading reading, DeviceModelsEnum deviceType)
        {
            InitializeComponent();
            BindingContext = new SensorTipsPageViewModel(reading, deviceType);
        }
    }
}