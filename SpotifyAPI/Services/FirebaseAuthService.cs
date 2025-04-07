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

            if (decodedToken.Claims.TryGetValue("email", out var emailObj) && emailObj is string email)
            {
                bool isEmailVerified = decodedToken.Claims.TryGetValue("email_verified", out var emailVerifiedObj)
                                        && emailVerifiedObj is bool emailVerified
                                        && emailVerified;

                if (isEmailVerified && (email.EndsWith("@admin.example.com") || email.Equals("thanh@gmail.com")))
                {
                    roles.Add("Admin");
                }
                else if (email.StartsWith("admin"))
                {
                    roles.Add("Admin");
                }
                else
                {
                    roles.Add("User");
                }                
            }
            else
            {
                roles.Add("User");
            }

            claims["roles"] = roles;
            return claims;
        }
    }
}
