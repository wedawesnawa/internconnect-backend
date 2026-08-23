using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InternconnectBackend.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly InternconnectDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(InternconnectDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Cek apakah email atau username sudah digunakan
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Username);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username atau Email sudah digunakan" });
                }

                // Buat user baru
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = "User"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Buat detail user
                var userDetail = new UserDetail
                {
                    Nama = "",
                    Telp = "",
                    Bio = "",
                    Alamat = "",
                    Instansi = "",
                    AlamatInstansi = "",
                    profileUrl = "",
                    Username = model.Username
                };

                _context.UserDetails.Add(userDetail);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { message = "User berhasil didaftarkan" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Terjadi kesalahan saat registrasi", error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized(new { message = "Username atau password salah" });
            }

            // Generate token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var expiresIn = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "3600");

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddMinutes(expiresIn),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Set HttpOnly Cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set ke true untuk production (HTTPS)
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(expiresIn)
            };

            // Hapus cookie lama jika ada
            Response.Cookies.Delete("token");
            // Set cookie baru
            Response.Cookies.Append("token", tokenString, cookieOptions);

            return Ok(new
            {
                access_token = tokenString,
                expires_in = expiresIn * 60,
                user = new
                {
                    username = user.Username,
                    email = user.Email,
                    role = user.Role
                }
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // Ambil username dari claim
                var username = User.FindFirst(ClaimTypes.Name)?.Value
                               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                // Cari user di database
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Return user data
                return Ok(new
                {
                    user = new
                    {
                        username = user.Username,
                        email = user.Email,
                        role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Hapus cookie token
            Response.Cookies.Delete("token");

            return Ok(new { message = "Logout successful" });
        }

        [HttpPost("assign-role")]
        [Authorize]
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            using var scope = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                user.Role = model.Role;
                await _context.SaveChangesAsync();

                await scope.CommitAsync();
                return Ok(new { message = "Role assigned successfully" });
            }
            catch (Exception ex)
            {
                await scope.RollbackAsync();
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}