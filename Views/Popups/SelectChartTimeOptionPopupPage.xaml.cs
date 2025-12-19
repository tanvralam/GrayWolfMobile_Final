using GrayWolf.Enums;
using GrayWolf.ViewModels;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectChartTimeOptionPopupPage
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

        public SelectChartTimeOptionPopupPage(
            GraphTimeOption selectedOption,
            TaskCompletionSource<GraphTimeOption> tcs)
        {
            InitializeComponent();
            BindingContext = new SelectChartTimeOptionPopupViewModel(selectedOption, tcs);
            DefaultLabelColor = Colors.White;
            App.Current.Resources.TryGetValue("FileSelectedColor", out var color);
            SelectedLabelColor = (Color)color;
        }
    }
}