namespace RealTimeChatApp.Interfaces
{
    public interface IEmailService
    {
        public Task SendResetPasswordEmail(string to, string resetLink);
    }
}