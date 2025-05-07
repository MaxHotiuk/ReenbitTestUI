using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ReenbitTest.UI.Interfaces;
using ReenbitTest.UI.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ReenbitTest.UI.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IChatHubService _chatHubService;
        private readonly string _authTokenKey = "authToken";
        private readonly string _userKey = "currentUser";

        public AuthService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider,
            IChatHubService chatHubService)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _chatHubService = chatHubService;
        }

        public string? AuthToken { get; private set; }
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result != null && result.Success)
            {
                await _localStorage.SetItemAsync(_authTokenKey, result.Token);
                await _localStorage.SetItemAsync(_userKey, result.User);
                
                AuthToken = result.Token;
                CurrentUser = result.User;
                
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", result.Token);
                
                ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(result.User);
            }

            return result!;
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result != null && result.Success)
            {
                await _localStorage.SetItemAsync(_authTokenKey, result.Token);
                await _localStorage.SetItemAsync(_userKey, result.User);
                
                AuthToken = result.Token;
                CurrentUser = result.User;
                
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", result.Token);
                
                ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(result.User);
            }

            return result!;
        }

        public async Task Logout()
        {
            // Stop the SignalR connection before clearing auth data
            try 
            {
                // Get the ChatHubService from DI (you'll need to add this as a property or parameter)
                if (_chatHubService != null && _chatHubService.IsConnected)
                {
                    await _chatHubService.StopConnectionAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error but continue with logout
                Console.WriteLine($"Error disconnecting from chat hub: {ex.Message}");
            }

            await _localStorage.RemoveItemAsync(_authTokenKey);
            await _localStorage.RemoveItemAsync(_userKey);
            
            _httpClient.DefaultRequestHeaders.Authorization = null;
            AuthToken = null;
            CurrentUser = null;
            
            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();
        }

        public async Task InitializeAuthState()
        {
            var token = await _localStorage.GetItemAsync<string>(_authTokenKey);
            var user = await _localStorage.GetItemAsync<User>(_userKey);

            if (!string.IsNullOrEmpty(token) && user != null)
            {
                AuthToken = token;
                CurrentUser = user;
                
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
                
                ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(user);
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(_authTokenKey);
        }
    }
}