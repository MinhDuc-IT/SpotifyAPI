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
    public interface IPlaylistService
    {
        Task<Playlist> CreatePlaylistAsync(string userIdToken, CreatePlaylistDto dto);
        Task<bool> AddSongToPlaylistAsync(string userIdToken, AddSongToPlaylistDto dto);
        Task<bool> RemoveSongFromPlaylistAsync(string userIdToken, RemoveSongFromPlaylistDto dto);
        Task<bool> DeletePlaylistAsync(string userIdToken, int playlistId);
        Task<List<PlayListDTO>> GetPlaylistsByUserIdAsync(string userId);
    }

    public class PlaylistService : IPlaylistService
    {
        private readonly SpotifyDbContext _context;

        public PlaylistService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlayListDTO>> GetPlaylistsByUserIdAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userId);
            if (user == null)
                return null;

            return await _context.Playlists
                .Where(p => p.UserID == user.UserID)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                .Select(p => new PlayListDTO
                {
                    PlaylistID = p.PlaylistID,
                    PlaylistName = p.PlaylistName,
                    CreatedDate = p.CreatedDate,
                    Description = p.Description,
                    IsPublic = p.IsPublic,
                    Songs = p.PlaylistSongs.Select(ps => new PlayListSongDTO
                    {
                        SongID = ps.Song.SongID,
                        SongName = ps.Song.SongName,
                        Audio = ps.Song.Audio,
                        Duration = ps.Song.Duration,
                        LyricUrl = ps.Song.LyricUrl,
                        Image = ps.Song.Image,
                        ArtistName = ps.Song.Artist.ArtistName
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<Playlist> CreatePlaylistAsync(string userIdToken, CreatePlaylistDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == userIdToken);
            if (user == null) return null;

            var playlist = new Playlist
            {
                PlaylistName = dto.PlaylistName,
                Description = dto.Description,
                IsPublic = dto.IsPublic,
                CreatedDate = DateTime.UtcNow,
                UserID = user.UserID
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<bool> DeletePlaylistAsync(string userIdToken, int playlistId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == userIdToken);
            if (user == null) return false;

            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(p => p.PlaylistID == playlistId && p.UserID == user.UserID);

            if (playlist == null) return false;

            // Xóa các bài hát trong playlist
            _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);

            // Xóa chính playlist
            _context.Playlists.Remove(playlist);

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> AddSongToPlaylistAsync(string userIdToken, AddSongToPlaylistDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == userIdToken);
            if (user == null) return false;

            var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.PlaylistID == dto.PlaylistID && p.UserID == user.UserID);
            if (playlist == null) return false;

            var exists = await _context.PlaylistSongs.AnyAsync(ps => ps.PlaylistID == dto.PlaylistID && ps.SongID == dto.SongID);
            if (exists) return false; // Đã tồn tại

            var playlistSong = new PlaylistSong
            {
                PlaylistID = dto.PlaylistID,
                SongID = dto.SongID,
                AddedAt = DateTime.UtcNow
            };

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSongFromPlaylistAsync(string userIdToken, RemoveSongFromPlaylistDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == userIdToken);
            if (user == null) return false;

            var playlistSong = await _context.PlaylistSongs
                .Include(ps => ps.Playlist)
                .FirstOrDefaultAsync(ps => ps.PlaylistID == dto.PlaylistID && ps.SongID == dto.SongID && ps.Playlist.UserID == user.UserID);

            if (playlistSong == null) return false;

            _context.PlaylistSongs.Remove(playlistSong);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
