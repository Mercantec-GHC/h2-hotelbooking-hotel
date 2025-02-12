namespace HotelsCommons.Models
{
    public class DiscountCode : Common
    {
        public string Code { get; set; }
        public int Percentage { get; set; }
    }
    public class CreateDCDTO
    {
        public string Code { get; set; }
        public int Percentage { get; set; }
    }
}