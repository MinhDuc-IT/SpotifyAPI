using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Models;

namespace SpotifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            var user = new User
            {
                UserID = 1,
                Email = "example@spotify.com",
                Password = "hashedpassword123", // Ideally, passwords should be hashed
                FullName = "John Doe",
                Avatar = null,
                DateJoined = DateTime.UtcNow,
                SubscriptionType = "Free",
                Role = "User",
                Subscriptions = new List<UserSubscription>(),
                Playlists = new List<Playlist>()
            };

            return Ok(user);
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
    }
}