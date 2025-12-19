using System.Windows.Input;



namespace GrayWolf.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordSoundPopupButton
    {
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
            nameof(ImageSource),
            typeof(ImageSource),
            typeof(RecordSoundPopupButton),
            default,
            BindingMode.OneWay);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(RecordSoundPopupButton),
            default,
            BindingMode.OneWay);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(RecordSoundPopupButton),
            default,
            BindingMode.OneWay);

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public RecordSoundPopupButton()
        {
            InitializeComponent();
        }
    }
}