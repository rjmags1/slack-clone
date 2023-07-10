using PersistenceService.Models;
using PersistenceService.Stores;
using Thread = PersistenceService.Models.Thread;

namespace DotnetTests.PersistenceService.Utils;

public class StoreTestUtils
{
    public static Workspace CreateTestWorkspace()
    {
        return new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + Store.GenerateRandomString(10)
        };
    }

    public static User CreateTestUser()
    {
        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        return new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[
                Store.random.Next(UserStore.timezones.Count)
            ].Id,
            UserName = username,
            Email = email,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = username.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
    }

    public static WorkspaceMember CreateTestWorkspaceMember(
        User testUser,
        Workspace testWorkspace
    )
    {
        return new WorkspaceMember
        {
            Title = "test title",
            User = testUser,
            Workspace = testWorkspace
        };
    }

    public static Channel CreateTestChannel(
        User testCreator,
        Workspace testWorkspace
    )
    {
        return new Channel
        {
            CreatedBy = testCreator,
            Description = "test-description",
            Name = "test-channel-name-" + Store.GenerateRandomString(5),
            Workspace = testWorkspace
        };
    }

    public static ChannelMember CreateTestChannelMember(
        User testUser,
        Channel testChannel
    )
    {
        return new ChannelMember { User = testUser, Channel = testChannel };
    }

    public static ChannelMessage CreateTestChannelMessage(
        Channel testChannel,
        User testAuthor
    )
    {
        return new ChannelMessage
        {
            Channel = testChannel,
            Content = "test content",
            User = testAuthor
        };
    }

    public static Thread CreateTestThread(
        Channel testChannel,
        ChannelMessage testFirstMessage,
        Workspace testWorkspace
    )
    {
        return new Thread
        {
            Channel = testChannel,
            FirstMessage = testFirstMessage,
            Workspace = testWorkspace
        };
    }

    public static ChannelMessageReply CreateTestReplyRecord(
        ChannelMessage testReply,
        ChannelMessage testFirstMessage,
        User testRepliedTo,
        User testReplier,
        Thread testThread
    )
    {
        return new ChannelMessageReply
        {
            ChannelMessage = testReply,
            MessageRepliedTo = testFirstMessage,
            RepliedTo = testRepliedTo,
            Replier = testReplier,
            Thread = testThread,
        };
    }

    public static ChannelMessageMention CreateTestChannelMessageMention(
        ChannelMessage testMessage,
        User testMentioner,
        User testMentioned
    )
    {
        return new ChannelMessageMention
        {
            ChannelMessage = testMessage,
            Mentioned = testMentioned,
            Mentioner = testMentioner
        };
    }

    public static ChannelMessageReaction CreateTestChannelMessageReaction(
        ChannelMessage testMessage,
        User testUser
    )
    {
        return new ChannelMessageReaction
        {
            ChannelMessage = testMessage,
            Emoji = "🌍",
            User = testUser
        };
    }

    public static ThreadWatch CreateTestThreadWatch(
        Thread testThread,
        User testUser
    )
    {
        return new ThreadWatch { Thread = testThread, User = testUser };
    }
}
