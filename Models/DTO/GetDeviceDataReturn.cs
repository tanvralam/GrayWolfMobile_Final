using Newtonsoft.Json;
using System.Collections.Generic;

namespace GrayWolf.Models.DTO
{
    public class GetDeviceDataReturn
    {
        [JsonProperty("dataSource")]
        public string DataSource { get; set; }

        [JsonProperty("devices")]
        public List<DataDevice> Devices { get; set; }
    }
}
