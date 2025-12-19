using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Models.DTO
{
    public class LogFileDTO
    {
        public float Version { get; set; }
        public string Generator { get; set; }
        public List<LogFileRowDTO> Rows { get; set; }
    }
}
