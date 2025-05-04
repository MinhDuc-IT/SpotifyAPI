using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.DTOs
{
    public class CreateSearchHistoryDTO
    {
        public int SearchID { get; set; }

        public int ResultID { get; set; }

        public string? AudioURL { get; set; }

        public string? ImageURL { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}

