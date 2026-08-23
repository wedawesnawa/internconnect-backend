using InternconnectBackend.Models.Domain;
using InternconnectBackend.Models;
using System.Security.Claims;
using System;
using InternconnectBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace InternconnectBackend.Services
{
    public class UserDetailService
    {
        private readonly InternconnectDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserDetailService(InternconnectDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUsernameFromToken()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? throw new UnauthorizedAccessException("User tidak terautentikasi");
        }

        public async Task<UserDetail?> GetUserDetail()
        {
            var username = GetUsernameFromToken();
            return await _context.UserDetails.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserDetail> AddUserDetail(UserDetailDto userDetailDto)
        {
            var username = GetUsernameFromToken();
            var userDetail = new UserDetail
            {
                Username = username,
                Nama = userDetailDto.Nama,
                Telp = userDetailDto.Telp,
                Bio = userDetailDto.Bio,
                Alamat = userDetailDto.Alamat,
                Instansi = userDetailDto.Instansi,
                AlamatInstansi = userDetailDto.AlamatInstansi
            };

            _context.UserDetails.Add(userDetail);
            await _context.SaveChangesAsync();
            return userDetail;
        }

        public async Task<UserDetail?> UpdateUserDetail(UserDetailDto userDetailDto)
        {
            var username = GetUsernameFromToken();
            var userDetail = await _context.UserDetails.FirstOrDefaultAsync(u => u.Username == username);

            if (userDetail == null)
            {
                return null;
            }

            userDetail.Nama = userDetailDto.Nama;
            userDetail.Telp = userDetailDto.Telp;
            userDetail.Bio = userDetailDto.Bio;
            userDetail.Alamat = userDetailDto.Alamat;
            userDetail.Instansi = userDetailDto.Instansi;
            userDetail.AlamatInstansi = userDetailDto.AlamatInstansi;

            await _context.SaveChangesAsync();
            return userDetail;
        }
        public async Task<bool> UpdateProfilePicture(string imageUrl)
        {
            var username = GetUsernameFromToken();
            var userDetail = await _context.UserDetails.FirstOrDefaultAsync(u => u.Username == username);

            if (userDetail == null)
            {
                return false;
            }

            userDetail.profileUrl = imageUrl;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<UserDetail> GetUserDetailByUsername(string username)
        {
            return await _context.UserDetails
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
