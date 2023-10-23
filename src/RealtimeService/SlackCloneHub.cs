using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.DataProtection;
using WorkspaceStore = PersistenceService.Stores.WorkspaceStore;

namespace RealtimeService.Hubs;

public class SlackCloneHub : Hub
{
    private IDataProtectionProvider DataProtectionProvider { get; set; }
    private IServiceProvider ServiceProvider { get; set; }

    static private readonly string REALTIME_KEY_COOKIE_NAME = "realtime-key";
    static private readonly string REALTIME_KEY_DATA_PROTECTION_PURPOSE =
        "realtime-key-encryption";
    static private readonly string REALTIME_KEYS_FILE_PATH =
        "../../keys/rtks.txt";

    public SlackCloneHub(
        IDataProtectionProvider dataProtectionProvider,
        IServiceProvider serviceProvider
    )
    {
        DataProtectionProvider = dataProtectionProvider;
        ServiceProvider = serviceProvider;
    }

    private string? GetSubFromQueryString(HttpContext context)
    {
        return context.Request.Query["sub"];
    }

    private string? GetWorkspaceIdFromQueryString(HttpContext context)
    {
        return context.Request.Query["workspace"];
    }

    private async Task<List<Guid>> GetGroupIds(Guid userId, Guid workspaceId)
    {
        using var scope = ServiceProvider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        List<Guid> groupIds = await workspaceStore.LoadUserGroups(
            userId,
            workspaceId
        );
        groupIds.Add(workspaceId);

        return groupIds;
    }

    private async Task AddToGroups(
        Guid userId,
        Guid workspaceId,
        List<Guid> groupIds
    )
    {
        foreach (Guid groupId in groupIds)
        {
            await AddToGroup(groupId);
        }
    }

    private async Task RemoveFromGroups(
        Guid userId,
        Guid workspaceId,
        List<Guid> groupIds
    )
    {
        foreach (Guid groupId in groupIds)
        {
            await RemoveFromGroup(groupId);
        }
    }

    private async Task AddToGroup(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
    }

    private async Task RemoveFromGroup(Guid groupId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            groupId.ToString()
        );
    }

    public async Task NewMessage(Guid userId, string message) =>
        await Clients.All.SendAsync("messageReceived", userId, message);

    public override async Task OnConnectedAsync()
    {
        try
        {
            var httpContext = Context.GetHttpContext()!;
            var userId = Guid.Parse(
                GetSubFromQueryString(httpContext)
                    ?? throw new InvalidOperationException()
            );
            var workspaceId = Guid.Parse(
                GetWorkspaceIdFromQueryString(httpContext)
                    ?? throw new InvalidOperationException()
            );
            var groupIds = await GetGroupIds(userId, workspaceId);
            AuthenticateConnection();
            await AddToGroups(userId, workspaceId, groupIds);

            // TODO: update db online status

            // TODO: send online notification

            Console.WriteLine($"authenticated and connected user {userId}!");
            await base.OnConnectedAsync();
        }
        catch (Exception e)
        {
            var httpContext = Context.GetHttpContext()!;
            var userId = Guid.Parse(GetSubFromQueryString(httpContext));
            var workspaceId = Guid.Parse(
                GetWorkspaceIdFromQueryString(httpContext)
            );
            if (userId != Guid.Empty && workspaceId != Guid.Empty)
            {
                var groupIds = await GetGroupIds(userId, workspaceId);
                await RemoveFromGroups(userId, workspaceId, groupIds);
            }
            Console.WriteLine(e);
            Context.Abort();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // TODO: update db user online status
        // TODO: send offline notification

        var httpContext = Context.GetHttpContext()!;
        var userId = Guid.Parse(GetSubFromQueryString(httpContext)!);
        var workspaceId = Guid.Parse(
            GetWorkspaceIdFromQueryString(httpContext)!
        );
        var groupIds = await GetGroupIds(userId, workspaceId);
        await RemoveFromGroups(userId, workspaceId, groupIds);
        Console.WriteLine(
            $"disconnected user {Context.GetHttpContext()!.Request.Query["sub"]}!"
        );

        await base.OnDisconnectedAsync(exception);
    }

    private void AuthenticateConnection()
    {
        var connectionCookies = Context.GetHttpContext()!.Request.Cookies!;
        if (!connectionCookies.ContainsKey(REALTIME_KEY_COOKIE_NAME))
        {
            throw new HubException("Connection refused");
        }
        var encryptedRealtimeKeyCookie = connectionCookies[
            REALTIME_KEY_COOKIE_NAME
        ];
        var protector = DataProtectionProvider.CreateProtector(
            REALTIME_KEY_DATA_PROTECTION_PURPOSE
        );
        string? realtimeKey;
        try
        {
            realtimeKey = protector.Unprotect(encryptedRealtimeKeyCookie);
            if (realtimeKey is null)
                throw new InvalidOperationException("decryption failed");
            ValidateRealtimeKey(realtimeKey);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new HubException("Connection refused");
        }
    }

    private void ValidateRealtimeKey(string? key)
    {
        var filePath =
            Directory.GetCurrentDirectory() + "/" + REALTIME_KEYS_FILE_PATH;
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Could not locate {filePath}");
        }
        using (StreamReader reader = new StreamReader(filePath))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Count() == 0)
                {
                    continue;
                }
                int i = line.IndexOf(",");
                if (i == -1)
                {
                    throw new InvalidOperationException(
                        "Invalid realtime key file format"
                    );
                }
                var entry = line.Substring(0, i);
                if (entry == key)
                {
                    int j = line.LastIndexOf(',');
                    var expiryString = line[(i + 1)..j];
                    var expiresAt = ParseHttpDateTimeString(expiryString);
                    TimeSpan timeDiff = DateTime.UtcNow - expiresAt;
                    if (timeDiff.TotalMinutes >= 30)
                    {
                        throw new InvalidOperationException(
                            "Expired realtime key"
                        );
                    }
                    var hc = Context.GetHttpContext()!;
                    var queryStringSub = hc.Request.Query["sub"];
                    var realtimeKeyAssociatedSub = line.Substring(j + 1);
                    if (queryStringSub != realtimeKeyAssociatedSub)
                    {
                        throw new InvalidOperationException(
                            "Invalid sub query parameter"
                        );
                    }
                    return;
                }
            }
        }
        throw new InvalidOperationException("Invalid key");
    }

    private DateTimeOffset ParseHttpDateTimeString(string s)
    {
        DateTimeOffset parsed;
        if (
            DateTimeOffset.TryParseExact(
                s,
                "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out parsed
            )
        )
        {
            return parsed;
        }
        else
        {
            throw new InvalidOperationException(
                "Invalid realtime key file format"
            );
        }
    }
}
