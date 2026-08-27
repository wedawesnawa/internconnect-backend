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
                    return Ok(new { message = "User role and file updated successfully" });

                return BadRequest(new { message = "Update failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("by-role")]
        public async Task<IActionResult> GetUsersByRole(
            [FromQuery] string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                {
                    return BadRequest(new
                    {
                        message = "Role is required"
                    });
                }

                var users = await _userService.GetUsersByRoleAsync(role);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
        }

    }
}
