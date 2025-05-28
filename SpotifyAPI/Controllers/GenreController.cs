//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SpotifyAPI.Services;

//namespace SpotifyAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class GenreController : ControllerBase
//    {
//        private readonly IGenreService _genreService;

//        public GenreController(IGenreService genreService)
//        {
//            _genreService = genreService;
//        }

//        [HttpGet("{id}/songs")]
//        public async Task<IActionResult> GetSongsByGenre(int id)
//        {
//            var songs = await _genreService.GetSongsByGenreIdAsync(id);
//            if (songs == null || !songs.Any())
//            {
//                return NotFound($"No songs found for GenreId: {id}");
//            }

//            return Ok(songs);
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var genres = await _genreService.GetAllGenresAsync();
//            return Ok(genres);
//        }
//    }
//}
