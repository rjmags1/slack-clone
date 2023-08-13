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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;
using System.Text.Json;
using ApiService.Auth;
using Microsoft.AspNetCore.Authorization;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.Audience = "bff";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = GetIssuerSigningKeys,
            ValidateIssuer = true,
            ValidIssuer = "https://localhost:5001",
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAlgorithms = new string[] { SecurityAlgorithms.RsaSha256 }
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async context =>
            {
                await Task.Run(() =>
                {
                    Console.WriteLine(context.Exception.Message);
                });
            }
        };
    });

builder.Services.AddSingleton<IAuthorizationHandler, RequiredScopeHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "HasApiScopeClaim",
        policy => policy.Requirements.Add(new ScopeRequirement("api"))
    );
});

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

IEnumerable<SecurityKey> GetIssuerSigningKeys(
    string token,
    SecurityToken securityToken,
    string kid,
    TokenValidationParameters validationParameters
)
{
    var httpClient = new HttpClient();
    var discoveryEndpoint =
        "https://localhost:5001/.well-known/openid-configuration";
    var response = httpClient.GetStringAsync(discoveryEndpoint).Result;
    var jwksUri = JsonDocument
        .Parse(response)
        .RootElement.GetProperty("jwks_uri")
        .GetString();

    var jwksResponse = httpClient.GetStringAsync(jwksUri).Result;
    var jwksDocument = JsonDocument.Parse(jwksResponse);
    var issuerSigningKeys = new List<SecurityKey>();

    foreach (
        var key in jwksDocument.RootElement.GetProperty("keys").EnumerateArray()
    )
    {
        var keyType = key.GetProperty("kty").GetString();

        if (keyType?.ToString() == "RSA")
        {
            var m = Base64UrlEncoder.DecodeBytes(
                key.GetProperty("n").GetString()
            );
            var e = Base64UrlEncoder.DecodeBytes(
                key.GetProperty("e").GetString()
            );
            var rsaParameters = new RSAParameters { Modulus = m, Exponent = e };

            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParameters);
            issuerSigningKeys.Add(new RsaSecurityKey(rsa));
        }
    }

    return issuerSigningKeys;
}
