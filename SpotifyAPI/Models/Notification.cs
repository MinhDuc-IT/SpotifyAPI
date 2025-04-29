using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SpotifyAPI.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Người gửi (nếu cần)
        public int? SenderUserId { get; set; }

        [JsonIgnore]
        public User? Sender { get; set; }

        // Navigation
        [JsonIgnore]
        public ICollection<NotificationReceiver> NotificationReceivers { get; set; } = new List<NotificationReceiver>();
    }
}
