namespace PriceSentry.WebApi.Models {
    public class AuthResponse {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}
