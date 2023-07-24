using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Stores;
using GraphQL;
using GraphQL.SystemTextJson;
using Microsoft.Extensions.Options;
using SlackCloneGraphQL;
using Models = PersistenceService.Models;
using Microsoft.AspNetCore.Identity;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer(
        "Bearer",
        options =>
        {
            options.Authority = "https://localhost:5001";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        }
    );

string connectionString = Environment.GetEnvironmentVariable(
    "DB_CONNECTION_STRING"
)!;
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(connectionString)
);

builder.Services
    .AddIdentity<Models.User, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddUserManager<UserStore>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<ChannelStore>();
builder.Services.AddScoped<DirectMessageGroupStore>();
builder.Services.AddScoped<FileStore>();
builder.Services.AddScoped<ThemeStore>();
builder.Services.AddScoped<UserStore>();
builder.Services.AddScoped<WorkspaceStore>();

builder.Services.Configure<GraphQLSettings>(options =>
{
    options.EnableMetrics = true;
});
builder.Services.AddGraphQL(
    b =>
        b.AddSchema<SlackCloneSchema>()
            .AddSystemTextJson()
            .AddGraphTypes(typeof(SlackCloneSchema).Assembly)
            .UseMemoryCache()
            .UseApolloTracing(
                options =>
                    options.RequestServices!
                        .GetRequiredService<IOptions<GraphQLSettings>>()
                        .Value.EnableMetrics
            )
);
builder.Services.AddSingleton<SlackCloneData>();
builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(
        opts =>
            opts.JsonSerializerOptions.Converters.Add(new InputsJsonConverter())
    );

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
