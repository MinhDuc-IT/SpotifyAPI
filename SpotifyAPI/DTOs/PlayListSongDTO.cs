namespace SpotifyAPI.DTOs
{
    public class PlayListSongDTO
    {
        public int SongID { get; set; }
        public string SongName { get; set; }
        public string Audio { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? LyricUrl { get; set; }
        public string? Image { get; set; }
        public string? ArtistName { get; set; }
    }
}
