using System;
using System.Collections.Generic;

namespace GrayWolf.Models.DTO
{
    public class DataDevice
    {
        public float Version { get; set; }
        public string Generator { get; set; }
        public string SerialNumber { get; set; }
        public string ComID { get; set; }
        public string StationID { get; set; }
        public string Token { get; set; }
        public string Battery { get; set; }
        public int BatteryCode { get; set; }
        public string Status { get; set; }
        public string SensorStatus { get; set; }
        public int StatusCode { get; set; }
        public int SensorStatusCode { get; set; }
        public string Uptime { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsOnline { get; set; }
        public List<DatumDTO> Data { get; set; }
    }
}
