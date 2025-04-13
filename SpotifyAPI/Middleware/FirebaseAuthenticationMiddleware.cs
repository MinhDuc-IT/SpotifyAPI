using FirebaseAdmin.Auth;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;

namespace SpotifyAPI.Middleware
{
    public class FirebaseAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<FirebaseAuthenticationMiddleware> _logger;

        public FirebaseAuthenticationMiddleware(
            RequestDelegate next,
            ILogger<FirebaseAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Bỏ qua xác thực cho các endpoint không yêu cầu
            if (EndpointAccess.PublicEndpoints.Contains(context.Request.Path.Value))
            {
                // Bỏ qua xác thực nếu là public endpoint
                await _next(context);
                return;
            }

            var header = context.Request.Headers[HeaderNames.Authorization].ToString();

            if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Missing or invalid Authorization header.");
                return;
            }

            var idToken = header.Substring("Bearer ".Length).Trim();

            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                var claims = BuildClaims(decodedToken);
                var identity = new ClaimsIdentity(claims, "Firebase");
                context.User = new ClaimsPrincipal(identity);
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogWarning(ex, "Firebase token verification failed.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync($"Unauthorized: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Firebase token verification.");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal Server Error during authentication.");
                return;
            }

            await _next(context);
        }

        private List<Claim> BuildClaims(FirebaseToken decodedToken)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                new Claim(ClaimTypes.Name, decodedToken.Uid)
            };

            // Thêm Roles
            if (decodedToken.Claims.TryGetValue("roles", out var rolesClaim))
            {
                switch (rolesClaim)
                {
                    case IEnumerable<object> roles:
                        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.ToString())));
                        break;
                    case string singleRole:
                        claims.Add(new Claim(ClaimTypes.Role, singleRole));
                        break;
                }
            }

            // Thêm các claim khác
            decodedToken.Claims.TryGetValue("email", out var email);
            if (email != null) claims.Add(new Claim(ClaimTypes.Email, email.ToString()));

            if (decodedToken.Claims.TryGetValue("email_verified", out var emailVerified) && emailVerified is bool isVerified)
                claims.Add(new Claim("email_verified", isVerified.ToString()));

            if (decodedToken.Claims.TryGetValue("firebase", out var firebaseObj) && firebaseObj is IDictionary<string, object> firebaseData)
                if (firebaseData.TryGetValue("sign_in_provider", out var signInProvider))
                    claims.Add(new Claim("sign_in_provider", signInProvider.ToString()));

            return claims;
        }
    }
}