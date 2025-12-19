using System;
using System.Collections.Generic;

namespace GrayWolf.Models.DTO
{
    public class LogFileRowDTO
    {
        public DateTime Timestamp { get; set; }
        public List<SourceDTO> Sources { get; set; }
    }
}
