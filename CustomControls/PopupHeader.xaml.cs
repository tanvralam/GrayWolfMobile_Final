using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;



namespace GrayWolf.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupHeader
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(PopupHeader),
            "",
            BindingMode.OneWay);

        public string Title
        {
            get => $"{GetValue(TitleProperty)}";
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty CloseCommandProperty = BindableProperty.Create(
            nameof(CloseCommand),
            typeof(ICommand),
            typeof(PopupHeader),
            null,
            BindingMode.OneWay);
        
        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public PopupHeader()
        {
            InitializeComponent();
        }
    }
}