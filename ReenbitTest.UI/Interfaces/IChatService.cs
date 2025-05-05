using ReenbitTest.UI.Models;

namespace ReenbitTest.UI.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatRoom>> GetChatRoomsAsync();
        Task<ChatRoom> GetChatRoomAsync(int id);
        Task<ChatRoom> CreateChatRoomAsync(CreateChatRoom chatRoom);
        Task<bool> AddUserToChatRoomAsync(int chatRoomId, string userId);
        Task<bool> RemoveUserFromChatRoomAsync(int chatRoomId, string userId);
        Task<List<Message>> GetMessagesForChatRoomAsync(int chatRoomId, int page = 1, int pageSize = 20);
    }
}