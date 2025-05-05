namespace ReenbitTest.UI.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public string SenderUserName { get; set; } = null!;
        public string SenderFullName { get; set; } = null!;
        public int ChatRoomId { get; set; }
        public string? SentimentLabel { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
