using GrayWolf.Interfaces;
using SQLite;
using System;

namespace GrayWolf.Models.DBO
{
    public class ReadingDBO : IDbo<string>
    {
        [PrimaryKey]
        public string Id { get; set; }

        public int UnitCode { get; set; }

        public string Value { get; set; }

        public DateTime TimeStamp { get; set; }

        public int Channel { get; set; }

        public int SensorId { get; set; }

        public string DeviceId { get; set; }

        public int SensorCode { get; set; }

        public string Status { get; set; }

        public string DeviceSerialNumber { get; set; }

        public bool IsLogged { get; set; }
    }
}
