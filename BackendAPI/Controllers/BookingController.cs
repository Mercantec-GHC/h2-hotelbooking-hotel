﻿using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly DatabaseContext _Context;

        public BookingController(DatabaseContext context)
        {
            _Context = context;
        }

        [HttpPost("CreateABooking")]
        public async Task<ActionResult> CreateBooking([FromForm] Booking booking)
        {
            var bookings = new Booking()
            {
                ID = Guid.NewGuid().ToString("N"),
                UserID = booking.UserID,
                RoomID = booking.RoomID,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };
            _Context.Bookings.Add(bookings);
            await _Context.SaveChangesAsync();

            return Ok(booking);
        }

        [HttpGet("GetAllBookings")]
        public async Task<ActionResult<Booking>> GetBookings()
        {
            var bookings = await _Context.Bookings.ToListAsync();


            return Ok(bookings);
        }

        [HttpGet("{id}GetBookingById")]
        public async Task<ActionResult<Room>> GetBookingById(string id)
        {
            var booking = await _Context.Bookings.FindAsync(id);

            return Ok(booking);
        }

        [HttpPut("{id}UpdateABooking")]
        public async Task<IActionResult> UpdateBooking(Booking BookingDTO, string id)
        {
            var booking = await _Context.Bookings.FindAsync(id);

            //Updates properties of the room
            booking.StartDate = BookingDTO.StartDate;
            booking.EndDate = BookingDTO.EndDate;
            booking.UpdatedAt = BookingDTO.UpdatedAt;

            await _Context.SaveChangesAsync();

            return Ok(booking);
        }

        [HttpDelete("DeleteABooking")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            var booking = await _Context.Rooms.FindAsync(id);

            _Context.Rooms.Remove(booking);

            await _Context.SaveChangesAsync();

            return StatusCode(200, $"Booking deleted succesfully {booking}");
        }

    }
}
