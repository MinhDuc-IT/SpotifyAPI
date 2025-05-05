namespace SpotifyAPI.DTOs
{
    public class UserSubscriptionDto
    {
        public string SubscriptionType { get; set; }  // "Free" or "Premium"
        public DateTime? PremiumExpiryDate { get; set; }
    }
}
