namespace SpotifyAPI.DTOs
{
    public class ArtistWithSongsDto
    {
        public int ArtistID { get; set; }
        public string ArtistName { get; set; }
        public string? Image { get; set; }

        public List<SongDto> Songs { get; set; } = new List<SongDto>();
    }
}
