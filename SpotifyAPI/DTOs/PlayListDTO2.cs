namespace SpotifyAPI.DTOs
{
    public class PlayListDTO2
    {
        public int PlaylistID { get; set; }
        public string PlaylistName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
    }
}
