using GrayWolf.Interfaces;
using SQLite;
using System;

namespace GrayWolf.Models.DBO
{
    public class GrayWolfDeviceDBO : IDbo<string>
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Source { get; set; }

        public string DeviceSerialNum { get; set; }

        public string DeviceName { get; set; }

        public int DeviceType { get; set; }

        public string Notes { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string DeviceTypeLabel { get; set; }

        public string LocationXML { get; set; }

        public string Status { get; set; }

        public string SensorStatus { get; set; }

        public bool IsOnline { get; set; }

        public int StatusCode { get; set; }

        public int SensorStatusCode { get; set; }

        public string BatteryStatus { get; set; }

        public int BatteryStatusCode { get; set; }

        public DateTime LastPing { get; set; }

        public DateTime LastFolderUpdate { get; set; }

        public string LatestReadings { get; set; }

        public string CalStatus { get; set; }

        public string DeviceTextForList { get; set; }

        public string SecurityToken { get; set; }

        public DateTime SecurityTokenExpiration { get; set; }

        public long Uptime { get; set; }

        public bool IsSelected { get; set; }

        public string Title { get; set; }

        public bool IsExpanded { get; set; }
    }
}
