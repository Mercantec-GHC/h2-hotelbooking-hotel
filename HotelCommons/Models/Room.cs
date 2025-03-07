using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HotelsCommons.Models
{
    public class Room : Common
    {
        public string HotelID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
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
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string HotelID { get; set; }

        [Required(ErrorMessage = "Daily Price is required.")]
        [Range(50.0, 1000000.0, ErrorMessage = "Daily Price must be between 50 and 1000000.")]
        public float DailyPrice { get; set; }
    }

    public class RoomResult
    {
        public string Id { get; set; }
        public string HotelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float DailyPrice { get; set; }
        public List<RoomBookinsResult> Bookings { get; set; } = new List<RoomBookinsResult>();
        public List<RoomImageResult> Images { get; set; } = new List<RoomImageResult>();
    }

    public class RoomBookinsResult
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RoomImageResult
    {
        public string FileName { get; set; }
    }
}