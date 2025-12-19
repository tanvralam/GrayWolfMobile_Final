using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace GrayWolf.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DoNotShowAgainPopupContentView
    {
        public static BindableProperty CloseCommandProperty = BindableProperty.Create(
            nameof(CloseCommand),
            typeof(ICommand),
            typeof(DoNotShowAgainPopupContentView),
            default,
            BindingMode.OneWay);

        public static BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(DoNotShowAgainPopupContentView),
            "",
            BindingMode.OneWay);

        public static BindableProperty DoNotShowAgainProperty = BindableProperty.Create(
            nameof(DoNotShowAgain),
            typeof(bool),
            typeof(DoNotShowAgainPopupContentView),
            false,
            BindingMode.TwoWay);

        public static BindableProperty MessageProperty = BindableProperty.Create(
            nameof(Message),
            typeof(string),
            typeof(DoNotShowAgainPopupContentView),
            "",
            BindingMode.OneWay);

        public static BindableProperty CheckBoxTapCommandProperty = BindableProperty.Create(
            nameof(CheckBoxTapCommand),
            typeof(ICommand),
            typeof(DoNotShowAgainPopupContentView),
            default,
            BindingMode.OneWay);

        public static BindableProperty OkCommandProperty = BindableProperty.Create(
            nameof(OkCommand),
            typeof(ICommand),
            typeof(DoNotShowAgainPopupContentView),
            default,
            BindingMode.OneWay);

        public static BindableProperty MessageTextSizeProperty = BindableProperty.Create(
            nameof(MessageTextSize),
            typeof(double),
            typeof(DoNotShowAgainPopupContentView),
            16.0,
            BindingMode.OneWay);

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public string Title
        {
            get => $"{GetValue(TitleProperty)}";
            set => SetValue(TitleProperty, value);
        }

        public bool DoNotShowAgain
        {
            get => (bool)GetValue(DoNotShowAgainProperty);
            set => SetValue(DoNotShowAgainProperty, value);
        }

        public string Message
        {
            get => $"{GetValue(MessageProperty)}";
            set => SetValue(MessageProperty, value);
        }

        public ICommand CheckBoxTapCommand
        {
            get => (ICommand)GetValue(CheckBoxTapCommandProperty);
            set => SetValue(CheckBoxTapCommandProperty, value);
        }

        public ICommand OkCommand
        {
            get => (ICommand)GetValue(OkCommandProperty);
            set => SetValue(OkCommandProperty, value);
        }

        public double MessageTextSize
        {
            get => (double)GetValue(MessageTextSizeProperty);
            set => SetValue(MessageTextSizeProperty, value);
        }

        public DoNotShowAgainPopupContentView()
        {
            InitializeComponent();
        }

        private void DoNotShowAgain_Tapped(object sender, System.EventArgs e)
        {
            CheckBoxTapCommand?.Execute(!DoNotShowAgain);
        }
    }
}