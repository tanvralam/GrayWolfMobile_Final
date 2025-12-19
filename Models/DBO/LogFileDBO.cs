using GrayWolf.Interfaces;
using SQLite;

namespace GrayWolf.Models.DBO
{
    public class LogFileDBO : IDbo<int>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool TrendLoggingActive { get; set; }

        public int LoggingInterval { get; set; }

        public bool IsSelected { get; set; }

        public bool HasContent { get; set; }
        
        public bool IsGraphAvailable { get; set; }

        public int ParameterNamesDisplayMode { get; set; }
    }
}
