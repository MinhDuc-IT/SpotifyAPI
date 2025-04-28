using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;

using SpotifyAPI.Utils;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SpotifyAPI.Services
{
    public interface ISearchService
    {
        Task<CreateSearchHistoryDTO> CreateSearchHistoryAsync(SearchHistoryDTO request, string uid);
        Task<List<SearchHistoryDTO>> GetAllSearchHistoryAsync(string uid, int page = 1, int limit = 10);
        Task<object> DeleteSearchHistoryAsync(DeleteSearchDTO request, string uid);
        Task<object> DeleteAllSearchHistoryAsync(string uid);
    }

    public class SearchService : ISearchService
    {
        private readonly SpotifyDbContext _context;
        public SearchService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<List<SearchHistoryDTO>> GetAllSearchHistoryAsync(string uid, int page, int limit)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);
            if (user == null)
                return null;

            var query = _context.SearchHistories
                        .Where(sh => sh.UserID == user.UserID);

            //var totalCount = await query.CountAsync();
            //var totalPages = (int)Math.Ceiling(totalCount / (double)limit);

            var searchHistories = await query
                .OrderByDescending(sh => sh.SearchedAt)
                //.Skip((page - 1) * limit)
                .Take(10)
                .Select(sh => new SearchHistoryDTO
                {
                    Id = sh.ResultID,
                    Audio = sh.AudioURL,
                    Image = sh.ImageURL,
                    Name = sh.Name,
                    Type = sh.Type
                })
                .ToListAsync();

            return searchHistories;
        }

        public async Task<CreateSearchHistoryDTO> CreateSearchHistoryAsync(SearchHistoryDTO searchHistoryDTO, string uid)
        {
            var existingSearchHistory = await _context.SearchHistories
                .FirstOrDefaultAsync(sh => sh.ResultID == searchHistoryDTO.Id && sh.Type == searchHistoryDTO.Type);
            if (existingSearchHistory != null) return null;

            var User = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);

            var searchHistory = new SearchHistory
            {
                ResultID = searchHistoryDTO.Id,
                AudioURL = searchHistoryDTO.Audio,
                ImageURL = searchHistoryDTO.Image,
                Name = searchHistoryDTO.Name,
                Type = searchHistoryDTO.Type,
                UserID = User.UserID,
            };
            await _context.SearchHistories.AddAsync(searchHistory);
            await _context.SaveChangesAsync();
            var result = new CreateSearchHistoryDTO
            {
                SearchID = searchHistory.SearchID,
                ResultID = searchHistory.ResultID,
                AudioURL = searchHistory.AudioURL,
                ImageURL = searchHistory.ImageURL,
                Name = searchHistory.Name,
                Type = searchHistory.Type
            };

            return result;
        }

        public async Task<object> DeleteSearchHistoryAsync(DeleteSearchDTO request, string uid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);
            if (user == null) return null;

            var history = await _context.SearchHistories
                .FirstOrDefaultAsync(sh => sh.UserID == user.UserID &&
                                           sh.ResultID == request.Id &&
                                           sh.Type == request.Type);

            if (history == null) return null;

            _context.SearchHistories.Remove(history);
            await _context.SaveChangesAsync();

            return new
            {
                DeletedId = request.Id,
                Type = request.Type
            };
        }

        public async Task<object> DeleteAllSearchHistoryAsync(string uid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == uid);
            if (user == null) return null;

            var histories = await _context.SearchHistories
                .Where(sh => sh.UserID == user.UserID)
                .ToListAsync();

            if (histories.Count == 0) return null;

            _context.SearchHistories.RemoveRange(histories);
            await _context.SaveChangesAsync();

            return new
            {
                Message = "All search histories deleted",
                Count = histories.Count
            };
        }
    }
}
