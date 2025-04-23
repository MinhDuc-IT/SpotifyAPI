using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;
using System.Diagnostics;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IFirebaseAuthService _firebaseAuthService;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IFirebaseAuthService firebaseAuthService,
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _firebaseAuthService = firebaseAuthService;
        _userService = userService;
        _logger = logger;
    }

    //[HttpPost("setCustomClaims")]
    //public async Task<IActionResult> SetCustomClaims([FromBody] TokenRequestDTO request)
    //{
    //    try
    //    {
    //        // Xác thực token với Firebase
    //        _logger.LogInformation("Starting verification of Firebase ID Token...");
    //        var stopwatch = Stopwatch.StartNew();

    //        var decodedToken = await _firebaseAuthService.VerifyIdTokenAsync(request.IdToken);

    //        stopwatch.Stop();
    //        _logger.LogInformation($"VerifyIdTokenAsync executed in {stopwatch.ElapsedMilliseconds} ms");
    //        var existingUser = await _firebaseAuthService.GetUserAsync(decodedToken.Uid);

    //        // Kiểm tra nếu đã có roles
    //        if (existingUser.CustomClaims.TryGetValue("roles", out var existingRoles))
    //        {
    //            return Ok(new
    //            {
    //                status = "success",
    //                message = "User already has roles",
    //                roles = existingRoles
    //            });
    //        }

    //        // Xác định roles mới
    //        var newClaims = _firebaseAuthService.DetermineUserRoles(decodedToken);
    //        await _firebaseAuthService.SetCustomUserClaimsAsync(decodedToken.Uid, newClaims);

    //        // Lấy thông tin từ token
    //        var email = decodedToken.Claims["email"].ToString();
    //        var firebaseName = decodedToken.Claims.TryGetValue("name", out var nameObj)
    //                            ? nameObj.ToString()
    //                            : email.Split('@')[0];
    //        var role = ((List<string>)newClaims["roles"]).First();

    //        // Đồng bộ user vào database
    //        var appUser = await _userService.GetOrCreateUserAsync(
    //            email,
    //            firebaseName,
    //            decodedToken.Uid,
    //            role
    //        );

    //        return Ok(new
    //        {
    //            status = "success",
    //            message = "Claims updated",
    //            roles = newClaims["roles"]
    //        });
    //    }
    //    catch (FirebaseAuthException ex)
    //    {
    //        return BadRequest(new { status = "error", message = ex.Message });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { status = "error", message = ex.Message });
    //    }
    //}

    [HttpPost("setCustomClaims")]
    public async Task<IActionResult> SetCustomClaims([FromBody] TokenRequestDTO request)
    {
        try
        {
            var totalStopwatch = Stopwatch.StartNew();
            _logger.LogInformation("🚀 [SetCustomClaims] Started processing request...");

            // 1. Verify ID Token
            _logger.LogInformation("🔐 Verifying Firebase ID Token...");
            var swVerify = Stopwatch.StartNew();
            var decodedToken = await _firebaseAuthService.VerifyIdTokenAsync(request.IdToken);
            swVerify.Stop();
            _logger.LogInformation($"✅ VerifyIdTokenAsync took {swVerify.ElapsedMilliseconds} ms");

            // 2. Get User Info
            _logger.LogInformation("👤 Fetching Firebase user info...");
            var swUser = Stopwatch.StartNew();
            var existingUser = await _firebaseAuthService.GetUserAsync(decodedToken.Uid);
            swUser.Stop();
            _logger.LogInformation($"✅ GetUserAsync took {swUser.ElapsedMilliseconds} ms");

            // 3. Check for existing roles
            if (existingUser.CustomClaims.TryGetValue("roles", out var existingRoles))
            {
                _logger.LogInformation("📌 User already has roles: {Roles}", existingRoles);
                totalStopwatch.Stop();
                _logger.LogInformation($"⏱️ Total execution time: {totalStopwatch.ElapsedMilliseconds} ms");

                return Ok(new
                {
                    status = "success",
                    message = "User already has roles",
                    roles = existingRoles
                });
            }

            // 4. Determine and set new roles
            _logger.LogInformation("🧠 Determining new roles...");
            var swClaims = Stopwatch.StartNew();
            var origin = Request.Headers["Origin"];
            _logger.LogInformation("Origin: {Origin}", origin);
            var newClaims = _firebaseAuthService.DetermineUserRoles(decodedToken, origin);
            await _firebaseAuthService.SetCustomUserClaimsAsync(decodedToken.Uid, newClaims);
            swClaims.Stop();
            _logger.LogInformation($"✅ Role determination + SetCustomUserClaimsAsync took {swClaims.ElapsedMilliseconds} ms");
            // 5. Extract info from token
            _logger.LogInformation("📥 Extracting email and name from token...");
            var swExtract = Stopwatch.StartNew();
            var email = decodedToken.Claims.TryGetValue("email", out var emailObj)
             ? emailObj.ToString()
             : $"{decodedToken.Uid}@noemail.firebase";
            var firebaseName = decodedToken.Claims.TryGetValue("name", out var nameObj)
                                ? nameObj.ToString()
                                : email.Split('@')[0];
            var role = ((List<string>)newClaims["roles"]).First();
            var provider = existingUser.ProviderData.FirstOrDefault()?.ProviderId;
            _logger.LogInformation("Provider: {Provider}", provider);
            var photoUrl = existingUser.PhotoUrl;
            _logger.LogInformation("Photo URL: {PhotoUrl}", photoUrl);
            swExtract.Stop();
            _logger.LogInformation($"✅ Extracting info from token took {swExtract.ElapsedMilliseconds} ms");

            // 6. Sync user to local DB
            _logger.LogInformation("🗃️ Syncing user to local database...");
            var swSync = Stopwatch.StartNew();

            var appUser = await _userService.GetOrCreateUserAsync(
                email,
                firebaseName,
                decodedToken.Uid,
                role, 
                provider,
                photoUrl
            );
            swSync.Stop();
            _logger.LogInformation($"✅ GetOrCreateUserAsync took {swSync.ElapsedMilliseconds} ms");

            totalStopwatch.Stop();
            _logger.LogInformation($"🏁 [SetCustomClaims] Completed in {totalStopwatch.ElapsedMilliseconds} ms");

            return Ok(new
            {
                status = "success",
                message = "Claims updated",
                roles = newClaims["roles"]
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError("❌ FirebaseAuthException: {Message}", ex.Message);
            return BadRequest(new { status = "error", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected exception occurred.");
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}