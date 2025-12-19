using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;

namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProbePage
    {
        //TODO : To Define Class Level Variables...
        private readonly ProbePageViewModel _prodePageVm;

        public ProbePage(GrayWolfDevice device)
        {
            InitializeComponent();
           _prodePageVm = new ProbePageViewModel(device);
           this.BindingContext = _prodePageVm;
        }
    }
}