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
        public ICollection<Room> Rooms { get; set; }
        //public List<User> Workers { get; set; }
        public ICollection<Booking> Bookings { get; set; }

        public ICollection<UserHotel> UserHotels { get; set; }

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
        public List<RoomResult> Rooms { get; set; }
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
