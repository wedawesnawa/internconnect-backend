using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternconnectBackend.Controllers
{
    [Authorize(Roles = "Pembimbing")]
    [Route("api/[controller]")]
    [ApiController]
    public class PembimbingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have accessed the Pembimbing controller.");
        }
    }
}
