using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;
using System;

namespace SpotifyAPI.Services
{
    public interface ISongGenreService
    {
        Task<IEnumerable<SongGenreDetailDto>> GetAllAsync();
        Task<SongGenreDto> GetByIdAsync(int songId, int genreId);
        Task AddAsync(SongGenreDto dto);
        Task DeleteAsync(int songId, int genreId);
    }

    public class SongGenreService : ISongGenreService
    {
        private readonly SpotifyDbContext _context;

        public SongGenreService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SongGenreDetailDto>> GetAllAsync()
        {
            return await _context.SongGenres
                .Include(sg => sg.Song)
                .Include(sg => sg.Genre)
                .Select(sg => new SongGenreDetailDto
                {
                    SongId = sg.SongId,
                    SongTitle = sg.Song.SongName,
                    GenreId = sg.GenreId,
                    GenreName = sg.Genre.GenreName
                })
                .ToListAsync();
        }

        public async Task<SongGenreDto> GetByIdAsync(int songId, int genreId)
        {
            var sg = await _context.SongGenres.FindAsync(songId, genreId);
            if (sg == null) return null;

            return new SongGenreDto
            {
                SongId = sg.SongId,
                GenreId = sg.GenreId
            };
        }

        public async Task AddAsync(SongGenreDto dto)
        {
            // Kiểm tra songId và genreId có tồn tại không
            var song = await _context.Songs.FindAsync(dto.SongId);
            var genre = await _context.Genres.FindAsync(dto.GenreId);
            if (song == null || genre == null)
            {
                throw new Exception("Bài hát hoặc thể loại không tồn tại.");
            }

            // Kiểm tra trùng
            var exists = await _context.SongGenres.AnyAsync(sg => sg.SongId == dto.SongId && sg.GenreId == dto.GenreId);
            if (exists)
            {
                throw new Exception("Bài hát đã thuộc thể loại này.");
            }

            var songGenre = new SongGenre
            {
                SongId = dto.SongId,
                GenreId = dto.GenreId
            };
            _context.SongGenres.Add(songGenre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int songId, int genreId)
        {
            var sg = await _context.SongGenres.FindAsync(songId, genreId);
            if (sg == null) return;
            _context.SongGenres.Remove(sg);
            await _context.SaveChangesAsync();
        }
    }

}
