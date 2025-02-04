namespace HotelsCommons.Models
{
    public class Room : Common
    {
        public string HotelID { get; set; }
        public string Price { get; set; }
        public List<Booking> Bookings { get; set; }
        public Hotel Hotel { get; set; }
    }
    public class RoomDTO
    {
        public string HotelID { get; set; }
        public string Price { get; set; }
    }
}