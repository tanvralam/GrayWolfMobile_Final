using GrayWolf.CustomControls;
using GrayWolf.ViewModels;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParameterNamesPopupPage
    {
        public ParameterNamesPopupPage()
        {
            InitializeComponent();
            BindingContext = new ParameterNamesViewModel();
        }
    }
}