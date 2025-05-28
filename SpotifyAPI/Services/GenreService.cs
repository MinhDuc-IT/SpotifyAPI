using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;
using System;

namespace SpotifyAPI.Services
{
    public interface IGenreService
    {
        Task<List<GenreDto>> GetAllGenresAsync();
        Task<GenreDto?> GetGenreByIdAsync(int id);
        Task<GenreDto> CreateGenreAsync(CreateGenreDto dto);
        Task<bool> UpdateGenreAsync(int id, UpdateGenreDto dto);
        Task<bool> DeleteGenreAsync(int id);
    }

    public class GenreService : IGenreService
    {
        private readonly SpotifyDbContext _context;

        public GenreService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<List<GenreDto>> GetAllGenresAsync()
        {
            return await _context.Genres
                .Select(g => new GenreDto
                {
                    GenreId = g.GenreId,
                    GenreName = g.GenreName,
                    Image = g.Image,
                    Songs = null
                }).ToListAsync();
        }

        public async Task<GenreDto?> GetGenreByIdAsync(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.SongGenres)
                    .ThenInclude(sg => sg.Song)
                        .ThenInclude(s => s.Artist)
                .Include(g => g.SongGenres)
                    .ThenInclude(sg => sg.Song)
                        .ThenInclude(s => s.Album)
                .FirstOrDefaultAsync(g => g.GenreId == id);

            if (genre == null) return null;

            return new GenreDto
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName,
                Image = genre.Image,
                Songs = genre.SongGenres.Select(sg => new SongDto
                {
                    SongId = sg.Song.SongID,
                    Title = sg.Song.SongName,
                    ArtistName = sg.Song.Artist.ArtistName,
                    Album = sg.Song.Album.AlbumName,
                    AlbumID = sg.Song.AlbumID,
                    ThumbnailUrl = sg.Song.Image,
                    Duration = sg.Song.Duration,
                    AudioUrl = sg.Song.Audio
                }).ToList()
            };
        }

        public async Task<GenreDto> CreateGenreAsync(CreateGenreDto dto)
        {
            var exists = await _context.Genres
                .AnyAsync(g => g.GenreName.ToLower() == dto.GenreName.ToLower());

            if (exists)
            {
                throw new Exception($"Genre '{dto.GenreName}' đã tồn tại rồi.");
                // Hoặc bạn có thể return null hoặc trả về một kết quả báo lỗi tùy cách bạn handle exception
            }

            var genre = new Genre
            {
                GenreName = dto.GenreName
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return new GenreDto
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName
            };
        }

        public async Task<bool> UpdateGenreAsync(int id, UpdateGenreDto dto)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return false;

            genre.GenreName = dto.GenreName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return false;

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
