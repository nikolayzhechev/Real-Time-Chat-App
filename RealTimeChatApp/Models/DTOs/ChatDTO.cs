namespace RealTimeChatApp.Models.DTOs
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? DirectKey { get; set; }
        public bool IsGroup { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ChatUser> Participants { get; set; }
        public List<MessageDTO> Messages { get; set; }
    }
}
