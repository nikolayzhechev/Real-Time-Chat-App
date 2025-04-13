using RealTimeChatApp.Authentication;

namespace RealTimeChatApp.Interfaces
{
    public interface IAuthService
    {
        public bool Register(RegisterRequest request);
        public string Login(LoginRequest request);
        public User ValidateUser(string email, string password);
    }
}
