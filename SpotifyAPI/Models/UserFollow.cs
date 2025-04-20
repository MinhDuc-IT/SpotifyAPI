using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyAPI.Models
{
    public class UserFollow
    {
        public int FollowerId { get; set; }
        //[ForeignKey(nameof(FollowerId))]
        public User Follower { get; set; }

        public int FollowedUserId { get; set; }
        //[ForeignKey(nameof(FollowedUserId))]
        public User FollowedUser { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}
