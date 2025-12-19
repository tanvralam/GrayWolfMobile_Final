using GrayWolf.Services;
using Newtonsoft.Json;

namespace GrayWolf.Models.DTO
{
    public class GetDevicesReturn
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
