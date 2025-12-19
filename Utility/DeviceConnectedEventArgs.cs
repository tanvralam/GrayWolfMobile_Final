using GrayWolf.Models.Domain;
using System;

namespace GrayWolf.Utility
{
    public class DeviceConnectedEventArgs : EventArgs
    {
        public BleDevice Device { get; }

        public DeviceConnectedEventArgs(BleDevice device)
        {
            Device = device;
        }
    }
}
