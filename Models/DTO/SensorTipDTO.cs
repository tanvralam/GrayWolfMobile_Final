namespace GrayWolf.Models.DTO
{
    public class SensorTipDTO
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public SensorTipFamilyDTO Family { get; set; }
    }
}
