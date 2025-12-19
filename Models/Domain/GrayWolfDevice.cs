using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Enums;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using MvvmHelpers;
using GeolocatorPlugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GrayWolf.Models.Domain
{
    public class GrayWolfDevice : ObservableObject, IEquatable<GrayWolfDevice>
    {
        private IGeolocationService GeolocationService { get; }

        public GrayWolfDevice()
        {
            GeolocationService = Ioc.Default.GetService<IGeolocationService>();
        }

        private DeviceSource _source;
        public DeviceSource Source
        {
            get { return _source; }
            set => SetProperty(ref _source, value);
        }

        private string _deviceId;
        public string DeviceID
        {
            get { return _deviceId; }
            set => SetProperty(ref _deviceId, value);
        }

        private string _deviceSerialNum;
        public string DeviceSerialNum
        {
            get { return _deviceSerialNum; }
            set => SetProperty(ref _deviceSerialNum, value);
        }

        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set => SetProperty(ref _deviceName, value);
        }

        private DeviceModelsEnum _deviceType;
        public DeviceModelsEnum DeviceType
        {
            get { return _deviceType; }
            set
            {
                SetProperty(ref _deviceType, value);
                OnPropertyChanged(nameof(DeviceModelName));
            }
        }

        public string DeviceModelName => GetDeviceModelName();

        public string DeviceDisplayName => GetDeviceDisplayName();

        public bool IsSimulated => DeviceDisplayName.ToLowerInvariant().Contains("simulated");

        private string _notes;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private bool _isDeleted;
        public bool IsDeleted
        {
            get => _isDeleted;
            set => SetProperty(ref _isDeleted, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        private string _deviceTypeLabel;
        public string DeviceTypeLabel
        {
            get => _deviceTypeLabel;
            set => SetProperty(ref _deviceTypeLabel, value);
        }

        private string _locationXML;
        public string LocationXML
        {
            get => _locationXML;
            set => SetProperty(ref _locationXML, value);
        }

        private ProbeStatus _statusEnum;
        public ProbeStatus StatusEnum
        {
            get => _statusEnum;
            set => SetProperty(ref _statusEnum, value);
        }

        private SensorStatus _sensorStatusEnum;
        public SensorStatus SensorStatusEnum
        {
            get => _sensorStatusEnum;
            set => SetProperty(ref _sensorStatusEnum, value);
        }

        public bool CanOpenProbePage => StatusEnum != ProbeStatus.UNKNOWN;

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _sensorStatus;
        public string SensorStatus
        {
            get => _sensorStatus;
            set => SetProperty(ref _sensorStatus, value);
        }

        private BatteryStatus _batteryStatusEnum;
        public BatteryStatus BatteryStatusEnum
        {
            get => _batteryStatusEnum;
            set => SetProperty(ref _batteryStatusEnum, value);
        }

        private string _batteryStatus;
        public string BatteryStatus
        {
            get => _batteryStatus;
            set => SetProperty(ref _batteryStatus, value);
        }

        private DateTime _lastPing;
        public DateTime LastPing
        {
            get { return _lastPing; }
            set => SetProperty(ref _lastPing, value);
        }

        private DateTime _lastFolderUpdate;
        public DateTime LastFolderUpdate
        {
            get => _lastFolderUpdate;
            set => SetProperty(ref _lastFolderUpdate, value);
        }

        private string _latestReadings;
        public string LatestReadings
        {
            get => _latestReadings;
            set => SetProperty(ref _latestReadings, value);
        }

        private string _callStatus;
        public string CalStatus
        {
            get => _callStatus;
            set => SetProperty(ref _callStatus, value);
        }

        private string _deviceTextForList;
        public string DeviceTextForList
        {
            get => _deviceTextForList;
            set => SetProperty(ref _deviceTextForList, value);
        }

        private string _securityToken;
        public string SecurityToken
        {
            get => _securityToken;
            set => SetProperty(ref _securityToken, value);
        }

        private DateTime _securityTokenExpiration;
        public DateTime SecurityTokenExpiration
        {
            get => _securityTokenExpiration;
            set => SetProperty(ref _securityTokenExpiration, value);
        }

        private long _uptime;
        public long Uptime
        {
            get => _uptime;
            set => SetProperty(ref _uptime, value);
        }

        private List<Reading> _data;
        public List<Reading> Data
        {
            get { return _data; }
            set => SetProperty(ref _data, value);
        }

        public List<Reading> UIReadings => Data
            .Where(x => x.IsLogged)
            .ToList();

        public List<Reading> LogReadings => Data
            .Where(x => x.IsLogged)
            .ToList();

        private Position _position = new Position();
        public Position Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set => SetProperty(ref _isSelected, value);
        }

        private string _title = "";
        public string Title
        {
            get { return _title; }
            set => SetProperty(ref _title, value);
        }

        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            set => SetProperty(ref _isOnline, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GrayWolfDevice)obj);
        }

        public void UpdateData(IEnumerable<Reading> newData)
        {
            if (newData == null)
            {
                return;
            }
            lock ("dataUpdate")
            {
                foreach (var reading in newData)
                {
                    if (_data.FirstOrDefault(p => p.Id == reading.Id) is Reading existing)
                    {
                        existing.OriginalUnit = reading.OriginalUnit;
                        existing.ConvertedUnit = reading.ConvertedUnit;
                        existing.TimeStamp = reading.TimeStamp;
                        existing.Status = reading.Status;
                        existing.SensorId = reading.SensorId;
                        existing.DeviceSerialNumber = reading.DeviceSerialNumber;
                        existing.Channel = reading.Channel;
                        existing.Name = reading.Name;
                        existing.IsLogged = reading.IsLogged;
                    }
                    else
                    {
                        if(Data.Count <= reading.Channel)
                        {
                            Data.Add(reading);
                        }
                        else
                        {
                            Data.Insert(reading.Channel, reading);
                        }
                    }
                }
                Data.RemoveAll(x => !newData.Any(y => y.Id == x.Id));
                OnPropertyChanged(nameof(Data));
                OnPropertyChanged(nameof(UIReadings));
                OnPropertyChanged(nameof(LogReadings));
            }
        }

        public void UpdatePosition()
        {
            Position = GeolocationService.CurrentPosition;
        }

        // TODO: Finalize what gets displayed on screen 
        private string GetDeviceDisplayName()
        {
            string displayname = "";
            if ((DeviceName ?? "").Length > 0) displayname = DeviceName;
            if (displayname.Length == 0) displayname = DeviceSerialNum.ToUpper();
            if (Source == DeviceSource.Cloud) displayname += " (Cloud)";
            return displayname;
        }

        private string GetDeviceModelName()
        {
            switch (DeviceType)
            {
                case DeviceModelsEnum.AdvancedSense:
                case DeviceModelsEnum.Tablet:
                case DeviceModelsEnum.Laptop:
                    return $"{DeviceType}";
                case DeviceModelsEnum.Particle_Counter:
                    return "Particle Counter";
                case DeviceModelsEnum.DirectSense_II_Probe:
                    return "DirectSense II";
                case DeviceModelsEnum.ZephyrXM:
                    return "Zephyr LDP";
                default:
                    return "Probe";
            }
        }

        public bool Equals(GrayWolfDevice other)
        {
            if (other is null)
            {
                return false;
            }
            return ReferenceEquals(this, other) || _deviceSerialNum == other._deviceSerialNum;
        }

        public override int GetHashCode()
        {
            return (_deviceSerialNum != null ? _deviceSerialNum.GetHashCode() : 0);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(DeviceName):
                case nameof(DeviceSerialNum):
                case nameof(Source):
                    OnPropertyChanged(nameof(DeviceDisplayName));
                    break;
                case nameof(StatusEnum):
                    OnPropertyChanged(nameof(CanOpenProbePage));
                    break;
                case nameof(DeviceDisplayName):
                    OnPropertyChanged(nameof(IsSimulated));
                    break;
            }
        }
    }
}
