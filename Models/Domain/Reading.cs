using GrayWolf.Converters;
using GrayWolf.Enums;
using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace GrayWolf.Models.Domain
{
    [JsonConverter(typeof(ReadingFromBleJsonSensorConverter))]
    public class Reading : ObservableObject
    {
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private SensorUnit _originalUnit;
        public SensorUnit OriginalUnit
        {
            get => _originalUnit;
            set => SetProperty(ref _originalUnit, value);
        }

        private SensorUnit _convertedUnit;
        public SensorUnit ConvertedUnit
        {
            get => _convertedUnit;
            set => SetProperty(ref _convertedUnit, value);
        }

        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }

        private int _channel;
        public int Channel
        {
            get => _channel;
            set => SetProperty(ref _channel, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _deviceId;
        public string DeviceId
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        private SensorType _sensorCode;
        public SensorType SensorCode
        {
            get => _sensorCode;
            set => SetProperty(ref _sensorCode, value);
        }


        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _deviceSerialNumber;
        public string DeviceSerialNumber
        {
            get => _deviceSerialNumber;
            set => SetProperty(ref _deviceSerialNumber, value);
        }

        private int _sensorId;
        public int SensorId
        {
            get => _sensorId;
            set => SetProperty(ref _sensorId, value);
        }

        private bool _isLogged;
        public bool IsLogged
        {
            get => _isLogged;
            set => SetProperty(ref _isLogged, value);
        }

        public string SensorIdentifier
        {
            get
            {
                try
                {
                    string di = "";

                    if (SensorId>10000)
                    {
                        di += _sensorId.ToString();
                    }
                    if (DeviceSerialNumber != null) di += " (" + DeviceSerialNumber + ")";
                    if (di.Length == 0) di = "Cloud " + DeviceId.ToString();
                    return di;
                }
                catch(Exception ex)
                {
                    return "";
                }
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(DeviceSerialNumber):
                case nameof(SensorId):
                case nameof(DeviceId):
                    OnPropertyChanged(nameof(SensorIdentifier));
                    break;
            }
        }
    }
}
