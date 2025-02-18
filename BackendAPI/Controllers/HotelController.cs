using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers;
using System.Security.Claims;

namespace BackendAPI.Controllers
{
    [Authorize(Roles = "GlobalAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController(DatabaseContext _context) : Controller
    {
       

        [HttpPost("CreateHotel")]
        public async Task<ActionResult> CreateHotel([FromBody]CreateHotelDTO hotelDto)
        {
            var hotel = new Hotel()
                {
                ID = Guid.NewGuid().ToString(),
                Name = hotelDto.Name,
                Description = hotelDto.Description,
                Country = hotelDto.Country,
                City = hotelDto.City,
                Region = hotelDto.Region,
                PostalCode = hotelDto.PostalCode,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
                
            };
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return Ok(hotelDto);
        }

        [HttpGet("GetAllHotels")]
        public async Task<ActionResult<List<HotelDTO>>> GetHotels()
        {
            var hoteler = await _context.Hotels
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.Bookings)
               
                .Include(h => h.Rooms)
                    
                .ToListAsync();

            var hotels = hoteler.Select(h => new HotelDTO
            {
                ID = h.ID,
                Name = h.Name,
                Description = h.Description,
                Country = h.Country,
                City = h.City,
                Region = h.Region,
                PostalCode = h.PostalCode,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
               
                Rooms = h.Rooms.Select(r => new RoomDTO
                {
                    ID = r.ID,
                    HotelID = r.HotelID,
                    DailyPrice = r.DailyPrice,
                  

                }).ToList()
            }).ToList();

            return Ok(hotels);
        }

        [HttpGet("Search")]
        public async Task<ActionResult<Hotel>> HotelSearch( string searchValue)
        {

            var hotels = await _context.Hotels
            .Where(h => h.Name == searchValue || h.Country == searchValue || h.City == searchValue || h.Region == searchValue || h.PostalCode == searchValue) 
            .Include(h => h.Rooms)
            .ThenInclude(r => r.Bookings)
            .ToListAsync();

            if (hotels == null || hotels.Count == 0)
                return NotFound("No hotels found matching your search.");

            var result = hotels
                .Select(h => new Hotel
                {
                    ID = h.ID,
                    Name = h.Name,
                    Description = h.Description,
                    Country = h.Country,
                    City = h.City,
                    Region = h.Region,
                    PostalCode = h.PostalCode,
                    CreatedAt = h.CreatedAt,
                    UpdatedAt = h.UpdatedAt,
                    Rooms = h.Rooms
                .Select(r => new Room
                {
                    DailyPrice = r.DailyPrice,
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
        public async Task<ActionResult<Hotel>> GetHotelsById(string id)
        {
           
            var hoteler = await _context.Hotels
                .Where(h => h.ID == id)
                .Include(h => h.Rooms)
                .ThenInclude(r => r.Bookings).FirstOrDefaultAsync();

            var hotel = new Hotel
            {
                ID = hoteler.ID,
                Name = hoteler.Name,
                Description = hoteler.Description,
                Country = hoteler.Country,
                City = hoteler.City,
                Region = hoteler.Region,
                PostalCode = hoteler.PostalCode,
                CreatedAt = hoteler.CreatedAt,
                UpdatedAt = hoteler.UpdatedAt,
                Rooms = hoteler.Rooms
                .Select(r => new Room
                {
                    DailyPrice = r.DailyPrice,
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
            };
                



            return Ok(hotel);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(CreateHotelDTO hotelDTO, string id)
        {
            var hotel = await _context.Hotels.FindAsync(id);

            //Updates properties of the room
            hotel.Name = hotelDTO.Name;
            hotel.Description = hotelDTO.Description;
            hotel.Country = hotelDTO.Country;
            hotel.City = hotelDTO.City;
            hotel.Region = hotelDTO.Region;
            hotel.PostalCode = hotelDTO.PostalCode;

            await _context.SaveChangesAsync();

            return Ok(hotel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(string id)
        {
            var hotel = await _context.Hotels.FindAsync(id);

            _context.Hotels.Remove(hotel);

            await _context.SaveChangesAsync();

            return StatusCode(200, $"Hotel deleted succesfully {hotel}");
        }

        [Authorize]
        [HttpPost("AddUserHotels")]
        public async Task<ActionResult> AddUserHotels([FromBody] UserHotelsDTO userHotelsDTO)
        {
            var userId = User.FindFirstValue("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var currentUser = await _context.Users
                .Where(u => u.ID == userId)
                .Select(u => new
                {
                    u.ID,
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                })
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                return BadRequest("Current user not found.");
            }

            var targetUser = await _context.Users
                .Where(u => u.ID == userHotelsDTO.UserId)
                .Select(u => new
                {
                    u.ID,
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                    ExistingHotelIds = u.UserHotels.Select(ur => ur.HotelId).ToList()
                })
                .FirstOrDefaultAsync();

            if (targetUser == null)
            {
                return BadRequest("Target user not found.");
            }

            foreach (var hotelId in userHotelsDTO.HotelIds)
            {
                var Hotels = await _context.Hotels
                    .FirstOrDefaultAsync(r => r.ID == hotelId);

                if (Hotels == null)
                {
                    return BadRequest($"Role with ID {hotelId} not found.");
                }
                               
                if (targetUser.ExistingHotelIds.Contains(hotelId))
                {
                    return BadRequest($"User with ID {userHotelsDTO.UserId} already has hotel with ID {hotelId}.");
                }

                var userHotel = new UserHotel()
                {
                    UserId = userHotelsDTO.UserId,
                    HotelId = hotelId,
                };

                _context.UserHotels.Add(userHotel);
            }

            await _context.SaveChangesAsync();

            return Ok("Hotels successfully added!");
        }

        [Authorize]
        [HttpGet("GetUserHotels")]
        public async Task<ActionResult> GetUserHotels([FromQuery] string userId)
        {
            var currentUserId = User.FindFirstValue("UserID");

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var currentUser = await _context.Users
                .Where(u => u.ID == currentUserId)
                .Select(u => new
                {
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                })
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                return BadRequest("Current user not found.");
            }

            var targetUser = await _context.Users
                .Where(u => u.ID == userId)
                .Select(u => new
                {
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                })
                .FirstOrDefaultAsync();

            if (targetUser == null)
            {
                return BadRequest("Target user not found.");
            }

            if (currentUser.RoleHierarchy <= targetUser.RoleHierarchy)
            {
                return Forbid("You do not have permission to view this user's hotels.");
            }

            var userHotels = await _context.UserHotels
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Hotel)
                .Select(r => new
                {
                    r.Name,
                    r.ID
                })
                .ToListAsync();

            return Ok(userHotels);
        }

        [Authorize]
        [HttpDelete("RemoveUserHotels")]
        public async Task<ActionResult> RemoveUserHotels([FromBody] UserHotelsDTO userHotelsDTO)
        {
            var userId = User.FindFirstValue("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var currentUser = await _context.Users
                .Where(u => u.ID == userId)
                .Select(u => new
                {
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                })
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                return BadRequest("Current user not found.");
            }

            var targetUser = await _context.Users
                .Where(u => u.ID == userHotelsDTO.UserId)
                .Select(u => new
                {
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                    ExistingHotelIds = u.UserHotels.Select(ur => ur.HotelId).ToList()
                })
                .FirstOrDefaultAsync();

            if (targetUser == null)
            {
                return BadRequest("Target user not found.");
            }

            foreach (var hotelId in userHotelsDTO.HotelIds)
            {
                var hotel = await _context.Hotels
                    .FirstOrDefaultAsync(r => r.ID == hotelId);

                if (hotel == null)
                {
                    return BadRequest($"Hotel with ID {hotelId} not found.");
                }
               
                var userHotel = await _context.UserHotels
                    .FirstOrDefaultAsync(ur => ur.UserId == userHotelsDTO.UserId && ur.HotelId == hotelId);

                if (userHotel == null)
                {
                    return BadRequest($"User with ID {userHotelsDTO.UserId} does not have hotel with ID {hotelId}.");
                }

                _context.UserHotels.Remove(userHotel);
            }

            await _context.SaveChangesAsync();

            return Ok("Hotels successfully removed!");
        }
    }
}
