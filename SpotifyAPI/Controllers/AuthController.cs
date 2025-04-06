using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SpotifyAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SpotifyDbContext _context;

        public AuthController(SpotifyDbContext context)
        {
            _context = context;
        }

        [HttpPost("setCustomClaims")]
        public async Task<IActionResult> SetCustomClaims([FromBody] TokenRequestDTO request)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
                string uid = decodedToken.Uid;

                var existingUser = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

                if (existingUser.CustomClaims.TryGetValue("roles", out object existingRolesObj))
                {
                    if (existingRolesObj is IEnumerable<object> rolesList)
                    {
                        var roles = rolesList.Select(r => r.ToString()).ToList();
                        return Ok(new { status = "success", message = "User already has assigned roles", roles });
                    }
                }

                var newClaims = new Dictionary<string, object>();

                // Kiểm tra điều kiện đặt vai trò
                if (decodedToken.Claims.TryGetValue("email", out object emailObj) &&
                    emailObj is string email)
                {
                    if (decodedToken.Claims.TryGetValue("email_verified", out object emailVerifiedObj) &&
                        emailVerifiedObj is bool verified && verified &&
                        email.EndsWith("@admin.example.com", StringComparison.OrdinalIgnoreCase))
                    {
                        newClaims["roles"] = new List<string> { "Admin" };
                    }
                    else if (email.Equals("thanh@gmail.com", StringComparison.OrdinalIgnoreCase))
                    {
                        newClaims["roles"] = new List<string> { "Admin" };
                    }
                    else
                    {
                        newClaims["roles"] = new List<string> { "User" };
                    }
                }
                else
                {
                    newClaims["roles"] = new List<string> { "User" };
                }

                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, newClaims);

                var user = await GetOrCreateUser(newClaims);

                return Ok(new { status = "success", message = "Claims updated successfully", roles = newClaims["roles"] });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"Invalid token: {ex.Message}" });
            }
        }
 

        private async Task<User> GetOrCreateUser(Dictionary<string, object> newClaims)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var firebaseName = User.FindFirstValue(ClaimTypes.Name);
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var role = "User";
            if (newClaims.TryGetValue("roles", out object rolesObj) && rolesObj is List<string> rolesList && rolesList.Count > 0)
            {
                role = rolesList[0];
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FullName = firebaseName ?? email.Split('@')[0],
                    Password = "FIREBASE_AUTH",
                    Role = role,
                    Avatar = "default-avatar.png",
                    SubscriptionType = "Free"
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    // Cập nhật thông tin mới nhất từ Firebase
            //    user.LastLogin = DateTime.UtcNow;
            //    user.DisplayName = firebaseName ?? user.DisplayName;
            //    await _context.SaveChangesAsync();
            //}

            return user;
        }
    }
}