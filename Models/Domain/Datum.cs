using GrayWolf.Enums;
using MvvmHelpers;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace GrayWolf.Models.Domain
{
    public class Datum : ObservableObject
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _deviceSerialNumber;
        public string DeviceSerialNumber
        {
            get => _deviceSerialNumber;
            set => SetProperty(ref _deviceSerialNumber, value);
        }

        private string _sensor;
        public string Sensor
        {
            get { return _sensor; }
            set => SetProperty(ref _sensor, value);
        }

        private SensorType _type;
        public SensorType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private SensorUnit _sourceUnit;
        public SensorUnit SourceUnit
        {
            get => _sourceUnit;
            set => SetProperty(ref _sourceUnit, value);
        }

        private SensorUnit _convertedUnit;
        public SensorUnit ConvertedUnit
        {
            get => _convertedUnit;
            set => SetProperty(ref _convertedUnit, value);
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set => SetProperty(ref _status, value);
        }

        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set => SetProperty(ref _timeStamp, value);
        }

        private int _channel;
        public int Channel
        {
            get => _channel;
            set => SetProperty(ref _channel, value);
        }

        private string _deviceId;
        public string DeviceId
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        public void Update(Datum newData)
        {
        }
    }
}
