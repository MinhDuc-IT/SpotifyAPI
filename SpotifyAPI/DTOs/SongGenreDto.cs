namespace SpotifyAPI.DTOs
{
    public class SongGenreDto
    {
        public int SongId { get; set; }
        public int GenreId { get; set; }
    }
    public class SongGenreDetailDto
    {
        public int SongId { get; set; }
        public string SongTitle { get; set; }

        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }

}
