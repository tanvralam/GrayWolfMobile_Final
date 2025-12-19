using System;
using System.Collections.Generic;
using System.Text;
using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using GrayWolf.Services;

namespace GrayWolf.Messages
{
    public class DeviceUpdateMsg
    {
        public DeviceSource Source { get; set; }
        public List<GrayWolfDevice> Devices { get; set; } 
        
        public DateTime Time { get; set; }

        public DeviceUpdateMsg()
        {
            Time= DateTime.Now;
        }
    }


}
