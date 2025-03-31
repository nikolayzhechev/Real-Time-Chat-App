using RealTimeChatApp.Interfaces;

namespace RealTimeChatApp.Authentication
{
    public class LoginRequest : IloginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
