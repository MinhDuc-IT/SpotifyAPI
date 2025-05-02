
﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Models;

using SpotifyAPI.DTOs;
using SpotifyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace SpotifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        //[HttpGet("profile")]
        //public IActionResult GetUserProfile()
        //{
        //    var user = new User
        //    {
        //        UserID = 1,
        //        Email = "example@spotify.com",
        //        Password = "hashedpassword123", // Ideally, passwords should be hashed
        //        FullName = "John Doe",
        //        Avatar = null,
        //        DateJoined = DateTime.UtcNow,
        //        SubscriptionType = "Free",
        //        Role = "User",
        //        Subscriptions = new List<UserSubscription>(),
        //        Playlists = new List<Playlist>()
        //    };

        //    return Ok(user);
        //}

        //[Authorize(Roles = "User")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // hoặc "sub" nếu bạn dùng JWT chuẩn
            if (userIdClaim == null)
                return Unauthorized();

            string userId = userIdClaim.Value;

            var userDto = await _userService.GetUserByFirebaseUidAsync(userId);
            if (userDto == null)
                return NotFound();

            return Ok(userDto);
        }

        [HttpGet("recently-played")]
        public IActionResult GetRecentlyPlayed()
        {
            var items = new List<object>();

            var trackNames = new[] { "Billie Jean", "Cruel Summer", "Blinding Lights", "Shape of You" };
            foreach (var name in trackNames)
            {
                items.Add(new
                {
                    track = new
                    {
                        name,
                        album = new
                        {
                            images = new[]
                            {
                        new { url = "https://cdn.pixabay.com/photo/2012/04/01/12/48/record-23281_1280.png" }
                    }
                        }
                    }
                });
            }

            return Ok(new { items });
        }

        [HttpGet("top-artists")]
        public IActionResult GetTopArtists()
        {
            var artists = new[]
            {
                new {
                    name = "Michael Jackson",
                    images = new[] {
                        new { url = "https://preview.redd.it/my-michael-jackson-wallpaper-collection-v0-5bhl227sj13e1.jpg?width=1080&crop=smart&auto=webp&s=892fa42f8038855313efdfcd444d365b415c4930" }
                    }
                },
                new {
                    name = "Taylor Swift",
                    images = new[] {
                        new { url = "https://www.shutterstock.com/editorial/image-editorial/M8T7Ac1cN4jdcfw1MTc4NDI=/taylor-swift-1500w-10481442fs.jpg" }
                    }
                },
                new {
                    name = "Charlie Puth",
                    images = new[] {
                        new { url = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/63/Charlie_Puth_2016.jpg/500px-Charlie_Puth_2016.jpg" }
                    }
                },
                new {
                    name = "Ed Sheeran",
                    images = new[] {
                        new { url = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/17/Ed_Sheeran_%288507720597%29.jpg/500px-Ed_Sheeran_%288507720597%29.jpg" }
                    }
                }
            };

            return Ok(new { items = artists });
        }


        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (users, totalCount) = await _userService.GetUsersAsync(pageNumber, pageSize);

            var result = new
            {
                Data = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 

            var createdUser = await _userService.CreateUserAsync(userCreateDto);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserID }, createdUser);
        }

        //[HttpPut("{id}")]
        //public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var updatedUser = await _userService.UpdateUserAsync(id, userUpdateDto);
        //    if (updatedUser == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(updatedUser);
        //}

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateUser([FromForm] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var firebaseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var updatedUser = await _userService.UpdateUserAsync(firebaseId, userUpdateDto);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("follow-info")]
        [Authorize]
        public async Task<IActionResult> GetFollowInfo([FromQuery] int? artistId)
        {
            var firebaseId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var followInfo = await _userService.GetFollowInfoAsync(firebaseId, artistId);
            return Ok(followInfo);
        }

        [HttpPost("upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
                return BadRequest("File ảnh không hợp lệ.");

            var avatarUrl = await _userService.UploadAvatar(avatar);

            return Ok(new { photoUrl = avatarUrl });
        }
    }
}

