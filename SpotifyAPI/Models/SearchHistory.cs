namespace SpotifyAPI.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public class SearchHistory
    {
        [Key]
        public int SearchID { get; set; }

        public int ResultID { get; set; }

        public string? AudioURL { get; set; }

        public string? ImageURL { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        // Foreign key
        [ForeignKey("UserID")]
        public int UserID { get; set; }

        // Navigation property
        public User User { get; set; }

        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
    }
}
