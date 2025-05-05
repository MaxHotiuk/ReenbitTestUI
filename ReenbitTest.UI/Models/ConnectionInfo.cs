namespace ReenbitTest.UI.Models
{
    public class ConnectionInfo
    {
        public string ConnectionId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public List<int> AvailableChatRooms { get; set; } = [];
        public List<string> ActiveGroups { get; set; } = [];
    }
}