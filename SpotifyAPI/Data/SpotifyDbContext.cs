using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Models;

namespace SpotifyAPI.Data
{
    public class SpotifyDbContext : DbContext
    {
        public SpotifyDbContext(DbContextOptions<SpotifyDbContext> options) : base(options)
        {
        }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<ListeningHistory> ListeningHistories { get; set; }
        public DbSet<ArtistFollow> ArtistFollows { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<LikedSong> LikedSongs { get; set; }
        public DbSet<LikedAlbum> LikedAlbums { get; set; }
        public DbSet<LikedPlaylist> LikedPlaylists { get; set; }
        public DbSet<SongGenre> SongGenres { get; set; }
        public DbSet<SongArtist> SongArtists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationReceiver> NotificationReceivers { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //ArtistFollow
            modelBuilder.Entity<ArtistFollow>()
                .HasKey(af => new { af.UserID, af.ArtistId });

            modelBuilder.Entity<ArtistFollow>()
                .HasOne(af => af.User)
                .WithMany(u => u.ArtistFollows)
                .HasForeignKey(af => af.UserID);

            modelBuilder.Entity<ArtistFollow>()
                .HasOne(af => af.Artist)
                .WithMany(a => a.Followers)
                .HasForeignKey(af => af.ArtistId);

            //LikedAlbum
            modelBuilder.Entity<LikedAlbum>()
                .HasKey(la => new { la.UserID, la.AlbumID });

            modelBuilder.Entity<LikedAlbum>()
                .HasOne(la => la.User)
                .WithMany(u => u.LikedAlbums)
                .HasForeignKey(la => la.UserID);

            modelBuilder.Entity<LikedAlbum>()
                .HasOne(la => la.Album)
                .WithMany(a => a.LikedByUsers)
                .HasForeignKey(la => la.AlbumID);

            //LikedPlaylist
            modelBuilder.Entity<LikedPlaylist>()
                .HasKey(lp => new { lp.UserID, lp.PlaylistID });

            modelBuilder.Entity<LikedPlaylist>()
                .HasOne(lp => lp.User)
                .WithMany(u => u.LikedPlaylists)
                .HasForeignKey(lp => lp.UserID);

            modelBuilder.Entity<LikedPlaylist>()
                .HasOne(lp => lp.Playlist)
                .WithMany(p => p.LikedByUsers)
                .HasForeignKey(lp => lp.PlaylistID);

            //LikedSong
            modelBuilder.Entity<LikedSong>()
                .HasKey(ls => new { ls.UserID, ls.SongID });

            modelBuilder.Entity<LikedSong>()
                .HasOne(ls => ls.User)
                .WithMany(u => u.LikedSongs)
                .HasForeignKey(ls => ls.UserID);

            modelBuilder.Entity<LikedSong>()
                .HasOne(ls => ls.Song)
                .WithMany(s => s.LikedByUsers)
                .HasForeignKey(ls => ls.SongID);

            //ListeningHistory
            //modelBuilder.Entity<ListeningHistory>()
            //    .HasKey(lh => new { lh.UserID, lh.SongId });  // Khóa chính tổng hợp
            modelBuilder.Entity<ListeningHistory>()
                .HasKey(lh => lh.Id);

            modelBuilder.Entity<ListeningHistory>()
                .HasOne(lh => lh.User)
                .WithMany(u => u.ListeningHistories)
                .HasForeignKey(lh => lh.UserID);

            modelBuilder.Entity<ListeningHistory>()
                .HasOne(lh => lh.Song)
                .WithMany(s => s.ListeningHistories)
                .HasForeignKey(lh => lh.SongId);

            //SongArtist
            modelBuilder.Entity<SongArtist>()
                .HasKey(sa => new { sa.SongId, sa.ArtistId });  // Khóa chính tổng hợp

            modelBuilder.Entity<SongArtist>()
                .HasOne(sa => sa.Song)
                .WithMany(s => s.SongArtists)  
                .HasForeignKey(sa => sa.SongId);

            modelBuilder.Entity<SongArtist>()
                .HasOne(sa => sa.Artist)
                .WithMany(a => a.SongArtists)  
                .HasForeignKey(sa => sa.ArtistId);

            //SongGenre
            modelBuilder.Entity<SongGenre>()
                .HasKey(sg => new { sg.SongId, sg.GenreId });  // Khóa chính tổng hợp

            modelBuilder.Entity<SongGenre>()
                .HasOne(sg => sg.Song)
                .WithMany(s => s.SongGenres) 
                .HasForeignKey(sg => sg.SongId);

            modelBuilder.Entity<SongGenre>()
                .HasOne(sg => sg.Genre)
                .WithMany(g => g.SongGenres)  
                .HasForeignKey(sg => sg.GenreId);

            //UserFollow
            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowedUserId });  // Khóa chính tổng hợp

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following) 
                .HasForeignKey(uf => uf.FollowerId);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.FollowedUser)
                .WithMany(u => u.Followers) 
                .HasForeignKey(uf => uf.FollowedUserId);

            //Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Sender)
                .WithMany(u => u.SentNotifications)
                .HasForeignKey(n => n.SenderUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<NotificationReceiver>()
                .HasOne(nr => nr.Notification)
                .WithMany(n => n.NotificationReceivers)
                .HasForeignKey(nr => nr.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationReceiver>()
                .HasOne(nr => nr.Receiver)
                .WithMany(u => u.NotificationReceivers)
                .HasForeignKey(nr => nr.ReceiverUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
            .HasOne(u => u.ArtistProfile)
            .WithOne(a => a.User)
            .HasForeignKey<Artist>(a => a.UserID);

            // SearchHistory
            modelBuilder.Entity<SearchHistory>()
                .HasOne(sh => sh.User)
                .WithMany(u => u.SearchHistories)
                .HasForeignKey(sh => sh.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
