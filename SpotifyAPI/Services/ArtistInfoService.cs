using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;

namespace SpotifyAPI.Services
{
    public interface IArtistInfoService
    {
        Task<ArtistInfoDTO?> GetArtistInfo(string artistName, string? firebaseUid);
        Task<List<ArtistInfoDTO>> GetFollowedArtistsByUserAsync(string firebaseUid);
        Task<ArtistInfoDTO?> GetArtistInfoByArtistId(int artistId, string? firebaseUid);
        Task<List<ArtistWithSongsDto>> GetArtistsWithSongs(string artistName);
        Task<List<SongDto>> GetSongWithArtist(int artistId);
        Task<ArtistStatsDTO> GetArtistStatsByIdAsync(int artistId);
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
        public async Task<ArtistInfoDTO?> GetArtistInfoByArtistId(int artistId, string? firebaseUid)
        {
            var artist = await _context.Artists
                .FirstOrDefaultAsync(a => a.ArtistID == artistId);

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

        public async Task<List<ArtistInfoDTO>> GetFollowedArtistsByUserAsync(string firebaseUid)
        {
            var user = await _context.Users
                .Include(u => u.ArtistFollows)
                    .ThenInclude(af => af.Artist)
                .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            if (user == null)
            {
                return new List<ArtistInfoDTO>();
            }

            var followedArtists = user.ArtistFollows
                .Select(af => new ArtistInfoDTO
                {
                    ArtistID = af.Artist.ArtistID,
                    ArtistName = af.Artist.ArtistName,
                    Bio = af.Artist.Bio,
                    Image = af.Artist.Image,
                    FormedDate = af.Artist.FormedDate,
                    IsFollowed = true
                })
                .ToList();

            return followedArtists;
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

        public async Task<List<SongDto>> GetSongWithArtist(int artistId)
        {
            var artist = await _context.Artists
                .Include(a => a.Songs)
                    .ThenInclude(s => s.Album)
                .FirstOrDefaultAsync(a => a.ArtistID == artistId);

            if (artist == null)
                return new List<SongDto>();

            var sortedSongs = artist.Songs
                .OrderByDescending(s => s.PlayCount)
                .Select(s => new SongDto
                {
                    SongId = s.SongID,
                    Title = s.SongName,
                    ArtistName = artist.ArtistName,
                    Album = s.Album?.AlbumName,
                    AlbumID = s.AlbumID,
                    ThumbnailUrl = s.Image,
                    Duration = s.Duration,
                    AudioUrl = s.Audio,
                })
                .ToList();

            return sortedSongs;
        }

        public async Task<ArtistStatsDTO> GetArtistStatsByIdAsync(int artistId)
        {
            var artist = await _context.Artists
            .Include(a => a.Songs)
            .Include(a => a.Followers)
            .FirstOrDefaultAsync(a => a.ArtistID == artistId);

            if (artist == null)
            {
                return null;
            }

            int totalPlays = artist.Songs.Sum(song => song.PlayCount);
            return new ArtistStatsDTO
            {
                FollowersCount = artist.Followers.Count,
                TotalPlays = totalPlays
            };
        }

    }
}
