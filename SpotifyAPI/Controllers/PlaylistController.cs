using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;
using SpotifyAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using SpotifyAPI.Utils;
using SpotifyAPI.Models;

namespace SpotifyAPI.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var playlist = await _playlistService.CreatePlaylistAsync(userIdClaim.Value, dto);
            if (playlist == null) return BadRequest();

            return Ok(playlist);
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("UserController is working!");
        }


        [HttpDelete("delete-playlist/{playlistId}")]
        public async Task<IActionResult> DeletePlaylist(int playlistId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            string userIdToken = userIdClaim.Value;
            var result = await _playlistService.DeletePlaylistAsync(userIdToken, playlistId);

            if (!result) return NotFound("Playlist không tồn tại hoặc không thuộc quyền xóa.");
            return Ok("Đã xóa playlist.");
        }


        [Authorize(Roles = "User")]
        [HttpPost("add-song")]
        public async Task<IActionResult> AddSongToPlaylist([FromBody] AddSongToPlaylistDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var success = await _playlistService.AddSongToPlaylistAsync(userIdClaim.Value, dto);
            if (!success) return BadRequest();

            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpPost("remove-song")]
        public async Task<IActionResult> RemoveSongFromPlaylist([FromBody] RemoveSongFromPlaylistDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var success = await _playlistService.RemoveSongFromPlaylistAsync(userIdClaim.Value, dto);
            if (!success) return BadRequest();

            return Ok();
        }
    }
}
