using SpotifyAPI.Models;

namespace SpotifyAPI.Utils
{
    public static class AdminRoundRobinManager
    {
        private static int _currentIndex = 0;
        private static object _lock = new object();

        public static User GetNextAdmin(List<User> admins)
        {
            lock (_lock)
            {
                if (admins.Count == 0) return null;

                var admin = admins[_currentIndex % admins.Count];
                _currentIndex++;
                return admin;
            }
        }
    }
}
