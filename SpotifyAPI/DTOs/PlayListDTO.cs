namespace SpotifyAPI.DTOs
{
    public class PlayListDTO
    {
        public int PlaylistID { get; set; }
        public string PlaylistName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }

        public List<PlayListSongDTO> Songs { get; set; } = new();
    }
}
