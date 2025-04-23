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

        public TimeSpan? Duration { get; set; }

        public string Audio { get; set; } // URL audio

        public string? LyricUrl { get; set; }

        public int PlayCount { get; set; }

        // FK
        public int ArtistID { get; set; }

        [ForeignKey("ArtistID")]
        public Artist Artist { get; set; }

        public int? AlbumID { get; set; }

        [ForeignKey("AlbumID")]
        public Album Album { get; set; }

        public string? Image { get; set; }
        public int? TrackNumber { get; set; }  //thứ tự trong album

        [Required]
        public SongStatus Status { get; set; } = SongStatus.PENDING;

        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
        public ICollection<LikedSong> LikedByUsers { get; set; } = new List<LikedSong>();
        public ICollection<ListeningHistory> ListeningHistories { get; set; } = new List<ListeningHistory>();
        public ICollection<SongArtist> SongArtists { get; set; } = new List<SongArtist>();
        public ICollection<SongGenre> SongGenres { get; set; } = new List<SongGenre>();

    }
}
