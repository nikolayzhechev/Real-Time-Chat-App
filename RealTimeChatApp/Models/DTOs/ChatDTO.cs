namespace RealTimeChatApp.Models.DTOs
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<string> ParticipantUsernames { get; set; }
        public List<MessageDTO> Messages { get; set; }
    }
}
