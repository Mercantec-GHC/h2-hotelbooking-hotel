﻿using BackendAPI.Data;
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
            var rooms = await _Context.Rooms.Include(r => r.Bookings).ToListAsync();
            var roomDTO = rooms.Select(r => new Room
            {
                HotelID = r.HotelID,
                Price = r.Price,
                ID = r.ID,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                Bookings = r.Bookings.Select(b => new Booking
                {
                    ID = b.ID,
                    RoomID = b.RoomID,
                    UserID = b.UserID,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                }).ToList().ToList()
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


        [HttpGet("{id}GetRoomById")]
        public async Task<ActionResult<Room>> GetSpecificRoom(string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

            return Ok(room);
        }

        [HttpPost("CreateARoom")]
        public async Task<IActionResult> CreateRoom([FromForm]RoomDTO roomDto)
        {

            var room = new Room()
            {
                ID = Guid.NewGuid().ToString("N"),
                HotelID = roomDto.HotelID,
                Price = roomDto.Price,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };
           
            _Context.Rooms.Add(room);
            await _Context.SaveChangesAsync();
            return Ok(roomDto);
        }

        [HttpPut("{id}UpdateRoom")]
        public async Task<IActionResult> UpdateRoom(Room roomDTO, string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

           //Updates properties of the room
            room.Price = roomDTO.Price;
            room.UpdatedAt = roomDTO.UpdatedAt;

            await _Context.SaveChangesAsync();

            return Ok(room);
        }

        [HttpDelete("DeleteRoom")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var room = await _Context.Rooms.FindAsync(id);

            _Context.Rooms.Remove(room);

            await _Context.SaveChangesAsync();

            return StatusCode(200,$"Room deleted succesfully {room}");
        }


    }
}
