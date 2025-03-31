namespace RealTimeChatApp.Interfaces
{
    public interface IUser
    {
        public string Email { get; }
        public string PasswordHash { get; }
    }
}
