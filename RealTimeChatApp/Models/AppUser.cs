using RealTimeChatApp.Authentication;

namespace RealTimeChatApp.Models
{
    public class AppUser : User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        // an AppUser can have many chat user records (users that AppUser has chatted with)
        public ICollection<ChatUser> ChatUsers { get; set; }
    }
}
