﻿using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : Controller
    {
        private readonly DatabaseContext _Context;

        public HotelController(DatabaseContext context)
        {
            _Context = context;
        }

        [HttpPost]
        public async Task<ActionResult> CreateHotel([FromForm]CreateHotelDTO hotelDto)
        {
            var hotel = new Hotel()
                {
                ID = Guid.NewGuid().ToString("N"),
                Name = hotelDto.Name,
                Description = hotelDto.Description,
                Location = hotelDto.Location,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };
            _Context.Hotels.Add(hotel);
            await _Context.SaveChangesAsync();

            return Ok(hotelDto);
        }

        [HttpGet]
        public async Task<ActionResult<Hotel>> GetHotels()
        {

            var hoteler = await _Context.Hotels.Include(h => h.Rooms)
                .ThenInclude(r => r.Bookings).ToListAsync();
            var hotels = hoteler    
                .Select(h => new Hotel
            {
                ID = h.ID,
                Name = h.Name,
                Description = h.Description,
                Location = h.Location,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
                Rooms = h.Rooms
                .Select(r => new Room
                {
                    Price = r.Price,
                    ID = r.ID,
                    HotelID = r.HotelID,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    Bookings = r.Bookings.Select(b => new Booking
                    {
                        ID = b.ID,
                        RoomID = b.RoomID,
                        UserID = b.UserID,
                        CreatedAt = b.CreatedAt,
                        UpdatedAt = b.UpdatedAt
                    }).ToList()
                    
                }).ToList()
            }).ToList();


           
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotelById( string id)
        {
            var hotels = await _Context.Hotels.FindAsync(id);
            return Ok(hotels);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(CreateHotelDTO hotelDTO, string id)
        {
            var hotel = await _Context.Hotels.FindAsync(id);

            //Updates properties of the room
            hotel.Name = hotelDTO.Name;
            hotel.Description = hotelDTO.Description;
            hotel.Location = hotelDTO.Location;

            await _Context.SaveChangesAsync();

            return Ok(hotel);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteHotel(string id)
        {
            var hotel = await _Context.Rooms.FindAsync(id);

            _Context.Rooms.Remove(hotel);

            await _Context.SaveChangesAsync();

            return StatusCode(200, $"Hotel deleted succesfully {hotel}");
        }
    }
}
