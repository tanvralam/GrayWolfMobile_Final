using Newtonsoft.Json;
using System.Collections.Generic;



namespace GrayWolf.Models.DTO
{
    public class LJH_Holder
    {
        [JsonProperty("version")]
        public float Version { get; set; }

        [JsonProperty("simulation")]
        public bool IsSimulated { get; set; }

        [JsonProperty("sets")]
        public List<LJH_Set> Sets { get; set; } = new List<LJH_Set>();

        [JsonProperty("softwareVersion")]
        public string SoftwareVersion { get; set; } = VersionTracking.CurrentVersion;

        [JsonProperty("platform")]
        public string Platform { get; set; } = Device.RuntimePlatform;
    }
}
