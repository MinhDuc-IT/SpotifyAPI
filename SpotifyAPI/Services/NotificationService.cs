using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;

using SpotifyAPI.Utils;

namespace SpotifyAPI.Services
{
    public interface INotificationService
    {
        Task<object> GetAllNotificationsById(string uid, int page, int limit);
        Task<Notification> CreateNotification(CreateNotificationDTO request, string uid);
        Task<NotificationReceiver> CreateNotificationReceiver(CreateNotificationReceiverDTO request, string uid);
    }
    public class NotificationService : INotificationService
    {
        private readonly SpotifyDbContext _context;
        private readonly ILogger<SongService> _logger;

        public NotificationService(SpotifyDbContext context, ILogger<SongService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<object> GetAllNotificationsById(string firebaseUid, int page, int limit)
        {
            // Tìm User theo firebaseUid
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            if (user == null)
            {
                return new
                {
                    message = "User not found"
                };
            }

            var query = _context.NotificationReceivers
                .Where(nr => nr.ReceiverUserId == user.UserID)
                .Include(nr => nr.Notification)
                    .ThenInclude(n => n.Sender) // 👈 Include người gửi
                .OrderByDescending(nr => nr.Notification.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)limit);

            var notifications = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(nr => new
                {
                    SenderEmail = nr.Notification.Sender != null ? nr.Notification.Sender.Email : null,
                    nr.Notification.NotificationId,
                    nr.Notification.Title,
                    nr.Notification.Body,
                    nr.Notification.CreatedAt,
                    IsRead = nr.IsRead
                })
                .ToListAsync();

            return new
            {
                currentPage = page,
                totalPages,
                totalItems,
                items = notifications
            };
        }


        public async Task<Notification> CreateNotification(CreateNotificationDTO request, string uid)
        {
            var Artist = await _context.Users.FirstOrDefaultAsync(a => a.FirebaseUid == uid);
            var notification = new Notification
            {
                Title = request.Title,
                Body = request.Body,
                SenderUserId = Artist.UserID,
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<NotificationReceiver> CreateNotificationReceiver(CreateNotificationReceiverDTO request, string uid)
        {
            var Artist = await _context.Users.FirstOrDefaultAsync(a => a.FirebaseUid == uid);
            var notificationReceiver = new NotificationReceiver
            {
                NotificationId = request.NotificationId,
                ReceiverUserId = request.ReceiverUserId,
            };

            await _context.NotificationReceivers.AddAsync(notificationReceiver);
            await _context.SaveChangesAsync();

            return notificationReceiver;
        }
    }

}
