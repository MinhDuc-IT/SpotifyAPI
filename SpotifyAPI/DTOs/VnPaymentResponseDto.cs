namespace SpotifyAPI.DTOs
{
    public class VnPaymentResponseDto
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderCode { get; set; }
        public double Amount { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string TransactionDate { get; set; }
    }
}
