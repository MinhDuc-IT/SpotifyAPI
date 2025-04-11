namespace SpotifyAPI.Middleware
{
    public static class PublicEndpoints
    {
        public static readonly HashSet<string> Endpoints = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/login",          
            "/signup"
        };
    }
}
