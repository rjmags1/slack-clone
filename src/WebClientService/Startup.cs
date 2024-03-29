using System.IdentityModel.Tokens.Jwt;
using Duende.Bff.Yarp;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication;
using WebClientService.Middleware;
using Microsoft.AspNetCore.DataProtection;

namespace WebClientService;

public class Startup
{
    static private readonly string DATA_PROTECTION_KEY_PATH =
        "../../keys/data_protection_keys";
    static private readonly string DATA_PROTECTION_APPLICATION_NAME =
        "slack-clone";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDataProtection()
            .SetApplicationName(DATA_PROTECTION_APPLICATION_NAME)
            .PersistKeysToFileSystem(
                new DirectoryInfo(DATA_PROTECTION_KEY_PATH)
            );

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
            .AddCookie(
                "Cookies",
                options =>
                {
                    options.Events.OnSigningOut = async e =>
                    {
                        await e.HttpContext.RevokeUserRefreshTokenAsync();
                    };
                }
            )
            .AddOpenIdConnect(
                "oidc",
                options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.ClientId = "bff";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("api");
                    options.Scope.Add("offline_access");
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                }
            );
        services.AddAuthorization();
        services.AddBff().AddRemoteApis();
        services.AddHttpClient();
        services.AddAccessTokenManagement();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRealtimeAuthMiddleware();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.Use(
            async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404)
                {
                    string[] clientSideRoutes = new string[]
                    {
                        "/test",
                        "/workspaces"
                    };
                    if (
                        clientSideRoutes.Contains(context.Request.Path.Value)
                        || context.Request.Path.Value?[..11] == "/workspace/"
                    )
                    {
                        var fileProvider = new PhysicalFileProvider(
                            env.WebRootPath
                        );
                        var fileInfo = fileProvider.GetFileInfo("index.html");
                        if (fileInfo.Exists)
                        {
                            context.Response.StatusCode = 200; // Reset status code to OK
                            await context.Response.SendFileAsync(fileInfo);
                        }
                    }
                }
            }
        );

        app.UseRouting();
        app.UseAuthentication();

        app.UseBff();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBffManagementEndpoints();
            endpoints
                .MapGet("/local/identity", LocalIdentityHandler)
                .AsBffApiEndpoint();
            endpoints
                .MapRemoteBffApiEndpoint("/remote", "https://localhost:6001")
                .RequireAccessToken(Duende.Bff.TokenType.User);
        });
    }

    [Authorize]
    static IResult LocalIdentityHandler(ClaimsPrincipal user)
    {
        var name =
            user.FindFirst("name")?.Value ?? user.FindFirst("sub")?.Value;
        return Results.Json(
            new { message = "Local API Success!", user = name }
        );
    }
}
