namespace ReenbitTest.UI.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = null!;
        public User User { get; set; } = null!;
        public List<string>? Errors { get; set; }
    }
}