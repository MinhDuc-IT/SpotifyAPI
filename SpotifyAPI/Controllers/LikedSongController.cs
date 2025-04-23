using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;

namespace SpotifyAPI.Controllers
{
    [Route("api/liked")]
    [ApiController]
    public class LikedSongController : ControllerBase
    {
        private readonly ILikedSongService _likedSongService;
        public LikedSongController(ILikedSongService likedSongService)
        {
            _likedSongService = likedSongService;
        }

        [Authorize(Roles = "User")]
        [HttpGet("liked-song")]
        public async Task<IActionResult> GetLikedSongs()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            string userIdToken = userIdClaim.Value;

            var likedSongs = await _likedSongService.GetLikedSongsAsync(userIdToken);
            return Ok(likedSongs);
        }

        [Authorize(Roles = "User")]
        [HttpPost("like/{songId}")]
        public async Task<IActionResult> LikeSong(int songId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var success = await _likedSongService.LikeSongAsync(songId, userId);
            return success ? Ok("Liked") : BadRequest("Already liked or user not found");
        }

        [Authorize(Roles = "User")]
        [HttpDelete("dislike/{songId}")]
        public async Task<IActionResult> DislikeSong(int songId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var success = await _likedSongService.DislikeSongAsync(songId, userId);
            return success ? Ok("Disliked") : NotFound("Not found or already removed");
        }
    }
}
