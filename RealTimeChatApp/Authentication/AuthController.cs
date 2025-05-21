using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using RealTimeChatApp.Services;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Data;
using Microsoft.EntityFrameworkCore;

namespace RealTimeChatApp.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly DbContext _dbContext;

        public AuthController(IAuthService authService, AppDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var loginResponse = _authService.Login(request);
            if(loginResponse != null)
            {
                return Ok(new { response = loginResponse });
            } else
            {
                return Unauthorized("Invalid username or password.");
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
