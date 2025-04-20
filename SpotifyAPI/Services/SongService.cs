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
    public interface ISongService
    {
        Task<object> GetAllSongsAsync(int page, int limit);
        Task<Song> CreateSongAsync(CreateSongDTO request);
        Task<Song> UpdateSongAsync(int id, UpdateSongDTO request);
        Task<bool> DeleteSongAsync(int id);

        Task<List<LyricResponseDTO>> GetLyric(int songId);
        //List<LyricResponseDTO> ParseLyricLines(string[] lines);
        Task<Song> CreateLyricAsync(int songId, CreateLyricDTO lyricDTO);
        Task<FileCallbackResult?> StreamSongAsync(int songId);

    }
    public class SongService : ISongService
    {
        private readonly SpotifyDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        private readonly ILogger<SongService> _logger;
        private readonly HttpClient _httpClient;

        public SongService(SpotifyDbContext context, CloudinaryService cloudinaryService, ILogger<SongService> logger, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();

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


        public async Task<Song> CreateLyricAsync(int songId, CreateLyricDTO lyricDTO)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.SongID == songId);
            if (song == null)
                throw new Exception("Bài hát không tồn tại.");

            var file = lyricDTO.Lyric;

            // Kiểm tra định dạng file phải là .lrc
            var fileExtension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(fileExtension) || !fileExtension.Equals(".lrc", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Chỉ chấp nhận file lời bài hát định dạng .lrc.");
            }

            var lyricUrl = await _cloudinaryService.UploadLyric(file);
            if (lyricUrl == null)
                throw new Exception("Không thể upload lời bài hát.");

            song.LyricUrl = lyricUrl;
            _context.Songs.Update(song);
            await _context.SaveChangesAsync();

            return song;
        }

        public async Task<List<LyricResponseDTO>> GetLyric(int songId)
        {
            var song = await _context.Songs.FindAsync(songId);
            if (song == null || string.IsNullOrEmpty(song.LyricUrl))
                throw new Exception("Lyrics not found");

            //using var httpClient = new HttpClient();
            //var content = await httpClient.GetStringAsync(song.LyricUrl);

            var content = await _httpClient.GetStringAsync(song.LyricUrl);

            // Split nội dung thành từng dòng
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var lyrics = LyricParserUtils.ParseLyricLines(lines);
            return lyrics;
        }

        public async Task<FileCallbackResult?> StreamSongAsync(int songId)
        {
            _logger.LogInformation("Streaming request received for song ID: {SongId}", songId);

            var song = await _context.Songs.FindAsync(songId);
            if (song == null || string.IsNullOrEmpty(song.Audio))
                return null;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await _httpClient.GetAsync(song.Audio, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Không thể tải nhạc. Status code: {StatusCode}", response.StatusCode);
                    return null;
                }

                var sourceStream = await response.Content.ReadAsStreamAsync();

                return new FileCallbackResult("audio/mpeg", async outputStream =>
                {
                    var buffer = new byte[8192];
                    int bytesRead;
                    long totalBytes = 0;
                    int count = 0;

                    while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytes += bytesRead;
                        count++;
                        _logger.LogInformation("{count} Streamed {BytesRead} bytes...", count, bytesRead);
                    }

                    stopwatch.Stop();
                    _logger.LogInformation("Stream hoàn tất: {TotalBytes} bytes, {Elapsed} ms", totalBytes, stopwatch.ElapsedMilliseconds);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi stream bài hát.");
                return null;
            }
        }

        //public List<LyricResponseDTO> ParseLyricLines(string[] lines)
        //{
        //    var lyrics = new List<LyricResponseDTO>();

        //    foreach (var line in lines)
        //    {
        //        // Parse theo định dạng LRC: [mm:ss.ff] lyric content
        //        var match = Regex.Match(line, @"\[(\d+):(\d+\.\d+)\](.*)");
        //        if (match.Success)
        //        {
        //            var minutes = int.Parse(match.Groups[1].Value);
        //            var seconds = double.Parse(match.Groups[2].Value);
        //            var text = match.Groups[3].Value.Trim();

        //            lyrics.Add(new LyricResponseDTO
        //            {
        //                StartTime = TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds),
        //                Text = text
        //            });
        //        }
        //    }

        //    return lyrics.OrderBy(l => l.StartTime).ToList();
        //}
    }

}
