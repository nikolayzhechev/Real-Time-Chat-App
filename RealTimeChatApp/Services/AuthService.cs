using RealTimeChatApp.Authentication;
using RealTimeChatApp.Interfaces;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using RealTimeChatApp.Data;
using RealTimeChatApp.Models;

namespace RealTimeChatApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        public AuthService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Register(RegisterRequest request)
        {
            if (_dbContext.AppUsers.Any(u => u.Email == request.Email))
                // TODO: return user name already taken
                return null;

            _dbContext.AppUsers.Add(new AppUser
                {
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                });

            LoginRequest currentLogin = new LoginRequest { Email = request.Email, Password = request.Password };

            string token = GetToken(currentLogin);
            return token;
        }
        public string Login(LoginRequest request)
        {
            AppUser user = _dbContext.AppUsers.FirstOrDefault(u => u.Email == request.Email)!;
            if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                string token = GetToken(request);
                return token;
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

        private string GetToken(LoginRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Generate key
            var keyBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            var key = new SymmetricSecurityKey(keyBytes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
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
