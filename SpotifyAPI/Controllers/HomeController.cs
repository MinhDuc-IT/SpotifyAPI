using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SpotifyAPI.Controllers
{
    [Route("api/home")]
    [ApiController]
    //[Authorize(Roles = "User")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetHomeData()
        {
            //var email = User.FindFirstValue(ClaimTypes.Email);
            //var roles = User.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList();
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"ClaimType: {claim.Type}, Value: {claim.Value}");
            }

            var response = new
            {
                message = "Welcome to the Home API!",
                //email = email,
                //roles = roles
            };

            return Ok(response);
        }
    }
}
