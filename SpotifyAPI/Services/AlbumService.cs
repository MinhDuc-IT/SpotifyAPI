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
    public interface IAlbumService
    {
        Task<List<SongDto>> GetSongsByAlbumIdAsync(int albumId);
    }

    public class AlbumService : IAlbumService
    {
        private readonly SpotifyDbContext _context;

        public AlbumService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<List<SongDto>> GetSongsByAlbumIdAsync(int albumId)
        {
            var songs = await _context.Songs
                .Where(s => s.Album.AlbumID == albumId)
                .Select(s => new SongDto
                {
                    SongId = s.SongID,
                    Title = s.SongName,
                    ArtistName = s.Artist.ArtistName,
                    Album = s.Album.AlbumName,
                    AlbumID = s.Album.AlbumID,
                    ThumbnailUrl = s.Image,
                    //Duration = s.Duration,
                    AudioUrl = s.Audio
                })
                .ToListAsync();

            return songs;
        }
    }
}
