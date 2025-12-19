using Acr.UserDialogs;
using GrayWolf.Extensions;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using RGPopup.Maui.Extensions;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using GrayWolf.Utility;
using MvvmHelpers;
using System.Collections.ObjectModel;
using GrayWolf.Enums;
using System.Linq;

namespace GrayWolf.ViewModels
{
    public class SensorStatusErrorPopupViewModel : BasePopupViewModel
    {
        #region variables
        private Interfaces.INavigationService NavigationService { get; }
        #endregion
        #region Properties
        private string _sensorStatusError;
        
        public string SensorStatusError
        {
            get { return _sensorStatusError; }
            set
            {
                _sensorStatusError = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<SensorStatusErrors> _sensorStatusErrors;

        public ObservableCollection<SensorStatusErrors> LstSensorStatusErrors
        {
            get { return _sensorStatusErrors; }
            set
            {
                _sensorStatusErrors = value;
                RaisePropertyChanged();
            }
        }

        private bool _isStatusOK;

        public bool IsStatusOK
        {
            get { return _isStatusOK; }
            set
            {
                _isStatusOK = value;
                RaisePropertyChanged();
            }
        }
        #endregion 
        #region commands
        #endregion

        public SensorStatusErrorPopupViewModel(object param)
        {
            try
            {
                LstSensorStatusErrors = (ObservableCollection<SensorStatusErrors>)param;
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class SensorStatusErrors : ObservableObject
    {
        private string _serialNo;
        public string SerialNo
        {
            get => _serialNo;
            set => SetProperty(ref _serialNo, value);
        }

        private string _sensorCode;
        public string SensorCode
        {
            get => _sensorCode;
            set => SetProperty(ref _sensorCode, value);
        }

        private string _sensorStatusError;
        public string SensorStatusError
        {
            get => _sensorStatusError;
            set => SetProperty(ref _sensorStatusError, value);
        }
    }
}
