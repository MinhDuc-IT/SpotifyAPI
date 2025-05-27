using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;

namespace SpotifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongGenreController : ControllerBase
    {
        private readonly ISongGenreService _service;

        public SongGenreController(ISongGenreService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{songId}/{genreId}")]
        public async Task<IActionResult> GetById(int songId, int genreId)
        {
            var result = await _service.GetByIdAsync(songId, genreId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SongGenreDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpDelete("{songId}/{genreId}")]
        public async Task<IActionResult> Delete(int songId, int genreId)
        {
            await _service.DeleteAsync(songId, genreId);
            return Ok();
        }
    }
}
