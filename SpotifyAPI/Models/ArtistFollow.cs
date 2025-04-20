using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyAPI.Models
{
    public class ArtistFollow
    {
        public int UserID { get; set; }
        //[ForeignKey(nameof(UserID))]
        public User User { get; set; }

        public int ArtistId { get; set; }
        //[ForeignKey(nameof(ArtistId))]
        public Artist Artist { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}
