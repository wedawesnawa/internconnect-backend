using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
            using var transaction = await _context.Database.BeginTransactionAsync(); // Mulai transaksi

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
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hash password
                    Role = "User" // Default role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // Simpan ke database


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
                await transaction.RollbackAsync(); // Batalkan transaksi jika terjadi kesalahan
                return StatusCode(500, new { message = "Terjadi kesalahan saat registrasi", error = ex.Message });
            }
        }


        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] Register model)
        //{
        //    // Cek apakah email atau username sudah digunakan
        //    var existingUser = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Username);

        //    if (existingUser != null)
        //    {
        //        return BadRequest(new { message = "Username atau Email sudah digunakan" });
        //    }

        //    // Buat user baru
        //    var user = new User
        //    {
        //        Username = model.Username,
        //        Email = model.Email,
        //        Password = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hash password
        //        Role = "User" // Default role
        //    };

        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "User berhasil didaftarkan" });
        //}
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
            var expiresIn = int.Parse(_configuration["Jwt:ExpiryMinutes"]!); // Waktu kedaluwarsa dalam menit

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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                access_token = tokenHandler.WriteToken(token),
                expires_in = expiresIn * 60, // dalam detik
                user = new
                {
                    username = user.Username,
                    email = user.Email,
                    role = user.Role
                }
            });
        }


        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            using var scope = _context.Database.BeginTransaction();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                user.Role = model.Role; // Update role user
                await _context.SaveChangesAsync();

                scope.Commit();
                return Ok(new { message = "Role assigned successfully" });
            }
            catch (Exception ex)
            {
                scope.Rollback();
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }



        //[HttpPost("add-role")]
        //public async Task<IActionResult> AddRole([FromBody] string role)
        //{
        //    if (!await _roleManager.RoleExistsAsync(role))
        //    {
        //        var result = await _roleManager.CreateAsync(new IdentityRole(role));
        //        if (result.Succeeded)
        //        {
        //            return Ok(new { message = "Role added successfully" });
        //        }

        //        return BadRequest(result.Errors);
        //    }

        //    return BadRequest("Role already exists");
        //}

        //[HttpPost("assign-role")]
        //public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.Username);
        //    if (user == null)
        //    {
        //        return BadRequest("User not found");
        //    }

        //    var result = await _userManager.AddToRoleAsync(user, model.Role);
        //    if (result.Succeeded)
        //    {
        //        return Ok(new { message = "Role assigned successfully" });
        //    }

        //    return BadRequest(result.Errors);
        //}
    }
}
