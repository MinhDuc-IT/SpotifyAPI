using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPI.Data;
using SpotifyAPI.Middleware;
using SpotifyAPI.Services;
using System.Security.Cryptography.X509Certificates;

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
    Credential = GoogleCredential.FromFile("spotifyapp-efafb-firebase-adminsdk-fbsvc-c36f82e520.json"),
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
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("ExpoPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:8081", "http://192.168.1.247:8081", "http://10.0.2.2", "http://192.168.1.247")
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

app.Run();
