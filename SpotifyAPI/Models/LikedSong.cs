using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyAPI.Models
{
    public class LikedSong
    {
        public int UserID { get; set; }
        //[ForeignKey(nameof(UserID))]
        public User User { get; set; }

        public int SongID { get; set; }
        //[ForeignKey(nameof(SongID))]
        public Song Song { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}
