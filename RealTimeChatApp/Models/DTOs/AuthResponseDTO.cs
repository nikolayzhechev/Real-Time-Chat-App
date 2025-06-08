namespace RealTimeChatApp.Models.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string StatusCode { get; set; }
        public UserDto User { get; set; }
    }
}
