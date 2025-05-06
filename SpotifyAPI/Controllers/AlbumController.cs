using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;

namespace SpotifyAPI.Controllers
{
    [Route("api/album")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _albumService;

        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet("{albumId}/songs")]
        public async Task<IActionResult> GetSongsByAlbum(int albumId)
        {
            var songs = await _albumService.GetSongsByAlbumIdAsync(albumId);
            return Ok(songs);
        }

        [HttpGet("liked-in-albums")]
        public async Task<IActionResult> GetAlbumsFromLikedSongs()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var albums = await _albumService.GetAlbumsFromLikedSongsAsync(userIdClaim.Value);
            if (albums == null || !albums.Any()) return NotFound("No albums found.");

            return Ok(albums);
        }
    }
}
