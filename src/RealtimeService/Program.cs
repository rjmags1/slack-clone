using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Cors;
using RealtimeService.Hubs;
using Microsoft.AspNetCore.DataProtection;
using RealtimeService.Kafka.Consumer;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Stores;
using Microsoft.EntityFrameworkCore;
using Models = PersistenceService.Models;
using Microsoft.AspNetCore.Identity;
using RealtimeService.ApiServiceClient;

const string DATA_PROTECTION_KEY_PATH = "../../keys/data_protection_keys";
const string DATA_PROTECTION_APPLICATION_NAME = "slack-clone";

const string WEB_CLIENT_URL = "https://localhost:5003";

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDataProtection()
    .SetApplicationName(DATA_PROTECTION_APPLICATION_NAME)
    .PersistKeysToFileSystem(new DirectoryInfo(DATA_PROTECTION_KEY_PATH));

builder.Services.AddSignalR();

string connectionString = Environment.GetEnvironmentVariable(
    "DB_CONNECTION_STRING"
)!;
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(connectionString)
);
builder.Services
    .AddIdentity<Models.User, IdentityRole<Guid>>()
    .AddUserManager<UserStore>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ChannelStore>();
builder.Services.AddScoped<DirectMessageGroupStore>();
builder.Services.AddScoped<FileStore>();
builder.Services.AddScoped<ThemeStore>();
builder.Services.AddScoped<UserStore>();
builder.Services.AddScoped<WorkspaceStore>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ApiClient>();

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
