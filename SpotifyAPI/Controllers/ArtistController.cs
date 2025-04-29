using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult<ArtistInfoDTO>> GetArtistInfo(string artistName)
        {
            artistName = Uri.UnescapeDataString(artistName);
            var artist = await _artistInfoService.GetArtistInfo(artistName);
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
            var artistWithSongs = await _artistInfoService.GetArtistWithSongs(artistName);
            if (artistWithSongs == null)
            {
                return NotFound(new { message = "Artist not found" });
            }
            return Ok(artistWithSongs);
        }
    }
}
