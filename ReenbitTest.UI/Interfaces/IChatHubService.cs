using ReenbitTest.UI.Models;

namespace ReenbitTest.UI.Interfaces
{
    public interface IChatHubService
    {
        event Action<Message> OnReceiveMessage;
        event Action<List<Message>> OnLoadRecentMessages;
        event Action<UserTypingInfo> OnUserTyping;
        event Action<UserTypingInfo> OnUserStoppedTyping;
        event Action<int, string, string> OnUserJoined;
        event Action<int, string, string> OnUserLeft;
        event Action<string> OnError;
        event Action<int> OnJoinedChatRoom;
        event Action<int> OnLeftChatRoom;
        
        bool IsConnected { get; }
        Task StartConnectionAsync(string token);
        Task StopConnectionAsync();
        Task JoinChatRoomAsync(int chatRoomId);
        Task LeaveChatRoomAsync(int chatRoomId);
        Task SendMessageAsync(CreateMessage message);
        Task NotifyUserTypingAsync(int chatRoomId);
        Task NotifyUserStoppedTypingAsync(int chatRoomId);
        Task<ConnectionInfo> GetConnectionInfoAsync();
        Task SendHeartbeatAsync();
        void ResetConnectionState();
    }
}