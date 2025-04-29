namespace SpotifyAPI.DTOs
{
    public class ArtistInfoDTO
    {
        public int ArtistID { get; set; }
        public string ArtistName { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }
        public DateTime FormedDate { get; set; }
        public bool IsFollowed { get; set; }
    }
}
