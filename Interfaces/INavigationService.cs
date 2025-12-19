using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;
using RGPopup.Maui.Pages;

namespace GrayWolf.Interfaces
{
    public interface INavigationService
    {
        INavigation Nav { get; }
        Task NavigateTo(Page pg);
        Task NavigateTo<T>() where T:Page,new();
        Task ShowPopup<T>() where T : PopupPage, new();
        Task ShowPopup(PopupPage pg);
    }
}