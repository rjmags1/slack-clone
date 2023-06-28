using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace PersistenceService.Stores;

public class ChannelStore : Store
{
    public ChannelStore(ApplicationDbContext context)
        : base(context) { }

    public async Task<ChannelInvite> InsertChannelInvite(
        Guid channelId,
        Guid adminId,
        Guid userId
    )
    {
        ChannelMember adminMembership = _context.ChannelMembers.First(
            cm => cm.UserId == adminId
        );
        if (!adminMembership.Admin)
        {
            throw new InvalidOperationException(
                "Only channel admins may send invites"
            );
        }

        bool invitedExists = _context.Users.Count(u => u.Id == userId) == 1;
        if (!invitedExists)
        {
            throw new InvalidOperationException("Could not invite user");
        }

        Guid workspaceId = _context.Channels
            .First(c => c.Id == channelId)
            .WorkspaceId;
        bool invitedIsWorkspaceMember =
            _context.WorkspaceMembers.Count(
                wm => wm.UserId == userId && wm.WorkspaceId == workspaceId
            ) == 1;
        if (!invitedIsWorkspaceMember)
        {
            throw new InvalidOperationException("Could not invite user");
        }

        ChannelInvite invite = new ChannelInvite
        {
            AdminId = adminId,
            ChannelId = channelId,
            UserId = userId,
            WorkspaceId = workspaceId
        };
        _context.Add(invite);
        await _context.SaveChangesAsync();

        return invite;
    }

    public async Task<List<Channel>> InsertChannels(List<Channel> channels)
    {
        _context.AddRange(channels);
        await _context.SaveChangesAsync();
        return channels;
    }

    public async Task<List<Channel>> InsertTestChannels(int numTestChannels)
    {
        string email = "test-email@test.com";
        string username =
            "tccreator-uname" + ChannelStore.GenerateRandomString(15);
        User channelCreator = new User
        {
            FirstName = "test-ccreator-fname",
            LastName = "test-channel-creator-lname",
            Timezone = UserStore.timezones[0].Id,
            UserName = username,
            Email = email,
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = username.ToUpper(),
            PhoneNumber = "1-234-456-7890",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        Workspace channelWorkspace = new Workspace
        {
            Description = "test-description",
            Name = "test-workspace-channels-name"
        };
        List<Channel> channels = new List<Channel>();
        for (int i = 0; i < numTestChannels; i++)
        {
            channels.Add(
                new Channel
                {
                    CreatedBy = channelCreator,
                    Description = "test-description",
                    Name = "test-channel-name-" + i.ToString(),
                    Workspace = channelWorkspace
                }
            );
        }

        _context.AddRange(channels);
        await _context.SaveChangesAsync();

        return channels;
    }
}
