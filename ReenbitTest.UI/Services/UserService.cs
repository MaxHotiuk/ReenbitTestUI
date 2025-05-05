using ReenbitTest.UI.Interfaces;
using ReenbitTest.UI.Models;
using System.Net.Http.Json;

namespace ReenbitTest.UI.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<User>>("api/users");
            return response ?? [];
        }

        public async Task<User> GetUserAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<User>($"api/users/{id}");
            return response!;
        }
    }
}