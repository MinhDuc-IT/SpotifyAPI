using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyAPI.Models
{
    public class SongGenre
    {
        public int SongId { get; set; }
        //[ForeignKey(nameof(SongId))]
        public Song Song { get; set; }

        public int GenreId { get; set; }
        //[ForeignKey(nameof(GenreId))]
        public Genre Genre { get; set; }
    }
}
