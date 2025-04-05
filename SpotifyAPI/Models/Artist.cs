using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.Models
{
    public class Artist
    {
        [Key]
        public int ArtistID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ArtistName { get; set; }

        public string? Bio { get; set; }

        public string? Image { get; set; }

        public ICollection<Song> Songs { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
