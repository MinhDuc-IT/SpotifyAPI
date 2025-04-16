using FirebaseAdmin.Auth;

namespace SpotifyAPI.Services
{
    public interface IFirebaseAuthService
    {
        Task<FirebaseToken> VerifyIdTokenAsync(string idToken);
        Task<UserRecord> GetUserAsync(string uid);
        Task SetCustomUserClaimsAsync(string uid, Dictionary<string, object> claims);
        Dictionary<string, object> DetermineUserRoles(FirebaseToken decodedToken);
    }

    public class FirebaseAuthService : IFirebaseAuthService
    {
        public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
        {
            return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        }

        public async Task<UserRecord> GetUserAsync(string uid)
        {
            return await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        }

        public async Task SetCustomUserClaimsAsync(string uid, Dictionary<string, object> claims)
        {
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);
        }

        public Dictionary<string, object> DetermineUserRoles(FirebaseToken decodedToken)
        {
            var claims = new Dictionary<string, object>();
            var roles = new List<string>();

            // Mặc định là User
            roles.Add("User");

            // 1. Nếu có email và đã xác minh
            if (decodedToken.Claims.TryGetValue("email", out var emailObj) &&
                emailObj is string email &&
                decodedToken.Claims.TryGetValue("email_verified", out var emailVerifiedObj) &&
                emailVerifiedObj is bool emailVerified &&
                emailVerified)
            {
                if (email.EndsWith("@admin.example.com") || email.Equals("thanh@gmail.com"))
                {
                    roles.Clear();
                    roles.Add("Admin");
                }
                else if (email.StartsWith("admin"))
                {
                    roles.Clear();
                    roles.Add("Admin");
                }
            }

            claims["roles"] = roles;
            return claims;
        }

    }
}
