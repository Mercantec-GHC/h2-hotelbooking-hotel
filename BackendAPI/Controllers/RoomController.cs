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

        [HttpGet]
        public async Task<ActionResult<Room>> GetRooms()
        {
            var rooms = await _Context.Rooms.Include(r => r.Bookings).ToListAsync();
            var roomDTO = rooms.Select(r => new Room
            {
                HotelID = r.HotelID,
                Price = r.Price,
                Bookings = r.Bookings.Select(b => new Booking { UserID = b.UserID }).ToList()
            }).ToList();


            return Ok(roomDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetSpecificRoom(string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromForm]RoomDTO roomDto)
        {
            
            var room = new Room()
            {
                ID = roomDto.ID,
                HotelID = roomDto.HotelID,
                Price = roomDto.Price,
                CreatedAt = roomDto.CreatedAt,
                UpdatedAt = roomDto.UpdatedAt,           
            };
           
            _Context.Rooms.Add(room);
            await _Context.SaveChangesAsync();
            return Ok(roomDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(RoomDTO roomDTO, string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

           //Updates properties of the room
            room.Price = roomDTO.Price;
            room.UpdatedAt = roomDTO.UpdatedAt;

            await _Context.SaveChangesAsync();

            return Ok(room);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

            _Context.Rooms.Remove(room);

            await _Context.SaveChangesAsync();

            return StatusCode(200,$"Room deleted succesfully {room}");
        }


    }
}
