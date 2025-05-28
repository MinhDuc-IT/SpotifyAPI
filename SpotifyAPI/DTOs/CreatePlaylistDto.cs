using System.Text.Json.Serialization;

namespace SpotifyAPI.DTOs
{
    public class CreatePlaylistDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        //public string? Description { get; set; }
        //public bool IsPublic { get; set; }
    }
}
