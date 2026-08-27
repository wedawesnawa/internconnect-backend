using InternconnectBackend.Data;
using InternconnectBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InternconnectBackend.Services
{
    public class UserService
    {
        private readonly InternconnectDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UserService(InternconnectDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<bool> UpdateUserRoleAsync(ClaimsPrincipal userClaims, UpdateUserRoleDto dto)
        {
            string? username = userClaims.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                throw new Exception("User not authenticated");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                throw new Exception("User not found");

            user.Role = dto.NewRole;
            await _context.SaveChangesAsync();

            var userDetail = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.Username == username);
            if (userDetail == null)
                throw new Exception("User detail not found");

            userDetail.Instansi = dto.Instansi; // Menyimpan instansi ke dalam database

            if (dto.File != null)
            {
                if (dto.File.Length > 10 * 1024 * 1024)
                    throw new Exception("File size exceeds 10MB");

                if (!dto.File.ContentType.Equals("application/pdf"))
                    throw new Exception("Invalid file format. Only PDF allowed");

                string uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(fileStream);
                }

                userDetail.FileUrl = $"/uploads/{uniqueFileName}";
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<UserRole>> GetUsersByRoleAsync(string role)
        {
            var users = await _context.Users
                .Where(u => u.Role == role)
                .Select(u => new UserRole
                {
                    Username = u.Username,
                    Role = u.Role
                })
                .ToListAsync();

            return users;
        }
    }
}