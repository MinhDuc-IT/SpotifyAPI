using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;

namespace SpotifyAPI.Controllers
{
    [Route("api/history")]
    [ApiController]
    public class ListeningHistoryController : ControllerBase
    {
        private readonly IListeningHistoryService _listeningHistoryService;
        public ListeningHistoryController(IListeningHistoryService listeningHistoryService)
        {
            _listeningHistoryService = listeningHistoryService;
        }
        [Authorize(Roles = "User")]
        [HttpGet("listening-history")]
        public async Task<IActionResult> GetListeningHistory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // hoặc "sub" nếu bạn dùng JWT chuẩn
            if (userIdClaim == null)
                return Unauthorized();

            string userIdToken = userIdClaim.Value;

            var history = await _listeningHistoryService.GetListeningHistoryAsync(userIdToken);
            return Ok(history);
        }

        [Authorize(Roles = "User")]
        [HttpGet("top-artists")]
        public async Task<IActionResult> GetTopArtists()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // hoặc "sub" nếu bạn dùng JWT chuẩn
            if (userIdClaim == null)
                return Unauthorized();

            string userIdToken = userIdClaim.Value;

            var topArtists = await _listeningHistoryService.GetTopArtistsAsync(userIdToken);
            return Ok(topArtists);
        }
    }
}
