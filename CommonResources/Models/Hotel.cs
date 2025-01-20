namespace CommonResources.Models
{
    public class Hotel : Common
    {
        public List<Room> Rooms { get; set; }
        public List<User> Users { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
