using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SpotifyAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetAdminDashboard()
        {
            var adminData = new
            {
                totalUsers = 100,
                activeUsers = 85,
                newRegistrations = 15
            };

            return Ok(adminData);
        }
    }
}
