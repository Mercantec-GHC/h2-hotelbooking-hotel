namespace HotelsCommons.Models
{
    public class Booking : Common
    {
        public string UserID { get; set; }
        public string RoomID { get; set; }
        public float Price { get; set; }
        public bool AllInclusive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public User User { get; set; }
        public Room Room { get; set; }
    }

    public class BookingDTO
    {
        public string UserID { get; set; }
        public string RoomID { get; set; }
        public float Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CreateBookingDTO
    {
        public string UserID { get; set; }
        public string RoomID { get; set; }
        public bool AllInclusive { get; set; }
        public string? DiscountCode {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class BookingResult
    {
        public string Id { get; set; }
        public string UserID { get; set; }
        public string RoomID { get; set; }
        public float Price { get; set; }
        public bool AllInclusive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
