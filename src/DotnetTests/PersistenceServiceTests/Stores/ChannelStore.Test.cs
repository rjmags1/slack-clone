using DotnetTests.Fixtures;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using Thread = PersistenceService.Models.Thread;
using PersistenceService.Stores;
using DotnetTests.PersistenceService.Utils;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace DotnetTests.PersistenceService.Stores;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class ChannelStoreTests1
{
    private readonly ApplicationDbContext _dbContext;

    private readonly ChannelStore _channelStore;

    public ChannelStoreTests1(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _channelStore = new ChannelStore(_dbContext);
    }

    [Fact]
    public async void LoadChannelMessages_ShouldWork()
    {
        var channelId = Guid.Parse("0d98a355-8ded-4e6b-b5c8-e3800311dcd3");
        var afterId = Guid.Parse("7650ac97-2a1e-427c-938b-53fc82d921c7"); // 5 after this
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
            "ChannelId",
            "IsReply",
            "LaterFlagId",
            "ReplyToId",
            "SentAt",
            "ThreadId",
        };

        (var messages1, var lastPage1) =
            await _channelStore.LoadChannelMessages(channelId, dbCols, 5);
        (var messages2, var lastPage2) =
            await _channelStore.LoadChannelMessages(
                channelId,
                dbCols,
                5,
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
    public async void LoadChannelMembers_ShouldWork()
    {
        List<string> cols =
            new()
            {
                "Id",
                "Admin",
                "EnableNotifications",
                "JoinedAt",
                "LastViewedAt",
                "Starred",
                "UserId"
            };

        var channelId = Guid.Parse("f5ce3455-07e6-41f3-a588-c89bbe42292a");
        var afterId = Guid.Parse("dd380ca2-53ff-4b0c-9bdf-189c8efca35b");

        (var members1, var lastPage1) = await _channelStore.LoadChannelMembers(
            3,
            cols,
            channelId
        );
        (var members2, var lastPage2) = await _channelStore.LoadChannelMembers(
            3,
            cols,
            channelId,
            afterId
        );

        Assert.False(lastPage1);
        Assert.Contains(afterId, members1.Select(m => m.Id));
        Assert.True(lastPage2);
        Assert.DoesNotContain(afterId, members2.Select(m => m.Id));
    }

    [Fact]
    public async void LoadChannels_ShouldWork()
    {
        List<string> cols =
            new()
            {
                "Id",
                "AllowThreads",
                "AvatarId",
                "AllowedPostersMask",
                "CreatedAt",
                "CreatedById",
                "Description",
                "Name",
                "NumMembers",
                "Private",
                "Topic",
                "WorkspaceId"
            };
        var workspaceId = Guid.Parse("985d9fea-bd6c-47ef-b811-f705ab34fcc6");
        var userId = Guid.Parse("fc9a8fb3-e95c-4968-8e09-c78456f49b3f");
        var afterId = Guid.Parse("36c1452f-f086-4b63-a7e6-5f12bf007ec2");
        (var channels1, var lastPage1) = await _channelStore.LoadChannels(
            workspaceId,
            userId,
            2,
            cols
        );
        (var channels2, var lastPage2) = await _channelStore.LoadChannels(
            workspaceId,
            userId,
            2,
            cols,
            afterId
        );
        Assert.Equal(2, channels1.Count);
        Assert.False(lastPage1);
        Assert.Single(channels2);
        Assert.True(lastPage2);
    }

        [Fact]
        public async void InsertMessageReaction_ShouldInsertReaction()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageReaction insertedReaction =
                await _channelStore.InsertMessageReaction(
                    testMessage.Id,
                    testUser.Id,
                    "🌍"
                );
    
            Assert.NotEqual(insertedReaction.Id, Guid.Empty);
            Assert.Equal(testMessage.Id, insertedReaction.ChannelMessageId);
            Assert.NotEqual(insertedReaction.CreatedAt, default(DateTime));
            Assert.Equal("🌍", insertedReaction.Emoji);
            Assert.Equal(testUser.Id, insertedReaction.UserId);
        }
    
        [Fact]
        public async void InsertMessageReaction_ShouldThrowOnNonMemberNonExistentMessageInvalidEmoji()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            await _dbContext.SaveChangesAsync();
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertMessageReaction(
                        Guid.Empty,
                        testUser.Id,
                        "🌍"
                    )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertMessageReaction(
                        testMessage.Id,
                        Guid.Empty,
                        "🌍"
                    )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertMessageReaction(
                        testMessage.Id,
                        testUser.Id,
                        ""
                    )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertMessageReaction(
                        testMessage.Id,
                        testUser.Id,
                        "🌍🌍"
                    )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertMessageReaction(
                        testMessage.Id,
                        testUser.Id,
                        "🌍x"
                    )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertMessageReaction(
                        testMessage.Id,
                        testUser.Id,
                        "x"
                    )
            );
    
            Assert.NotNull(
                await _channelStore.InsertMessageReaction(
                    testMessage.Id,
                    testUser.Id,
                    "🌍"
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
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
    
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ChannelMessage testReply = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            testReply.Thread = testThread;
            _dbContext.Add(testReply);
    
            ChannelMessageReply testReplyRecord =
                StoreTestUtils.CreateTestReplyRecord(
                    testReply,
                    testFirstMessage,
                    testUser,
                    testUser,
                    testThread
                );
            _dbContext.Add(testReplyRecord);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageNotification insertedReplyNotif =
                await _channelStore.InsertReplyNotification(testReplyRecord);
    
            Assert.NotEqual(insertedReplyNotif.Id, Guid.Empty);
            Assert.Equal(testReply.Id, insertedReplyNotif.ChannelMessageId);
            Assert.Equal(1, insertedReplyNotif.ChannelMessageNotificationType);
            Assert.NotEqual(insertedReplyNotif.CreatedAt, default(DateTime));
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
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
    
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ChannelMessage testReply = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            testReply.Thread = testThread;
            _dbContext.Add(testReply);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageReply testReplyRecord =
                StoreTestUtils.CreateTestReplyRecord(
                    testReply,
                    testFirstMessage,
                    testUser,
                    testUser,
                    testThread
                );
            _dbContext.Add(testReplyRecord);
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertReplyNotification(testReplyRecord)
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
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            ChannelMessageMention testMention =
                StoreTestUtils.CreateTestChannelMessageMention(
                    testMessage,
                    testUser,
                    testUser
                );
            _dbContext.Add(testMention);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageNotification? insertedMentionNotif = (
                await _channelStore.InsertMentionNotifications(
                    new List<ChannelMessageMention> { testMention }
                )
            ).FirstOrDefault();
    
            Assert.NotNull(insertedMentionNotif);
            Assert.NotEqual(insertedMentionNotif.Id, Guid.Empty);
            Assert.Equal(insertedMentionNotif.ChannelMessageId, testMessage.Id);
            Assert.Equal(2, insertedMentionNotif.ChannelMessageNotificationType);
            Assert.NotEqual(insertedMentionNotif.CreatedAt, default(DateTime));
            Assert.False(insertedMentionNotif.Seen);
            Assert.Equal(insertedMentionNotif.UserId, testUser.Id);
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
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageMention testMention =
                StoreTestUtils.CreateTestChannelMessageMention(
                    testMessage,
                    testUser,
                    testUser
                );
            _dbContext.Add(testMention);
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertMentionNotifications(
                        new List<ChannelMessageMention> { testMention }
                    )
            );
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
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            ChannelMessageReaction testReaction =
                StoreTestUtils.CreateTestChannelMessageReaction(
                    testMessage,
                    testUser
                );
            _dbContext.Add(testReaction);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageNotification insertedReactionNotif =
                await _channelStore.InsertReactionNotification(testReaction);
    
            Assert.NotEqual(insertedReactionNotif.Id, Guid.Empty);
            Assert.Equal(insertedReactionNotif.ChannelMessageId, testMessage.Id);
            Assert.Equal(4, insertedReactionNotif.ChannelMessageNotificationType);
            Assert.NotEqual(insertedReactionNotif.CreatedAt, default(DateTime));
            Assert.False(insertedReactionNotif.Seen);
            Assert.Equal(insertedReactionNotif.UserId, testUser.Id);
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
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageReaction testReaction =
                StoreTestUtils.CreateTestChannelMessageReaction(
                    testMessage,
                    testUser
                );
            _dbContext.Add(testReaction);
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertReactionNotification(testReaction)
            );
        }
    
        [Fact]
        public async void InsertThreadWatchNotifications_ShouldInsertThreadWatchNotifications()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
    
            testMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            ThreadWatch testThreadWatch = StoreTestUtils.CreateTestThreadWatch(
                testThread,
                testUser
            );
            _dbContext.Add(testThreadWatch);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageNotification? insertedTwNotif = (
                await _channelStore.InsertThreadWatchNotifications(
                    new List<ThreadWatch> { testThreadWatch },
                    testMessage.Id
                )
            ).FirstOrDefault();
    
            Assert.NotNull(insertedTwNotif);
            Assert.NotEqual(insertedTwNotif.Id, Guid.Empty);
            Assert.Equal(insertedTwNotif.ChannelMessageId, testMessage.Id);
            Assert.Equal(8, insertedTwNotif.ChannelMessageNotificationType);
            Assert.NotEqual(insertedTwNotif.CreatedAt, default(DateTime));
            Assert.False(insertedTwNotif.Seen);
            Assert.Equal(insertedTwNotif.UserId, testUser.Id);
        }
    
        [Fact]
        public async void InsertThreadWatchNotifications_ShouldThrowOnInvalidThreadWatchOrChannelMessageId()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
    
            testMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            ThreadWatch testThreadWatch = StoreTestUtils.CreateTestThreadWatch(
                testThread,
                testUser
            );
            _dbContext.Add(testThreadWatch);
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertThreadWatchNotifications(
                        new List<ThreadWatch> { testThreadWatch },
                        testMessage.Id
                    )
            );
    
            await _dbContext.SaveChangesAsync();
    
            Assert.NotNull(
                await _channelStore.InsertThreadWatchNotifications(
                    new List<ThreadWatch> { testThreadWatch },
                    testMessage.Id
                )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertThreadWatchNotifications(
                        new List<ThreadWatch> { testThreadWatch },
                        Guid.Empty
                    )
            );
        }
    
        [Fact]
        public async void InsertChannelMessageLaterFlag_ShouldInsertChannelMessageLaterFlag()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessageLaterFlag insertedLaterFlag =
                await _channelStore.InsertChannelMessageLaterFlag(
                    testMessage.Id,
                    testUser.Id
                );
    
            Assert.NotEqual(insertedLaterFlag.Id, Guid.Empty);
            Assert.Equal(testChannel.Id, insertedLaterFlag.ChannelId);
            Assert.Equal(1, insertedLaterFlag.ChannelLaterFlagStatus);
            Assert.Equal(testMessage.Id, insertedLaterFlag.ChannelMessageId);
            Assert.NotEqual(insertedLaterFlag.CreatedAt, default(DateTime));
            Assert.Equal(testUser.Id, insertedLaterFlag.UserId);
            Assert.Equal(testWorkspace.Id, insertedLaterFlag.WorkspaceId);
        }
    
        [Fact]
        public async void InsertChannelMessageLaterFlag_ShouldThrowOnNonexistentIds()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            Assert.NotNull(
                await _channelStore.InsertChannelMessageLaterFlag(
                    testMessage.Id,
                    testUser.Id
                )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertChannelMessageLaterFlag(
                        Guid.Empty,
                        testUser.Id
                    )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertChannelMessageLaterFlag(
                        testMessage.Id,
                        Guid.Empty
                    )
            );
        }
    
        [Fact]
        public async void InsertChannelMessageLaterFlag_ShouldThrowOnNonChannelMember()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testMessage = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testMessage);
    
            await _dbContext.SaveChangesAsync();
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertChannelMessageLaterFlag(
                        testMessage.Id,
                        testUser.Id
                    )
            );
        }
    
        [Fact]
        public async void InsertChannelMessage_ShouldInsertChannelMessage()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessage insertedChannelMessage =
                await _channelStore.InsertChannelMessage(
                    testChannel.Id,
                    "test content",
                    testUser.Id,
                    null,
                    null,
                    null,
                    null
                );
    
            Assert.NotEqual(insertedChannelMessage.Id, Guid.Empty);
            Assert.Equal(insertedChannelMessage.ChannelId, testChannel.Id);
            Assert.Equal("test content", insertedChannelMessage.Content);
            Assert.NotEqual(insertedChannelMessage.ConcurrencyStamp, Guid.Empty);
            Assert.NotEqual(insertedChannelMessage.CreatedAt, default(DateTime));
            Assert.False(insertedChannelMessage.Deleted);
            Assert.Null(insertedChannelMessage.LastEdit);
            Assert.NotNull(insertedChannelMessage.SentAt);
            Assert.NotEqual(insertedChannelMessage.SentAt, default(DateTime));
            Assert.Null(insertedChannelMessage.ThreadId);
            Assert.Equal(insertedChannelMessage.UserId, testUser.Id);
            Assert.Empty(insertedChannelMessage.Mentions);
            Assert.Empty(insertedChannelMessage.Reactions);
            Assert.Empty(insertedChannelMessage.Replies);
    
            ChannelMessage insertedChannelMessage2 =
                await _channelStore.InsertChannelMessage(
                    testChannel.Id,
                    "test content",
                    testUser.Id,
                    null,
                    testThread.Id,
                    testFirstMessage.Id,
                    testUser.Id,
                    true
                );
    
            Assert.Equal(3, testThread.NumMessages);
    
            Assert.NotEqual(insertedChannelMessage2.Id, Guid.Empty);
            Assert.Equal(insertedChannelMessage2.ChannelId, testChannel.Id);
            Assert.Equal("test content", insertedChannelMessage2.Content);
            Assert.NotEqual(insertedChannelMessage2.ConcurrencyStamp, Guid.Empty);
            Assert.NotEqual(insertedChannelMessage2.CreatedAt, default(DateTime));
            Assert.False(insertedChannelMessage2.Deleted);
            Assert.Null(insertedChannelMessage2.LastEdit);
            Assert.Null(insertedChannelMessage2.SentAt);
            Assert.NotEqual(insertedChannelMessage2.ThreadId, Guid.Empty);
            Assert.Equal(insertedChannelMessage2.UserId, testUser.Id);
            Assert.Empty(insertedChannelMessage.Mentions);
            Assert.Empty(insertedChannelMessage.Reactions);
            Assert.Equal(1, testFirstMessage.Replies.Count);
    
            ChannelMessageReply reply = testFirstMessage.Replies.First();
            Assert.NotEqual(reply.Id, Guid.Empty);
            Assert.Equal(reply.ChannelMessageId, insertedChannelMessage2.Id);
            Assert.Equal(reply.MessageRepliedToId, testFirstMessage.Id);
            Assert.Equal(reply.RepliedToId, testUser.Id);
            Assert.Equal(reply.ReplierId, testUser.Id);
            Assert.Equal(reply.ThreadId, testThread.Id);
        }
    
        [Fact]
        public async void InsertChannelMessage_ShouldThrowOnNonexistentIdsEmptyContent()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            Assert.NotNull(
                await _channelStore.InsertChannelMessage(
                    testChannel.Id,
                    "test content",
                    testUser.Id,
                    null,
                    null,
                    null,
                    null
                )
            );
    
            Assert.NotNull(
                await _channelStore.InsertChannelMessage(
                    testChannel.Id,
                    "test content",
                    testUser.Id,
                    null,
                    testThread.Id,
                    testFirstMessage.Id,
                    testUser.Id
                )
            );
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        Guid.Empty,
                        "test content",
                        testUser.Id,
                        null,
                        null,
                        null,
                        null
                    )
            );
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "",
                        testUser.Id,
                        null,
                        null,
                        null,
                        null
                    )
            );
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "test content",
                        Guid.Empty,
                        null,
                        null,
                        null,
                        null
                    )
            );
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "test content",
                        testUser.Id,
                        null,
                        Guid.Empty,
                        testFirstMessage.Id,
                        testUser.Id
                    )
            );
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "test content",
                        testUser.Id,
                        null,
                        testThread.Id,
                        Guid.Empty,
                        testUser.Id
                    )
            );
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "test content",
                        testUser.Id,
                        null,
                        testThread.Id,
                        testFirstMessage.Id,
                        Guid.Empty
                    )
            );
        }
    
        [Fact]
        public async void InsertChannelMessage_ShouldThrowOnInvalidNullArgCombos()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            Assert.NotNull(
                await _channelStore.InsertChannelMessage(
                    testChannel.Id,
                    "test content",
                    testUser.Id,
                    null,
                    testThread.Id,
                    testFirstMessage.Id,
                    testUser.Id
                )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "test content",
                        testUser.Id,
                        null,
                        null,
                        testFirstMessage.Id,
                        testUser.Id
                    )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "test content",
                        testUser.Id,
                        null,
                        testThread.Id,
                        null,
                        testUser.Id
                    )
            );
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "test content",
                        testUser.Id,
                        null,
                        testThread.Id,
                        testFirstMessage.Id,
                        null
                    )
            );
        }
    
        [Fact]
        public async void InsertChannelMessage_ShouldInsertMentionsOnSend()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessage insertedMessage =
                await _channelStore.InsertChannelMessage(
                    testChannel.Id,
                    "test content",
                    testUser.Id,
                    new List<Guid> { testUser.Id },
                    null,
                    null,
                    null
                );
    
            Assert.Equal(1, insertedMessage.Mentions.Count);
    
            ChannelMessageMention navMention = insertedMessage.Mentions.First();
            ChannelMessageMention? insertedMention =
                _dbContext.ChannelMessageMentions
                    .Where(
                        m =>
                            m.ChannelMessageId == insertedMessage.Id
                            && m.MentionedId == testUser.Id
                            && m.MentionerId == testUser.Id
                    )
                    .FirstOrDefault();
            Assert.NotNull(insertedMention);
            Assert.Equal(navMention.Id, insertedMention.Id);
            Assert.Equal(insertedMention.ChannelMessageId, insertedMessage.Id);
            Assert.NotEqual(insertedMention.CreatedAt, default(DateTime));
            Assert.Equal(insertedMention.MentionedId, testUser.Id);
            Assert.Equal(insertedMention.MentionerId, testUser.Id);
        }
    
        [Fact]
        public async void InsertChannelMessage_ShouldNotInsertDraftMentions()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMessage inserted = await _channelStore.InsertChannelMessage(
                testChannel.Id,
                "test content",
                testUser.Id,
                new List<Guid> { testUser.Id },
                testThread.Id,
                testFirstMessage.Id,
                testUser.Id,
                true
            );
    
            Assert.Equal(
                0,
                _dbContext.ChannelMessageMentions
                    .Where(
                        m =>
                            m.ChannelMessageId == inserted.Id
                            && m.MentionedId == testUser.Id
                            && m.MentionerId == testUser.Id
                    )
                    .Count()
            );
        }
    
        [Fact]
        public async void InsertChannelMessage_ShouldThrowOnInvalidMentions()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _channelStore.InsertChannelMessage(
                        testChannel.Id,
                        "test content",
                        testUser.Id,
                        new List<Guid> { Guid.Empty },
                        null,
                        null,
                        null
                    )
            );
    
            Assert.NotNull(
                await _channelStore.InsertChannelMessage(
                    testChannel.Id,
                    "test content",
                    testUser.Id,
                    new List<Guid> { testUser.Id },
                    null,
                    null,
                    null
                )
            );
        }
    
        [Fact]
        public async void InsertThreadWatch_ShouldInsertThreadWatch()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            Thread testThread = StoreTestUtils.CreateTestThread(
                testChannel,
                testFirstMessage,
                testWorkspace
            );
            _dbContext.Add(testThread);
    
            await _dbContext.SaveChangesAsync();
            testFirstMessage.ThreadId = testThread.Id;
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
    
            User testUser1 = StoreTestUtils.CreateTestUser();
            User testUser2 = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser1);
            _dbContext.Add(testUser2);
    
            WorkspaceMember testWorkspaceMembership1 =
                StoreTestUtils.CreateTestWorkspaceMember(testUser1, testWorkspace);
            _dbContext.Add(testWorkspaceMembership1);
            WorkspaceMember testWorkspaceMembership2 =
                StoreTestUtils.CreateTestWorkspaceMember(testUser2, testWorkspace);
            _dbContext.Add(testWorkspaceMembership2);
    
            Channel testChannel1 = StoreTestUtils.CreateTestChannel(
                testUser1,
                testWorkspace
            );
            _dbContext.Add(testChannel1);
            Channel testChannel2 = StoreTestUtils.CreateTestChannel(
                testUser2,
                testWorkspace
            );
            _dbContext.Add(testChannel2);
    
            ChannelMember testChannelMembership1 =
                StoreTestUtils.CreateTestChannelMember(
                    testUser1,
                    testChannel1,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership1);
            ChannelMember testChannelMembership2 =
                StoreTestUtils.CreateTestChannelMember(
                    testUser2,
                    testChannel2,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership2);
    
            ChannelMessage testFirstMessage1 =
                StoreTestUtils.CreateTestChannelMessage(testChannel1, testUser1);
            _dbContext.Add(testFirstMessage1);
            ChannelMessage testFirstMessage2 =
                StoreTestUtils.CreateTestChannelMessage(testChannel2, testUser2);
            _dbContext.Add(testFirstMessage2);
    
            Thread testThread1 = StoreTestUtils.CreateTestThread(
                testChannel1,
                testFirstMessage1,
                testWorkspace
            );
            Thread testThread2 = StoreTestUtils.CreateTestThread(
                testChannel2,
                testFirstMessage2,
                testWorkspace
            );
            _dbContext.Add(testThread1);
            _dbContext.Add(testThread2);
    
            await _dbContext.SaveChangesAsync();
    
            testFirstMessage1.ThreadId = testThread1.Id;
            testFirstMessage2.ThreadId = testThread2.Id;
    
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            ChannelMessage testReply = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testReply);
    
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            ChannelMessage testReply = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testReply);
    
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
    
            User testUser1 = StoreTestUtils.CreateTestUser();
            User testUser2 = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser1);
            _dbContext.Add(testUser2);
    
            WorkspaceMember testWorkspaceMembership1 =
                StoreTestUtils.CreateTestWorkspaceMember(testUser1, testWorkspace);
            _dbContext.Add(testWorkspaceMembership1);
            WorkspaceMember testWorkspaceMembership2 =
                StoreTestUtils.CreateTestWorkspaceMember(testUser2, testWorkspace);
            _dbContext.Add(testWorkspaceMembership2);
    
            Channel testChannel1 = StoreTestUtils.CreateTestChannel(
                testUser1,
                testWorkspace
            );
            _dbContext.Add(testChannel1);
            Channel testChannel2 = StoreTestUtils.CreateTestChannel(
                testUser2,
                testWorkspace
            );
            _dbContext.Add(testChannel2);
    
            ChannelMember testChannelMembership1 =
                StoreTestUtils.CreateTestChannelMember(
                    testUser1,
                    testChannel1,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership1);
            ChannelMember testChannelMembership2 =
                StoreTestUtils.CreateTestChannelMember(
                    testUser2,
                    testChannel2,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership2);
    
            ChannelMessage testFirstMessage1 =
                StoreTestUtils.CreateTestChannelMessage(testChannel1, testUser1);
            _dbContext.Add(testFirstMessage1);
            ChannelMessage testFirstMessage2 =
                StoreTestUtils.CreateTestChannelMessage(testChannel2, testUser2);
            _dbContext.Add(testFirstMessage2);
    
            ChannelMessage testReply1 = StoreTestUtils.CreateTestChannelMessage(
                testChannel1,
                testUser1
            );
            _dbContext.Add(testReply1);
            ChannelMessage testReply2 = StoreTestUtils.CreateTestChannelMessage(
                testChannel2,
                testUser2
            );
            _dbContext.Add(testReply2);
    
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
    
            User testUser = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testUser);
    
            WorkspaceMember testWorkspaceMembership1 =
                StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
            _dbContext.Add(testWorkspaceMembership1);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testUser,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testChannelMembership1 =
                StoreTestUtils.CreateTestChannelMember(
                    testUser,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership1);
    
            ChannelMessage testFirstMessage =
                StoreTestUtils.CreateTestChannelMessage(testChannel, testUser);
            _dbContext.Add(testFirstMessage);
    
            ChannelMessage testReply1 = StoreTestUtils.CreateTestChannelMessage(
                testChannel,
                testUser
            );
            _dbContext.Add(testReply1);
    
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            List<User> testMembers = new List<User>();
            List<WorkspaceMember> testWorkspaceMemberships =
                new List<WorkspaceMember>();
            for (int i = 0; i < 10; i++)
            {
                User user = StoreTestUtils.CreateTestUser();
                WorkspaceMember workspaceMembership =
                    StoreTestUtils.CreateTestWorkspaceMember(user, testWorkspace);
                testMembers.Add(user);
                testWorkspaceMemberships.Add(workspaceMembership);
            }
            _dbContext.AddRange(testMembers);
            _dbContext.AddRange(testWorkspaceMemberships);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testMembers.First(),
                testWorkspace
            );
            StoreTestUtils.CreateTestChannelMember(
                testMembers.First(),
                testChannel,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            await _dbContext.SaveChangesAsync();
    
            List<User> nonCreators = testMembers.TakeLast(9).ToList();
            List<ChannelMember> insertedMembers =
                await _channelStore.InsertChannelMembers(
                    testChannel.Id,
                    testWorkspace.Id,
                    nonCreators.Select(u => u.Id).ToList()
                );
    
            Assert.Equal(testMembers.Count, testChannel.NumMembers);
            foreach (
                (
                    ChannelMember channelMembership,
                    User member
                ) in insertedMembers.Zip(nonCreators)
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testMember = StoreTestUtils.CreateTestUser();
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testMember, testWorkspace);
            _dbContext.Add(testMember);
            _dbContext.Add(testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testMember,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            await _dbContext.SaveChangesAsync();
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMembers(
                        testChannel.Id,
                        testWorkspace.Id,
                        new List<Guid> { Guid.Empty }
                    )
            );
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMembers(
                        Guid.Empty,
                        testWorkspace.Id,
                        new List<Guid> { testMember.Id }
                    )
            );
    
            Assert.NotNull(
                await _channelStore.InsertChannelMembers(
                    testChannel.Id,
                    testWorkspace.Id,
                    new List<Guid> { testMember.Id }
                )
            );
        }
    
        [Fact]
        public async void InsertChannelMembers_ShouldThrowOnNonWorkspaceMembers()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testMember = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testMember);
            _dbContext.Add(testWorkspace);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testMember,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            await _dbContext.SaveChangesAsync();
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMembers(
                        testChannel.Id,
                        testWorkspace.Id,
                        new List<Guid> { testMember.Id }
                    )
            );
        }
    
        [Fact]
        public async void InsertChannelMembers_ShouldThrowOnAlreadyChannelMembers()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testMember = StoreTestUtils.CreateTestUser();
            WorkspaceMember testWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(testMember, testWorkspace);
            _dbContext.Add(testMember);
            _dbContext.Add(testWorkspace);
            _dbContext.Add(testWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testMember,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            await _dbContext.SaveChangesAsync();
    
            ChannelMember testChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testMember,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testChannelMembership);
    
            await _dbContext.SaveChangesAsync();
    
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _channelStore.InsertChannelMembers(
                        testChannel.Id,
                        testWorkspace.Id,
                        new List<Guid> { testMember.Id }
                    )
            );
        }
    
        [Fact]
        public async void InsertChannelInvites_ShouldInsertChannelInvites()
        {
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testInviter = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInviter);
            _dbContext.Add(testWorkspace);
            List<WorkspaceMember> testWorkspaceMemberships =
                new List<WorkspaceMember>();
            WorkspaceMember testInviterWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(
                    testInviter,
                    testWorkspace
                );
            _dbContext.Add(testInviterWorkspaceMembership);
            testWorkspaceMemberships.Add(testInviterWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testInviter,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testInviterChannelMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testInviter,
                    testChannel,
                    testWorkspace
                );
            testInviterChannelMembership.Admin = true;
            _dbContext.Add(testInviterChannelMembership);
    
            List<User> testInviteds = new List<User>();
            for (int i = 0; i < 10; i++)
            {
                User user = StoreTestUtils.CreateTestUser();
                WorkspaceMember workspaceMembership =
                    StoreTestUtils.CreateTestWorkspaceMember(user, testWorkspace);
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
            User testInviter = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInviter);
            User testInvited = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInvited);
    
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
            List<WorkspaceMember> testWorkspaceMemberships =
                new List<WorkspaceMember>();
            testWorkspaceMemberships.Add(
                StoreTestUtils.CreateTestWorkspaceMember(testInviter, testWorkspace)
            );
            testWorkspaceMemberships.Add(
                StoreTestUtils.CreateTestWorkspaceMember(testInvited, testWorkspace)
            );
            _dbContext.AddRange(testWorkspaceMemberships);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testInviter,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testInviterMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testInviter,
                    testChannel,
                    testWorkspace
                );
            testInviterMembership.Admin = true;
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
            User testInviter = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInviter);
            User testInvited = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInvited);
    
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
            List<WorkspaceMember> testWorkspaceMemberships =
                new List<WorkspaceMember>();
            testWorkspaceMemberships.Add(
                StoreTestUtils.CreateTestWorkspaceMember(testInviter, testWorkspace)
            );
            testWorkspaceMemberships.Add(
                StoreTestUtils.CreateTestWorkspaceMember(testInvited, testWorkspace)
            );
            _dbContext.AddRange(testWorkspaceMemberships);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testInviter,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testInviterMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testInviter,
                    testChannel,
                    testWorkspace
                );
            testInviterMembership.Admin = true;
            _dbContext.Add(testInviterMembership);
            ChannelMember testInvitedMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testInvited,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testInvitedMembership);
    
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
            User testInviter = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInviter);
            User testInvited = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInvited);
    
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
            List<WorkspaceMember> testWorkspaceMemberships =
                new List<WorkspaceMember>();
            testWorkspaceMemberships.Add(
                StoreTestUtils.CreateTestWorkspaceMember(testInviter, testWorkspace)
            );
            testWorkspaceMemberships.Add(
                StoreTestUtils.CreateTestWorkspaceMember(testInvited, testWorkspace)
            );
            _dbContext.AddRange(testWorkspaceMemberships);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testInviter,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testInviterMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testInviter,
                    testChannel,
                    testWorkspace
                );
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
            User testInviter = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInviter);
            User testInvited = StoreTestUtils.CreateTestUser();
            _dbContext.Add(testInvited);
    
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
            WorkspaceMember inviterWorkspaceMembership =
                StoreTestUtils.CreateTestWorkspaceMember(
                    testInviter,
                    testWorkspace
                );
            _dbContext.Add(inviterWorkspaceMembership);
    
            Channel testChannel = StoreTestUtils.CreateTestChannel(
                testInviter,
                testWorkspace
            );
            _dbContext.Add(testChannel);
    
            ChannelMember testInviterMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testInviter,
                    testChannel,
                    testWorkspace
                );
            testInviterMembership.Admin = true;
            _dbContext.Add(testInviterMembership);
            ChannelMember testInvitedMembership =
                StoreTestUtils.CreateTestChannelMember(
                    testInvited,
                    testChannel,
                    testWorkspace
                );
            _dbContext.Add(testInvitedMembership);
    
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
            Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
            _dbContext.Add(testWorkspace);
    
            User testUser = StoreTestUtils.CreateTestUser();
            List<Channel> channels = new List<Channel>();
            for (int i = 0; i < 10; i++)
            {
                channels.Add(
                    new Channel
                    {
                        CreatedBy = testUser,
                        Description = "test-description",
                        Name = "test-channel-name-" + i.ToString(),
                        Workspace = testWorkspace
                    }
                );
            }
    
            List<Channel> inserted = await _channelStore.InsertChannels(channels);
    
            foreach ((Channel ic, Channel c) in inserted.Zip(channels))
            {
                Assert.NotEqual(ic.Id, Guid.Empty);
                Assert.True(ic.AllowThreads);
                Assert.Null(ic.Avatar);
                Assert.Null(ic.AvatarId);
                Assert.Equal(1, ic.AllowedPostersMask);
                Assert.NotEqual(ic.CreatedAt, default(DateTime));
                Assert.Equal(ic.CreatedBy, testUser);
                Assert.Equal(ic.CreatedById, testUser.Id);
                Assert.NotEqual(ic.ConcurrencyStamp, Guid.Empty);
                Assert.Equal(ic.Description, c.Description);
                Assert.NotNull(ic.ChannelMembers);
                Assert.NotNull(ic.ChannelMessages);
                Assert.Equal(ic.Name, c.Name);
                Assert.False(ic.Private);
                Assert.Null(ic.Topic);
                Assert.Equal(ic.Workspace, testWorkspace);
                Assert.Equal(ic.WorkspaceId, testWorkspace.Id);
            }
        }
        */
}

[Trait("Category", "Order 2")]
[Collection("Database collection 2")]
public class ChannelStoreTests2
{
    private readonly ChannelStore _channelStore;

    private readonly ApplicationDbContext _dbContext;

    public ChannelStoreTests2(
        FilledApplicationDbContextFixture filledApplicationDbContextFixture
    )
    {
        _dbContext = filledApplicationDbContextFixture.Context;
        _channelStore = new ChannelStore(_dbContext);
    }

    [Fact]
    public void SeedHappened()
    {
        Assert.True(_dbContext.Channels.Count() > 0);
        Assert.True(_dbContext.Channels.Where(c => c.Private).Count() > 0);
        Assert.True(_dbContext.ChannelMembers.Count() > 0);
        Assert.True(_dbContext.ChannelMessages.Count() > 0);
        Assert.True(_dbContext.ChannelMessageLaterFlags.Count() > 0);
        Assert.True(_dbContext.ChannelMessageReactions.Count() > 0);
    }
}
