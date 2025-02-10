using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(DatabaseContext _context) : ControllerBase
    {
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        //{
        //    var User = await _context.Roles.Select(user => new UserDTO
        //    {
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        ID = user.ID,
        //        Email = user.Email
        //    })
        //    .ToListAsync();

        //    return Ok(User);
        //}
    }
}
