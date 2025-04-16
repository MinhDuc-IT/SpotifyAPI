using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;
namespace SpotifyAPI.Services
{
    public interface ISongService
    {
        Task<object> GetAllSongsAsync(int page, int limit);
        Task<Song> CreateSongAsync(CreateSongDTO request);
        Task<Song> UpdateSongAsync(int id, UpdateSongDTO request);
        Task<bool> DeleteSongAsync(int id);
    }
    public class SongService : ISongService
    {
        private readonly SpotifyDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public SongService(SpotifyDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<object> GetAllSongsAsync(int page, int limit)
        {
            var totalItems = await _context.Songs.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)limit);

            var songs = await _context.Songs
                .OrderBy(s => s.SongID)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(s => new
                {
                    s.SongID,
                    s.SongName,
                    s.Image,
                    s.Audio,
                    s.PlayCount
                })
                .ToListAsync();

            return new
            {
                currentPage = page,
                totalPages,
                totalItems,
                items = songs
            };
        }

        public async Task<Song> CreateSongAsync(CreateSongDTO request)
        {
            var imageUrl = await _cloudinaryService.UploadImage(request.Image);
            if (imageUrl == null)
                throw new Exception("Image upload failed.");

            var audioUrl = await _cloudinaryService.UploadAudio(request.Audio);
            if (audioUrl == null)
                throw new Exception("Audio upload failed.");

            var song = new Song
            {
                SongName = request.SongName,
                Audio = audioUrl,
                Image = imageUrl,
                ArtistID = 1, // TODO: lấy từ claim sau
                PlayCount = 0,
                AlbumID = 1
            };

            await _context.Songs.AddAsync(song);
            await _context.SaveChangesAsync();

            return song;
        }

        public async Task<Song> UpdateSongAsync(int id, UpdateSongDTO request)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
                throw new Exception("Song not found.");

            if (request.Image != null)
            {
                var imageUrl = await _cloudinaryService.UploadImage(request.Image);
                if (imageUrl == null)
                    throw new Exception("Image upload failed.");
                song.Image = imageUrl;
            }

            if (request.Audio != null)
            {
                var audioUrl = await _cloudinaryService.UploadAudio(request.Audio);
                if (audioUrl == null)
                    throw new Exception("Audio upload failed.");
                song.Audio = audioUrl;
            }

            song.SongName = request.SongName;
            _context.Songs.Update(song);
            await _context.SaveChangesAsync();

            return song;
        }

        public async Task<bool> DeleteSongAsync(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
                return false;

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return true;
        }
    }   
}
