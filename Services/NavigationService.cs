using System;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using GrayWolf.Interfaces;
using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Pages;
namespace GrayWolf.Services
{
    public class NavigationService : INavigationService
    {
        private INavigation Navigation => Application.Current?.MainPage?.Navigation;
        public static INavigationService Instance
        {
            get { return Ioc.Default.GetService<INavigationService>(); }
        }

        public INavigation Nav
        {
            get { return SimpleIoc.Default.GetInstance<INavigation>(); }
        }

        public async Task NavigateTo(Page pg)
        {
            await Nav.PushAsync(pg);
        }

        public async Task NavigateTo<T>() where T : Page, new()
        {
            await Nav.PushAsync(new T());
        }

        public async Task ShowPopup<T>() where T : PopupPage, new()
        {
            await Nav.PushPopupAsync(new T());
        }

        public async Task ShowPopup(PopupPage pg)
        {
            await Nav.PushPopupAsync(pg);
        }
    }

}
