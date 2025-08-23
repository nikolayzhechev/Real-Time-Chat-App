namespace RealTimeChatApp.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public bool IsGroup { get; set; }
        public string? Name { get; set; }
        public string? DirectKey { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
    }
}
