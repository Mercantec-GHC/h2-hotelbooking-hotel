using BackendAPI.Data;
using HotelsCommons.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly DatabaseContext _Context;

        public RoomController(DatabaseContext context)
        {
            _Context = context;
        }

        [HttpGet("GetRoomWithBooking")]
        public async Task<ActionResult<Room>> GetRooms()
        {
            var rooms = await _Context.Rooms
       .Include(r => r.Bookings)
       .Include(r => r.Images) // Include Room Images
       .ToListAsync();

            var roomDTO = rooms.Select(r => new
            {
                r.ID,
                r.HotelID,
                r.DailyPrice,
                r.CreatedAt,
                r.UpdatedAt,
                Bookings = r.Bookings.Select(b => new
                {
                    b.ID,
                    b.RoomID,
                    b.UserID,
                    b.CreatedAt,
                    b.UpdatedAt
                }).ToList(),
                Images = r.Images.Select(img => img.ImagePath).ToList() // Include image paths
            });


            return Ok(roomDTO);
        }

   
        [HttpGet("GetAllAvailableRooms")]
        public async Task<ActionResult<List<Room>>> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            var availableRooms = await _Context.Rooms
                .Where(room => !_Context.Bookings.Any(b =>
                    b.RoomID == room.ID &&
                    b.StartDate < endDate &&  // Booking starts before the selected end date
                    b.EndDate > startDate     // Booking ends after the selected start date
                ))
                .ToListAsync();

            return Ok(availableRooms);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetSpecificRoom(string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

            return Ok(room);
        }

        [HttpPost("CreateRoom")]
        public async Task<IActionResult> CreateRoom([FromBody]CreateRoomDTO roomDto)
        {

            var room = new Room()
            {
                ID = Guid.NewGuid().ToString("N"),
                HotelID = roomDto.HotelID,
                DailyPrice = roomDto.DailyPrice,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
                
            };
            var images = DefaultImages.RoomImages.Select(imagePath => new RoomImage { ImagePath = imagePath }).ToList();
            room.Images = images;

            var hotel = await _Context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(r => r.ID == roomDto.HotelID);

            hotel.Rooms.Add(room);
            _Context.Rooms.Add(room);
            await _Context.SaveChangesAsync();
            return Ok(roomDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(Room roomDTO, string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

           //Updates properties of the room
            room.DailyPrice = roomDTO.DailyPrice;
            room.UpdatedAt = roomDTO.UpdatedAt;

            await _Context.SaveChangesAsync();

            return Ok(room);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

            _Context.Rooms.Remove(room);

            await _Context.SaveChangesAsync();

            return StatusCode(200,$"Room deleted succesfully {room}");
        }


    }
}
