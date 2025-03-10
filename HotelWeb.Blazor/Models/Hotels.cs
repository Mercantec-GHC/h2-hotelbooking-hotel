using HotelsCommons.Models;

namespace HotelWeb.Blazor.Models
{
    public class AllHotelsResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
    }

    public class HotelRoomResult
    {
        public string HotelID { get; set; }
        public int DailyPrice { get; set; }
        public string Id { get; set; }
    }

    public class HotelResult
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Id { get; set; }
        public List<HotelRoomResult> Rooms { get; set; }
    }

}
