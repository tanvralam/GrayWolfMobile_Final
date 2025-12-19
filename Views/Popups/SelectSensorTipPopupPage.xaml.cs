using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectSensorTipPopupPage
    {
        public static BindableProperty DefaultLabelColorProperty = BindableProperty.Create(
            nameof(DefaultLabelColor),
            typeof(Color),
            typeof(SelectChartTimeOptionPopupPage),
            default,
            BindingMode.OneWay);

        public static BindableProperty SelectedLabelColorProperty = BindableProperty.Create(
            nameof(SelectedLabelColor),
            typeof(Color),
            typeof(SelectChartTimeOptionPopupPage),
            default,
            BindingMode.OneWay);

        public Color DefaultLabelColor
        {
            get => (Color)GetValue(DefaultLabelColorProperty);
            set => SetValue(DefaultLabelColorProperty, value);
        }

        public Color SelectedLabelColor
        {
            get => (Color)GetValue(SelectedLabelColorProperty);
            set => SetValue(SelectedLabelColorProperty, value);
        }

        public SelectSensorTipPopupPage(List<SensorTip> sensorTips, TaskCompletionSource<SensorTip> tcs)
        {
            InitializeComponent();
            DefaultLabelColor =Colors.White;
            App.Current.Resources.TryGetValue("FileSelectedColor", out var color);
            SelectedLabelColor = (Color)color;
            BindingContext = new SelectSensorTipPopupViewModel(sensorTips, tcs);
        }
    }
}