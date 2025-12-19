using GrayWolf.Models.Domain;
using System;
using System.Collections.Generic;

namespace GrayWolf.Messages
{
    public class BleDevicesUpdatedMessage
    {
        public List<BleDevice> Devices { get; }

        public DateTime Time { get; set; }


        public BleDevicesUpdatedMessage(List<BleDevice> devices)
        {
            Devices = devices;
            Time = DateTime.Now;
        }
    }
}
