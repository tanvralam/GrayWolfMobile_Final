using Newtonsoft.Json;
using System.Collections.Generic;

namespace GrayWolf.Models.DTO
{
    public class LJH_Set
    {
        [JsonProperty("setID")]
        public int SetID { get; set; }

        [JsonProperty("columns")]
        public List<LJH_Column> Columns = new List<LJH_Column>();

        [JsonProperty("devices")]
        public List<LJH_Device> Devices = new List<LJH_Device>();

        [JsonProperty("tag")]
        public string TAG { get; set; }
    }
}
