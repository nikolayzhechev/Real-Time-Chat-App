using RealTimeChatApp.Authentication;
using RealTimeChatApp.Interfaces;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using RealTimeChatApp.Data;
using RealTimeChatApp.Models;
using System.Threading.Tasks;
using RealTimeChatApp.Models.DTOs;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace RealTimeChatApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public AuthService(AppDbContext dbContext, ILogger<AuthService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> Register(RegisterRequest request)
        {
            if (_dbContext.AppUsers.Any(u => u.Email == request.Email))
                return null;

            var userEntry = _dbContext.AppUsers.Add(new AppUser
            {
                Username = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            });

            await _dbContext.SaveChangesAsync();

            LoginRequest currentLogin = new LoginRequest { Email = request.Email, Password = request.Password };

            string token = GetToken(currentLogin, userEntry.Entity.Id);
            return token;
        }
        public AuthResponseDTO Login(LoginRequest request)
        {
            var user = _dbContext.AppUsers.FirstOrDefault(u => u.Email == request.Email)!;
            if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                };

                string token = GetToken(request, userDto.Id);

                return new AuthResponseDTO
                {
                    Token = token,
                    User = userDto
                };
            }
            return null;
        }
        public bool Logout(User user)
        {
            AppUser foundUser = _dbContext.AppUsers.FirstOrDefault(u => u.Email == user.Email)!;
            if (foundUser != null)
            {
                _dbContext.AppUsers.Remove(foundUser);
                return true;
            } else
            {
                return false;
            }
        }
        public User ValidateUser(string email, string password)
        {
            var user = _dbContext.AppUsers.FirstOrDefault(u => u.Email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        private string GetToken(LoginRequest request, int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            _logger.LogInformation($"Key: {key.KeySize}");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, request.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
