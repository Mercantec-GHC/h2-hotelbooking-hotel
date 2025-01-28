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
            var rooms = await _Context.Rooms.ToListAsync();


            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetSpecificRoom(string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomDTO roomDto)
        {
            var room = new RoomDTO()
            {

            };
            _Context.Rooms.Add(room);
            await _Context.SaveChangesAsync();
            return Ok();
        }



    }
}
