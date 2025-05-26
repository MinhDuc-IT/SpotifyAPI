using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPI.Data;
using SpotifyAPI.Middleware;
using SpotifyAPI.Services;
using System.Security.Cryptography.X509Certificates;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SpotifyAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SpotifyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SpotifyDbConnection")));

FirebaseApp.Create(new AppOptions()
{

    Credential = GoogleCredential.FromFile("spotifyapp-efafb-firebase-adminsdk-fbsvc-d42de84563.json"),

    //Credential = GoogleCredential.FromFile("spotifyapp-efafb-firebase-adminsdk-fbsvc-3eb01a5f4c.json"),

});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Authority = "https://securetoken.google.com/spotifyapp-efafb";
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = "https://securetoken.google.com/spotifyapp-efafb",
//            ValidateAudience = true,
//            ValidAudience = "spotifyapp-efafb",
//            ValidateLifetime = true,
//            //RoleClaimType = "roles" // Map the "roles" claim to Role claims
//        };
//    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/spotifyapp-efafb";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/spotifyapp-efafb",

            ValidateAudience = true,
            ValidAudience = "spotifyapp-efafb",

            ValidateLifetime = true,
            //RoleClaimType = "roles"
        };

        // 👇 Add SignalR JWT support
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Cho phép truyền token qua query string khi dùng WebSocket (SignalR)
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("ExpoPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:8081", "http://localhost:3000", "http://192.168.1.247:8081", "http://10.0.2.2", "http://192.168.1.247")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();

        //builder.AllowAnyOrigin()
        //       .AllowAnyHeader()
        //       .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddScoped<IFirebaseUserSyncService, FirebaseUserSyncService>();

builder.Services.AddScoped<ISongService, SongService>();

builder.Services.AddScoped<IListeningHistoryService, ListeningHistoryService>();
builder.Services.AddScoped<ILikedSongService, LikedSongService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IArtistInfoService, ArtistInfoService>();
builder.Services.AddScoped<IArtistFollowService, ArtistFollowService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ISongGenreService, SongGenreService>();
//builder.Services.AddScoped<IPlaylistService, PlaylistService>();

//builder.Services.AddScoped<VnPayService>();


//builder.Services.AddScoped<IPlaylistService, PlaylistService>();
//builder.Services.AddScoped<VnPayService>();

builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<VnPayService>();
builder.Services.AddScoped<IGenreService, GenreService>();


builder.Services.AddScoped<CloudinaryService>();

// Config Cloudinary
var cloudinaryConfig = builder.Configuration.GetSection("CloudinarySettings");
var account = new Account(
    cloudinaryConfig["CloudName"],
    cloudinaryConfig["ApiKey"],
    cloudinaryConfig["ApiSecret"]
);

builder.Services.AddSingleton(new Cloudinary(account));

builder.Services.AddScoped<IFirebaseUserSyncService, FirebaseUserSyncService>();



builder.Services.AddHttpClient();


builder.Services.AddSignalR();



builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ExpoPolicy");

//app.UseMiddleware<FirebaseAuthenticationMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
