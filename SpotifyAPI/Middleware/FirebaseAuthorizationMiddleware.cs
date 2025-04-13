namespace SpotifyAPI.Middleware;

public class FirebaseAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public FirebaseAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        // Nếu là public endpoint => bỏ qua phân quyền
        if (EndpointAccess.PublicEndpoints.Contains(path))
        {
            await _next(context);
            return;
        }

        var user = context.User;
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Not authenticated.");
            return;
        }

        // Check phân quyền theo endpoint
        if (EndpointAccess.AdminEndpoints.Any(path.StartsWith))
        {
            if (!user.IsInRole("Admin"))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden: Admin role required.");
                return;
            }
        }
        else if (EndpointAccess.ClientEndpoints.Any(path.StartsWith))
        {
            if (!user.IsInRole("User") && !user.IsInRole("Client"))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden: User role required.");
                return;
            }
        }

        await _next(context);
    }
}
