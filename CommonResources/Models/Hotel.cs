namespace CommonResources.Models
{
    public class Hotel : Common
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public List<Room> Rooms { get; set; }
        public List<User> Workers { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}

public class CreateHotelDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
}