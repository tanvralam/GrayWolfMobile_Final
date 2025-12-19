using GeolocatorPlugin.Abstractions;
using System;
using System.Collections.Generic;

namespace GrayWolf.Models.DTO
{
    public class LogDeviceDTO
    {
        public string Id { get; set; }
        public float Version { get; set; }
        public string Generator { get; set; }
        public string SerialNumber { get; set; }
        public string ComID { get; set; }
        public string StationID { get; set; }
        public string Token { get; set; }
        public string Battery { get; set; }
        public string Status { get; set; }
        public string Uptime { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<LogDatumDTO> Data { get; set; }
        public Position Position { get; set; }
        public bool IsSimulated { get; set; }
        public string SensorStatus { get; internal set; }
    }
}
