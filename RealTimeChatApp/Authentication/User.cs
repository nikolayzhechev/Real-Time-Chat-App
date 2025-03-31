using RealTimeChatApp.Interfaces;

namespace RealTimeChatApp.Authentication
{
    public class User : IUser
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
