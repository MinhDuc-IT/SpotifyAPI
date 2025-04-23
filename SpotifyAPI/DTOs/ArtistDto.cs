namespace SpotifyAPI.DTOs
{
    public class ArtistDto
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int TotalPlays { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
