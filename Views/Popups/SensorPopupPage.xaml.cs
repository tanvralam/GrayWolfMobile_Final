using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;

using Microsoft.Maui.Controls;

namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorPopupPage
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

        public SensorPopupPage(string readingId)
        {
            InitializeComponent();
            App.Current.Resources.TryGetValue("FileSelectedColor", out var color);
            SelectedLabelColor =  Colors.White; ;// (Color)color;
            BindingContext = new SensorPopupViewModel(readingId);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Content.HeightRequest = height * 0.7;
            Content.WidthRequest = width * 0.8;
        }
    }
}