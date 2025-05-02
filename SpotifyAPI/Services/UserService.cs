using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;
using SpotifyAPI.Services;

namespace SpotifyAPI.Services
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserAsync(string email, string firebaseName, string firebaseUid, string role, string provider, string photoUrl);
        Task<List<User>> GetAllAdminsAsync();
        Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersAsync(int pageNumber, int pageSize);
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UserDto> UpdateUserAsync(string firebaseId, UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<UserDto> GetUserByFirebaseUidAsync(string userId);
        Task<FollowInfoDto> GetFollowInfoAsync(string firebaseId, int? artistId);
        Task<string> UploadAvatar(IFormFile avatar);
    }

    public class UserService : IUserService
    {
        private readonly SpotifyDbContext _context;
        private readonly IFirebaseUserSyncService _firebaseUserAsyncService;
        private readonly IArtistService _artistService;
        private readonly CloudinaryService _cloudinaryService;

        public UserService(SpotifyDbContext context, IFirebaseUserSyncService firebaseUserAsyncService, IArtistService artistService, CloudinaryService cloudinaryService)
        {
            _context = context;
            _firebaseUserAsyncService = firebaseUserAsyncService;
            _artistService = artistService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<User> GetOrCreateUserAsync(string email, string firebaseName, string firebaseUid, string role, string provider, string photoUrl)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FullName = firebaseName ?? email.Split('@')[0],
                    Password = "FIREBASE_AUTH",
                    Role = role,
                    Avatar = photoUrl ?? "https://placehold.co/400",
                    SubscriptionType = "Free",
                    EmailVerified = true,
                    DateJoined = DateTime.UtcNow,
                    SignInProvider = provider,
                    FirebaseUid = firebaseUid,
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            if(role == "Artist")
            {
                await _artistService.CreateArtistAsync(user.UserID, user.FullName);
            }

            return user;
        }

        public async Task<List<User>> GetAllAdminsAsync()
        {
            var admins = await _context.Users
                .Where(u => u.Role == "Admin")
                .ToListAsync();
            return admins;
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

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.FirebaseUid == userId);

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

        public async Task<UserDto> GetUserByFirebaseUidAsync(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.FirebaseUid == userId);

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

        public async Task<UserDto> UpdateUserAsync(string firebaseId, UserUpdateDto userUpdateDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseId);

            if (user != null)
            {
                user.FullName = userUpdateDto.FullName ?? user.FullName;
                user.Role = userUpdateDto.Role ?? user.Role;

                var avartarUrl = await _cloudinaryService.UploadImage(userUpdateDto.Avatar);
                user.Avatar = avartarUrl ?? user.Avatar;

                user.SubscriptionType = userUpdateDto.SubscriptionType ?? user.SubscriptionType;

                await _context.SaveChangesAsync();

                // Cập nhật Firebase nếu có UID
                //if (!string.IsNullOrEmpty(user.FullName))
                //{
                //    await _firebaseUserAsyncService.UpdateUserAsync(
                //        user.FullName,
                //        user.FullName,
                //        user.Avatar
                //    );
                //}

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

        public async Task<FollowInfoDto> GetFollowInfoAsync(string firebaseId, int? artistId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseId);
            if (user == null)
            {
                return new FollowInfoDto
                {
                    FollowerCount = 0,
                    FollowingCount = 0,
                    IsFollowedByCurrentUser = false,
                };
            }

            // Số người theo dõi user này
            var followerCount = await _context.UserFollows
                .CountAsync(f => f.FollowedUserId == user.UserID);

            // Số người user này đang theo dõi
            var followingUserCount = await _context.UserFollows
                .CountAsync(f => f.FollowerId == user.UserID);

            // Số nghệ sĩ user này đang theo dõi (nếu cần dùng riêng)
            var followingArtistCount = await _context.ArtistFollows
                .CountAsync(f => f.UserID == user.UserID);

            // Có đang follow artist (nếu artistId được truyền)
            bool isFollowed = false;
            if (artistId.HasValue)
            {
                isFollowed = await _context.ArtistFollows.AnyAsync(f =>
                    f.ArtistId == artistId.Value &&
                    f.UserID == user.UserID);
            }

            return new FollowInfoDto
            {
                FollowerCount = followerCount,
                FollowingCount = followingUserCount + followingArtistCount,
                IsFollowedByCurrentUser = isFollowed
            };
        }

        public async Task<string> UploadAvatar(IFormFile avatar)
        {
            var avartarUrl = await _cloudinaryService.UploadImage(avatar);
            return avartarUrl;
        }
    }
}
