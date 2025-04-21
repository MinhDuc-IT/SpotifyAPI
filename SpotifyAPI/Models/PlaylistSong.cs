using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.Models
{
    public class PlaylistSong
    {
        [Key]
        public int PlaylistSongsID { get; set; }

        public int PlaylistID { get; set; }

        [ForeignKey("PlaylistID")]
        public Playlist Playlist { get; set; }

        public int SongID { get; set; }

        [ForeignKey("SongID")]
        public Song Song { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public int? Order { get; set; }
    }
}
