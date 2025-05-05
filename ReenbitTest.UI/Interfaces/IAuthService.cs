using ReenbitTest.UI.Models;

namespace ReenbitTest.UI.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginRequest request);
        Task<AuthResponse> Register(RegisterRequest request);
        Task Logout();
        bool IsAuthenticated { get; }
        User? CurrentUser { get; }
        string? AuthToken { get; }
        Task InitializeAuthState();
    }
}