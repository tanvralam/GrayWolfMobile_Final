using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectVOCItemPopupPage
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
        
        public SelectVOCItemPopupPage(List<VOCItem> items, TaskCompletionSource<VOCItem> tcs)
        {
            InitializeComponent();
            DefaultLabelColor = Colors.White;
            App.Current.Resources.TryGetValue("FileSelectedColor", out var color);
            SelectedLabelColor = (Color)color;
            BindingContext = new SelectVOCItemPopupViewModel(items, tcs);
        }
    }
}