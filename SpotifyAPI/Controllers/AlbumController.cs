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
    }
}
