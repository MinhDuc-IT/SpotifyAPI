using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpotifyAPI.Models
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Features { get; set; } // Bạn có thể lưu dưới dạng JSON hoặc mô tả ở dạng text
    }
}
