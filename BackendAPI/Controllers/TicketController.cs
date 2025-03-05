using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Security.Claims;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController(DatabaseContext _context, IConfiguration _configuration) : ControllerBase
    {
        private static readonly string _hotelWorkerRole = "HotelWorker";
        private static readonly string _userID = "UserID";

        [Authorize]
        [HttpGet("listTickets")]
        public async Task<IActionResult> GetTickets()
        {
            var userId = User.FindFirstValue(_userID);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Where(u => u.ID == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            var hasHotelWorkerRole = User.IsInRole(_hotelWorkerRole);

            var tickets = await _context.Tickets
                .Where(t => hasHotelWorkerRole || t.UserID == userId)
                .Select(t => new
                {
                    t.ID,
                    t.Topic,
                    t.Status,
                    t.CreatedAt
                })
                .ToListAsync();

            return Ok(tickets);
        }

        [Authorize]
        [HttpGet("MyTickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userId = User.FindFirstValue(_userID);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Where(u => u.ID == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            var tickets = await _context.Tickets
                .Where(t => t.UserID == userId)
                .Select(t => new
                {
                    t.ID,
                    t.Topic,
                    t.Status,
                    t.CreatedAt
                })
                .ToListAsync();

            return Ok(tickets);
        }

        [Authorize]
        [HttpGet("getTicket")]
        public async Task<IActionResult> GetTicket([FromQuery] string ticketId)
        {
            var userId = User.FindFirstValue(_userID);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Where(u => u.ID == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            var hasHotelWorkerRole = User.IsInRole(_hotelWorkerRole);

            var ticket = await _context.Tickets
                .Where(t => hasHotelWorkerRole || t.UserID == userId)
                .Where(t => t.ID == ticketId)
                .Select(t => new
                {
                    t.ID,
                    t.Topic,
                    t.Status,
                    t.CreatedAt,
                    Messages = t.Messages
                        .OrderBy(m => m.CreatedAt)
                        .Select(m => new
                        {
                            m.ID,
                            m.UserID,
                            UserName = $"{m.User.FirstName} {m.User.LastName}",
                            m.Message,
                            m.CreatedAt
                        })
                })
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return Forbid();
            }

            return Ok(ticket);
        }

        [Authorize]
        [HttpPost("createTicket")]
        public async Task<IActionResult> CreateTicket([FromBody] TicketCreateDTO ticketCreateDTO)
        {
            var userId = User.FindFirstValue(_userID);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Where(u => u.ID == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            var ticketID = Guid.NewGuid().ToString();
            var Ticket = new Ticket
            {
                ID = ticketID,
                UserID = user.ID,
                Topic = ticketCreateDTO.Topic,
                Status = "Open",
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1)
            };
            _context.Tickets.Add(Ticket);

            var message = new TicketMessage
            {
                ID = Guid.NewGuid().ToString(),
                Message = ticketCreateDTO.Message,
                TicketId = ticketID,
                UserID = userId,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1)
            };
            _context.TicketMessages.Add(message);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPatch("updateStatus")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string ticketId, [FromBody] TicketStatusDTO ticketStatusDTO)
        {
            var userId = User.FindFirstValue(_userID);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Where(u => u.ID == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            if (!User.IsInRole(_hotelWorkerRole))
            {
                return Forbid();
            }

            var ticket = await _context.Tickets
                .Where(t => t.ID == ticketId)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = ticketStatusDTO.Status;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("createMessage")]
        public async Task<IActionResult> CreateMessage([FromBody] MessageCreateDTO messageCreateDTO)
        {
            var userId = User.FindFirstValue(_userID);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Where(u => u.ID == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            var ticket = await _context.Tickets
                .Where(t => t.ID == messageCreateDTO.TicketId)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return NotFound(messageCreateDTO.TicketId);
            }

            if (ticket.UserID != userId && !User.IsInRole(_hotelWorkerRole))
            {
                return Forbid();
            }

            var message = new TicketMessage
            {
                ID = Guid.NewGuid().ToString(),
                Message = messageCreateDTO.Message,
                TicketId = messageCreateDTO.TicketId,
                UserID = userId,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1)
            };
            _context.TicketMessages.Add(message);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
