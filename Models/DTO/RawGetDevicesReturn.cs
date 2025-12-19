using Newtonsoft.Json;

namespace GrayWolf.Models.DTO
{
    public class RawGetDevicesReturn
    {
        [JsonProperty("d")]
        public string D { get; set; }
    }
}
