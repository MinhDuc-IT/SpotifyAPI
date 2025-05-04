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
    public interface IArtistService
    {
        Task<Artist> CreateArtistAsync(int UserID, string ArtistName);
    }

    public class ArtistService : IArtistService
    {
        private readonly SpotifyDbContext _context;

        public ArtistService(SpotifyDbContext context)
        {
            _context = context;
        }

        public async Task<Artist> CreateArtistAsync(int UserID, string ArtistName)
        {
            var artist = new Artist
            {
                UserID = UserID,
                ArtistName = ArtistName,
                FormedDate = DateTime.UtcNow,
            };

            await _context.Artists.AddAsync(artist);
            await _context.SaveChangesAsync();

            return artist;
        }
    }
}
