using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IFirebaseAuthService _firebaseAuthService;
    private readonly IUserService _userService;

    public AuthController(
        IFirebaseAuthService firebaseAuthService,
        IUserService userService)
    {
        _firebaseAuthService = firebaseAuthService;
        _userService = userService;
    }

    [HttpPost("setCustomClaims")]
    public async Task<IActionResult> SetCustomClaims([FromBody] TokenRequestDTO request)
    {
        try
        {
            // Xác thực token với Firebase
            var decodedToken = await _firebaseAuthService.VerifyIdTokenAsync(request.IdToken);
            var existingUser = await _firebaseAuthService.GetUserAsync(decodedToken.Uid);

            // Kiểm tra nếu đã có roles
            if (existingUser.CustomClaims.TryGetValue("roles", out var existingRoles))
            {
                return Ok(new
                {
                    status = "success",
                    message = "User already has roles",
                    roles = existingRoles
                });
            }

            // Xác định roles mới
            var newClaims = _firebaseAuthService.DetermineUserRoles(decodedToken);
            await _firebaseAuthService.SetCustomUserClaimsAsync(decodedToken.Uid, newClaims);

            // Lấy thông tin từ token
            var email = decodedToken.Claims["email"].ToString();
            var firebaseName = decodedToken.Claims.TryGetValue("name", out var nameObj)
                                ? nameObj.ToString()
                                : email.Split('@')[0];
            var role = ((List<string>)newClaims["roles"]).First();

            // Đồng bộ user vào database
            var appUser = await _userService.GetOrCreateUserAsync(
                email,
                firebaseName,
                decodedToken.Uid,
                role
            );

            return Ok(new
            {
                status = "success",
                message = "Claims updated",
                roles = newClaims["roles"]
            });
        }
        catch (FirebaseAuthException ex)
        {
            return BadRequest(new { status = "error", message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}