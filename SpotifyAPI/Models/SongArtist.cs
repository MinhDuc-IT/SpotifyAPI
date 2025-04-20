using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyAPI.Models
{
    public class SongArtist
    {
        public int SongId { get; set; }
        //[ForeignKey(nameof(SongId))]
        public Song Song { get; set; }

        public int ArtistId { get; set; }
        //[ForeignKey(nameof(ArtistId))]
        public Artist Artist { get; set; }
    }
}
