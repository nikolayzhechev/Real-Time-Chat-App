using RealTimeChatApp.Authentication;
using RealTimeChatApp.Models.DTOs;

namespace RealTimeChatApp.Interfaces
{
    public interface IAuthService
    {
        public Task<string> Register(RegisterRequest request);
        public AuthResponseDTO Login(LoginRequest request);
        public bool Logout(User user);
        public User ValidateUser(string email, string password);
    }
}
