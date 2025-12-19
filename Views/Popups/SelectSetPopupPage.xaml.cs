using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectSetPopupPage 
    {

        public static BindableProperty DefaultLabelColorProperty = BindableProperty.Create(
            nameof(DefaultLabelColor),
            typeof(Color),
            typeof(SelectSetPopupPage),
            default,
            BindingMode.OneWay);

        public static BindableProperty SelectedLabelColorProperty = BindableProperty.Create(
            nameof(SelectedLabelColor),
            typeof(Color),
            typeof(SelectSetPopupPage),
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

        public SelectSetPopupPage(
            List<DataGridSet> sets,
            TaskCompletionSource<DataGridSet> tcs,
            DataGridSet currentSet)
        {
            InitializeComponent();
            BindingContext = new SelectSetPopupViewModel(
                sets,
                tcs,
                currentSet);
            DefaultLabelColor = Colors.White;
            App.Current.Resources.TryGetValue("FileSelectedColor", out var color);
            SelectedLabelColor = (Color)color;
        }
    }
}