using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Views.Popups;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;


namespace GrayWolf.ViewModels
{
    public class ZipPasswordPopupViewModel : BasePopupViewModel, IOnCustomPasswordConfirmedListener
    {
        #region variables
        public Color SelectedItemColor => App.Current.Resources["FileSelectedColor"] is Color selectedColor ? selectedColor : DefaultItemColor;
        public Color DefaultItemColor => Colors.Transparent;

        private ZipProtectionMode _protectionMode;
        public ZipProtectionMode ProtectionMode
        {
            get => _protectionMode;
            private set => SetProperty(ref _protectionMode, value);
        }

        private List<ZipProtectionMode> _protectionModes;
        public List<ZipProtectionMode> ProtectionModes
        {
            get => _protectionModes;
            private set => SetProperty(ref _protectionModes, value);
        }

        private string _customPassword;
        public string CustomPassword
        {
            get => _customPassword;
            set => SetProperty(ref _customPassword, value);
        }

        private bool _isError;
        public bool IsError
        {
            get => _isError;
            private set => SetProperty(ref _isError, value);
        }

        private bool _exportCsv;
        public bool ExportCsv
        {
            get => _exportCsv;
            private set => SetProperty(ref _exportCsv, value);
        }
        #endregion

        #region commands
        public ICommand ConfirmCommand { get; }
        public ICommand ChangeExportCsvCommand { get; }
        public ICommand SelectCommand { get; }
        public ICommand InfoCommand { get; }
        #endregion

        public ZipPasswordPopupViewModel()
        {
            ConfirmCommand = new Command(Confirm);
            SelectCommand = new Command<ZipProtectionMode>(Select);
            ChangeExportCsvCommand = new Command(ChangeExportCsv);
            InfoCommand = new Command(GoToExportInfo);
        }

        #region override
        public override void OnAppearing()
        {
            base.OnAppearing();
            Init();
        }

        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
            if (propertyName == nameof(CustomPassword))
            {
                IsError = false;
            }
        }
        #endregion

        #region methods
        private async void Init()
        {
            try
            {
                var array = Enum.GetValues(typeof(ZipProtectionMode));
                var list = new List<ZipProtectionMode>();
                foreach (var val in array)
                {
                    if (val is ZipProtectionMode mode)
                    {
                        list.Add(mode);
                    }
                }

                ExportCsv = Settings.IncludeCsvIntoExport;
                ProtectionModes = list;
                CustomPassword = await Settings.GetCustomZipPasswordAsync();
                ProtectionMode = Settings.ProtectionMode;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex);
                await Alert.DisplayError(ex);
                await OnBacksAsync();
            }
        }

        private async void Confirm()
        {
            if (!SetBusy())
            {
                return;
            }

            try
            {
                if (ProtectionMode == ZipProtectionMode.CustomPassword)
                {
                    IsError = !Validate();
                    if (IsError)
                    {
                        return;
                    }
                    await Navigation.PushPopupAsync(new ConfirmCustomPasswordPopupPage(CustomPassword, this));
                }
                else
                {
                    await ConfirmSettingsAsync("");
                    await OnBacksAsync();
                }
            }
            catch (Exception ex)
            {
                await Alert.DisplayError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Select(ZipProtectionMode mode)
        {
            ProtectionMode = mode;
            IsError = false;
        }

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(CustomPassword) && Regex.IsMatch(CustomPassword, Constants.CUSTOM_PASSWORD_REGEX);
        }

        private void ChangeExportCsv()
        {
            ExportCsv = !ExportCsv;
        }

        private async void GoToExportInfo()
        {
            if (!SetBusy())
            {
                return;
            }

            await Navigation.PushPopupAsync(new CsvExportInfoPopupPage());

            IsBusy = false;
        }

        public Task OnPasswordConfirmedAsync(string password)
        {
            return ConfirmSettingsAsync(password);
        }

        private Task ConfirmSettingsAsync(string password)
        {
            Settings.ProtectionMode = ProtectionMode;
            Settings.IncludeCsvIntoExport = ExportCsv;
            return Settings.SetCustomZipPasswordAsync(password);
        }
        #endregion
    }

    public interface IOnCustomPasswordConfirmedListener
    {
        Task OnPasswordConfirmedAsync(string password);
    }
}
