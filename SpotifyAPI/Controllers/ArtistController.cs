using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Models;
using SpotifyAPI.Services;

namespace SpotifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistInfoService _artistInfoService;

        public ArtistController(IArtistInfoService artistInfoService)
        {
            _artistInfoService = artistInfoService;
        }

        // GET: api/artist/info/{artistName}
        [HttpGet("info/{artistName}")]
        [Authorize]
        public async Task<ActionResult<ArtistInfoDTO>> GetArtistInfo(string artistName)
        {
            artistName = Uri.UnescapeDataString(artistName);
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var artist = await _artistInfoService.GetArtistInfo(artistName, firebaseUid);
            if (artist == null)
            {
                return NotFound(new { message = "Artist not found" });
            }
            return Ok(artist);
        }

        // GET: api/artist/details/{artistName}
        [HttpGet("related/{artistName}")]
        public async Task<ActionResult<ArtistWithSongsDto>> GetArtistWithSongs(string artistName)
        {
            artistName = Uri.UnescapeDataString(artistName);
            var artistWithSongs = await _artistInfoService.GetArtistsWithSongs(artistName);
            if (artistWithSongs == null)
            {
                return NotFound(new { message = "Artist not found" });
            }
            return Ok(artistWithSongs);
        }
    }
}
