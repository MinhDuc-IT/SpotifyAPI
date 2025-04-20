using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.Models
{
    public class Playlist
    {
        [Key]
        public int PlaylistID { get; set; }

        [Required]
        [MaxLength(100)]
        public string PlaylistName { get; set; }

        public DateTime CreatedDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsPublic { get; set; } = false;
        
        // FK
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
        public ICollection<LikedPlaylist> LikedByUsers { get; set; } = new List<LikedPlaylist>();

    }
}
