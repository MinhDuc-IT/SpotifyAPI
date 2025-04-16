namespace SpotifyAPI.DTOs
{
    public class UpdateSongDTO
    {
        public string SongName { get; set; }
        public IFormFile? Audio { get; set; }
        public IFormFile? Image { get; set; }
    }
}
