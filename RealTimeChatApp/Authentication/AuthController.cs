using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using RealTimeChatApp.Services;
using RealTimeChatApp.Interfaces;

namespace RealTimeChatApp.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Hardcode TEST credentials
            // TODO: update with database user check
            if (request.Email == "test@example.com" & request.Password == "password1234")
            {
                var tokenString = _authService.Login(request);
                return Ok(new { token = tokenString });
            }

            return Unauthorized();
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var registration = _authService.Register(request);
            if(!registration)
            {
                return BadRequest("User already exists.");
            }
            return Ok();
        }
    }
}
