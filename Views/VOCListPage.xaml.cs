using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;
using System.Collections.Generic;

namespace GrayWolf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VOCListPage
    {
        public VOCListPage(List<VOCItem> items, VOCItem item)
        {
            InitializeComponent();
            BindingContext = new VOCListPageViewModel(items, item);
        }
    }
}