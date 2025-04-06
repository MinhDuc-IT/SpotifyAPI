using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPI.Data;
using SpotifyAPI.Middleware;

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
    Credential = GoogleCredential.FromFile("reactnativeapp-7cd31-firebase-adminsdk-fbsvc-6d99501c1a.json"),
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/reactnativeapp-7cd31";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/reactnativeapp-7cd31",
            ValidateAudience = true,
            ValidAudience = "reactnativeapp-7cd31",
            ValidateLifetime = true,
            //RoleClaimType = "roles" // Map the "roles" claim to Role claims
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
