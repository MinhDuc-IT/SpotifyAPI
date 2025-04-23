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
        public string? FullName { get; set; }

        public bool EmailVerified { get; set; }

        //[Column("profile_picture_url")]
        //public string? ProfilePictureUrl { get; set; }

        public string? Avatar { get; set; }

        [MaxLength(50)]
        public string? SignInProvider { get; set; }

        [MaxLength(50)]
        public string? FirebaseUid { get; set; }

        [MaxLength(50)]
        public string? GoogleUid { get; set; }

        [MaxLength(50)]
        public string? FaccebookUid { get; set; }

        public DateTime DateJoined { get; set; }

        [Required]
        public string SubscriptionType { get; set; }  // "Free" or "Premium"

        [Required]
        public string Role { get; set; }  // "User" or "Admin"

        // Navigation
        public ICollection<UserSubscription> Subscriptions { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<ArtistFollow> ArtistFollows { get; set; } = new List<ArtistFollow>();
        public ICollection<LikedAlbum> LikedAlbums { get; set; } = new List<LikedAlbum>();
        public ICollection<LikedPlaylist> LikedPlaylists { get; set; } = new List<LikedPlaylist>();
        public ICollection<LikedSong> LikedSongs { get; set; } = new List<LikedSong>();
        public ICollection<ListeningHistory> ListeningHistories { get; set; } = new List<ListeningHistory>();
        public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();  // Những người mà User đang theo dõi
        public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();  // Những người theo dõi User
        public ICollection<NotificationReceiver> NotificationReceivers { get; set; } = new List<NotificationReceiver>();
        public ICollection<Notification> SentNotifications { get; set; } = new List<Notification>();
    }
}
