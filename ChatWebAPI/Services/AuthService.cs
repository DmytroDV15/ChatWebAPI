using ChatWebAPI.Database;
using ChatWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ChatDbContext _context;

        public AuthService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task RegisterUserAsync(string userName, string email, string password)
        {
            // Перевірка наявності користувача з таким самим email
            if (await _context.RegisterModels.AnyAsync(u => u.Email == email))
            {
                throw new Exception("Email already exists");
            }

            // Хешування пароля
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Створення нового користувача
            var user = new RegistrationModel
            {
                UserName = userName,
                Email = email,
                Password = hashedPassword,
                Chats = new List<Chat>() // Ініціалізація списку чатів
            };

            await _context.RegisterModels.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<RegistrationModel?> AuthenticateUserAsync(string email, string password)
        {
            var user = await _context.RegisterModels.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return user;
        }
    }
}
