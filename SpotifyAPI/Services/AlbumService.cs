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
        Task<List<AlbumDto>> GetAlbumsFromLikedSongsAsync(string userIdToken);

    }

    public class AlbumService : IAlbumService
    {
        private readonly SpotifyDbContext _context;

        public AlbumService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<List<AlbumDto>> GetAlbumsFromLikedSongsAsync(string userIdToken)
        {
            var user = await _context.Users
                .Include(u => u.LikedSongs)
                    .ThenInclude(ls => ls.Song)
                        .ThenInclude(s => s.Album)
                            .ThenInclude(a => a.Artist)
                .FirstOrDefaultAsync(u => u.FirebaseUid == userIdToken);


            if (user == null) return null;

            var albums = user.LikedSongs
                .Where(ls => ls.Song.Album != null)
                .Select(ls => ls.Song.Album)
                .Distinct()
                .Select(album => new AlbumDto
                {
                    AlbumId = album.AlbumID,
                    AlbumName = album.AlbumName,
                    ArtistName = album.Artist.ArtistName,
                    ThumbnailUrl = album.Image
                })
                .ToList();

            return albums;
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
