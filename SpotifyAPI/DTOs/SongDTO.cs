namespace SpotifyAPI.DTOs
{
    public class SongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string Album { get; set; }
        public int AlbumID { get; set; }
        public string ThumbnailUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public string AudioUrl { get; set; }

    }

}
