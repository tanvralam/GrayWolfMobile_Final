using Newtonsoft.Json;

namespace GrayWolf.Models.DTO
{
    public class LJH_Column
    {
        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; } // this is Serial Number of probe that collected data

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("sensor")]
        public string Sensor { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("unitCode")]
        public int UnitCode { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; } // this is ID of the sensor
    }
}
