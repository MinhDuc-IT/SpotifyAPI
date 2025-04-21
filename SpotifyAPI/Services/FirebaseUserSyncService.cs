using FirebaseAdmin.Auth;
using SpotifyAPI.Models;

namespace SpotifyAPI.Services
{
    public interface IFirebaseUserSyncService
    {
        Task<string> CreateUserAsync(string email, string password, string fullName, string avatarUrl);
        Task<bool> UpdateUserAsync(string uid, string fullName, string avatarUrl);
        Task<bool> DeleteUserAsync(string uid);
    }

    public class FirebaseUserSyncService : IFirebaseUserSyncService
    {
        public async Task<string> CreateUserAsync(string email, string password, string fullName, string avatarUrl)
        {
            var userRecordArgs = new UserRecordArgs
            {
                Email = email,
                Password = password,
                DisplayName = fullName,
                PhotoUrl = avatarUrl
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
            return userRecord.Uid;
        }

        public async Task<bool> UpdateUserAsync(string uid, string? fullName, string? avatarUrl)
        {
            var updateArgs = new UserRecordArgs
            {
                Uid = uid,
            };
            if (!string.IsNullOrEmpty(avatarUrl))
                updateArgs.PhotoUrl = avatarUrl;

            try
            {
                var updatedUser = await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateArgs);
                Console.WriteLine($"User updated: {updatedUser.Uid}");
                return true;
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string uid)
        {
            try
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
                Console.WriteLine($"User deleted: {uid}");
                return true;
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                throw;
            }
        }
    }
}
