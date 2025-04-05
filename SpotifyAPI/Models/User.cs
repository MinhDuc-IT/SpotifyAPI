namespace SpotifyAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public string? Avatar { get; set; }

        public DateTime DateJoined { get; set; }

        [Required]
        public string SubscriptionType { get; set; }  // "Free" or "Premium"

        [Required]
        public string Role { get; set; }  // "User" or "Admin"

        // Navigation
        public ICollection<UserSubscription> Subscriptions { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}
