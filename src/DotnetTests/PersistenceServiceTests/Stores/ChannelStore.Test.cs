using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using Thread = PersistenceService.Models.Thread;
using PersistenceService.Stores;

namespace DotnetTests.PersistenceService.Stores;

[Collection("Database collection")]
public class ChannelStoreTests
{
    private readonly ApplicationDbContext _dbContext;

    private readonly ChannelStore _channelStore;

    public ChannelStoreTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _channelStore = new ChannelStore(_dbContext);
    }

    [Fact]
    public async void InsertThreadWatch_ShouldInsertThreadWatch()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testUser = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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
        _dbContext.Add(testUser);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            User = testUser,
            Workspace = testWorkspace
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        Channel testChannel = new Channel
        {
            CreatedBy = testUser,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        await _dbContext.SaveChangesAsync();

        ChannelMember testChannelMembership = new ChannelMember
        {
            User = testUser,
            Channel = testChannel
        };
        _dbContext.Add(testChannelMembership);

        await _dbContext.SaveChangesAsync();

        ChannelMessage testFirstMessage = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test message",
            UserId = testUser.Id
        };
        _dbContext.Add(testFirstMessage);

        await _dbContext.SaveChangesAsync();

        Thread testThread = new Thread
        {
            ChannelId = testChannel.Id,
            FirstMessageId = testFirstMessage.Id,
            WorkspaceId = testWorkspace.Id
        };

        testFirstMessage.Thread = testThread;

        await _dbContext.SaveChangesAsync();

        ThreadWatch insertedThreadWatch = await _channelStore.InsertThreadWatch(
            testUser.Id,
            testThread.Id
        );

        Assert.Equal(insertedThreadWatch.UserId, testUser.Id);
        Assert.Equal(insertedThreadWatch.ThreadId, testThread.Id);
    }

    [Fact]
    public async void InsertThreadWatch_ShouldThrowOnNonExistentIds()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testUser = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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
        _dbContext.Add(testUser);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            User = testUser,
            Workspace = testWorkspace
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        Channel testChannel = new Channel
        {
            CreatedBy = testUser,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        await _dbContext.SaveChangesAsync();

        ChannelMember testChannelMembership = new ChannelMember
        {
            User = testUser,
            Channel = testChannel
        };
        _dbContext.Add(testChannelMembership);

        await _dbContext.SaveChangesAsync();

        ChannelMessage testFirstMessage = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test message",
            UserId = testUser.Id
        };
        _dbContext.Add(testFirstMessage);

        await _dbContext.SaveChangesAsync();

        Thread testThread = new Thread
        {
            ChannelId = testChannel.Id,
            FirstMessageId = testFirstMessage.Id,
            WorkspaceId = testWorkspace.Id
        };

        testFirstMessage.Thread = testThread;

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThreadWatch(Guid.Empty, testThread.Id)
        );
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThreadWatch(testUser.Id, Guid.Empty)
        );
        Assert.NotNull(
            await _channelStore.InsertThreadWatch(testUser.Id, testThread.Id)
        );
    }

    [Fact]
    public async void InsertThreadWatch_ShouldThrowOnNonMatchingIds()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email1 = UserStore.GenerateTestEmail(10);
        string username1 = UserStore.GenerateTestUserName(10);
        string email2 = UserStore.GenerateTestEmail(10);
        string username2 = UserStore.GenerateTestUserName(10);
        User testUser1 = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = username1,
            Email = email1,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = email1.ToUpper(),
            NormalizedUserName = username1.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        User testUser2 = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = username2,
            Email = email2,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = email2.ToUpper(),
            NormalizedUserName = username2.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testUser1);
        _dbContext.Add(testUser2);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember testWorkspaceMembership1 = new WorkspaceMember
        {
            Title = "Member",
            User = testUser1,
            Workspace = testWorkspace
        };
        _dbContext.Add(testWorkspaceMembership1);
        WorkspaceMember testWorkspaceMembership2 = new WorkspaceMember
        {
            Title = "Member",
            User = testUser2,
            Workspace = testWorkspace
        };
        _dbContext.Add(testWorkspaceMembership2);

        await _dbContext.SaveChangesAsync();

        Channel testChannel1 = new Channel
        {
            CreatedBy = testUser1,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        Channel testChannel2 = new Channel
        {
            CreatedBy = testUser2,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel1);

        await _dbContext.SaveChangesAsync();

        ChannelMember testChannelMembership1 = new ChannelMember
        {
            User = testUser1,
            Channel = testChannel1
        };
        _dbContext.Add(testChannelMembership1);
        ChannelMember testChannelMembership2 = new ChannelMember
        {
            User = testUser2,
            Channel = testChannel2
        };
        _dbContext.Add(testChannelMembership2);
        await _dbContext.SaveChangesAsync();

        ChannelMessage testFirstMessage1 = new ChannelMessage
        {
            ChannelId = testChannel1.Id,
            Content = "test message",
            UserId = testUser1.Id
        };
        _dbContext.Add(testFirstMessage1);
        ChannelMessage testFirstMessage2 = new ChannelMessage
        {
            ChannelId = testChannel2.Id,
            Content = "test message",
            UserId = testUser2.Id
        };
        _dbContext.Add(testFirstMessage2);
        await _dbContext.SaveChangesAsync();

        Thread testThread1 = new Thread
        {
            ChannelId = testChannel1.Id,
            FirstMessageId = testFirstMessage1.Id,
            WorkspaceId = testWorkspace.Id
        };

        testFirstMessage1.Thread = testThread1;
        Thread testThread2 = new Thread
        {
            ChannelId = testChannel2.Id,
            FirstMessageId = testFirstMessage2.Id,
            WorkspaceId = testWorkspace.Id
        };

        testFirstMessage2.Thread = testThread2;
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThreadWatch(
                    testUser1.Id,
                    testThread2.Id
                )
        );
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThreadWatch(
                    testUser2.Id,
                    testThread1.Id
                )
        );

        Assert.NotNull(
            await _channelStore.InsertThreadWatch(testUser1.Id, testThread1.Id)
        );
        Assert.NotNull(
            await _channelStore.InsertThreadWatch(testUser2.Id, testThread2.Id)
        );
    }

    [Fact]
    public async void InsertThread_ShouldInsertThread()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testUser = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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
        _dbContext.Add(testUser);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            User = testUser,
            Workspace = testWorkspace
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        Channel testChannel = new Channel
        {
            CreatedBy = testUser,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        await _dbContext.SaveChangesAsync();

        ChannelMember testChannelMembership = new ChannelMember
        {
            User = testUser,
            Channel = testChannel
        };
        _dbContext.Add(testChannelMembership);

        await _dbContext.SaveChangesAsync();

        ChannelMessage testFirstMessage = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test message",
            UserId = testUser.Id
        };
        _dbContext.Add(testFirstMessage);

        ChannelMessage testReply = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test message",
            UserId = testUser.Id
        };

        await _dbContext.SaveChangesAsync();

        Thread insertedThread = await _channelStore.InsertThread(
            testChannel.Id,
            testFirstMessage.Id,
            testReply
        );

        Assert.NotEqual(insertedThread.Id, Guid.Empty);
        Assert.Equal(insertedThread.ChannelId, testChannel.Id);
        Assert.NotEqual(insertedThread.ConcurrencyStamp, Guid.Empty);
        Assert.Equal(insertedThread.FirstMessageId, testFirstMessage.Id);
        Assert.Equal(2, insertedThread.NumMessages);
        Assert.Equal(insertedThread.WorkspaceId, testWorkspace.Id);
        Assert.Equal(2, insertedThread.Messages.Count);

        ChannelMessage navFirstMessage = insertedThread.Messages.First(
            cm => cm.Id == testFirstMessage.Id
        );
        Assert.Equal(navFirstMessage.Id, testFirstMessage.Id);
        Assert.Equal(navFirstMessage.ThreadId, insertedThread.Id);

        ChannelMessage navReplyMessage = insertedThread.Messages.First(
            cm => cm.Id == testReply.Id
        );
        Assert.Equal(navReplyMessage.Id, testReply.Id);
        Assert.Equal(navReplyMessage.ThreadId, insertedThread.Id);

        Assert.Equal(1, navFirstMessage.Replies.Count);
        Assert.Equal(
            navFirstMessage.Replies.First().ChannelMessageId,
            testReply.Id
        );
        Assert.Equal(
            navFirstMessage.Replies.First().ChannelMessageId,
            navReplyMessage.Id
        );

        Assert.Equal(
            1,
            _dbContext.ChannelMessageReplies
                .Where(cmr => cmr.ChannelMessageId == testReply.Id)
                .Where(cmr => cmr.MessageRepliedToId == testFirstMessage.Id)
                .Where(cmr => cmr.RepliedToId == testUser.Id)
                .Where(cmr => cmr.ReplierId == testUser.Id)
                .Where(cmr => cmr.ThreadId == insertedThread.Id)
                .Count()
        );
    }

    [Fact]
    public async void InsertThread_ShouldThrowOnNonExistentIdArgs()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testUser = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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
        _dbContext.Add(testUser);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            User = testUser,
            Workspace = testWorkspace
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        Channel testChannel = new Channel
        {
            CreatedBy = testUser,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        await _dbContext.SaveChangesAsync();

        ChannelMember testChannelMembership = new ChannelMember
        {
            User = testUser,
            Channel = testChannel
        };
        _dbContext.Add(testChannelMembership);

        await _dbContext.SaveChangesAsync();

        ChannelMessage testFirstMessage = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test message",
            UserId = testUser.Id
        };
        _dbContext.Add(testFirstMessage);

        ChannelMessage testReply = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test message",
            UserId = testUser.Id
        };

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThread(
                    Guid.Empty,
                    testFirstMessage.Id,
                    testReply
                )
        );
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThread(
                    testChannel.Id,
                    Guid.Empty,
                    testReply
                )
        );
        Assert.NotNull(
            await _channelStore.InsertThread(
                testChannel.Id,
                testFirstMessage.Id,
                testReply
            )
        );
    }

    [Fact]
    public async void InsertThread_ShouldThrowOnNonmatchingIdArgs()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testUser = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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
        _dbContext.Add(testUser);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            User = testUser,
            Workspace = testWorkspace
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        Channel testChannel1 = new Channel
        {
            CreatedBy = testUser,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        Channel testChannel2 = new Channel
        {
            CreatedBy = testUser,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel1);
        _dbContext.Add(testChannel2);

        await _dbContext.SaveChangesAsync();

        ChannelMember testChannelMembership1 = new ChannelMember
        {
            User = testUser,
            Channel = testChannel1
        };
        ChannelMember testChannelMembership2 = new ChannelMember
        {
            User = testUser,
            Channel = testChannel2
        };
        _dbContext.Add(testChannelMembership1);
        _dbContext.Add(testChannelMembership2);

        await _dbContext.SaveChangesAsync();

        ChannelMessage testFirstMessage1 = new ChannelMessage
        {
            ChannelId = testChannel1.Id,
            Content = "test message",
            UserId = testUser.Id
        };
        ChannelMessage testFirstMessage2 = new ChannelMessage
        {
            ChannelId = testChannel2.Id,
            Content = "test message",
            UserId = testUser.Id
        };
        _dbContext.Add(testFirstMessage1);
        _dbContext.Add(testFirstMessage2);

        ChannelMessage testReply1 = new ChannelMessage
        {
            ChannelId = testChannel1.Id,
            Content = "test message",
            UserId = testUser.Id
        };
        ChannelMessage testReply2 = new ChannelMessage
        {
            ChannelId = testChannel2.Id,
            Content = "test message",
            UserId = testUser.Id
        };

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThread(
                    testChannel1.Id,
                    testFirstMessage1.Id,
                    testReply2
                )
        );
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThread(
                    testChannel1.Id,
                    testFirstMessage2.Id,
                    testReply1
                )
        );
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThread(
                    testChannel2.Id,
                    testFirstMessage1.Id,
                    testReply1
                )
        );
        Assert.NotNull(
            await _channelStore.InsertThread(
                testChannel1.Id,
                testFirstMessage1.Id,
                testReply1
            )
        );
        Assert.NotNull(
            await _channelStore.InsertThread(
                testChannel2.Id,
                testFirstMessage2.Id,
                testReply2
            )
        );
    }

    [Fact]
    public async void InsertThread_ShouldThrowOnInsufficientReplyInfo()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testUser = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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
        _dbContext.Add(testUser);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            User = testUser,
            Workspace = testWorkspace
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        Channel testChannel = new Channel
        {
            CreatedBy = testUser,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        await _dbContext.SaveChangesAsync();

        ChannelMember testChannelMembership = new ChannelMember
        {
            User = testUser,
            Channel = testChannel
        };
        _dbContext.Add(testChannelMembership);

        await _dbContext.SaveChangesAsync();

        ChannelMessage testFirstMessage = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test message",
            UserId = testUser.Id
        };
        _dbContext.Add(testFirstMessage);

        ChannelMessage testReply1 = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test message",
            UserId = testUser.Id
        };

        await _dbContext.SaveChangesAsync();

        Assert.NotNull(
            await _channelStore.InsertThread(
                testChannel.Id,
                testFirstMessage.Id,
                testReply1
            )
        );

        ChannelMessage testReply2 = new ChannelMessage
        {
            ChannelId = Guid.Empty,
            Content = "test message",
            UserId = testUser.Id
        };
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThread(
                    testChannel.Id,
                    testFirstMessage.Id,
                    testReply2
                )
        );

        ChannelMessage testReply3 = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "",
            UserId = testUser.Id
        };
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThread(
                    testChannel.Id,
                    testFirstMessage.Id,
                    testReply3
                )
        );

        ChannelMessage testReply4 = new ChannelMessage
        {
            ChannelId = testChannel.Id,
            Content = "test-message",
            UserId = Guid.Empty
        };
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _channelStore.InsertThread(
                    testChannel.Id,
                    testFirstMessage.Id,
                    testReply4
                )
        );
    }

    [Fact]
    public async void InsertChannelMembers_ShouldInsertChannelMembers()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        List<User> testMembers = new List<User>();
        List<WorkspaceMember> testWorkspaceMemberships =
            new List<WorkspaceMember>();
        for (int i = 0; i < 10; i++)
        {
            string email = UserStore.GenerateTestEmail(10);
            string username = UserStore.GenerateTestUserName(10);
            User user = new User
            {
                FirstName = UserStore.GenerateTestFirstName(10),
                LastName = UserStore.GenerateTestLastName(10),
                Timezone = UserStore.timezones[1].Id,
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
            WorkspaceMember workspaceMembership = new WorkspaceMember
            {
                Title = "Member",
                User = user,
                Workspace = testWorkspace
            };
            testMembers.Add(user);
            testWorkspaceMemberships.Add(workspaceMembership);
        }
        _dbContext.AddRange(testMembers);
        _dbContext.AddRange(testWorkspaceMemberships);

        Channel testChannel = new Channel
        {
            CreatedBy = testMembers[0],
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        await _dbContext.SaveChangesAsync();

        List<ChannelMember> insertedMembers =
            await _channelStore.InsertChannelMembers(
                testChannel.Id,
                testMembers.Select(u => u.Id).ToList()
            );

        foreach (
            (
                ChannelMember channelMembership,
                User member
            ) in insertedMembers.Zip(testMembers)
        )
        {
            Assert.NotEqual(channelMembership.Id, Guid.Empty);
            Assert.False(channelMembership.Admin);
            Assert.Equal(channelMembership.ChannelId, testChannel.Id);
            Assert.True(channelMembership.EnableNotifications);
            Assert.Null(channelMembership.LastViewedAt);
            Assert.False(channelMembership.Starred);
            Assert.Equal(channelMembership.UserId, member.Id);
        }
    }

    [Fact]
    public async void InsertChannelMembers_ShouldThrowOnNonExistentIds()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testMember = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            User = testMember,
            Workspace = testWorkspace
        };
        _dbContext.Add(testMember);
        _dbContext.Add(testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        Channel testChannel = new Channel
        {
            CreatedBy = testMember,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelMembers(
                    testChannel.Id,
                    new List<Guid> { Guid.Empty }
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelMembers(
                    Guid.Empty,
                    new List<Guid> { testMember.Id }
                )
        );

        Assert.NotNull(
            await _channelStore.InsertChannelMembers(
                testChannel.Id,
                new List<Guid> { testMember.Id }
            )
        );
    }

    [Fact]
    public async void InsertChannelMembers_ShouldThrowOnNonWorkspaceMembers()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testMember = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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

        _dbContext.Add(testMember);
        _dbContext.Add(testWorkspace);

        Channel testChannel = new Channel
        {
            CreatedBy = testMember,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelMembers(
                    testChannel.Id,
                    new List<Guid> { testMember.Id }
                )
        );
    }

    [Fact]
    public async void InsertChannelMembers_ShouldThrowOnAlreadyChannelMembers()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string email = UserStore.GenerateTestEmail(10);
        string username = UserStore.GenerateTestUserName(10);
        User testMember = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
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

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            User = testMember,
            Workspace = testWorkspace
        };
        _dbContext.Add(testMember);
        _dbContext.Add(testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        Channel testChannel = new Channel
        {
            CreatedBy = testMember,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        ChannelMember testChannelMembership = new ChannelMember
        {
            Channel = testChannel,
            User = testMember
        };
        _dbContext.Add(testChannelMembership);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelMembers(
                    testChannel.Id,
                    new List<Guid> { testMember.Id }
                )
        );
    }

    [Fact]
    public async void InsertChannelInvites_ShouldInsertChannelInvites()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string inviterEmail = UserStore.GenerateTestEmail(10);
        string inviterUsername = UserStore.GenerateTestUserName(10);
        User testInviter = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = inviterUsername,
            Email = inviterEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = inviterEmail.ToUpper(),
            NormalizedUserName = inviterUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInviter);
        List<WorkspaceMember> testWorkspaceMemberships =
            new List<WorkspaceMember>();
        testWorkspaceMemberships.Add(
            new WorkspaceMember
            {
                Title = "Member",
                User = testInviter,
                Workspace = testWorkspace
            }
        );

        Channel testChannel = new Channel
        {
            CreatedBy = testInviter,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        ChannelMember testInviterMembership = new ChannelMember
        {
            Admin = true,
            Channel = testChannel,
            User = testInviter
        };
        _dbContext.Add(testInviterMembership);

        List<User> testInviteds = new List<User>();
        for (int i = 0; i < 10; i++)
        {
            string email = UserStore.GenerateTestEmail(10);
            string username = "test-ci-username" + i.ToString();
            User user = new User
            {
                FirstName = UserStore.GenerateTestFirstName(10),
                LastName = UserStore.GenerateTestLastName(10),
                Timezone = UserStore.timezones[1].Id,
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
            WorkspaceMember workspaceMembership = new WorkspaceMember
            {
                Title = "Member",
                User = user,
                Workspace = testWorkspace
            };
            testInviteds.Add(user);
            testWorkspaceMemberships.Add(workspaceMembership);
        }
        _dbContext.AddRange(testInviteds);
        _dbContext.AddRange(testWorkspaceMemberships);

        await _dbContext.SaveChangesAsync();
        List<ChannelInvite> insertedInvites = new List<ChannelInvite>();
        foreach (Guid invitedId in testInviteds.Select(u => u.Id))
        {
            insertedInvites.Add(
                await _channelStore.InsertChannelInvite(
                    testChannel.Id,
                    testInviter.Id,
                    invitedId
                )
            );
        }
        foreach (
            (ChannelInvite insertedInvite, User invited) in insertedInvites.Zip(
                testInviteds
            )
        )
        {
            Assert.NotEqual(insertedInvite.Id, Guid.Empty);
            Assert.Equal(insertedInvite.AdminId, testInviter.Id);
            Assert.Equal(insertedInvite.ChannelId, testChannel.Id);
            Assert.Equal(1, insertedInvite.ChannelInviteStatus);
            Assert.NotEqual(insertedInvite.CreatedAt, default(DateTime));
            Assert.Equal(insertedInvite.UserId, invited.Id);
            Assert.Equal(insertedInvite.WorkspaceId, testWorkspace.Id);
        }
    }

    [Fact]
    public async void InsertChannelInvite_ShouldThrowOnNonExistentIds()
    {
        string inviterEmail = UserStore.GenerateTestEmail(10);
        string inviterUsername = UserStore.GenerateTestUserName(10);
        string invitedEmail = UserStore.GenerateTestEmail(10);
        string invitedUsername = UserStore.GenerateTestUserName(10);
        User testInviter = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = inviterUsername,
            Email = inviterEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = inviterEmail.ToUpper(),
            NormalizedUserName = inviterUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInviter);

        User testInvited = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = invitedUsername,
            Email = invitedEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = invitedEmail.ToUpper(),
            NormalizedUserName = invitedUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInvited);

        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);
        List<WorkspaceMember> testWorkspaceMemberships =
            new List<WorkspaceMember>();
        testWorkspaceMemberships.Add(
            new WorkspaceMember
            {
                Title = "Member",
                User = testInvited,
                Workspace = testWorkspace
            }
        );
        testWorkspaceMemberships.Add(
            new WorkspaceMember
            {
                Title = "Member",
                User = testInviter,
                Workspace = testWorkspace
            }
        );
        _dbContext.AddRange(testWorkspaceMemberships);

        Channel testChannel = new Channel
        {
            CreatedBy = testInviter,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        ChannelMember testInviterMembership = new ChannelMember
        {
            Admin = true,
            Channel = testChannel,
            User = testInviter
        };
        _dbContext.Add(testInviterMembership);

        await _dbContext.SaveChangesAsync();

        Assert.NotNull(
            await _channelStore.InsertChannelInvite(
                testChannel.Id,
                testInviter.Id,
                testInvited.Id
            )
        );
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelInvite(
                    Guid.Empty,
                    testInviter.Id,
                    testInvited.Id
                )
        );
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelInvite(
                    testChannel.Id,
                    Guid.Empty,
                    testInvited.Id
                )
        );
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelInvite(
                    testChannel.Id,
                    testInviter.Id,
                    Guid.Empty
                )
        );
    }

    [Fact]
    public async void InsertChannelInvite_ShouldThrowOnAlreadyChannelMember()
    {
        string inviterEmail = UserStore.GenerateTestEmail(10);
        string inviterUsername = UserStore.GenerateTestUserName(10);
        string invitedEmail = UserStore.GenerateTestEmail(10);
        string invitedUsername = UserStore.GenerateTestUserName(10);
        User testInviter = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = inviterUsername,
            Email = inviterEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = inviterEmail.ToUpper(),
            NormalizedUserName = inviterUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInviter);

        User testInvited = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = invitedUsername,
            Email = invitedEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = invitedEmail.ToUpper(),
            NormalizedUserName = invitedUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInvited);

        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);
        List<WorkspaceMember> testWorkspaceMemberships =
            new List<WorkspaceMember>();
        testWorkspaceMemberships.Add(
            new WorkspaceMember
            {
                Title = "Member",
                User = testInvited,
                Workspace = testWorkspace
            }
        );
        testWorkspaceMemberships.Add(
            new WorkspaceMember
            {
                Title = "Member",
                User = testInviter,
                Workspace = testWorkspace
            }
        );
        _dbContext.AddRange(testWorkspaceMemberships);

        Channel testChannel = new Channel
        {
            CreatedBy = testInviter,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        ChannelMember testInviterMembership = new ChannelMember
        {
            Admin = true,
            Channel = testChannel,
            User = testInviter
        };
        _dbContext.Add(testInviterMembership);

        _dbContext.Add(
            new ChannelMember { Channel = testChannel, User = testInvited }
        );

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelInvite(
                    testChannel.Id,
                    testInviter.Id,
                    testInvited.Id
                )
        );
    }

    [Fact]
    public async void InsertChannelInvite_ShouldThrowOnInviteByNonAdmin()
    {
        string inviterEmail = UserStore.GenerateTestEmail(10);
        string inviterUsername = UserStore.GenerateTestUserName(10);
        string invitedEmail = UserStore.GenerateTestEmail(10);
        string invitedUsername = UserStore.GenerateTestUserName(10);
        User testInviter = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = inviterUsername,
            Email = inviterEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = inviterEmail.ToUpper(),
            NormalizedUserName = inviterUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInviter);

        User testInvited = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = invitedUsername,
            Email = invitedEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = invitedEmail.ToUpper(),
            NormalizedUserName = invitedUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInvited);

        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);
        List<WorkspaceMember> testWorkspaceMemberships =
            new List<WorkspaceMember>();
        testWorkspaceMemberships.Add(
            new WorkspaceMember
            {
                Title = "Member",
                User = testInvited,
                Workspace = testWorkspace
            }
        );
        testWorkspaceMemberships.Add(
            new WorkspaceMember
            {
                Title = "Member",
                User = testInviter,
                Workspace = testWorkspace
            }
        );
        _dbContext.AddRange(testWorkspaceMemberships);

        Channel testChannel = new Channel
        {
            CreatedBy = testInviter,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        ChannelMember testInviterMembership = new ChannelMember
        {
            Admin = false,
            Channel = testChannel,
            User = testInviter
        };
        _dbContext.Add(testInviterMembership);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelInvite(
                    testChannel.Id,
                    testInviter.Id,
                    testInvited.Id
                )
        );
    }

    [Fact]
    public async void InsertChannelInvite_ShouldThrowOnInviteToNonWorkspaceMember()
    {
        string inviterEmail = UserStore.GenerateTestEmail(10);
        string inviterUsername = UserStore.GenerateTestUserName(10);
        string invitedEmail = UserStore.GenerateTestEmail(10);
        string invitedUsername = UserStore.GenerateTestUserName(10);
        User testInviter = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = inviterUsername,
            Email = inviterEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = inviterEmail.ToUpper(),
            NormalizedUserName = inviterUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInviter);

        User testInvited = new User
        {
            FirstName = UserStore.GenerateTestFirstName(10),
            LastName = UserStore.GenerateTestLastName(10),
            Timezone = UserStore.timezones[1].Id,
            UserName = invitedUsername,
            Email = invitedEmail,
            PhoneNumber = "1-234-567-8901",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            NormalizedEmail = invitedEmail.ToUpper(),
            NormalizedUserName = invitedUsername.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        _dbContext.Add(testInvited);

        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);
        _dbContext.Add(
            new WorkspaceMember
            {
                Title = "Member",
                User = testInviter,
                Workspace = testWorkspace
            }
        );

        Channel testChannel = new Channel
        {
            CreatedBy = testInviter,
            Description = "test-description",
            Name = "test-channel-name-" + ChannelStore.GenerateRandomString(5),
            Workspace = testWorkspace
        };
        _dbContext.Add(testChannel);

        ChannelMember testInviterMembership = new ChannelMember
        {
            Admin = true,
            Channel = testChannel,
            User = testInviter
        };
        _dbContext.Add(testInviterMembership);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _channelStore.InsertChannelInvite(
                    testChannel.Id,
                    testInviter.Id,
                    testInvited.Id
                )
        );
    }

    [Fact]
    public async void InsertChannels_ShouldInsertChannels()
    {
        string email = "test-email@test.com";
        string username =
            "test-ccreator-username" + ChannelStore.GenerateRandomString(5);
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
        for (int i = 0; i < 10; i++)
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

        List<Channel> inserted = await _channelStore.InsertChannels(channels);

        // navigation entities created if Ids autopopulated
        Assert.NotEqual(channelCreator.Id, Guid.Empty);
        Assert.NotEqual(channelWorkspace.Id, Guid.Empty);

        foreach ((Channel ic, Channel c) in inserted.Zip(channels))
        {
            Assert.NotEqual(ic.Id, Guid.Empty);
            Assert.True(ic.AllowThreads);
            Assert.Null(ic.Avatar);
            Assert.Null(ic.AvatarId);
            Assert.Equal(1, ic.AllowedChannelPostersMask);
            Assert.NotEqual(ic.CreatedAt, default(DateTime));
            Assert.Equal(ic.CreatedBy, channelCreator);
            Assert.Equal(ic.CreatedById, channelCreator.Id);
            Assert.NotEqual(ic.ConcurrencyStamp, Guid.Empty);
            Assert.Equal(ic.Description, c.Description);
            Assert.NotNull(ic.ChannelMembers);
            Assert.NotNull(ic.ChannelMessages);
            Assert.Equal(ic.Name, c.Name);
            Assert.False(ic.Private);
            Assert.Equal("", ic.Topic);
            Assert.Equal(ic.Workspace, channelWorkspace);
            Assert.Equal(ic.WorkspaceId, channelWorkspace.Id);
        }
    }

    [Fact]
    public async void InsertTestChannels_ShouldInsertTestChannels()
    {
        int initialChannels = _dbContext.Channels.Count();
        await _channelStore.InsertTestChannels(100);
        int currentChannels = _dbContext.Channels.Count();
        Assert.Equal(initialChannels + 100, currentChannels);
    }
}
