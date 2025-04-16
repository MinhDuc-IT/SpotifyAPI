using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;

namespace SpotifyAPI.Controllers
{
    [Route("api/song")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSong(int page = 1, int limit = 10)
        {
            var result = await _songService.GetAllSongsAsync(page, limit);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSong([FromForm] CreateSongDTO request)
        {
            if (request.Image == null || request.Audio == null)
                return BadRequest("Image or Audio file is required.");

            try
            {
                var song = await _songService.CreateSongAsync(request);
                return Ok(song);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromForm] UpdateSongDTO request)
        {
            try
            {
                var song = await _songService.UpdateSongAsync(id, request);
                return Ok(song);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Song not found.")
                    return NotFound(ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var success = await _songService.DeleteSongAsync(id);
            if (!success)
                return NotFound("Song not found.");
            return Ok("Song deleted successfully.");
        }
    }
}
