using System.Collections;
using System.Runtime.CompilerServices;



namespace GrayWolf.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupOptionsView
    {
        public static BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(PopupOptionsView),
            default,
            BindingMode.OneWay);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public PopupOptionsView()
        {
            InitializeComponent();
        }
    }
}