using Microsoft.AspNetCore.SignalR;

namespace RealtimeService.Hubs;

public class SlackCloneHub : Hub
{
    public async Task NewMessage(Guid userId, string message) =>
        await Clients.All.SendAsync("messageReceived", userId, message);

    public override async Task OnConnectedAsync()
    {
        // TODO: add to correct groups using Context member
        // TODO: update db online status
        // TODO: send online notification

        var cookies = Context.GetHttpContext().Request.Cookies;
        foreach (var c in cookies)
        {
            Console.WriteLine($"{c.Key}->{c.Value}");
        }
        Console.WriteLine("########");
        Console.WriteLine("connected!");
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
}
