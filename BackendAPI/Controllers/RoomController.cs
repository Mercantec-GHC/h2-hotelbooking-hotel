using BackendAPI.Data;
using BackendAPI.Utilities;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController(DatabaseContext _context, IConfiguration _configuration) : ControllerBase
    {
        private static readonly string HotelAdminRole = "HotelAdmin";

        

        [HttpGet("GetRoomWithBooking")]
        public async Task<ActionResult<Room>> GetRooms([FromBody] CreateRoomDTO roomDTO)
        {
            var rooms = await _context.Rooms
        .Include(r => r.Bookings)
        
        .ToListAsync();

            var room = rooms.Select(r => new
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
            });

            return Ok(room);
        }

   
        [HttpGet("GetAllAvailableRooms")]
        public async Task<ActionResult<List<Room>>> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            var availableRooms = await _context.Rooms
                .Where(room => !_context.Bookings.Any(b =>
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
            var room = await _context.Rooms.FindAsync(id);

            return Ok(room);
        }

        [Authorize(Roles = "HotelAdmin")]
        [HttpPost("CreateRoom")]
        public async Task<IActionResult> CreateRoom([FromForm] CreateRoomDTO roomDto)
        {
            var room = new Room()
            {
                ID = Guid.NewGuid().ToString(),
                HotelID = roomDto.HotelID,
                DailyPrice = roomDto.DailyPrice,
                CreatedAt = DateTime.UtcNow.AddHours(1),    
                UpdatedAt = DateTime.UtcNow.AddHours(1),
            };

           

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        [Authorize(Roles = "HotelAdmin")]
        [HttpPost("UploadRoomImage")]
        public async Task<IActionResult> UploadRoomImage([FromQuery] string roomId, IFormFile image)
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
                    u.Hotels,
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                })
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                return BadRequest("Current user not found.");
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.ID == roomId);
            if (room == null)
            {
                return BadRequest("Room was not found.");
            }
            Console.WriteLine(room.HotelID);

            var userHotel = await _context.UserHotels.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.HotelId == room.HotelID);
            if (userHotel == null)
            {
                return BadRequest("You do not work at this hotel.");
            }
                

            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file uploaded.");
            }

            const long MaxFileSize = 4 * 1024 * 1024; // 4MB limit
            if (image.Length > MaxFileSize)
            {
                return BadRequest("Image file size exceeds 4MB.");
            }
            string fileExtension = Path.GetExtension(image.FileName).ToLower();
            
             string mimeType = image.ContentType.ToLower();

            if (!MimeTypes.IsValidImageExtension(fileExtension))
            {
                return BadRequest("Invalid file extension. Only image formats are allowed.");
            }
           
            // Folder path to save the image
            string uploadsFolder = _configuration["path:images"] ?? Environment.GetEnvironmentVariable("IMAGES_PATH");
            Console.WriteLine(uploadsFolder);
            // Ensure the folder exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);

            }

            // Generate a unique filename for the uploaded image
            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            Console.WriteLine(uniqueFileName);
            
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            Console.WriteLine(filePath);

            try
            {
                using (var stream = image.OpenReadStream())
                using (var img = await SixLabors.ImageSharp.Image.LoadAsync(stream))  // Ensures it's a valid image
                {
                    // Resize if width and height are provided
                    //if (width.HasValue && height.HasValue)
                    //{
                    //    img.Mutate(x => x.Resize(new Size(width.Value, height.Value)));

                    //}
                    using (var memoryStream = new MemoryStream())
                    {
                        var jpegEncoder = new JpegEncoder { Quality = 75 };
                        // Save the image to memory (this will give you the size after resizing)
                        await img.SaveAsync(memoryStream, jpegEncoder);

                        // Check if the image exceeds the 4MB limit after resizing
                        if (memoryStream.Length > MaxFileSize)
                        {
                            return BadRequest("Image file size exceeds 4MB after resizing.");
                        }

                        // If the image is within the size limit, save it to the file system
                        memoryStream.Seek(0, SeekOrigin.Begin); // Rewind the stream before saving
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await memoryStream.CopyToAsync(fileStream);
                        }
                    }


                }
            }
            catch
            {
                return BadRequest("Invalid image file.");
            }

           
            // Return the file path (you can return the URL if needed)
            var fileUrl = $"/RoomImages/{uniqueFileName}";

            var roomImage = new RoomImage
            {
                FileName = uniqueFileName,
                RoomID = roomId,
            };

            _context.RoomImages.Add(roomImage);
            await _context.SaveChangesAsync();

            return Ok(new { ImageUrl = fileUrl });
        }


        [HttpGet("GetRoomImages")]
        public async Task<ActionResult> GetRoomImages([FromQuery] string roomId)
        {
            var images = await _context.RoomImages.Where(r => r.RoomID == roomId)
                .Select(r => new
                {
                    r.RoomID,
                    r.FileName
                })
                .ToListAsync();

            if(images.Count() == 0)
            {
                return NotFound();
            }

            return Ok(images);
        }

        [HttpGet("GetImage/{image}")]
        public async Task<ActionResult> GetImage(string image)
        {
            string ImagesFolder = _configuration["path:images"] ?? Environment.GetEnvironmentVariable("IMAGES_PATH");
            string filePath = Path.Combine(ImagesFolder, image);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var mimeType = MimeTypes.GetMimeType(image);

            return PhysicalFile(filePath, mimeType);
        }

        [Authorize(Roles = "HotelAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(Room roomDTO, string id)
        {
            var room = await _context.Rooms.FindAsync(id);

           //Updates properties of the room
            room.DailyPrice = roomDTO.DailyPrice;
            room.UpdatedAt = roomDTO.UpdatedAt;

            await _context.SaveChangesAsync();

            return Ok(room);
        }

        [Authorize(Roles = "HotelAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            var room = await _context.Rooms.FindAsync(id);

            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();

            return StatusCode(200,$"Room deleted succesfully {room}");
        }
      

    }
}
