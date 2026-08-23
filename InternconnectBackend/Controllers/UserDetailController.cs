using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Models.Domain;
using InternconnectBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace InternconnectBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailController : ControllerBase
    {

        private readonly UserDetailService _userDetailService;

        public UserDetailController(UserDetailService userDetailService)
        {
            _userDetailService = userDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetail()
        {
            var userDetail = await _userDetailService.GetUserDetail();
            if (userDetail == null)
            {
                return NotFound(new { message = "User detail tidak ditemukan" });
            }
            return Ok(userDetail);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserDetail([FromBody] UserDetailDto userDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDetail = await _userDetailService.AddUserDetail(userDetailDto);
            return CreatedAtAction(nameof(GetUserDetail), new { id = userDetail.UserId }, userDetail);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserDetail([FromBody] UserDetailDto userDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUserDetail = await _userDetailService.UpdateUserDetail(userDetailDto);
            if (updatedUserDetail == null)
            {
                return NotFound(new { message = "User detail tidak ditemukan" });
            }
            return Ok(updatedUserDetail);
        }

        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "File tidak boleh kosong" });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/{fileName}";
            var success = await _userDetailService.UpdateProfilePicture(imageUrl);

            if (!success)
            {
                return NotFound(new { message = "User tidak ditemukan" });
            }

            return Ok(new { profileUrl = imageUrl });
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserDetail(string username)
        {
            var userDetail = await _userDetailService.GetUserDetailByUsername(username);
            if (userDetail == null)
            {
                return NotFound(new { message = "User detail tidak ditemukan" });
            }
            return Ok(userDetail);
        }

    }

}
