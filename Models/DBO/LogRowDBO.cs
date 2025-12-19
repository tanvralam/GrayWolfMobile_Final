using GrayWolf.Interfaces;
using SQLite;

namespace GrayWolf.Models.DBO
{
    public class LogRowDBO : IDbo<int>
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string TimeStamp { get; set; }

        public string Json { get; set; }
    }
}
