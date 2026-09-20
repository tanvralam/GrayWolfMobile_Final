using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using System;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class FirstDemoFormPopupViewModel : BasePopupViewModel
    {
        public DemoModeForm Form { get; } = new DemoModeForm();

        public ICommand SubmitCommand { get; }

        public IWufooService WufooService { get; }

        public FirstDemoFormPopupViewModel()
        {
            SubmitCommand = new Command(Submit);
            WufooService = Ioc.Default.GetService<IWufooService>();
        }

        public override Task OnBacksAsync()
        {
            Settings.IsFirstDemoRun = false;
            return base.OnBacksAsync();
        }

        private async void Submit()
        {
            if (!SetBusy())
            {
                return;
            }
            try
            {
                var isValid = await ValidateAsync(Form);
                if (!isValid)
                {
                    return;
                }

                // The demo has already started at this point. A failure in the
                // optional contact-form submission must not block demo mode or
                // repeatedly show an error to the user.
                try
                {
                    await WufooService.SubmitDemoFormAsync(Form);
                }
                catch (Exception ex)
                {
                    AnalyticsService.TrackError(ex, GetType().Name);
                }

                Settings.IsFirstDemoRun = false;
                await OnBacksAsync();
            }
            catch(Exception ex)
            {
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<bool> ValidateAsync(DemoModeForm form)
        {
            if (form.Name.IsNullOrEmpty() || form.Email.IsNullOrEmpty())
            {
                await Alert.ShowAlert(Localization.Localization.DemoForm_EmptyDataMessage);
                return false;
            }
            return true;
        }
    }
}
