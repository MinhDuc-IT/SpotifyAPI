using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.DTOs;
using SpotifyAPI.Services;
using SpotifyAPI.Utils;
using SpotifyAPI.Models;

namespace SpotifyAPI.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly IUserService _userService;

        public SearchController(ISearchService searchService, IUserService userService)
        {
            _searchService = searchService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSearchHistory(int page = 1, int limit = 10)
        {
            try
            {
                var user = HttpContext.User;
                var uid = user.FindFirst("user_id")?.Value;
                var result = await _searchService.GetAllSearchHistoryAsync(uid, page, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSearchHistory(SearchHistoryDTO request)
        {
            try
            {
                var user = HttpContext.User;
                var uid = user.FindFirst("user_id")?.Value;

                var searchHistory = await _searchService.CreateSearchHistoryAsync(request, uid);

                return Ok(new
                {
                    message = "search History created successfully",
                    searchHistory
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("one")]
        public async Task<IActionResult> DeleteSearchHistory([FromBody] DeleteSearchDTO request)
        {
            try
            {
                var user = HttpContext.User;
                var uid = user.FindFirst("user_id")?.Value;
                var result = await _searchService.DeleteSearchHistoryAsync(request, uid);

                if (result == null)
                    return NotFound("Search history not found.");

                return Ok(new
                {
                    message = "Search history deleted successfully",
                    result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllSearchHistory()
        {
            try
            {
                var user = HttpContext.User;
                var uid = user.FindFirst("user_id")?.Value;
                var result = await _searchService.DeleteAllSearchHistoryAsync(uid);

                if (result == null)
                    return NotFound("Search history not found.");

                return Ok(new
                {
                    message = "Search history deleted successfully",
                    result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
