using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace WebClientService.Middleware;

// TODO:
// move this logic to identity server
// memcached server for realtime keys

public class RealtimeAuthMiddleware
{
    private readonly RequestDelegate _next;
    private IDataProtectionProvider DataProtectionProvider { get; set; }

    static private readonly string SESSION_PATH = "/bff/user";
    static private readonly string REALTIME_KEY_COOKIE_NAME = "realtime-key";
    static private readonly string REALTIME_KEYS_FILE_PATH =
        "../../keys/rtks.txt";
    static private readonly string REALTIME_KEY_DATA_PROTECTION_PURPOSE =
        "realtime-key-encryption";

    public RealtimeAuthMiddleware(
        RequestDelegate next,
        IDataProtectionProvider dataProtectionProvider
    )
    {
        _next = next;
        DataProtectionProvider = dataProtectionProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        bool missingValidSignalRKey = false;
        if (context.Request.Path.Value == SESSION_PATH)
        {
            if (!context.Request.Cookies.ContainsKey(REALTIME_KEY_COOKIE_NAME))
            {
                missingValidSignalRKey = true;
            }
            else
            {
                var encryptedRealtimeKey = context.Request.Cookies[
                    REALTIME_KEY_COOKIE_NAME
                ]!;
                var protector = DataProtectionProvider.CreateProtector(
                    REALTIME_KEY_DATA_PROTECTION_PURPOSE
                );
                string cookieValue;
                try
                {
                    cookieValue = protector.Unprotect(encryptedRealtimeKey);
                }
                catch
                {
                    Console.WriteLine("invalid realtime key cookie");
                    context.Response.StatusCode =
                        StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Internal Server Error");
                    return;
                }
            }
        }

        if (missingValidSignalRKey)
        {
            var result = await context.AuthenticateAsync();
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not authenticate user during realtime key setup"
                );
            }
            string sub = result.Principal.Claims
                .First(c => c.Type == "sub")
                .Value;
            context.Response.OnStarting(
                () => InsertRealtimeKeyCookie(context, sub)
            );
        }

        await _next(context);
    }

    private async Task InsertRealtimeKeyCookie(HttpContext context, string sub)
    {
        await Task.Run(() =>
        {
            if (
                context.Request.Path.Value != SESSION_PATH
                || !context.User.Identity!.IsAuthenticated
            )
            {
                return;
            }

            var realtimeKey = Guid.NewGuid().ToString();
            var now = DateTime.Now;
            var expiresAt = now.AddMinutes(30).ToUniversalTime();
            PersistRealtimeKey(realtimeKey, expiresAt.ToString("R"), sub);

            var protector = DataProtectionProvider.CreateProtector(
                REALTIME_KEY_DATA_PROTECTION_PURPOSE
            );
            var signedCookieValue = protector.Protect(realtimeKey);

            // TODO: add domain attribute
            context.Response.Cookies.Append(
                REALTIME_KEY_COOKIE_NAME,
                signedCookieValue,
                new CookieOptions
                {
                    Expires = expiresAt,
                    HttpOnly = true,
                    Path = "/realtime-hub",
                    SameSite = SameSiteMode.None,
                    Secure = true,
                }
            );
        });
    }

    private void PersistRealtimeKey(string key, string expiresAt, string sub)
    {
        var filePath =
            Directory.GetCurrentDirectory() + "/" + REALTIME_KEYS_FILE_PATH;
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Could not locate {filePath}");
        }
        File.AppendAllText(filePath, $"{key},{expiresAt},{sub}\n");
    }
}

public static class RealtimeAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseRealtimeAuthMiddleware(
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<RealtimeAuthMiddleware>();
    }
}
