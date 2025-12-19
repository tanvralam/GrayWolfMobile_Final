using GrayWolf.Models.DTO;

namespace GrayWolf.Messages
{
    public class LogRowAddedMessage
    {
        public LJH_Holder Holder { get; }

        public string Line { get; }

        public LogRowAddedMessage(LJH_Holder holder, string line)
        {
            Holder = holder;
            Line = line;
        }
    }
}
