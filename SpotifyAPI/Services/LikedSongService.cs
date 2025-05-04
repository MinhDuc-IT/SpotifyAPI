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
    public interface ILikedSongService
    {
        Task<List<SongDto>> GetLikedSongsAsync(string userIdToken);
        Task<bool> LikeSongAsync(int songId, string userIdentifier);
        Task<bool> DislikeSongAsync(int songId, string userIdentifier);
    }

    public class LikedSongService : ILikedSongService
    {
        private readonly SpotifyDbContext _context;

        public LikedSongService(SpotifyDbContext context)
        {
            _context = context;

        }

        public async Task<List<SongDto>> GetLikedSongsAsync(string userIdToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userIdToken);

            if (user == null)
                return null;
            var userId = user.UserID;

            var likedSongs = await _context.LikedSongs
                .Where(ls => ls.UserID == userId)
                .OrderByDescending(ls => ls.LikedAt)
                .Select(ls => new SongDto
                {
                    SongId = ls.SongID,
                    Title = ls.Song.SongName,
                    ArtistName = ls.Song.Artist.ArtistName,
                    Album = ls.Song.Album.AlbumName,
                    ThumbnailUrl = ls.Song.Image,
                    //Duration = ls.Song.Duration,
                    AudioUrl = ls.Song.Audio
                }).ToListAsync();

            return likedSongs;
        }

        public async Task<bool> LikeSongAsync(int songId, string userIdentifier)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userIdentifier);
            if (user == null) return false;

            var existing = await _context.LikedSongs
                .FirstOrDefaultAsync(l => l.UserID == user.UserID && l.SongID == songId);
            if (existing != null) return false;

            var liked = new LikedSong
            {
                UserID = user.UserID,
                SongID = songId,
                LikedAt = DateTime.UtcNow
            };

            _context.LikedSongs.Add(liked);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DislikeSongAsync(int songId, string userIdentifier)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userIdentifier);
            if (user == null) return false;

            var liked = await _context.LikedSongs
                .FirstOrDefaultAsync(l => l.UserID == user.UserID && l.SongID == songId);

            if (liked == null) return false;

            _context.LikedSongs.Remove(liked);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
