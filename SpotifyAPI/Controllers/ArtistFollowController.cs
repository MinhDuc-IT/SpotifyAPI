using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Services;
using System.Security.Claims;

namespace SpotifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistFollowController : ControllerBase
    {
        private readonly IArtistFollowService _artistFollowService;

        public ArtistFollowController(IArtistFollowService artistFollowService)
        {
            _artistFollowService = artistFollowService;
        }

        [HttpPost("follow")]
        [Authorize]
        public async Task<IActionResult> FollowArtist([FromBody] FollowArtistRequest request)
        {
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _artistFollowService.FollowArtistAsync(firebaseUid, request.ArtistID);
            if (result)
            {
                return Ok("Followed the artist successfully");
            }

            return BadRequest("Unable to follow artist.");
        }

        [HttpPost("unfollow")]
        [Authorize]
        public async Task<IActionResult> UnfollowArtist([FromBody] FollowArtistRequest request)
        {
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(firebaseUid)) return Unauthorized();

            var result = await _artistFollowService.UnfollowArtistAsync(firebaseUid, request.ArtistID);
            if (result)
            {
                return Ok("Unfollowed the artist successfully");
            }

            return BadRequest("Unable to unfollow artist.");
        }

        [HttpGet("is-following/{artistId}")]
        [Authorize]
        public async Task<IActionResult> IsFollowing(int artistId)
        {
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(firebaseUid)) return Unauthorized();

            var isFollowing = await _artistFollowService.IsFollowingArtistAsync(firebaseUid, artistId);
            return Ok(new { isFollowing });
        }
    }

    public class FollowArtistRequest
    {
        public int ArtistID { get; set; }
    }
}
