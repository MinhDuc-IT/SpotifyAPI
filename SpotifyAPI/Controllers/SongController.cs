using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;
using SpotifyAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using SpotifyAPI.Utils;
using SpotifyAPI.Models;

namespace SpotifyAPI.Controllers
{
    [Route("api/song")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public SongController(ISongService songService, IHubContext<NotificationHub> hubContext, IUserService userService, INotificationService notificationService)
        {
            _songService = songService;
            _hubContext = hubContext;
            _userService = userService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSong(int page = 1, int limit = 10)
        {
            var user = HttpContext.User;
            var uid = user.FindFirst("user_id")?.Value;
            var result = await _songService.GetAllSongsAsync(page, limit, uid);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSong([FromForm] CreateSongDTO request)
        {
            if (request.Image == null || request.Audio == null)
                return BadRequest("Image or Audio file is required.");
            try
            {
                var user = HttpContext.User;
                var uid = user.FindFirst("user_id")?.Value;

                var song = await _songService.CreateSongAsync(request, uid);

                var admins = await _userService.GetAllAdminsAsync();
                if (admins.Count == 0)
                    return StatusCode(500, "Không có admin để duyệt bài hát.");

                // 🔁 Lấy admin theo round-robin
                var admin = AdminRoundRobinManager.GetNextAdmin(admins);
                var assignedAdminUid = admin.FirebaseUid;
                int UserId = admin.UserID;

                Console.WriteLine($"Assigned Admin UID: {assignedAdminUid}");

                // 🛠️ Gửi cho admin được chọn
                Console.WriteLine($"Sending notification to admin: {assignedAdminUid}");
                // Lưu dữ liệu cho thông báo xuống db
                var notification = await _notificationService.CreateNotification(new CreateNotificationDTO
                {
                    Title = "Yêu cầu duyệt bài hát mới",
                    Body = $"vừa tạo một bài hát cần duyệt",
                }, uid);
                await _notificationService.CreateNotificationReceiver(new CreateNotificationReceiverDTO
                {
                    NotificationId = notification.NotificationId,
                    ReceiverUserId = UserId,
                }, uid);
                await _hubContext.Clients.User(assignedAdminUid).SendAsync("ReceiveNotification", new
                {
                    title = "Yêu cầu duyệt bài hát mới",
                    body = $"Artist vừa tạo một bài hát cần duyệt",
                    songName = request.SongName
                });
                Console.WriteLine($"Notification sent to admin: {assignedAdminUid}");

                return Ok(new
                {
                    message = "Song created successfully",
                    assignedTo = assignedAdminUid,
                    song
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromForm] UpdateSongDTO request)
        {
            try
            {
                var song = await _songService.UpdateSongAsync(id, request);
                return Ok(song);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Song not found.")
                    return NotFound(ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var success = await _songService.DeleteSongAsync(id);
            if (!success)
                return NotFound("Song not found.");
            return Ok("Song deleted successfully.");
        }


        [HttpGet("{songId}/lyric")]
        public async Task<IActionResult> GetLyricAsync(int songId)
        {
            try
            {
                var lyrics = await _songService.GetLyric(songId);
                return Ok(lyrics); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{songId}/lyric")]
        public async Task<IActionResult> CreateLyricAsync(int songId, [FromForm] CreateLyricDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var song = await _songService.CreateLyricAsync(songId, request);
                return Ok(song);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{songId}/stream")]
        public async Task<IActionResult> StreamSong(int songId)
        {
            var result = await _songService.StreamSongAsync(songId);
            if (result == null)
                return NotFound("Không tìm thấy bài hát hoặc lỗi khi stream.");

            return result;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchSongsByName([FromQuery] string keyword, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required.");
            }

            const int limit = 10;
            var result = await _songService.SearchSongsByNameAsync(keyword, page, limit);
            return Ok(result);
        }

    }
}
