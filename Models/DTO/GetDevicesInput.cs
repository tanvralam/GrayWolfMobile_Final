using Newtonsoft.Json;

namespace GrayWolf.Models.DTO
{
    public class GetDevicesInput
    {
        [JsonProperty("dataParameters")]
        public string DataParameters { get; set; }
    }
}
