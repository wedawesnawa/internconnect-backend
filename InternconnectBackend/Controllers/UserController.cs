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
    }
}
