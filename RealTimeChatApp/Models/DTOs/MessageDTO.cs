namespace RealTimeChatApp.Models.DTOs
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }
    }
}
