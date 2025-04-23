using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;
using SpotifyAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using SpotifyAPI.Utils;
using SpotifyAPI.Models;

namespace SpotifyAPI.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public NotificationController(IUserService userService, INotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotificationsById(int page = 1, int limit = 10)
        {
            var user = HttpContext.User;
            var uid = user.FindFirst("user_id")?.Value;
            var result = await _notificationService.GetAllNotificationsById(uid, page, limit);
            return Ok(result);
        }
    }
}
