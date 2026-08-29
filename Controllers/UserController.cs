using InternconnectBackend.Models;
using InternconnectBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InternconnectBackend.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateUserRole([FromForm] UpdateUserRoleDto dto)
        {
            try
            {
                bool success = await _userService.UpdateUserRoleAsync(User, dto);
                if (success)
                {
                    // Ambil URL file yang baru diupload
                    var username = User.FindFirstValue(ClaimTypes.Name);
                    var fileUrl = await _userService.GetUserFileUrlAsync(username);

                    return Ok(new
                    {
                        message = "User role and file updated successfully",
                        fileUrl = fileUrl // Kirim URL ke frontend
                    });
                }

                return BadRequest(new { message = "Update failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Endpoint untuk mendapatkan file URL
        [HttpGet("file-url")]
        public async Task<IActionResult> GetUserFileUrl()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(username))
                    return Unauthorized(new { message = "User not authenticated" });

                var fileUrl = await _userService.GetUserFileUrlAsync(username);

                if (string.IsNullOrEmpty(fileUrl))
                    return NotFound(new { message = "File not found" });

                return Ok(new { fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("by-role")]
        public async Task<IActionResult> GetUsersByRole([FromQuery] string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                {
                    return BadRequest(new { message = "Role is required" });
                }

                var users = await _userService.GetUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}