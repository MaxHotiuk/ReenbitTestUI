namespace ReenbitTest.UI.Models
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<User> Users { get; set; } = [];
        public int MessageCount { get; set; }
    }
}