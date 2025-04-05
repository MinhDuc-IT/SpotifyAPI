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

        // FK
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}
