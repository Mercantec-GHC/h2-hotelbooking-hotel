using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly DatabaseContext _Context;

        public UserRoleController(DatabaseContext context)
        {
            _Context = context;
        }

        [HttpPost("AddUserRole")]
        public async Task<ActionResult> CreateHotel([FromBody] UserRoleDTO userRoleDTO)
        {

            var userRole = new UserRole()
            {
                UserId = userRoleDTO.UserId,
                RoleId = userRoleDTO.RoleId,
            };

            var user = await _Context.Users
                .FirstOrDefaultAsync(u => u.ID == userRoleDTO.UserId);

            var role = await _Context.Roles
                .FirstOrDefaultAsync(r => r.ID == userRoleDTO.RoleId);


            _Context.UserRoles.Add(userRole);
            await _Context.SaveChangesAsync();

            return Ok("Role successfully added!");
        }

        [HttpGet]
        public async Task<ActionResult<DiscountCode>> GetUserRoles()
        {
            var userRoles = await _Context.UserRoles.ToListAsync();
            return Ok(userRoles);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountCode(string id)
        {
            var userRole = await _Context.UserRoles.FindAsync(id);

            _Context.UserRoles.Remove(userRole);

            await _Context.SaveChangesAsync();

            return StatusCode(200, $"User role deleted succesfully!");
        }
    }
}
