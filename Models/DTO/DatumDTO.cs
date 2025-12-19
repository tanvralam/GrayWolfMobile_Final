using GrayWolf.Enums;

namespace GrayWolf.Models.DTO
{
    public class DatumDTO
    {
        public SensorType Code { get; set; }

        public SensorUnit UnitCode { get; set; }

        public string Sensor { get; set; }

        public string Unit { get; set; }

        public double Value { get; set; }

        public int Col { get; set; }

        public int Id { get; set; }
    }
}
