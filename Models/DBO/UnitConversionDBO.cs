using GrayWolf.Interfaces;
using SQLite;

namespace GrayWolf.Models.DBO
{
    public class UnitConversionDBO : IDbo<string>
    {
        [PrimaryKey]
        public string Id { get; set; }

        public int TargetUnitCode { get; set; }
    }
}
