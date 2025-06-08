namespace RealTimeChatApp.Models.DTOs
{
    public class CreateChatRequestDTO
    {
        public List<int> ParticipantIds { get; set; }
        public string? Title { get; set; }
        public string? InitialMessage { get; set; }
    }
}
