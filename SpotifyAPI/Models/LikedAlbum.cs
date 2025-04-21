using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyAPI.Models
{
    public class LikedAlbum
    {
        public int UserID { get; set; }
        //[ForeignKey(nameof(UserID))]
        public User User { get; set; }

        public int AlbumID { get; set; }
        //[ForeignKey(nameof(AlbumID))]
        public Album Album { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}
