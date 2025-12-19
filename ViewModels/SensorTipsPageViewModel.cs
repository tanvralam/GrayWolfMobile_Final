using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Enums;
using GrayWolf.Interfaces;
using GrayWolf.Models.Domain;
using GrayWolf.Views.Popups;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class SensorTipsPageViewModel : BaseViewModel
    {
        #region variables
        private List<SensorTip> _sensorTips;
        public List<SensorTip> SensorTips
        {
            get => _sensorTips;
            private set => SetProperty(ref _sensorTips, value);
        }

        private SensorTip _sensorTip;
        public SensorTip SensorTip
        {
            get => _sensorTip;
            set => SetProperty(ref _sensorTip, value);
        }

        private HtmlWebViewSource _html;
        public HtmlWebViewSource Html
        {
            get => _html;
            private set => SetProperty(ref _html, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        private Reading Reading { get; }
        private DeviceModelsEnum DeviceType { get; }
        #endregion

        #region services
        private ISensorTipsService SensorTipsService { get; }
        #endregion

        #region commands
        public ICommand SelectSensorTipCommand { get; }
        #endregion

        public SensorTipsPageViewModel(Reading reading, DeviceModelsEnum deviceType)
        {
            SensorTipsService = Ioc.Default.GetService<ISensorTipsService>();
            Reading = reading;
            DeviceType = deviceType;
            SelectSensorTipCommand = new Command(SelectTip);
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                SensorTips = await SensorTipsService.GetSensorTipsAsync(Reading.SensorCode, DeviceType, Reading.Name);
                var result = await LoadTipAsync(SensorTips.FirstOrDefault());
                if (!result)
                    await OnBacksAsync();
            }
            catch (Exception ex)
            {
                await Alert.DisplayError(ex);
                await OnBacksAsync();
            }
        }

        private CancellationTokenSource cts = null;
        private async Task<bool> LoadTipAsync(SensorTip sensorTip)
        {
            try
            {
                try
                {
                    cts?.Cancel();
                }
                catch (Exception) { }
                cts = new CancellationTokenSource();
                SensorTip = null;
                IsLoading = true;

                var html = await SensorTipsService.GetHtmlTextForCategoryAsync(sensorTip.CategoryId, Reading.SensorCode, DeviceType, cts.Token);
                SensorTip = sensorTip;
                string img = @"<a href=" + "'https:\\\\graywolfsensing.com\\tipcontent\\ApproximateParticleSizes.jpg'"+"><img src=" + "'https:\\\\graywolfsensing.com\\tipcontent\\ApproximateParticleSizes.jpg'" + "alt=" + "'Approximate Particle Sizes'" + "/></a>";
                html = html.Replace("<<!!PARTICLEIMAGE!!>>", img);
                Html = new HtmlWebViewSource
                {
                    Html = html,
                };
                SensorTip = sensorTip;
                IsLoading = false;
                cts.Dispose();
                cts = null;
                return true;
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException tcEx && tcEx.CancellationToken.IsCancellationRequested)
                {
                    return false;
                }
                IsLoading = false;
                await Alert.DisplayError(ex);
                return false;
            }
        }

        private async void SelectTip()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                var tcs = new TaskCompletionSource<SensorTip>();
                await Navigation.PushPopupAsync(new SelectSensorTipPopupPage(SensorTips, tcs));
                var result = await tcs.Task;
                await LoadTipAsync(result);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                await Alert.DisplayError(ex);
            }

            IsBusy = false;
        }
    }
}
