using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.Models
{
    public class UserSubscription
    {
        [Key]
        public int SubscriptionID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // FK
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        //FK
        public int PlanId { get; set; }
        [ForeignKey("PlanId")]
        public Plan Plan { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
