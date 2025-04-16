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
            // TODO: update with database
            var loginToken = _authService.Login(request);
            if(loginToken != null)
            {
                return Ok(new { token = loginToken });
            } else
            {
                return Unauthorized();
            }
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var registrationToken = _authService.Register(request);
            if(registrationToken != null)
            {
                return Ok(new { token = registrationToken });
            } else
            {
                return BadRequest("User already exists.");
            }
        }
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] User user)
        {
            bool response = _authService.Logout(user);
            if (response)
                return Ok(user);

            return NotFound(user);
        }
    }
}
