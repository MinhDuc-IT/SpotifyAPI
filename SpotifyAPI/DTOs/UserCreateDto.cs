using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.DTOs
{
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string FullName { get; set; }
        public string? Avatar { get; set; }

        [Required]
        public string SubscriptionType { get; set; }  // "Free" or "Premium"

        [Required]
        public string Role { get; set; }  // "User" or "Admin"
    }
}
