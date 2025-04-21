using SpotifyAPI.DTOs;
using System.Text.RegularExpressions;

namespace SpotifyAPI.Utils
{
    public static class LyricParserUtils
    {
        public static List<LyricResponseDTO> ParseLyricLines(string[] lines)
        {
            var lyrics = new List<LyricResponseDTO>();

            foreach (var line in lines)
            {
                // LRC format: [mm:ss.ff] Lyric text
                var match = Regex.Match(line, @"\[(\d+):(\d+\.\d+)\](.*)");
                if (match.Success)
                {
                    var minutes = int.Parse(match.Groups[1].Value);
                    var seconds = double.Parse(match.Groups[2].Value);
                    var text = match.Groups[3].Value.Trim();

                    lyrics.Add(new LyricResponseDTO
                    {
                        StartTime = TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds),
                        Text = text
                    });
                }
            }

            return lyrics.OrderBy(l => l.StartTime).ToList();
        }
    }
}
