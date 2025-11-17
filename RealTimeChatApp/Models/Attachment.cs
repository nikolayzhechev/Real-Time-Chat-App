namespace RealTimeChatApp.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public int MessageId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FileUrl { get; set; }
        public int ChatId { get; set; }
        public DateTime UploadedAt { get; set; }
        public Message Message { get; set; }
    }
}
