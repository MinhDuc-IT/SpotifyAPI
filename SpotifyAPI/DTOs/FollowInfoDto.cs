namespace SpotifyAPI.DTOs
{
    public class FollowInfoDto
    {
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }
    }
}
