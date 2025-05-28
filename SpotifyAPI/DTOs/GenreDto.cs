using SpotifyAPI.Models;

namespace SpotifyAPI.DTOs
{
    public class GenreDto
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public string? Image { get; set; }
        public IEnumerable<SongDto> Songs { get; set; }
    }

    public class CreateGenreDto
    {
        public string GenreName { get; set; }
    }

    public class UpdateGenreDto
    {
        public string GenreName { get; set; }
    }
}
