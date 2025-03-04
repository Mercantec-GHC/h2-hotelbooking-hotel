using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly DatabaseContext _Context;

        public BookingController(DatabaseContext context)
        {
            _Context = context;
        }

        [HttpPost("CreateBooking")]
        public async Task<ActionResult> CreateBooking([FromBody] CreateBookingDTO booking)
        {
            var isRoomBooked = await _Context.Bookings.AnyAsync(b =>
             b.RoomID == booking.RoomID &&
             ((booking.StartDate >= b.StartDate && booking.StartDate < b.EndDate) ||
              (booking.EndDate > b.StartDate && booking.EndDate <= b.EndDate) ||
              (booking.StartDate <= b.StartDate && booking.EndDate >= b.EndDate))
            );

            if (isRoomBooked)
            {
                return BadRequest("The room is already booked for the selected dates.");
            }

            var room = await _Context.Rooms
                .FirstOrDefaultAsync(r => r.ID == booking.RoomID);
            if (room == null)
            {
                throw new Exception($"Room with ID {booking.RoomID} not found.");
            }
            var discountCode = await _Context.DiscountCodes
                .FirstOrDefaultAsync(dc => dc.Code == booking.DiscountCode);

            float finalPrice;
            var DaysBetween = (booking.EndDate - booking.StartDate).Days;
            var NewPrice = DaysBetween * room.DailyPrice;
            if (discountCode != null)
            {
                var DiscountedPrice = NewPrice / 100 * discountCode.Percentage;
                finalPrice = NewPrice - DiscountedPrice;
            } else
            {
                finalPrice = NewPrice;
            }

            var bookings = new Booking()
            {
                ID = Guid.NewGuid().ToString(),
                UserID = booking.UserID,
                RoomID = booking.RoomID,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                AllInclusive = booking.AllInclusive,
                Price = finalPrice,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };
            _Context.Bookings.Add(bookings);
            await _Context.SaveChangesAsync();

            //return Ok(booking);
            return Ok("Booking created.");
        }

        [HttpGet("GetAllBookings")]
        public async Task<IActionResult> GetBookings()
        {
            var userId = User.FindFirstValue("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _Context.Users
                .Where(u => u.ID == userId)
                .Select(u => new
                {
                    IsWorker = u.UserRoles.Where(r => r.Role.Name == "HotelWorker").FirstOrDefault() != null
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            var bookings = await _Context.Bookings
                .Where(b => b.UserID == userId || user.IsWorker)
                .Select(b => new BookingResult
                {
                    Id = b.ID,
                    RoomID = b.RoomID,
                    UserID = b.UserID,
                    Price = b.Price,
                    AllInclusive = b.AllInclusive,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate
                }).ToListAsync();

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(string id)
        {
            var userId = User.FindFirstValue("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _Context.Users
                .Where(u => u.ID == userId)
                .Select(u => new
                {
                    IsWorker = u.UserRoles.Where(r => r.Role.Name == "HotelWorker").FirstOrDefault() != null
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            var booking = await _Context.Bookings
                .Where(b => b.ID == id)
                .Where(b => b.UserID == userId || user.IsWorker)
                .Select(b => new BookingResult
                {
                    Id = b.ID,
                    RoomID = b.RoomID,
                    UserID = b.UserID,
                    Price = b.Price,
                    AllInclusive = b.AllInclusive,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate
                }).FirstOrDefaultAsync();

            if (booking == null)
            {
                return NotFound("Current booking not found.");
            }

            return Ok(booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Booking BookingDTO, string id)
        {
            var booking = await _Context.Bookings.FindAsync(id);

            //Updates properties of the room
            booking.StartDate = BookingDTO.StartDate;
            booking.EndDate = BookingDTO.EndDate;
            booking.Price = BookingDTO.Price;
            booking.UpdatedAt = BookingDTO.UpdatedAt;

            await _Context.SaveChangesAsync();

            return Ok(booking);
        }

        [Authorize(Roles = "HotelWorker")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            var booking = await _Context.Rooms.FindAsync(id);

            _Context.Rooms.Remove(booking);

            await _Context.SaveChangesAsync();

            return StatusCode(200, $"Booking deleted succesfully {booking}");
        }

    }
}
