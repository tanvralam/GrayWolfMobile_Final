using GrayWolf.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using RGPopup.Maui.Extensions;

namespace GrayWolf.ViewModels
{
    public class BasePopupViewModel : BaseViewModel
    {
        public BasePopupViewModel() : base()
        {
        }
       
        public bool IsLogActive => LogService.IsLogging;
        public override Task OnBacksAsync()
        {
            return NavigationService.Instance.Nav.PopPopupAsync();
        }
    }
}
