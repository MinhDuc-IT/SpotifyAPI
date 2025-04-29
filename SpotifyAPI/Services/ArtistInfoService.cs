using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;

namespace SpotifyAPI.Services
{
    public interface IArtistInfoService
    {
        Task<ArtistInfoDTO?> GetArtistInfo(string artistName);
        Task<ArtistWithSongsDto?> GetArtistWithSongs(string artistName);
    }

    public class ArtistInfoService : IArtistInfoService
    {
        private readonly SpotifyDbContext _context;

        public ArtistInfoService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<ArtistInfoDTO?> GetArtistInfo(string artistName)
        {
            var artist = await _context.Artists
                .FirstOrDefaultAsync(a => a.ArtistName == artistName);

            if (artist == null) return null;

            return new ArtistInfoDTO
            {
                ArtistID = artist.ArtistID,
                ArtistName = artist.ArtistName,
                Bio = artist.Bio,
                Image = artist.Image,
                FormedDate = artist.FormedDate,
            };
        }

        public async Task<ArtistWithSongsDto?> GetArtistWithSongs(string artistName)
        {
            var artist = await _context.Artists
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.ArtistName == artistName);

            if (artist == null) return null;

            return new ArtistWithSongsDto
            {
                ArtistID = artist.ArtistID,
                ArtistName = artist.ArtistName,
                Image = artist.Image,
                Songs = artist.Songs.Select(song => new SongDto
                {
                    SongId = song.SongID,
                    Title = song.SongName,
                    ArtistName = artist.ArtistName, // Lấy từ artist luôn
                    Album = song.Album?.AlbumName,  // Nếu bài hát có album
                    AlbumID = song.AlbumID,
                    ThumbnailUrl = song.Image,
                    Duration = song.Duration,
                    AudioUrl = song.Audio
                }).ToList()
            };
        }
    }
}
