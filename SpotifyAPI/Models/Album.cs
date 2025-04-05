using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.Models
{
    public class Album
    {
        [Key]
        public int AlbumID { get; set; }

        [Required]
        [MaxLength(150)]
        public string AlbumName { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string? Image { get; set; }

        // FK
        public int ArtistID { get; set; }

        [ForeignKey("ArtistID")]
        public Artist Artist { get; set; }

        public ICollection<Song> Songs { get; set; }
    }
}
