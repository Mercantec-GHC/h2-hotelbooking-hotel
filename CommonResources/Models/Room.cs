namespace CommonResources.Models
{
    public class Room : Common
    {
        public List<Booking> Bookings { get; set; }
        public string HotelID { get; set; }
        public Hotel Hotel { get; set; }
    }
}