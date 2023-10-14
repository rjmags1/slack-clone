using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.DataProtection;

namespace RealtimeService.Hubs;

public class SlackCloneHub : Hub
{
    private IDataProtectionProvider DataProtectionProvider { get; set; }

    static private readonly string REALTIME_KEY_COOKIE_NAME = "realtime-key";
    static private readonly string REALTIME_KEY_DATA_PROTECTION_PURPOSE =
        "realtime-key-encryption";
    static private readonly string REALTIME_KEYS_FILE_PATH =
        "../../keys/rtks.txt";

    public SlackCloneHub(IDataProtectionProvider dataProtectionProvider)
    {
        DataProtectionProvider = dataProtectionProvider;
    }

    public async Task NewMessage(Guid userId, string message) =>
        await Clients.All.SendAsync("messageReceived", userId, message);

    public override async Task OnConnectedAsync()
    {
        // TODO: add to correct groups using Context member
        // TODO: update db online status
        // TODO: send online notification

        AuthenticateConnection();

        Console.WriteLine("########");
        Console.WriteLine("authenticated and connected!");
        Console.WriteLine("########");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // TODO: update db user online status
        // TODO: send offline notification

        Console.WriteLine("########");
        Console.WriteLine("disconnected!");
        Console.WriteLine("########");

        await base.OnDisconnectedAsync(exception);
    }

    private void AddToGroups()
    {
        // TODO: get channel memberships from db to add user to Groups
    }

    private void AuthenticateConnection()
    {
        var connectionCookies = Context.GetHttpContext().Request.Cookies;
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
        string realtimeKey;
        try
        {
            realtimeKey = protector.Unprotect(encryptedRealtimeKeyCookie);
            ValidateRealtimeKey(realtimeKey);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new HubException("Connection refused");
        }
    }

    private void ValidateRealtimeKey(string key)
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
                    var expiryString = line.Substring(i + 1);
                    var expiresAt = ParseHttpDateTimeString(expiryString);
                    TimeSpan timeDiff = DateTime.UtcNow - expiresAt;
                    if (timeDiff.TotalMinutes >= 30)
                    {
                        throw new InvalidOperationException(
                            "Expired realtime key"
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
