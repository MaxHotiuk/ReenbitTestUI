using ReenbitTest.UI.Interfaces;
using ReenbitTest.UI.Models;
using System.Net.Http.Json;

namespace ReenbitTest.UI.Services
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public ChatService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<List<ChatRoom>> GetChatRoomsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<ChatRoom>>("api/chatrooms");
            return response ?? [];
        }

        public async Task<ChatRoom> GetChatRoomAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ChatRoom>($"api/chatrooms/{id}");
            return response!;
        }

        public async Task<ChatRoom> CreateChatRoomAsync(CreateChatRoom chatRoom)
        {
            var response = await _httpClient.PostAsJsonAsync("api/chatrooms", chatRoom);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ChatRoom>() ?? throw new InvalidOperationException("Failed to create chat room");
        }

        public async Task<bool> AddUserToChatRoomAsync(int chatRoomId, string userId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/chatrooms/{chatRoomId}/users", userId);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveUserFromChatRoomAsync(int chatRoomId, string userId)
        {
            var response = await _httpClient.DeleteAsync($"api/chatrooms/{chatRoomId}/users/{userId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Message>> GetMessagesForChatRoomAsync(int chatRoomId, int page = 1, int pageSize = 20)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Message>>(
                $"api/chatrooms/{chatRoomId}/messages?page={page}&pageSize={pageSize}");
            
            var currentUserId = _authService.CurrentUser?.Id;
            if (response != null && currentUserId != null)
            {
                // Mark messages from the current user
                foreach (var message in response)
                {
                    message.IsCurrentUser = message.SenderUserName == _authService.CurrentUser!.UserName;
                }
            }
            
            return response ?? [];
        }
    }
}