namespace CommonResources.Models
{
    public class Booking : Common
    {
        public string UserID { get; set; }
        public string RoomID { get; set; }

        public User User { get; set; }
        public Room Room { get; set; }
    }

    public class BookingDTO
    {
        public string UserID { get; set; }
        public string RoomID { get; set; }
    }
}