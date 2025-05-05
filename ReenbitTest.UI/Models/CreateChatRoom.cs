namespace ReenbitTest.UI.Models
{
    public class CreateChatRoom
    {
        public string Name { get; set; } = null!;
        public List<string> UserIds { get; set; } = [];
    }
}