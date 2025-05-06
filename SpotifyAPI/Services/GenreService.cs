using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;

namespace SpotifyAPI.Services
{
    public interface IGenreService
    {
        Task<IEnumerable<SongDto>> GetSongsByGenreIdAsync(int genreId);
        Task<IEnumerable<GenreDTO>> GetAllGenresAsync();

    }

    public class GenreService : IGenreService
    {
        private readonly SpotifyDbContext _context;

        public GenreService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SongDto>> GetSongsByGenreIdAsync(int genreId)
        {
            var songs = await _context.SongGenres
                .Include(g => g.Song)
                .ThenInclude(s => s.Artist)
                .Where(sg => sg.GenreId == genreId)
                .Select(sg => new SongDto
                {
                    SongId = sg.Song.SongID,
                    Title = sg.Song.SongName,
                    ArtistName = sg.Song.Artist.ArtistName,
                    Album = "",
                    AlbumID = -1,
                    ThumbnailUrl = sg.Song.Image,
                    Duration = sg.Song.Duration,
                    AudioUrl = sg.Song.Audio,
                })
                .ToListAsync();

            return songs;
        }
        public async Task<IEnumerable<GenreDTO>> GetAllGenresAsync()
        {
            return await _context.Genres
                .Select(g => new GenreDTO
                {
                    GenreId = g.GenreId,
                    GenreName = g.GenreName
                })
                .ToListAsync();
        }
    }
}
