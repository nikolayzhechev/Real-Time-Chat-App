namespace RealTimeChatApp.Interfaces;
using RealTimeChatApp.Authentication;

public interface IUserService
{
    User ValidateUser(string email, string password);
}
