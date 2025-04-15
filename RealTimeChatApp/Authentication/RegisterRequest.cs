using Microsoft.AspNetCore.Identity.Data;
using RealTimeChatApp.Interfaces;

namespace RealTimeChatApp.Authentication
{
    public class RegisterRequest : IRegisterRequest
    {
        public string Email {  get; set; }

        public string UserName {  get; set; }

        public string Password {  get; set; }
    }
}
