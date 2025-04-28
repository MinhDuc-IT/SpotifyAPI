using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        public DateTime FormedDate { get; set; }

        //[ForeignKey("User")]
        public int UserID { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public ICollection<Song> Songs { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<ArtistFollow> Followers { get; set; } = new List<ArtistFollow>();
        public ICollection<SongArtist> SongArtists { get; set; } = new List<SongArtist>();

    }
}
