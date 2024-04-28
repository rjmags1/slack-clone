using DotnetTests.Fixtures;
using DotnetTests.PersistenceService.Utils;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace DotnetTests.PersistenceService.Stores;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class DirectMessageGroupStoreTests1
{
    private readonly ApplicationDbContext _dbContext;

    private readonly DirectMessageGroupStore _directMessageGroupStore;

    public DirectMessageGroupStoreTests1(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _directMessageGroupStore = new DirectMessageGroupStore(_dbContext);
    }

    [Fact]
    public async void LoadDirectMessages_ShouldWork()
    {
        var dmgId = Guid.Parse("09ff2bb0-4285-4a9f-a6b7-4f935d3b3d3d");
        var afterId = Guid.Parse("be905f54-957d-44d3-84c3-b60faae9f492"); // 3 after this
        var dbCols = new List<string>()
        {
            "Id",
            "Mentions",
            "Reactions",
            "Files",
            "UserId",
            "Content",
            "CreatedAt",
            "LastEdit",
            "DirectMessageGroupId",
            "IsReply",
            "LaterFlagId",
            "ReplyToId",
            "SentAt",
        };

        (var messages1, var lastPage1) =
            await _directMessageGroupStore.LoadDirectMessages(dmgId, dbCols, 3);
        (var messages2, var lastPage2) =
            await _directMessageGroupStore.LoadDirectMessages(
                dmgId,
                dbCols,
                3,
                afterId
            );

        Assert.False(lastPage1);
        var sortedBySentDesc1 = messages1
            .Select(m => m.SentAt)
            .OrderByDescending(dt => dt);
        Assert.Equal(sortedBySentDesc1, messages1.Select(m => m.SentAt));

        Assert.True(lastPage2);
        var sortedBySentDesc2 = messages2
            .Select(m => m.SentAt)
            .OrderByDescending(dt => dt);
        Assert.Equal(sortedBySentDesc2, messages2.Select(m => m.SentAt));
        Assert.DoesNotContain(afterId, messages2.Select(m => m.Id));
    }

    /*
    [Fact]
    public async void LoadDirectMessageGroups_ShouldWork()
    {
        var workspaceId = Guid.Parse("23e33ae1-c69b-4e33-bb16-79a1be666392");
        var userId = Guid.Parse("1903d315-3d90-4a82-8ccb-c23ec7bf834b");
        var afterId = Guid.Parse("de867492-491e-4e37-ae00-e99592b60532");
        List<string> dbCols = new() { "Id", "CreatedAt" };
        (var dmgs1, var lastPage1) =
            await _directMessageGroupStore.LoadDirectMessageGroups(
                workspaceId,
                userId,
                3,
                dbCols
            );
        (var dmgs2, var lastPage2) =
            await _directMessageGroupStore.LoadDirectMessageGroups(
                workspaceId,
                userId,
                3,
                dbCols,
                afterId
            );
        dbCols.Add("WorkspaceId");
        (var dmgs3, var lastPage3) =
            await _directMessageGroupStore.LoadDirectMessageGroups(
                workspaceId,
                userId,
                3,
                dbCols,
                afterId
            );
        Assert.Equal(3, dmgs1.Count);
        Assert.False(lastPage1);
        Assert.Equal(3, dmgs2.Count);
        Assert.True(lastPage2);
        Assert.Equal(3, dmgs3.Count);
        Assert.True(lastPage3);
        foreach ((var dmg1, var dmg2, var dmg3) in dmgs1.Zip(dmgs2, dmgs3))
        {
            Assert.True(dmg1.Name.Split(",").Length > 1);
            Assert.True(dmg2.Name.Split(",").Length > 1);
            Assert.True(dmg3.Name.Split(",").Length > 1);
        }
    }

    [Fact]
    public async void InsertDirectMessageGroupMembers_ShouldInsertDirectMessageGroupMembers()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        await _dbContext.SaveChangesAsync();

        DirectMessageGroupMember? insertedMember = (
            await _directMessageGroupStore.InsertDirectMessageGroupMembers(
                testGroup.Id,
                testWorkspace.Id,
                new List<Guid> { testUser.Id }
            )
        ).FirstOrDefault();

        Assert.NotNull(insertedMember);
        Assert.NotEqual(insertedMember.Id, Guid.Empty);
        Assert.Equal(insertedMember.DirectMessageGroupId, testGroup.Id);
        Assert.Null(insertedMember.LastViewedAt);
        Assert.Equal(insertedMember.UserId, testUser.Id);
    }

    [Fact]
    public async void InsertDirectMessageGroupMembers_ShouldThrowOnInvalidIds()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessageGroupMembers(
                    Guid.Empty,
                    testWorkspace.Id,
                    new List<Guid> { testUser.Id }
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessageGroupMembers(
                    testGroup.Id,
                    testWorkspace.Id,
                    new List<Guid> { Guid.Empty }
                )
        );

        DirectMessageGroupMember testGroupMember =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testGroupMember);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _directMessageGroupStore.InsertDirectMessageGroupMembers(
                    testGroup.Id,
                    testWorkspace.Id,
                    new List<Guid> { testUser.Id }
                )
        );
    }

    [Fact]
    public async void InsertMessageReaction_ShouldInsertMessageReaction()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testMessage);

        await _dbContext.SaveChangesAsync();

        DirectMessageReaction insertedReaction =
            await _directMessageGroupStore.InsertMessageReaction(
                testMessage.Id,
                testUser.Id,
                "🇺🇸"
            );

        Assert.NotEqual(Guid.Empty, insertedReaction.Id);
        Assert.NotEqual(default(DateTime), insertedReaction.CreatedAt);
        Assert.Equal(testMessage.Id, insertedReaction.DirectMessageId);
        Assert.Equal("🇺🇸", insertedReaction.Emoji);
        Assert.Equal(testUser.Id, insertedReaction.UserId);
    }

    [Fact]
    public async void InsertMessageReaction_ShouldThrowOnInvalidArgs()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testMessage);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertMessageReaction(
                    Guid.Empty,
                    testUser.Id,
                    "🇺🇸"
                )
        );

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertMessageReaction(
                    testMessage.Id,
                    Guid.Empty,
                    "🇺🇸"
                )
        );

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertMessageReaction(
                    testMessage.Id,
                    Guid.Empty,
                    ""
                )
        );

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertMessageReaction(
                    testMessage.Id,
                    Guid.Empty,
                    " 🇺🇸"
                )
        );

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertMessageReaction(
                    testMessage.Id,
                    Guid.Empty,
                    "a"
                )
        );

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertMessageReaction(
                    testMessage.Id,
                    Guid.Empty,
                    "a🇺🇸"
                )
        );

        Assert.NotNull(
            await _directMessageGroupStore.InsertMessageReaction(
                testMessage.Id,
                testUser.Id,
                "🇺🇸"
            )
        );
    }

    [Fact]
    public async void InsertReplyNotification_ShouldInsertReplyNotification()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testMessage);

        DirectMessage testReply = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testReply);

        DirectMessageReply testReplyRecord =
            StoreTestUtils.CreateTestDirectMessageReplyRecord(
                testReply,
                testMessage,
                testUser,
                testUser
            );
        _dbContext.Add(testReplyRecord);

        await _dbContext.SaveChangesAsync();

        DirectMessageNotification insertedReplyNotif =
            await _directMessageGroupStore.InsertReplyNotification(
                testReplyRecord
            );

        Assert.NotEqual(Guid.Empty, insertedReplyNotif.Id);
        Assert.NotEqual(default(DateTime), insertedReplyNotif.CreatedAt);
        Assert.Equal(testReply.Id, insertedReplyNotif.DirectMessageId);
        Assert.Equal(1, insertedReplyNotif.DirectMessageNotificationType);
        Assert.False(insertedReplyNotif.Seen);
        Assert.Equal(testUser.Id, insertedReplyNotif.UserId);
    }

    [Fact]
    public async void InsertReplyNotification_ShouldThrowOnInvalidReplyRecord()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testMessage);

        DirectMessage testReply = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testReply);

        DirectMessageReply testReplyRecord =
            StoreTestUtils.CreateTestDirectMessageReplyRecord(
                testReply,
                testMessage,
                testUser,
                testUser
            );

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertReplyNotification(
                    testReplyRecord
                )
        );

        _dbContext.Add(testReplyRecord);
        await _dbContext.SaveChangesAsync();

        Assert.NotNull(
            await _directMessageGroupStore.InsertReplyNotification(
                testReplyRecord
            )
        );
    }

    [Fact]
    public async void InsertMentionNotifications_ShouldInsertMentionNotifications()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testMessage);

        DirectMessageMention testMention =
            StoreTestUtils.CreateTestDirectMessageMention(
                testMessage,
                testUser,
                testUser
            );
        _dbContext.Add(testMention);

        await _dbContext.SaveChangesAsync();

        DirectMessageNotification? insertedMentionNotif = (
            await _directMessageGroupStore.InsertMentionNotifications(
                new List<DirectMessageMention> { testMention }
            )
        ).FirstOrDefault();

        Assert.NotNull(insertedMentionNotif);
        Assert.NotEqual(default(DateTime), insertedMentionNotif.CreatedAt);
        Assert.Equal(testMessage.Id, insertedMentionNotif.DirectMessageId);
        Assert.Equal(2, insertedMentionNotif.DirectMessageNotificationType);
        Assert.False(insertedMentionNotif.Seen);
        Assert.Equal(testUser.Id, insertedMentionNotif.UserId);
    }

    [Fact]
    public async void InsertMentionNotifications_ShouldThrowOnInvalidMentionRecord()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testMessage);

        DirectMessageMention testMention =
            StoreTestUtils.CreateTestDirectMessageMention(
                testMessage,
                testUser,
                testUser
            );

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertMentionNotifications(
                    new List<DirectMessageMention> { testMention }
                )
        );

        _dbContext.Add(testMention);
        await _dbContext.SaveChangesAsync();

        DirectMessageNotification? insertedMentionNotif = (
            await _directMessageGroupStore.InsertMentionNotifications(
                new List<DirectMessageMention> { testMention }
            )
        ).FirstOrDefault();
    }

    [Fact]
    public async void InsertReactionNotification_ShouldInsertReactionNotification()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testMessage);

        await _dbContext.SaveChangesAsync();

        DirectMessageReaction testReaction =
            StoreTestUtils.CreateTestDirectMessageReaction(
                testMessage,
                testUser
            );
        _dbContext.Add(testReaction);

        await _dbContext.SaveChangesAsync();

        DirectMessageNotification insertedReactionNotif =
            await _directMessageGroupStore.InsertReactionNotification(
                testReaction
            );

        Assert.NotEqual(Guid.Empty, insertedReactionNotif.Id);
        Assert.NotEqual(default(DateTime), insertedReactionNotif.CreatedAt);
        Assert.Equal(testMessage.Id, insertedReactionNotif.DirectMessageId);
        Assert.Equal(4, insertedReactionNotif.DirectMessageNotificationType);
        Assert.False(insertedReactionNotif.Seen);
        Assert.Equal(testUser.Id, insertedReactionNotif.UserId);
    }

    [Fact]
    public async void InsertReactionNotification_ShouldThrowOnInvalidReactionRecord()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
        _dbContext.Add(testMessage);

        await _dbContext.SaveChangesAsync();

        DirectMessageReaction testReaction =
            StoreTestUtils.CreateTestDirectMessageReaction(
                testMessage,
                testUser
            );
        _dbContext.Add(testReaction);

        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _directMessageGroupStore.InsertReactionNotification(
                    testReaction
                )
        );

        await _dbContext.SaveChangesAsync();

        DirectMessageNotification insertedReactionNotif =
            await _directMessageGroupStore.InsertReactionNotification(
                testReaction
            );
    }

    [Fact]
    public async void InsertDirectMessageLaterFlag_ShouldInsertDirectMessageLaterFlag()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
        _dbContext.Add(testDmgMembership);

        DirectMessage testMessage = StoreTestUtils.CreateTestDirectMessage(
            testGroup,
            testUser
        );
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        DirectMessageGroup testGroup =
            StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace);
        _dbContext.Add(testGroup);

        DirectMessageGroupMember testDmgMembership =
            StoreTestUtils.CreateTestDirectMessageGroupMember(
                testUser,
                testWorkspace,
                testGroup
            );
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
        List<List<User>> testMembers = new List<List<User>>();
        List<DirectMessageGroup> testGroups = new List<DirectMessageGroup>();
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        for (int i = 0; i < 10; i++)
        {
            User dmgUser1 = StoreTestUtils.CreateTestUser();
            User dmgUser2 = StoreTestUtils.CreateTestUser();
            testGroups.Add(
                StoreTestUtils.CreateTestDirectMessageGroup(testWorkspace)
            );
            testMembers.Add(new List<User> { dmgUser1, dmgUser2 });
        }
        _dbContext.Add(testWorkspace);
        _dbContext.AddRange(testMembers.SelectMany(pair => pair).ToList());

        await _dbContext.SaveChangesAsync();

        var inserted = await _directMessageGroupStore.InsertDirectMessageGroups(
            testGroups,
            testMembers
                .Select(pair => pair.Select(member => member.Id).ToList())
                .ToList(),
            testWorkspace.Id
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
            Assert.Equal(ig.Workspace, testWorkspace);
            Assert.Equal(ig.WorkspaceId, testWorkspace.Id);
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
    */
}

[Trait("Category", "Order 2")]
[Collection("Database collection 2")]
public class DirectMessageGroupStoreTests2
{
    private readonly DirectMessageGroupStore _directMessageGroupStore;

    private readonly ApplicationDbContext _dbContext;

    public DirectMessageGroupStoreTests2(
        FilledApplicationDbContextFixture filledApplicationDbContextFixture
    )
    {
        _dbContext = filledApplicationDbContextFixture.Context;
        _directMessageGroupStore = new DirectMessageGroupStore(_dbContext);
    }

    [Fact]
    public void SeedHappened()
    {
        Assert.True(_dbContext.DirectMessageGroups.Count() > 0);
        Assert.True(_dbContext.DirectMessages.Count() > 0);
        Assert.True(_dbContext.DirectMessageLaterFlags.Count() > 0);
        Assert.True(_dbContext.DirectMessageReactions.Count() > 0);
    }
}
