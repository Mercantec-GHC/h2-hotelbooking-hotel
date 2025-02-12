namespace HotelsCommons.Models
{
    public class Hotel : Common
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public List<Room> Rooms { get; set; }
        public List<User> Workers { get; set; }
        public List<Booking> Bookings { get; set; }
        
    }

    public class HotelDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<RoomDTO> Rooms { get; set; }
    }
    public class CreateHotelDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
       
    }
}
