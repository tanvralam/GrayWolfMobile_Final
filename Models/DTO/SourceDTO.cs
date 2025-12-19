using System.Collections.Generic;

namespace GrayWolf.Models.DTO
{
    public class SourceDTO
    {
        public string DataSource { get; set; }
        public List<LogDeviceDTO> Devices { get; set; }
    }
}
