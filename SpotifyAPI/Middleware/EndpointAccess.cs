namespace SpotifyAPI.Middleware
{
    public static class EndpointAccess
    {
        public static readonly HashSet<string> PublicEndpoints = new()
        {
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/setCustomClaims",
            "/swagger/index.html",
            "/swagger/v1/swagger.json"
        };

        public static readonly HashSet<string> ClientEndpoints = new()
        {
            "/api/home",
            "/api/orders",
            "/api/profile",
            "/api/playlist"
        };

        public static readonly HashSet<string> AdminEndpoints = new()
        {
            "/api/users",
            "/api/analytics",
            "/api/admin"
        };
    }

}
