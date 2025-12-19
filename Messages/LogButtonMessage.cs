using GrayWolf.Models.Domain;

namespace GrayWolf.Messages
{
    public class LogButtonMessage
    {
        public GrayWolfDevice GrayWolfDevice { get; }

        public LogButtonMessage(GrayWolfDevice device)
        {
            GrayWolfDevice = device;
        }
    }
}
