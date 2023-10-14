using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Cors;
using RealtimeService.Hubs;
using Microsoft.AspNetCore.DataProtection;
using RealtimeService.Kafka.Consumer;

const string DATA_PROTECTION_KEY_PATH = "../../keys/data_protection_keys";
const string DATA_PROTECTION_APPLICATION_NAME = "slack-clone";

const string WEB_CLIENT_URL = "https://localhost:5003";

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDataProtection()
    .SetApplicationName(DATA_PROTECTION_APPLICATION_NAME)
    .PersistKeysToFileSystem(new DirectoryInfo(DATA_PROTECTION_KEY_PATH));

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

var consumer = new KafkaConsumer(
    app.Services.GetRequiredService<IHubContext<SlackCloneHub>>()
);
consumer.InitConsumer(new List<string> { "dev-realtime-messages" });

app.Run();
