using Google;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.Models;

namespace SpotifyAPI.Services
{
    public interface IArtistFollowService
    {
        Task<bool> FollowArtistAsync(string firebaseUid, int artistId);
        Task<bool> UnfollowArtistAsync(string firebaseUid, int artistId);
        Task<bool> IsFollowingArtistAsync(string firebaseUid, int artistId);

    }

    public class ArtistFollowService : IArtistFollowService
    {
        private readonly SpotifyDbContext _context;

        public ArtistFollowService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> FollowArtistAsync(string firebaseUid, int artistId)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (existingUser == null) return false;

            var existingArtist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistID == artistId);
            if (existingArtist == null) return false;

            var existingFollow = await _context.ArtistFollows
                .FirstOrDefaultAsync(f => f.UserID == existingUser.UserID && f.ArtistId == artistId);

            if (existingFollow != null)
            {
                return false; 
            }

            var follow = new ArtistFollow
            {
                UserID = existingUser.UserID,
                ArtistId = artistId,
                FollowedAt = DateTime.UtcNow
            };

            _context.ArtistFollows.Add(follow);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnfollowArtistAsync(string firebaseUid, int artistId)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (existingUser == null) return false;

            var existingArtist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistID == artistId);
            if (existingArtist == null) return false;

            var existingFollow = await _context.ArtistFollows
                .FirstOrDefaultAsync(f => f.UserID == existingUser.UserID && f.ArtistId == artistId);

            if (existingFollow == null)
            {
                return false; 
            }

            _context.ArtistFollows.Remove(existingFollow);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> IsFollowingArtistAsync(string firebaseUid, int artistId)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (existingUser == null) return false;

            var existingArtist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistID == artistId);
            if (existingArtist == null) return false;

            var existingFollow = await _context.ArtistFollows
                .AnyAsync(f => f.UserID == existingUser.UserID && f.ArtistId == artistId);

            return existingFollow;
        }

    }
}
