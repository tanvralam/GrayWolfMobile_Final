using GrayWolf.Models.Domain;

namespace GrayWolf.Messages
{
    public class DeviceConfigurationChangedMessage
    {
        public GrayWolfDevice Device { get; set; }
    }
}
