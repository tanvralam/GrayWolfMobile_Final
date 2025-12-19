using GrayWolf.Models.Domain;

namespace GrayWolf.Utility
{
    public class DeviceDisconnectedEventArgs
    {
        public BleDevice Device { get; }

        public bool IsWaitingForReconnection { get; set; }

        public DeviceDisconnectedEventArgs(BleDevice device, bool isWaitingForReconnection)
        {
            Device = device;
            IsWaitingForReconnection = isWaitingForReconnection;
        }
    }
}
