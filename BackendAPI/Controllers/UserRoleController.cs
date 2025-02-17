using BackendAPI.Data;
using BackendAPI.Migrations;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController(DatabaseContext _context) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAvailableRoles")]
        public async Task<ActionResult> GetAvailableRoles()
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
                return StatusCode(500);
            }

            var roles = await _context.Roles
                .Where(r => r.Hierarki < currentUser.RoleHierarchy)
                .Select(r => new
                {
                    r.ID,
                    r.Name,
                    r.Hierarki
                })
                .ToListAsync();

            return Ok(roles);
        }

        [Authorize]
        [HttpPost("AddUserRoles")]
        public async Task<ActionResult> AddUserRoles([FromBody] UserRoleDTO userRoleDTO)
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
                .Where(u => u.ID == userRoleDTO.UserId)
                .Select(u => new
                {
                    u.ID,
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                    ExistingRoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList()
                })
                .FirstOrDefaultAsync();

            if (targetUser == null)
            {
                return BadRequest("Target user not found.");
            }

            foreach (var roleId in userRoleDTO.RoleId)
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.ID == roleId);

                if (role == null)
                {
                    return BadRequest($"Role with ID {roleId} not found.");
                }

                if (currentUser.RoleHierarchy <= role.Hierarki || currentUser.RoleHierarchy <= targetUser.RoleHierarchy)
                {
                    return BadRequest($"You cannot assign role with ID {roleId} to user with ID {userRoleDTO.UserId}.");
                }

                if (targetUser.ExistingRoleIds.Contains(roleId))
                {
                    return BadRequest($"User with ID {userRoleDTO.UserId} already has role with ID {roleId}.");
                }

                var userRole = new UserRole()
                {
                    UserId = userRoleDTO.UserId,
                    RoleId = roleId,
                };

                _context.UserRoles.Add(userRole);
            }

            await _context.SaveChangesAsync();

            return Ok("Roles successfully added!");
        }

        [Authorize]
        [HttpGet("GetUserRoles")]
        public async Task<ActionResult> GetUserRoles([FromQuery] string userId)
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
                return Forbid("You do not have permission to view this user's roles.");
            }

            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .Select(r => new
                {
                    r.Name,
                    r.ID
                })
                .ToListAsync();

            return Ok(userRoles);
        }

        [Authorize]
        [HttpDelete("RemoveUserRoles")]
        public async Task<ActionResult> RemoveUserRoles([FromBody] UserRoleDTO userRoleDTO)
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
                .Where(u => u.ID == userRoleDTO.UserId)
                .Select(u => new
                {
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                    ExistingRoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList()
                })
                .FirstOrDefaultAsync();

            if (targetUser == null)
            {
                return BadRequest("Target user not found.");
            }

            foreach (var roleId in userRoleDTO.RoleId)
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.ID == roleId);

                if (role == null)
                {
                    return BadRequest($"Role with ID {roleId} not found.");
                }

                if (currentUser.RoleHierarchy <= role.Hierarki || currentUser.RoleHierarchy <= targetUser.RoleHierarchy)
                {
                    return BadRequest($"You cannot remove role with ID {roleId} from user with ID {userRoleDTO.UserId}.");
                }

                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userRoleDTO.UserId && ur.RoleId == roleId);

                if (userRole == null)
                {
                    return BadRequest($"User with ID {userRoleDTO.UserId} does not have role with ID {roleId}.");
                }

                _context.UserRoles.Remove(userRole);
            }

            await _context.SaveChangesAsync();

            return Ok("Roles successfully removed!");
        }
    }
}
