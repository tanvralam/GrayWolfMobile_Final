using GrayWolf.Enums;
using GrayWolf.Extensions;
using GrayWolf.Helpers;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;
using GrayWolf.Services;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GrayWolf.Converters
{
    public static class DeviceConverters
    {

        public static DeviceModelsEnum ToDeviceModelFromBits(byte PreSensorSerialNum)
        {
            DeviceModelsEnum model = DeviceModelsEnum.Unknown;

            uint mdl = (uint)PreSensorSerialNum >> 5;
            if (mdl == 0) model = DeviceModelsEnum.DirectSense_II_Probe;
            if (mdl == 1) model = DeviceModelsEnum.Particle_Counter;
            if (mdl == 2) model = DeviceModelsEnum.WolfRadio;
            if (mdl == 3) model = DeviceModelsEnum.Particulate_Meter;
            if (mdl == 4) model = DeviceModelsEnum.ZephyrXM; 

            

            return model;
        }

        public static GrayWolfDevice ToGrayWolfDevice(this GrayWolfDeviceDBO dbo, List<Reading> readings = null)
        {
            if (readings == null)
            {
                readings = new List<Reading>();
            }

            DeviceModelsEnum enumVal;
            if (Enum.IsDefined(typeof(DeviceModelsEnum), dbo.DeviceType))
            {
                enumVal = (DeviceModelsEnum)dbo.DeviceType;
                enumVal = enumVal == DeviceModelsEnum.Default ? DeviceModelsEnum.DirectSense_II_Probe : enumVal;
            }
            else
            {
                enumVal = DeviceModelsEnum.DirectSense_II_Probe;
            }

            var domain = new GrayWolfDevice
            {
                DeviceID = dbo.Id,
                Source = Enum.TryParse(dbo.Source, out DeviceSource source) ? source : DeviceSource.Default,
                DeviceSerialNum = dbo.DeviceSerialNum,
                DeviceType = enumVal,
                DeviceName = dbo.DeviceName,
                Notes = dbo.Notes,
                IsDeleted = dbo.IsDeleted,
                IsActive = dbo.IsActive,
                DeviceTypeLabel = dbo.DeviceTypeLabel,
                LocationXML = dbo.LocationXML,
                Status = dbo.Status,
                SensorStatus = dbo.SensorStatus,
                StatusEnum = TryParseStatus(dbo.StatusCode),
                SensorStatusEnum = TryParseSensorStatus(dbo.SensorStatusCode),
                BatteryStatusEnum = TryParseBatteryStatus(dbo.BatteryStatusCode),
                BatteryStatus = dbo.BatteryStatus,
                LastPing = dbo.LastPing,
                LastFolderUpdate = dbo.LastFolderUpdate,
                LatestReadings = dbo.LatestReadings,
                CalStatus = dbo.CalStatus,
                DeviceTextForList = dbo.DeviceTextForList,
                SecurityToken = dbo.SecurityToken,
                SecurityTokenExpiration = dbo.SecurityTokenExpiration,
                Uptime = dbo.Uptime,
                IsSelected = dbo.IsSelected,
                IsExpanded = dbo.IsExpanded,
                Title = dbo.Title,
                IsOnline = dbo.IsOnline,
                Data = readings?.ToList() ?? new List<Reading>()
            };
            return domain;
        }

        public static GrayWolfDeviceDBO ToGrayWolfDeviceDBO(this GrayWolfDevice domain)
        {
            var dbo = new GrayWolfDeviceDBO
            {
                Id = domain.DeviceID,
                Source = $"{domain.Source}",
                DeviceSerialNum = domain.DeviceSerialNum,
                DeviceName = domain.DeviceName,
                DeviceType = (int)domain.DeviceType,
                Notes = domain.Notes,
                IsDeleted = domain.IsDeleted,
                IsActive = domain.IsActive,
                DeviceTypeLabel = domain.DeviceTypeLabel,
                LocationXML = domain.LocationXML,
                Status = domain.Status,
                SensorStatus = domain.SensorStatus,
                StatusCode = (int)domain.StatusEnum,
                SensorStatusCode = (int)domain.SensorStatusEnum,
                BatteryStatus = domain.BatteryStatus,
                BatteryStatusCode = (int)domain.BatteryStatusEnum,
                LastPing = domain.LastPing,
                LastFolderUpdate = domain.LastFolderUpdate,
                LatestReadings = domain.LatestReadings,
                CalStatus = domain.CalStatus,
                DeviceTextForList = domain.DeviceTextForList,
                SecurityToken = domain.SecurityToken,
                SecurityTokenExpiration = domain.SecurityTokenExpiration,
                Uptime = domain.Uptime,
                IsSelected = domain.IsSelected,
                IsExpanded = domain.IsExpanded,
                Title = domain.Title,
                IsOnline = domain.IsOnline
            };
            return dbo;
        }

        public static GrayWolfDevice ToGrayWolfDevice(this DataDevice dto, string deviceId, ISensorsService sensorsService, ISettingsService settings)
        {
            var device = new GrayWolfDevice
            {
                Title = "Cloud",
                DeviceID = deviceId,
                DeviceSerialNum = dto.SerialNumber,
                SecurityToken = dto.Token,
                BatteryStatus = dto.Battery,
                BatteryStatusEnum = TryParseBatteryStatus(dto.BatteryCode),
                Status = dto.Status,
                SensorStatus = dto.SensorStatus,
                StatusEnum = TryParseStatus(dto.StatusCode),
                Uptime = int.TryParse(dto.Uptime, out int upTime) ? upTime : 0,
                IsOnline = dto.IsOnline,
                Data = dto.Data?
                .Select(x => x.ToReading(dto.TimeStamp, deviceId, dto.SerialNumber, sensorsService, settings))?
                .ToList() ?? new List<Reading>()
            };
            foreach (var sensor in device.Data)
            {
                sensor.TimeStamp = dto.TimeStamp;
            }
            return device;
        }

        public static LogDeviceDTO ToLogDevice(this GrayWolfDevice gwDevice)
        {
            gwDevice.UpdatePosition();
            var logDevice = new LogDeviceDTO
            {
                Id = gwDevice.DeviceID,
                Battery = gwDevice.BatteryStatus,
                SerialNumber = gwDevice.DeviceSerialNum,
                TimeStamp = gwDevice.LastPing,
                Status = gwDevice.Status,
                SensorStatus = gwDevice.SensorStatus,
                StationID = gwDevice.DeviceName,
                Data = gwDevice.LogReadings
                    .Select(p => p.ToLogDatum())
                    .ToList(),
                Position = gwDevice.Position,
                Generator = gwDevice.Title,
                IsSimulated = gwDevice.DeviceDisplayName.ToLowerInvariant().Contains("simulated")
            };

            return logDevice;
        }

        public static LJH_Device ToLjhDevice(this LogDeviceDTO dto)
        {
            return new LJH_Device
            {
                Id = dto.Id,
                Battery = dto.Battery,
                Generator = dto.Generator,
                StationID = dto.StationID,
                Position = dto.Position,
                SerialNumber = dto.SerialNumber,
                Status = dto.Status,
                SensorStatus = dto.SensorStatus,
                IsSimulated = dto.IsSimulated
            };
        }

        private static BatteryStatus TryParseBatteryStatus(int status) =>
            Enum.IsDefined(typeof(BatteryStatus), status) ? (BatteryStatus)status : BatteryStatus.UNKNOWN;

        private static ProbeStatus TryParseStatus(int status) =>
            Enum.IsDefined(typeof(ProbeStatus), status) ? (ProbeStatus)status : ProbeStatus.UNKNOWN;

        private static SensorStatus TryParseSensorStatus(int status) =>
            Enum.IsDefined(typeof(SensorStatus), status) ? (SensorStatus)status : SensorStatus.ERROR;

        public static bool IsProbeStatusOK(ProbeStatus status)
        {
            return ((status == ProbeStatus.OK) || (status == ProbeStatus.READY) ||
                (status == ProbeStatus.STABILIZING) || (status == ProbeStatus.LOGGING));
        }

        public static bool IsSensorOK(SensorStatus status)
        {
            return ((status == SensorStatus.STABILIZING) || (status == SensorStatus.OK) || (status == SensorStatus.DEMO));
        }

        public static bool IsProbeSensorError(ProbeStatus status)
        {
            switch (status)
            {
                case ProbeStatus.ERROR_BADSENSORVOLTAGE:
                case ProbeStatus.ERROR_NORTHTEMP:
                case ProbeStatus.ERROR_NOSENSORS:
                case ProbeStatus.HCHO_HI_CO:
                case ProbeStatus.HCHO_LO_RH:
                case ProbeStatus.HCHO_NO_CO:
                case ProbeStatus.LASER_ERROR:
                case ProbeStatus.PUMP_ERROR: return true;
            }
            return false;
        }


    }




}
