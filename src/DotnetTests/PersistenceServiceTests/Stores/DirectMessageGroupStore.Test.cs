using DotnetTests.Fixtures;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace DotnetTests.PersistenceService.Stores;

[Collection("Database collection")]
public class DirectMessageGroupStoreTests
{
    private readonly ApplicationDbContext _dbContext;

    private readonly DirectMessageGroupStore _directMessageGroupStore;

    public DirectMessageGroupStoreTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _directMessageGroupStore = new DirectMessageGroupStore(_dbContext);
    }

    [Fact]
    public async void InsertDirectMessageLaterFlag_ShouldInsertDirectMessageLaterFlag()
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

        DirectMessageGroup testGroup = new DirectMessageGroup
        {
            WorkspaceId = testWorkspace.Id
        };
        _dbContext.Add(testGroup);

        await _dbContext.SaveChangesAsync();

        DirectMessageGroupMember testDmgMembership =
            new DirectMessageGroupMember
            {
                DirectMessageGroupId = testGroup.Id,
                UserId = testUser.Id
            };
        _dbContext.Add(testDmgMembership);

        await _dbContext.SaveChangesAsync();

        DirectMessage testMessage = new DirectMessage
        {
            Content = "test content",
            DirectMessageGroupId = testGroup.Id,
            SentAt = DateTime.Now,
            UserId = testUser.Id
        };
        _dbContext.Add(testMessage);

        await _dbContext.SaveChangesAsync();

        DirectMessageLaterFlag insertedLaterFlag =
            await _directMessageGroupStore.InsertDirectMessageLaterFlag(
                testMessage.Id,
                testUser.Id
            );

        Assert.NotEqual(Guid.Empty, insertedLaterFlag.Id);
        Assert.NotEqual(default(DateTime), insertedLaterFlag.CreatedAt);
        Assert.Equal(1, insertedLaterFlag.DirectMessageLaterFlagStatus);
        Assert.Equal(testGroup.Id, insertedLaterFlag.DirectMessageGroupId);
        Assert.Equal(testMessage.Id, insertedLaterFlag.DirectMessageId);
        Assert.Equal(testUser.Id, insertedLaterFlag.UserId);
        Assert.Equal(testWorkspace.Id, insertedLaterFlag.WorkspaceId);
    }

    [Fact]
    public async void InsertDirectMessageLaterFlag_ShouldThrowOnInvalidIds()
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

        DirectMessageGroup testGroup = new DirectMessageGroup
        {
            WorkspaceId = testWorkspace.Id
        };
        _dbContext.Add(testGroup);

        await _dbContext.SaveChangesAsync();

        DirectMessageGroupMember testDmgMembership =
            new DirectMessageGroupMember
            {
                DirectMessageGroupId = testGroup.Id,
                UserId = testUser.Id
            };
        _dbContext.Add(testDmgMembership);

        await _dbContext.SaveChangesAsync();

        DirectMessage testMessage = new DirectMessage
        {
            Content = "test content",
            DirectMessageGroupId = testGroup.Id,
            SentAt = DateTime.Now,
            UserId = testUser.Id
        };
        _dbContext.Add(testMessage);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessageLaterFlag(
                    Guid.Empty,
                    testUser.Id
                )
        );
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessageLaterFlag(
                    testMessage.Id,
                    Guid.Empty
                )
        );

        Assert.NotNull(
            await _directMessageGroupStore.InsertDirectMessageLaterFlag(
                testMessage.Id,
                testUser.Id
            )
        );
    }

    [Fact]
    public async void InsertDirectMessage_ShouldInsertDirectMessage()
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

        DirectMessageGroup testGroup = new DirectMessageGroup
        {
            WorkspaceId = testWorkspace.Id
        };
        _dbContext.Add(testGroup);

        await _dbContext.SaveChangesAsync();

        DirectMessageGroupMember testDmgMembership =
            new DirectMessageGroupMember
            {
                DirectMessageGroupId = testGroup.Id,
                UserId = testUser.Id
            };
        _dbContext.Add(testDmgMembership);

        await _dbContext.SaveChangesAsync();

        DirectMessage insertedDm =
            await _directMessageGroupStore.InsertDirectMessage(
                testGroup.Id,
                "test content",
                testUser.Id,
                new List<Guid> { testUser.Id },
                null,
                null
            );

        Assert.NotEqual(Guid.Empty, insertedDm.Id);
        Assert.NotEqual(Guid.Empty, insertedDm.ConcurrencyStamp);
        Assert.Equal("test content", insertedDm.Content);
        Assert.NotEqual(default(DateTime), insertedDm.CreatedAt);
        Assert.False(insertedDm.Deleted);
        Assert.Equal(testGroup.Id, insertedDm.DirectMessageGroupId);
        Assert.False(insertedDm.Draft);
        Assert.Equal(0, insertedDm.Files.Count);
        Assert.Null(insertedDm.LastEdit);
        Assert.Equal(1, insertedDm.Mentions.Count);
        Assert.Equal(0, insertedDm.Reactions.Count);
        Assert.Equal(0, insertedDm.Replies.Count);
        Assert.NotEqual(default(DateTime), insertedDm.SentAt);
        Assert.Equal(testUser.Id, insertedDm.UserId);

        DirectMessage insertedDm2 =
            await _directMessageGroupStore.InsertDirectMessage(
                testGroup.Id,
                "test content",
                testUser.Id,
                null,
                insertedDm.Id,
                testUser.Id,
                true
            );

        Assert.NotEqual(Guid.Empty, insertedDm2.Id);
        Assert.NotEqual(Guid.Empty, insertedDm2.ConcurrencyStamp);
        Assert.Equal("test content", insertedDm2.Content);
        Assert.NotEqual(default(DateTime), insertedDm2.CreatedAt);
        Assert.False(insertedDm2.Deleted);
        Assert.Equal(testGroup.Id, insertedDm2.DirectMessageGroupId);
        Assert.True(insertedDm2.Draft);
        Assert.Equal(0, insertedDm2.Files.Count);
        Assert.Null(insertedDm2.LastEdit);
        Assert.Equal(0, insertedDm2.Mentions.Count);
        Assert.Equal(0, insertedDm2.Reactions.Count);
        Assert.Equal(1, insertedDm.Replies.Count);
        Assert.Equal(0, insertedDm2.Replies.Count);
        Assert.Null(insertedDm2.SentAt);
        Assert.Equal(testUser.Id, insertedDm2.UserId);

        DirectMessageReply? insertedReply = insertedDm.Replies.FirstOrDefault();
        Assert.NotNull(insertedReply);
        Assert.NotEqual(Guid.Empty, insertedReply.Id);
        Assert.Equal(insertedReply.DirectMessageId, insertedDm2.Id);
        Assert.Equal(testUser.Id, insertedReply.RepliedToId);
        Assert.Equal(testUser.Id, insertedReply.ReplierId);
        Assert.Equal(insertedDm.Id, insertedReply.MessageRepliedToId);

        DirectMessageMention? insertedMention =
            insertedDm.Mentions.FirstOrDefault();
        Assert.NotNull(insertedMention);
        Assert.NotEqual(Guid.Empty, insertedMention.Id);
        Assert.NotEqual(default(DateTime), insertedMention.CreatedAt);
        Assert.Equal(insertedDm.Id, insertedMention.DirectMessageId);
        Assert.Equal(testUser.Id, insertedMention.MentionedId);
        Assert.Equal(testUser.Id, insertedMention.MentionerId);
    }

    [Fact]
    public async void InsertDirectMessage_ShouldThrowOnNonexistentIdsEmptyContent()
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

        DirectMessageGroup testGroup = new DirectMessageGroup
        {
            WorkspaceId = testWorkspace.Id
        };
        _dbContext.Add(testGroup);

        await _dbContext.SaveChangesAsync();

        DirectMessageGroupMember testDmgMembership =
            new DirectMessageGroupMember
            {
                DirectMessageGroupId = testGroup.Id,
                UserId = testUser.Id
            };
        _dbContext.Add(testDmgMembership);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessage(
                    Guid.Empty,
                    "test content",
                    testUser.Id,
                    null,
                    null,
                    null
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessage(
                    testGroup.Id,
                    "",
                    testUser.Id,
                    null,
                    null,
                    null
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessage(
                    testGroup.Id,
                    "test content",
                    Guid.Empty,
                    null,
                    null,
                    null
                )
        );

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessage(
                    testGroup.Id,
                    "test content",
                    testUser.Id,
                    new List<Guid> { Guid.Empty },
                    null,
                    null
                )
        );

        DirectMessage insertedDm =
            await _directMessageGroupStore.InsertDirectMessage(
                testGroup.Id,
                "test content",
                testUser.Id,
                null,
                null,
                null
            );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessage(
                    testGroup.Id,
                    "test content",
                    testUser.Id,
                    null,
                    Guid.Empty,
                    testUser.Id
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessage(
                    testGroup.Id,
                    "test content",
                    testUser.Id,
                    null,
                    insertedDm.Id,
                    Guid.Empty
                )
        );
    }

    [Fact]
    public async void InsertDirectMessage_ShouldThrowOnInvalidNullArgCombos()
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

        DirectMessageGroup testGroup = new DirectMessageGroup
        {
            WorkspaceId = testWorkspace.Id
        };
        _dbContext.Add(testGroup);

        await _dbContext.SaveChangesAsync();

        DirectMessageGroupMember testDmgMembership =
            new DirectMessageGroupMember
            {
                DirectMessageGroupId = testGroup.Id,
                UserId = testUser.Id
            };
        _dbContext.Add(testDmgMembership);

        await _dbContext.SaveChangesAsync();

        DirectMessage insertedDm =
            await _directMessageGroupStore.InsertDirectMessage(
                testGroup.Id,
                "test content",
                testUser.Id,
                null,
                null,
                null
            );

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessage(
                    testGroup.Id,
                    "test content",
                    testUser.Id,
                    null,
                    insertedDm.Id,
                    null
                )
        );

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessage(
                    testGroup.Id,
                    "test content",
                    testUser.Id,
                    null,
                    null,
                    testUser.Id
                )
        );
    }

    [Fact]
    public async void InsertDirectMessage_ShouldNotInsertDraftMentions()
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

        DirectMessageGroup testGroup = new DirectMessageGroup
        {
            WorkspaceId = testWorkspace.Id
        };
        _dbContext.Add(testGroup);

        await _dbContext.SaveChangesAsync();

        DirectMessageGroupMember testDmgMembership =
            new DirectMessageGroupMember
            {
                DirectMessageGroupId = testGroup.Id,
                UserId = testUser.Id
            };
        _dbContext.Add(testDmgMembership);

        await _dbContext.SaveChangesAsync();

        DirectMessage insertedDm =
            await _directMessageGroupStore.InsertDirectMessage(
                testGroup.Id,
                "test content",
                testUser.Id,
                new List<Guid> { testUser.Id },
                null,
                null,
                true
            );

        Assert.NotEqual(Guid.Empty, insertedDm.Id);
        Assert.NotEqual(Guid.Empty, insertedDm.ConcurrencyStamp);
        Assert.Equal("test content", insertedDm.Content);
        Assert.NotEqual(default(DateTime), insertedDm.CreatedAt);
        Assert.False(insertedDm.Deleted);
        Assert.Equal(testGroup.Id, insertedDm.DirectMessageGroupId);
        Assert.True(insertedDm.Draft);
        Assert.Equal(0, insertedDm.Files.Count);
        Assert.Null(insertedDm.LastEdit);
        Assert.Equal(0, insertedDm.Mentions.Count);
        Assert.Equal(0, insertedDm.Reactions.Count);
        Assert.Equal(0, insertedDm.Replies.Count);
        Assert.NotEqual(default(DateTime), insertedDm.SentAt);
        Assert.Equal(testUser.Id, insertedDm.UserId);
    }

    [Fact]
    public async void InsertDirectMessageGroups_ShouldInsertDirectMessagesGroups()
    {
        string emailPrefix = UserStore.GenerateTestEmail(10);
        string usernamePrefix = "dgcreator-un";
        List<List<User>> testMembers = new List<List<User>>();
        List<DirectMessageGroup> testGroups = new List<DirectMessageGroup>();
        Workspace directMessageGroupWorkspace = new Workspace
        {
            Description = "test-description",
            Name = "test-workspace-direct-message-group-name"
        };
        for (int i = 0; i < 10; i++)
        {
            string email1 =
                emailPrefix + DirectMessageGroupStore.GenerateRandomString(15);
            string email2 =
                emailPrefix + DirectMessageGroupStore.GenerateRandomString(15);
            string username1 =
                usernamePrefix
                + DirectMessageGroupStore.GenerateRandomString(10);
            string username2 =
                usernamePrefix
                + DirectMessageGroupStore.GenerateRandomString(10);
            User dmgUser1 = new User
            {
                FirstName = "test-ccreator-fname",
                LastName = "test-dmg-creator-lname",
                Timezone = UserStore.timezones[0].Id,
                UserName = username1,
                Email = email1,
                NormalizedEmail = email1.ToUpper(),
                NormalizedUserName = username1.ToUpper(),
                PhoneNumber = "1-234-456-7890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    UserStore.testPassword,
                    4
                ),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            User dmgUser2 = new User
            {
                FirstName = "test-ccreator-fname",
                LastName = "test-dmg-creator-lname",
                Timezone = UserStore.timezones[0].Id,
                UserName = username2,
                Email = email2,
                NormalizedEmail = email2.ToUpper(),
                NormalizedUserName = username2.ToUpper(),
                PhoneNumber = "1-234-456-7890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    UserStore.testPassword,
                    4
                ),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            testGroups.Add(
                new DirectMessageGroup
                {
                    Workspace = directMessageGroupWorkspace,
                }
            );
            testMembers.Add(new List<User> { dmgUser1, dmgUser2 });
        }
        _dbContext.AddRange(testMembers.SelectMany(pair => pair).ToList());
        await _dbContext.SaveChangesAsync();
        var inserted = await _directMessageGroupStore.InsertDirectMessageGroups(
            testGroups,
            testMembers
                .Select(pair => pair.Select(member => member.Id).ToList())
                .ToList()
        );

        foreach (
            (DirectMessageGroup ig, List<User> members) in inserted.Zip(
                testMembers
            )
        )
        {
            Assert.NotEqual(ig.Id, Guid.Empty);
            Assert.NotEqual(ig.ConcurrencyStamp, Guid.Empty);
            Assert.NotEqual(ig.CreatedAt, default(DateTime));
            Assert.NotNull(ig.DirectMessages);
            Assert.NotNull(ig.Files);
            Assert.Equal(ig.Workspace, directMessageGroupWorkspace);
            Assert.Equal(ig.WorkspaceId, directMessageGroupWorkspace.Id);
            foreach (
                (
                    DirectMessageGroupMember igMember,
                    User member
                ) in ig.DirectMessageGroupMembers.Zip(members)
            )
            {
                Assert.Equal(igMember.DirectMessageGroupId, ig.Id);
                Assert.Equal(igMember.User, member);
                Assert.Equal(igMember.UserId, member.Id);
            }
        }
    }

    [Fact]
    public async void InsertTestDirectMessageGroups_ShouldInsertTestDirectMessageGroups()
    {
        int initialDirectMessageGroups = _dbContext.DirectMessageGroups.Count();
        await _directMessageGroupStore.InsertTestDirectMessageGroups(100);
        int currentDirectMessageGroups = _dbContext.DirectMessageGroups.Count();
        Assert.Equal(
            initialDirectMessageGroups + 100,
            currentDirectMessageGroups
        );
    }
}
