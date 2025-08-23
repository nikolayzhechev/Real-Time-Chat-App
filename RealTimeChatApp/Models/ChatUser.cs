using System;

namespace RealTimeChatApp.Models
{
    public class ChatUser
    {
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
