// File: Hubs/NotificationHub.cs
using Microsoft.AspNetCore.SignalR;

namespace SpotifyAPI.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; 
            var connectionId = Context.ConnectionId;

            Console.WriteLine($"Connected User: {userId} with ConnectionId: {connectionId}");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}

