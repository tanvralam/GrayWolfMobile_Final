using GrayWolf.Converters;
using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using GrayWolf.ViewModels;
using GrayWolf.Views.Popups;
using RGPopup.Maui.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GrayWolf.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePageProbeView : ContentView
    {
        public static readonly BindableProperty ProbeProperty =
            BindableProperty.Create("Probe", typeof(GrayWolfDevice), typeof(HomePageProbeView), default(GrayWolfDevice));
        ObservableCollection<SensorStatusErrors> LstSensorStatusErrors;
        public GrayWolfDevice Probe
        {
            get { return (GrayWolfDevice)GetValue(ProbeProperty); }
            set { SetValue(ProbeProperty, value); }
        }

        public static readonly BindableProperty IsStatusOKProperty =
            BindableProperty.Create("IsStatusOK", typeof(bool), typeof(HomePageProbeView), default(bool));



        public bool IsStatusOK
        {
            get { return (bool)GetValue(IsStatusOKProperty); }
            set { SetValue(IsStatusOKProperty, value); }
        }

        public static readonly BindableProperty ProbeTappedCommandProperty = BindableProperty.Create(
            nameof(ProbeTappedCommand),
            typeof(ICommand),
            typeof(HomePageProbeView),
            default(ICommand));

        public ICommand ProbeTappedCommand
        {
            get { return (ICommand)GetValue(ProbeTappedCommandProperty); }
            set { SetValue(ProbeTappedCommandProperty, value); }
        }

        public static readonly BindableProperty SensorClickCommandProperty = BindableProperty.Create(
            nameof(SensorClickCommand),
            typeof(ICommand),
            typeof(HomePageProbeView),
            default,
            BindingMode.OneWay);

        public ICommand SensorClickCommand
        {
            get => (ICommand)GetValue(SensorClickCommandProperty);
            set => SetValue(SensorClickCommandProperty, value);
        }

        public HomePageProbeView()
        {
            InitializeComponent();
            Children[0].BindingContext = this;
        }

        private void Expand_Tapped(object sender, EventArgs e)
        {
            Probe.IsExpanded = !Probe.IsExpanded;
        }

        private void SensorStatusError_Tapped(object sender, EventArgs e)
        {
            try
            {
                var param = ((TappedEventArgs)e).Parameter;
                SetStatusErrorMessage((GrayWolfDevice)param);
                if (LstSensorStatusErrors.Count > 0)
                    NavigationService.Instance.Nav.PushPopupAsync(new SensorStatusErrorPopupPage(LstSensorStatusErrors));

            }
            catch (Exception ex)
            {

            }
        }

        private void SetStatusErrorMessage(GrayWolfDevice device)
        {
            LstSensorStatusErrors = new ObservableCollection<SensorStatusErrors>();

            int pidStabWarningCnt = 0;
            int generalStabWarningCnt = 0;

            foreach (var item in device.UIReadings)
            {
                string msg = "";
                if (string.IsNullOrWhiteSpace(item.Status))
                {
                    //msg = "Error";
                    return;
                }
                //else
                //{
                if (item.Status.ToLower() == "error")
                {
                    msg = Localization.Localization.Error_GeneralError;
                }
                else if (item.Status.ToUpper() == "HCHO_HI_CO")// I think these should be TO UPPER
                {
                    msg = Localization.Localization.Error_HCHO_HiCO; // msg = "High CO levels may influence HCHO readings.";
                }
                else if (item.Status.ToUpper() == "HCHO_LO_RH")// I think these should be TO UPPER
                {
                    msg = Localization.Localization.Error_HCHO_LoRH; // msg = "Low %RH levels may influence HCHO readings.";
                }
                else if (item.Status.ToUpper() == "HCHO_NO_CO")
                {
                    msg = Localization.Localization.Error_HCHO_NoCO; // msg = "No CO sensor – cannot display HCHO cross-sensitivity messages.";
                }
                else if (item.Status.ToUpper() == "GENERALERROR")
                {
                    msg = Localization.Localization.Error_GeneralError;// msg = "GENERAL ERROR";
                }
                else if (item.Status.ToUpper() == "CHECKCALIBRATION")
                {
                    msg = Localization.Localization.Error_CheckCal;  //msg = "Check Probe Calibration";
                }
                else if (item.Status.ToUpper() == "ERROR_BATTERYCRITICAL")
                {
                    msg = Localization.Localization.Error_ProbeBatteryCritical; //msg = "Probe battery critical";
                }
                else if (item.Status.ToUpper() == "ERROR_NORTHTEMP")
                {
                    msg = Localization.Localization.Error_MissingTRH;//                     msg = "Missing Temp/RH Sensor";
                }
                else if (item.Status.ToUpper() == "ERROR_BADRTC")
                {
                    msg = "Bad Real Time Clock reading";
                }
                else if (item.Status.ToUpper() == "ERROR_NOSENSORS")
                {
                    msg = Localization.Localization.Error_NoSensorsInstalled; // msg = "No sensors installed in Probe";
                }
                else if (item.Status.ToUpper() == "ERROR_BADSENSORVOLTAGE")
                {
                    msg = "Bad Sensor Voltage";
                }
                else if (item.Status.ToUpper() == "ERROR_FANFAULT")
                {
                    msg = Localization.Localization.Error_FanFault;//                   msg = "Fan Fault";
                }
                else if (item.Status.ToUpper() == "ERROR_INTERNALTEMP")
                {
                    msg = "High Internal Probe Temperature";
                }
                else if (item.Status.ToUpper() == "ERROR_WIFIDISCONNECT")
                {
                    msg = Localization.Localization.Error_WiFi_Disco;//                    msg = "Wi-Fi is disconnected";
                }
                else if (item.Status.ToUpper() == "ERROR_LOWMEMORY")
                {
                    msg = "Probe Memory Error";
                }


                if (device.StatusEnum == ProbeStatus.STABILIZING)
                {
                    if ((pidStabWarningCnt == 0) && (SensorsService.IsPID_static((int)item.SensorCode)))
                    {
                        SensorStatusErrors sensor = new SensorStatusErrors { SerialNo = device.DeviceSerialNum, SensorCode = "PID", SensorStatusError = Localization.Localization.SensorStabilization_PID};
                        LstSensorStatusErrors.Add(sensor);
                        pidStabWarningCnt++;
                    }
                    else if (generalStabWarningCnt==0)
                    {
                        SensorStatusErrors sensor = new SensorStatusErrors { SerialNo = device.DeviceSerialNum, SensorCode = "All Sensors", SensorStatusError = Localization.Localization.SensorStabilization_Generic};
                        LstSensorStatusErrors.Add(sensor);
                        generalStabWarningCnt++;
                    }

                }


                if (!string.IsNullOrWhiteSpace(msg))
                {
                    SensorStatusErrors sensor = new SensorStatusErrors { SerialNo = device.DeviceSerialNum, SensorCode = item.SensorCode.ToString(), SensorStatusError = msg };
                    LstSensorStatusErrors.Add(sensor);
                }
            }





        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(Probe) && Probe != null && NameLabel != null)
            {
                var d = Probe.UIReadings;
                if(DeviceConverters.IsProbeSensorError(Probe.StatusEnum))
                {
                    NameLabel.TextColor = Colors.Red;
                }
                else
                {
                    NameLabel.TextColor = Colors.Black;
                }

                SetStatusErrorMessage(Probe);
                if (LstSensorStatusErrors.Count > 0)
                {
                    IsStatusOK = GrayWolf.Converters.DeviceConverters.IsSensorOK(Probe.SensorStatusEnum);
                }
                else
                {
                    IsStatusOK = true;
                }
            }
        }
    }
}