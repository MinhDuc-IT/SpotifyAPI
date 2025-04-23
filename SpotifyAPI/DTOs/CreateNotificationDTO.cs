using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.DTOs
{
    public class CreateNotificationDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public int? SenderUserId { get; set; }
    }
}
