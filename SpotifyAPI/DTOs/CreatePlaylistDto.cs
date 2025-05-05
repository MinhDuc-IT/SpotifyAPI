namespace SpotifyAPI.DTOs
{
    public class CreatePlaylistDto
    {
        public string PlaylistName { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
    }
}
