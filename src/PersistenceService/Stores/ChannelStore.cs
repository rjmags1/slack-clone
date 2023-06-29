using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace PersistenceService.Stores;

public class ChannelStore : Store
{
    public ChannelStore(ApplicationDbContext context)
        : base(context) { }

    public async Task<Models.Thread> InsertThread(
        Guid channelId,
        Guid repliedToId,
        ChannelMessage reply
    )
    {
        ChannelMessage repliedTo;
        Guid workspaceId;
        try
        {
            repliedTo = _context.ChannelMessages
                .Where(cm => cm.Id == repliedToId)
                .Where(cm => cm.ChannelId == channelId)
                .First();
            workspaceId = _context.Channels
                .Where(c => c.Id == channelId)
                .First()
                .WorkspaceId;
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid arguments");
        }

        bool replyInChannel = reply.ChannelId == channelId;
        bool replyUserSpecified = reply.UserId != default(Guid);
        bool replyContentSpecified = reply.Content?.Length > 0;
        if (!replyInChannel || !replyUserSpecified || !replyContentSpecified)
        {
            throw new ArgumentException("Invalid arguments");
        }

        Models.Thread thread = new Models.Thread
        {
            ChannelId = channelId,
            FirstMessageId = repliedToId,
            WorkspaceId = workspaceId
        };
        _context.Add(thread);

        repliedTo.Thread = thread;

        _context.Attach(reply);
        reply.Thread = thread;

        ChannelMessageReply replyEntry = new ChannelMessageReply
        {
            ChannelMessage = reply,
            MessageRepliedToId = repliedToId,
            RepliedToId = repliedTo.UserId,
            ReplierId = reply.UserId,
            Thread = thread
        };
        _context.Add(replyEntry);

        await _context.SaveChangesAsync();

        return thread;
    }

    public async Task<List<ChannelMember>> InsertChannelMembers(
        Guid channelId,
        List<Guid> userIds
    )
    {
        Guid workspaceId = _context.Channels
            .First(c => c.Id == channelId)
            .WorkspaceId;
        bool allUsersAreWorkspaceMembers =
            _context.WorkspaceMembers
                .Where(member => member.WorkspaceId == workspaceId)
                .Where(member => userIds.Contains(member.UserId))
                .Count() == userIds.Count;
        bool allUsersNotChannelMembers =
            _context.ChannelMembers
                .Where(member => userIds.Contains(member.UserId))
                .Count() == 0;
        if (!allUsersAreWorkspaceMembers || !allUsersNotChannelMembers)
        {
            throw new InvalidOperationException(
                "Users must be workspace members and not already channel members"
            );
        }

        List<ChannelMember> channelMembers = new List<ChannelMember>();
        foreach (Guid userId in userIds)
        {
            channelMembers.Add(
                new ChannelMember { ChannelId = channelId, UserId = userId }
            );
        }

        _context.AddRange(channelMembers);
        await _context.SaveChangesAsync();

        return channelMembers;
    }

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

        bool invitedExists =
            _context.Users.Where(u => u.Id == userId).Count() == 1;
        if (!invitedExists)
        {
            throw new InvalidOperationException("Could not invite user");
        }

        Guid workspaceId = _context.Channels
            .First(c => c.Id == channelId)
            .WorkspaceId;
        bool invitedIsWorkspaceMember =
            _context.WorkspaceMembers
                .Where(wm => wm.UserId == userId)
                .Where(wm => wm.WorkspaceId == workspaceId)
                .Count() == 1;
        bool invitedAlreadyChannelMember =
            _context.ChannelMembers
                .Where(cm => cm.ChannelId == channelId)
                .Where(cm => cm.UserId == userId)
                .Count() == 1;
        if (!invitedIsWorkspaceMember || invitedAlreadyChannelMember)
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
