namespace SpotifyAPI.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public class NotificationReceiver
    {
        [Key]
        public int NotiReceiverId { get; set; }

        // FK đến Notification
        public int NotificationId { get; set; }
        public Notification Notification { get; set; }

        // FK đến User
        public int ReceiverUserId { get; set; }
        public User Receiver { get; set; }

        public bool IsRead { get; set; } = false;

        // Nếu cần thêm trạng thái:
        public bool IsSentRealtime { get; set; } = false;
    }
}