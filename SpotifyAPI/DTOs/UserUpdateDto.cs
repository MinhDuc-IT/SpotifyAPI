using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.DTOs
{
    public class UserUpdateDto
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        public string? Avatar { get; set; }

        public string? SubscriptionType { get; set; }  // "Free" or "Premium"

        public string? Role { get; set; }  // "User" or "Admin"
    }
}
