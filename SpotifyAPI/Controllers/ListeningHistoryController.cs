using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;
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

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddListeningHistory([FromBody] ListenHistoryDto history)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // hoặc "sub" nếu bạn dùng JWT chuẩn
            if (userIdClaim == null)
                return Unauthorized(new { message = "Unauthorized: user claim not found" });

            string userIdToken = userIdClaim.Value;

            try
            {
                var success = await _listeningHistoryService.AddAsync(userIdToken, history.SongId, history.DeviceInfo);

                if (success)
                    return Ok(new { success = true, message = "Listening history added successfully" });
                else
                    return BadRequest(new { success = false, message = "Failed to add listening history" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", details = ex.Message });
            }
        }

    }

    public class ListenHistoryDto
    {
        public int SongId { get; set; }
        public string? DeviceInfo { get; set; }
    }
}
