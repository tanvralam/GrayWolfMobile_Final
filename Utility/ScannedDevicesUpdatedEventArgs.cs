using GrayWolf.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GrayWolf.Utility
{
    public class ScannedDevicesUpdatedEventArgs : EventArgs
    {
        public List<BleDevice> Devices { get; }

        public ScannedDevicesUpdatedEventArgs(IEnumerable<BleDevice> devices)
        {
            Devices = devices.ToList();
        }
    }
}
