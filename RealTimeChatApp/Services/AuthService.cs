using RealTimeChatApp.Authentication;
using RealTimeChatApp.Interfaces;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RealTimeChatApp.Services
{
    public class AuthService : IAuthService
    {
        // TODO: update hardcoded value
        // Testing only
        private readonly List<User> _users = new List<User>
        {
            new User { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1234") }
        };
        public string Register(RegisterRequest request)
        {
            if (_users.Any(u => u.Email == request.Email))
                return null;

            _users.Add(new User
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
            if (_users.Any(u => u.Email == request.Email & u.PasswordHash == request.Password))
            {
                string token = GetToken(request);
                return token;
            }
            return null;
        }
        public bool Logout(User user)
        {
            User foundUser = _users.Find(u => u.Email == user.Email);
            if (foundUser != null)
            {
                _users.Remove(foundUser);
                return true;
            } else
            {
                return false;
            }
        }
        public User ValidateUser(string email, string password)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
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
