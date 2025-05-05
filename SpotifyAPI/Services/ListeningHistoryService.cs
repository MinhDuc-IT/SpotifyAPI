using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;

using SpotifyAPI.Utils;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SpotifyAPI.Services
{
    public interface IListeningHistoryService
    {
        Task<List<SongDto>> GetListeningHistoryAsync(string userIdToken, int limit = 4);
        Task<List<ArtistDto>> GetTopArtistsAsync(string userIdToken, int limit = 5);
        Task<bool> AddAsync(string userIdToken, int songId, string? deviceInfo = null);
    }

    public class ListeningHistoryService : IListeningHistoryService
    {
        private readonly SpotifyDbContext _context;

        public ListeningHistoryService(SpotifyDbContext context)
        {
            _context = context;

        }

        public async Task<List<SongDto>> GetListeningHistoryAsync(string userIdToken, int limit = 4)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.FirebaseUid == userIdToken);

            if (user == null)
                return null;
            var userId = user.UserID;
            var history = await _context.ListeningHistories
                .Where(h => h.UserID == userId)
                .OrderByDescending(h => h.PlayedAt)
                .Take(limit)
                .Select(h => new SongDto
                {
                    SongId = h.SongId,
                    Title = h.Song.SongName,
                    ArtistName = h.Song.Artist.ArtistName,
                    Album = h.Song.Album.AlbumName,
                    AlbumID = h.Song.AlbumID ?? 0,
                    ThumbnailUrl = h.Song.Image,
                    //Duration = h.Song.Duration ?? null,
                    AudioUrl = h.Song.Audio,
                })
                .ToListAsync();

            return history;
        }

        public async Task<List<ArtistDto>> GetTopArtistsAsync(string userIdToken, int limit = 5)
        {
            // Lấy thông tin người dùng từ tên người dùng (userIdToken)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.FirebaseUid == userIdToken);

            if (user == null)
                return null;

            var userId = user.UserID;

            // Truy vấn lịch sử nghe và tính số lần nghe cho từng Artist
            var topArtists = await _context.ListeningHistories
                .Where(h => h.UserID == userId)
                .GroupBy(h => h.Song.Artist)
                .Select(g => new
                {
                    Artist = g.Key,
                    PlayCount = g.Count()
                })
                .OrderByDescending(a => a.PlayCount)
                .Take(limit)
                .Select(a => new ArtistDto
                {
                    ArtistId = a.Artist.ArtistID,
                    ArtistName = a.Artist.ArtistName,
                    TotalPlays = a.PlayCount,
                    ThumbnailUrl = a.Artist.Image,
                })
                .ToListAsync();

            return topArtists;
        }

        public async Task<bool> AddAsync(string userIdToken, int songId, string? deviceInfo = null)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userIdToken);
                if (user == null)
                    throw new ArgumentException("Invalid user.");

                var song = await _context.Songs.FindAsync(songId);
                if (song == null)
                    throw new ArgumentException("Song not found.");

                var history = new ListeningHistory
                {
                    UserID = user.UserID,
                    SongId = songId,
                    PlayedAt = DateTime.UtcNow,
                    DeviceInfo = deviceInfo ?? "Unknown"
                };

                song.PlayCount = song.PlayCount + 1;
                _context.ListeningHistories.Add(history);
                await _context.SaveChangesAsync();
                Console.WriteLine($"History added: UserID={user.UserID}, SongID={songId}, Device={deviceInfo}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add listening history: {ex.Message}");
                return false;
            }
        }
    }
}
