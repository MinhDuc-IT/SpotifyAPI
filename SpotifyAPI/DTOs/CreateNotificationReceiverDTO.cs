using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.DTOs
{
    public class CreateNotificationReceiverDTO
    {
        [Required]
        public int NotificationId { get; set; }

        [Required]
        public int ReceiverUserId { get; set; }
    }
}