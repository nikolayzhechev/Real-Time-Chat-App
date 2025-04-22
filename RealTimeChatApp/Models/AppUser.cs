namespace RealTimeChatApp.Models
{
    public class AppUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; }
    }
}
