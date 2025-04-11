using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.Models;

namespace SpotifyAPI.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserAsync(string email, string firebaseName, string firebaseUid, string role);
    }

    public class UserService : IUserService
    {
        private readonly SpotifyDbContext _context;

        public UserService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetOrCreateUserAsync(string email, string firebaseName, string firebaseUid, string role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FullName = firebaseUid ?? email.Split('@')[0],
                    Password = "FIREBASE_AUTH",
                    Role = role,
                    Avatar = "default-avatar.png",
                    SubscriptionType = "Free"
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            return user;
        }
    }
}
