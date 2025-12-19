using Newtonsoft.Json;
using GeolocatorPlugin.Abstractions;

namespace GrayWolf.Models.DTO
{
    public class LJH_Device
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("stationID")]
        public string StationID { get; set; }

        [JsonProperty("generator")]
        public string Generator { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("sensorStatus")]
        public string SensorStatus { get; set; }

        [JsonProperty("battery")]
        public string Battery { get; set; }

        [JsonProperty("position")]
        public Position Position { get; set; }

        [JsonIgnore]
        public bool IsSimulated { get; set; }
    }
}
