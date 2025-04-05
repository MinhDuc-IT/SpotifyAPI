using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.Models
{
    public class Song
    {
        [Key]
        public int SongID { get; set; }

        [Required]
        [MaxLength(200)]
        public string SongName { get; set; }

        public TimeSpan Duration { get; set; }

        public string Audio { get; set; } // URL audio

        public int PlayCount { get; set; }

        // FK
        public int ArtistID { get; set; }

        [ForeignKey("ArtistID")]
        public Artist Artist { get; set; }

        public int AlbumID { get; set; }

        [ForeignKey("AlbumID")]
        public Album Album { get; set; }

        public string? Image { get; set; }

        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}
