using CareSync.DTOs;
using CareSync.Models;
using CareSync.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace CareSync.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.GetUserByEmailAsync(dto.Email!) != null)
                return "Email already in use.";

            var hashedPassword = HashPassword(dto.Password!);
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = hashedPassword
            };

            await _userRepository.AddUserAsync(user);
            return "Registration successful.";
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email!);
            if (user == null || !VerifyPassword(dto.Password!, user.Password!))
                return "Invalid email or password.";

            return "Login successful.";
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}
