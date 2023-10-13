using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Cors;
using RealtimeService.Hubs;

const string WEB_CLIENT_URL = "https://localhost:5003";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowSpecificOrigin",
        builder =>
        {
            builder
                .WithOrigins(WEB_CLIENT_URL)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.MapHub<SlackCloneHub>("/realtime-hub");

app.Run();
