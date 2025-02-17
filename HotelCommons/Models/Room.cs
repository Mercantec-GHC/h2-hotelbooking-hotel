using Microsoft.AspNetCore.Http;

namespace HotelsCommons.Models
{
    public class Room : Common
    {
        public string HotelID { get; set; }
        public float DailyPrice { get; set; }
        public List<Booking> Bookings { get; set; }
        public Hotel Hotel { get; set; }
        public List<RoomImage> Images { get; set; } = new List<RoomImage>();

    }
    public class RoomDTO
    {
        public string ID { get; set; }
        public string HotelID { get; set; }
        public float DailyPrice { get; set; }
   
    }
    public class CreateRoomDTO
    {
        public string HotelID { get; set; }
        public float DailyPrice { get; set; }
        

    }
}