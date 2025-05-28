using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string GenreName { get; set; }
        public string? Image { get; set; }

        public ICollection<SongGenre> SongGenres { get; set; } = new List<SongGenre>();

    }
}
