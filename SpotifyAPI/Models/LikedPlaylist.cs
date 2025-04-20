using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyAPI.Models
{
    public class LikedPlaylist
    {
        public int UserID { get; set; }
        //[ForeignKey(nameof(UserID))]
        public User User { get; set; }

        public int PlaylistID { get; set; }
        //[ForeignKey(nameof(PlaylistID))]
        public Playlist Playlist { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}
