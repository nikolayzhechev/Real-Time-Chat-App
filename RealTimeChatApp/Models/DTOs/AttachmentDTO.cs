namespace RealTimeChatApp.Models.DTOs
{
    public class AttachmentDTO
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileUrl { get; set; }
        public int MessageId { get; set; }
    }
}
