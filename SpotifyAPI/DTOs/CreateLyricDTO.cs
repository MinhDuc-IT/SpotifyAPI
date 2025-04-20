using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.DTOs
{
    public class CreateLyricDTO
    {
        [Required]
        public IFormFile Lyric { get; set; }
    }
}
