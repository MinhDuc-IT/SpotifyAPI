using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;

namespace SpotifyAPI.Services
{
    public interface IArtistInfoService
    {
        Task<ArtistInfoDTO?> GetArtistInfo(string artistName, string? firebaseUid);
        Task<List<ArtistWithSongsDto>> GetArtistsWithSongs(string artistName);
    }

    public class ArtistInfoService : IArtistInfoService
    {
        private readonly SpotifyDbContext _context;

        public ArtistInfoService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<ArtistInfoDTO?> GetArtistInfo(string artistName, string? firebaseUid)
        {
            // Chỉ lấy phần trước dấu ',' nếu có
            var cleanName = artistName.Split(',')[0].Trim();

            var artist = await _context.Artists
                .FirstOrDefaultAsync(a => a.ArtistName == cleanName);

            if (artist == null) return null;

            bool isFollowed = false;

            if (!string.IsNullOrEmpty(firebaseUid))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
                if (user != null)
                {
                    isFollowed = await _context.ArtistFollows
                        .AnyAsync(f => f.UserID == user.UserID && f.ArtistId == artist.ArtistID);
                }
            }

            return new ArtistInfoDTO
            {
                ArtistID = artist.ArtistID,
                ArtistName = artist.ArtistName,
                Bio = artist.Bio,
                Image = artist.Image,
                FormedDate = artist.FormedDate,
                IsFollowed = isFollowed,
            };
        }

        //public async Task<ArtistWithSongsDto?> GetArtistWithSongs(string artistName)
        //{
        //    var artist = await _context.Artists
        //        .Include(a => a.Songs)
        //        .FirstOrDefaultAsync(a => a.ArtistName == artistName);

        //    if (artist == null) return null;

        //    return new ArtistWithSongsDto
        //    {
        //        ArtistID = artist.ArtistID,
        //        ArtistName = artist.ArtistName,
        //        Image = artist.Image,
        //        Songs = artist.Songs.Select(song => new SongDto
        //        {
        //            SongId = song.SongID,
        //            Title = song.SongName,
        //            ArtistName = artist.ArtistName, // Lấy từ artist luôn
        //            Album = song.Album?.AlbumName,  // Nếu bài hát có album
        //            AlbumID = song.AlbumID,
        //            ThumbnailUrl = song.Image,
        //            Duration = song.Duration,
        //            AudioUrl = song.Audio
        //        }).ToList()
        //    };
        //}

        public async Task<List<ArtistWithSongsDto>> GetArtistsWithSongs(string artistNames)
        {
            // Cắt chuỗi thành danh sách tên nghệ sĩ
            var artistNameList = artistNames
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(name => name.Trim())
                .ToList();

            var artists = await _context.Artists
                .Where(a => artistNameList.Contains(a.ArtistName))
                .Include(a => a.Songs)
                .ToListAsync();

            // Nếu không tìm thấy ai hết
            if (artists == null || artists.Count == 0) return new List<ArtistWithSongsDto>();

            // Chuyển dữ liệu thành list DTO
            var result = artists.Select(artist => new ArtistWithSongsDto
            {
                ArtistID = artist.ArtistID,
                ArtistName = artist.ArtistName,
                Image = artist.Image,
                Songs = artist.Songs.Select(song => new SongDto
                {
                    SongId = song.SongID,
                    Title = song.SongName,
                    ArtistName = artist.ArtistName,
                    Album = song.Album?.AlbumName,
                    AlbumID = song.AlbumID,
                    ThumbnailUrl = song.Image,
                    Duration = song.Duration,
                    AudioUrl = song.Audio
                }).ToList()
            }).ToList();

            return result;
        }
    }
}
