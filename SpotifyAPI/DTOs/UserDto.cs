namespace SpotifyAPI.DTOs
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? Avatar { get; set; }
        public DateTime DateJoined { get; set; }
        public string SubscriptionType { get; set; }  // "Free" or "Premium"
        public string Role { get; set; }  // "User" or "Admin"
    }
}
