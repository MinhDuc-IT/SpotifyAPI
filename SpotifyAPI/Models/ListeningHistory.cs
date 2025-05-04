using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyAPI.Models
{
    public class ListeningHistory
    {
        public int Id { get; set; }

        public int UserID { get; set; }
        //[ForeignKey(nameof(UserID))]
        public User User { get; set; }

        public int SongId { get; set; }
        //[ForeignKey(nameof(SongId))]
        public Song Song { get; set; }

        public DateTime PlayedAt { get; set; }

        public string DeviceInfo { get; set; }
    }
}
