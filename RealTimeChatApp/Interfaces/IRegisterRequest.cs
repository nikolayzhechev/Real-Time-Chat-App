namespace RealTimeChatApp.Interfaces
{
    public interface IRegisterRequest
    {
        public string Email { get; }
        public string UserName { get; }
        public string Password { get; }
    }
}
