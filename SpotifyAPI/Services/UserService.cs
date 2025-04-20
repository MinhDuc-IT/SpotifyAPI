using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;

namespace SpotifyAPI.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserAsync(string email, string firebaseName, string firebaseUid, string role);
        Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersAsync(int pageNumber, int pageSize);
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(int userId);
    }

    public class UserService : IUserService
    {
        private readonly SpotifyDbContext _context;
        private readonly IFirebaseUserSyncService _firebaseUserAsyncService;

        public UserService(SpotifyDbContext context, IFirebaseUserSyncService firebaseUserAsyncService)
        {
            _context = context;
            _firebaseUserAsyncService = firebaseUserAsyncService;
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
                    Avatar = "https://placehold.co/400",
                    SubscriptionType = "Free"
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.DateJoined)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = users.Select(user => new UserDto
            {
                UserID = user.UserID,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
                DateJoined = user.DateJoined,
                SubscriptionType = user.SubscriptionType,
                Role = user.Role
            });

            return (userDtos, totalCount);
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
                return null;

            // Chuyển đổi từ User sang UserDto
            return new UserDto
            {
                UserID = user.UserID,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
                DateJoined = user.DateJoined,
                SubscriptionType = user.SubscriptionType,
                Role = user.Role
            };
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var firebaseUid = await _firebaseUserAsyncService.CreateUserAsync(
                userCreateDto.Email,
                userCreateDto.Password,
                userCreateDto.FullName,
                userCreateDto.Avatar
            );

            // Chuyển từ UserCreateDto sang User (entity)
            var user = new User
            {
                Email = userCreateDto.Email,
                FullName = userCreateDto.FullName,
                Password = userCreateDto.Password,
                Role = userCreateDto.Role,
                Avatar = userCreateDto.Avatar,
                SubscriptionType = userCreateDto.SubscriptionType
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Trả về UserDto
            return new UserDto
            {
                UserID = user.UserID,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
                DateJoined = user.DateJoined,
                SubscriptionType = user.SubscriptionType,
                Role = user.Role
            };
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);

            if (user != null)
            {
                user.FullName = userUpdateDto.FullName ?? user.FullName;
                user.Role = userUpdateDto.Role ?? user.Role;
                user.Avatar = userUpdateDto.Avatar ?? user.Avatar;
                user.SubscriptionType = userUpdateDto.SubscriptionType ?? user.SubscriptionType;

                await _context.SaveChangesAsync();

                // Cập nhật Firebase nếu có UID
                if (!string.IsNullOrEmpty(user.FullName))
                {
                    await _firebaseUserAsyncService.UpdateUserAsync(
                        user.FullName,
                        user.FullName,
                        user.Avatar
                    );
                }

                // Trả về UserDto
                return new UserDto
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    FullName = user.FullName,
                    Avatar = user.Avatar,
                    DateJoined = user.DateJoined,
                    SubscriptionType = user.SubscriptionType,
                    Role = user.Role
                };
            }

            return null;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(user.FullName))
            {
                await _firebaseUserAsyncService.DeleteUserAsync(user.FullName);
            }

            return true;
        }
    }
}
