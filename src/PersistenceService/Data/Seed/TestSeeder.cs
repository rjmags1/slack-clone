using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;
using Thread = PersistenceService.Models.Thread;

namespace PersistenceService.Data.SeedData;

// TODO: seed some channel and direct message group memberships as starred

public class TestSeeder
{
    public ApplicationDbContext context { get; private set; }
    public static string[] Emojis = { "🙋", "😃", "👍", "😠", "👎" };
    private readonly ThemeStore _themeStore;
    private readonly UserStore _userStore;
    private readonly WorkspaceStore _workspaceStore;
    private readonly ChannelStore _channelStore;
    private readonly DirectMessageGroupStore _directMessageGroupStore;
    public const string TestPassword = "Test_password1";
    public string HashedTestPassword = BCrypt.Net.BCrypt.HashPassword(
        TestPassword
    );
    public const string Large = "LARGE";
    public const string Small = "SMALL";

    public TestSeeder(ApplicationDbContext dbContext, UserStore userStore)
    {
        context = dbContext;
        _themeStore = new ThemeStore(context);
        _userStore = userStore;
        _workspaceStore = new WorkspaceStore(context);
        _channelStore = new ChannelStore(context);
        _directMessageGroupStore = new DirectMessageGroupStore(context);
    }

    public async Task Seed(string size)
    {
        if (size != Large && size != Small)
        {
            throw new ArgumentException(
                "Can only seed 'Large' or 'Small' sample data sizes"
            );
        }
        var startTime = DateTime.Now.TimeOfDay.TotalSeconds;
        Console.WriteLine($"Database seeding start. Seed size: {size}");

        int numUsers = size == Large ? 10000 : 100;
        int numWorkspaces = size == Large ? 100 : 5;
        int minChannelsPerWorkspace = 2;
        int maxChannelsPerWorkspace = size == Large ? 10 : 6;
        int minMembersPerChannel = 2;
        int dmGroupsPerWorkspace = size == Large ? 1000 : 10;
        int threadsPerChannel = size == Large ? 10 : 2;
        int messagesPerChannel = size == Large ? 400 : 20;
        int messagesInLargeThread = size == Large ? 200 : 10;
        int messagesInSmallThreads = size == Large ? 5 : 3;
        int messagesPerDmGroup = size == Large ? 20 : 5;

        List<Theme> shippedThemes = await _themeStore.InsertShippedThemes();

        List<User> testUsers = await InsertTestUsers(numUsers);

        User devUser = testUsers.First();

        List<Workspace> testWorkspaces = await InsertTestWorkspaces(
            numWorkspaces
        );

        List<List<WorkspaceMember>> testWorkspaceMembers =
            await EnrollIntoWorkspaces(devUser, testUsers, testWorkspaces);

        List<List<Channel>> testChannels = await InsertTestChannels(
            testWorkspaceMembers,
            testWorkspaces,
            minChannelsPerWorkspace,
            maxChannelsPerWorkspace
        );

        await MakeSomeChannelsPrivate(testChannels);

        List<List<List<ChannelMember>>> channelMembers =
            await EnrollChannelMembers(
                devUser,
                testChannels,
                testWorkspaceMembers,
                minMembersPerChannel
            );

        (
            List<List<DirectMessageGroup>> testDirectMessageGroups,
            List<List<List<Guid>>> testDirectMessageGroupMembers // Guids are user ids
        ) = await InsertTestDirectMessageGroups(
            testWorkspaceMembers,
            testWorkspaces,
            dmGroupsPerWorkspace
        );

        (
            List<List<List<ChannelMessage>>> testChannelMessages,
            List<List<List<Thread>>> testChannelThreads
        ) = await InsertTestChannelMessages(
            testChannels,
            channelMembers,
            threadsPerChannel,
            messagesPerChannel,
            messagesInLargeThread,
            messagesInSmallThreads
        );

        List<List<List<DirectMessage>>> testDirectMessages =
            await InsertTestDirectMessages(
                testDirectMessageGroups,
                testDirectMessageGroupMembers,
                messagesPerDmGroup
            );

        await InsertChannelMessageReactionsLaterFlags(
            testChannelMessages,
            channelMembers
        );

        await InsertDirectMessageReactionsLaterFlags(
            testDirectMessages,
            testDirectMessageGroupMembers
        );

        var stopTime = DateTime.Now.TimeOfDay.TotalSeconds;
        var elapsedTime = stopTime - startTime;
        Console.WriteLine($"Seeding complete. Elapsed time: {elapsedTime}");
    }

    public async Task InsertChannelMessageReactionsLaterFlags(
        List<List<List<ChannelMessage>>> testChannelMessages,
        List<List<List<ChannelMember>>> testChannelMembers
    )
    {
        foreach (
            (
                List<List<ChannelMessage>> workspaceChannelMessages,
                List<List<ChannelMember>> workspaceChannelMembers
            ) in testChannelMessages.Zip(testChannelMembers)
        )
        {
            foreach (
                (
                    List<ChannelMessage> channelMessages,
                    List<ChannelMember> channelMembers
                ) in workspaceChannelMessages.Zip(workspaceChannelMembers)
            )
            {
                foreach (
                    (ChannelMessage message, int i) in channelMessages.Select(
                        (m, i) => (m, i)
                    )
                )
                {
                    if (i >= channelMessages.Count - 5)
                    {
                        for (int j = 0; j < Store.random.Next(12); j++)
                        {
                            await _channelStore.InsertMessageReaction(
                                message.Id,
                                channelMembers[
                                    Store.random.Next(channelMembers.Count)
                                ].UserId,
                                Emojis[Store.random.Next(Emojis.Count())]
                            );
                        }
                    }

                    if (
                        i == channelMessages.Count - 1
                        || Store.random.Next(10) > 7
                    )
                    {
                        await _channelStore.InsertChannelMessageLaterFlag(
                            message.Id,
                            channelMembers[
                                Store.random.Next(channelMembers.Count)
                            ].UserId
                        );
                    }
                }
            }
        }
    }

    public async Task InsertDirectMessageReactionsLaterFlags(
        List<List<List<DirectMessage>>> testDirectMessages,
        List<List<List<Guid>>> testDirectMessageGroupMembers
    )
    {
        foreach (
            (
                List<List<DirectMessage>> workspaceDirectMessages,
                List<List<Guid>> workspaceDirectMessageMembers
            ) in testDirectMessages.Zip(testDirectMessageGroupMembers)
        )
        {
            foreach (
                (
                    List<DirectMessage> directMessages,
                    List<Guid> groupMembers
                ) in workspaceDirectMessages.Zip(workspaceDirectMessageMembers)
            )
            {
                foreach (
                    (DirectMessage message, int i) in directMessages.Select(
                        (m, i) => (m, i)
                    )
                )
                {
                    if (i >= directMessages.Count - 5)
                    {
                        for (int j = 0; j < Store.random.Next(12); j++)
                        {
                            await _directMessageGroupStore.InsertMessageReaction(
                                message.Id,
                                groupMembers[
                                    Store.random.Next(groupMembers.Count)
                                ],
                                Emojis[Store.random.Next(Emojis.Count())]
                            );
                        }
                    }

                    if (
                        i == directMessages.Count - 1
                        || Store.random.Next(10) == 1
                    )
                    {
                        await _directMessageGroupStore.InsertDirectMessageLaterFlag(
                            message.Id,
                            groupMembers[Store.random.Next(groupMembers.Count)]
                        );
                    }
                }
            }
        }
    }

    public async Task<List<List<List<DirectMessage>>>> InsertTestDirectMessages(
        List<List<DirectMessageGroup>> testDirectMessageGroups,
        List<List<List<Guid>>> testDirectMessageGroupMembers,
        int messagesPerGroup,
        bool lastMessageIsDraft = true
    )
    {
        List<List<List<DirectMessage>>> messages =
            new List<List<List<DirectMessage>>>();
        foreach (
            (
                List<DirectMessageGroup> workspaceGroups,
                List<List<Guid>> workspaceGroupMembers
            ) in testDirectMessageGroups.Zip(testDirectMessageGroupMembers)
        )
        {
            List<List<DirectMessage>> workspaceMessages =
                new List<List<DirectMessage>>();
            foreach (
                (
                    DirectMessageGroup group,
                    List<Guid> members
                ) in workspaceGroups.Zip(workspaceGroupMembers)
            )
            {
                List<DirectMessage> groupMessages = new List<DirectMessage>();
                for (int i = 0; i < messagesPerGroup; i++)
                {
                    Guid authorId = members[i % members.Count];
                    bool isReply = i > 0 && i % 7 == 0;
                    groupMessages.Add(
                        await _directMessageGroupStore.InsertDirectMessage(
                            group.Id,
                            "test content",
                            authorId,
                            null,
                            isReply ? groupMessages[i - 1].Id : null,
                            isReply ? members[(i - 1) % members.Count] : null,
                            lastMessageIsDraft && i == messagesPerGroup - 1
                        )
                    );
                }
                workspaceMessages.Add(groupMessages);
            }
            messages.Add(workspaceMessages);
        }

        return messages;
    }

    private async Task<(
        List<List<List<ChannelMessage>>>,
        List<List<List<Thread>>>
    )> InsertTestChannelMessages(
        List<List<Channel>> testChannels,
        List<List<List<ChannelMember>>> testMembers,
        int threadsPerChannel,
        int numMessagesPerChannel,
        int messagesInLongThread,
        int messagesInShortThreads,
        bool lastNonThreadMsgIsDraft = true,
        bool watchThreads = true
    )
    {
        if (threadsPerChannel < 1)
        {
            throw new ArgumentException("Invalid threadsPerChannel");
        }
        int topLevelNonThreadMessages =
            numMessagesPerChannel
            - messagesInLongThread
            - ((threadsPerChannel - 1) * messagesInShortThreads);
        if (topLevelNonThreadMessages < 0)
        {
            throw new ArgumentException("Too many thread messages");
        }

        List<List<List<ChannelMessage>>> messages =
            new List<List<List<ChannelMessage>>>();
        List<List<List<Thread>>> threads = new List<List<List<Thread>>>();
        foreach (
            (
                List<Channel> workspaceChannels,
                List<List<ChannelMember>> workspaceChannelMembers
            ) in testChannels.Zip(testMembers)
        )
        {
            List<List<ChannelMessage>> workspaceChannelMessages =
                new List<List<ChannelMessage>>();
            List<List<Thread>> workspaceChannelThreads =
                new List<List<Thread>>();
            foreach (
                (
                    Channel channel,
                    List<ChannelMember> channelMembers
                ) in workspaceChannels.Zip(workspaceChannelMembers)
            )
            {
                List<ChannelMessage> channelMessages =
                    new List<ChannelMessage>();
                List<Thread> channelThreads = new List<Thread>();
                for (int m = 0; m < topLevelNonThreadMessages; m++)
                {
                    channelMessages.Add(
                        await _channelStore.InsertChannelMessage(
                            channel.Id,
                            "test content",
                            channelMembers[m % channelMembers.Count].UserId,
                            null,
                            null,
                            null,
                            null,
                            m == topLevelNonThreadMessages - 1
                                && lastNonThreadMsgIsDraft
                        )
                    );
                }

                for (int t = 0; t < threadsPerChannel; t++)
                {
                    int numMessages =
                        t == 0 ? messagesInLongThread : messagesInShortThreads;
                    (
                        Thread insertedThread,
                        List<ChannelMessage> threadMessages
                    ) = await InsertTestThread(
                        channel,
                        channelMembers,
                        numMessages,
                        watchThreads
                    );

                    channelThreads.Add(insertedThread);
                    channelMessages.AddRange(threadMessages);
                }
                workspaceChannelThreads.Add(channelThreads);
                workspaceChannelMessages.Add(channelMessages);
            }
            threads.Add(workspaceChannelThreads);
            messages.Add(workspaceChannelMessages);
        }

        return (messages, threads);
    }

    private async Task<(Thread, List<ChannelMessage>)> InsertTestThread(
        Channel testChannel,
        List<ChannelMember> testMembers,
        int numMessages,
        bool watchThreads = true,
        bool addMentions = true
    )
    {
        ChannelMessage firstMessage = await _channelStore.InsertChannelMessage(
            testChannel.Id,
            "test content",
            testMembers[0].UserId,
            new List<Guid>() { testMembers[1].UserId }
        );
        ChannelMessage reply = CreateTestChannelMessage(
            testChannel.Id,
            testMembers[1].UserId
        );
        Thread thread = await _channelStore.InsertThread(
            testChannel.Id,
            firstMessage.Id,
            reply
        );

        List<ChannelMessage> messages = new List<ChannelMessage>
        {
            firstMessage,
            reply
        };
        HashSet<Guid> watching = new HashSet<Guid>();
        for (int i = 2; i < numMessages; i++)
        {
            int k = Store.random.Next(1, 3);
            Guid repliedToId = testMembers[(i - k) % testMembers.Count].UserId;
            Guid authorId = testMembers[i % testMembers.Count].UserId;
            bool includeMention = addMentions && Store.random.Next(100) < 15;
            messages.Add(
                await _channelStore.InsertChannelMessage(
                    testChannel.Id,
                    "test content",
                    authorId,
                    !includeMention ? null : new List<Guid> { repliedToId },
                    thread.Id,
                    messages[i - k].Id,
                    repliedToId
                )
            );
            if (Store.random.Next(10) < 3)
            {
                watching.Add(authorId);
            }
        }
        foreach (Guid userId in watching)
        {
            await _channelStore.InsertThreadWatch(userId, thread.Id);
        }

        return (thread, messages);
    }

    private async Task<(
        List<List<DirectMessageGroup>>,
        List<List<List<Guid>>>
    )> InsertTestDirectMessageGroups(
        List<List<WorkspaceMember>> testWorkspaceMembers,
        List<Workspace> testWorkspaces,
        int groupsPerWorkspace
    )
    {
        List<List<DirectMessageGroup>> groups =
            new List<List<DirectMessageGroup>>();
        List<List<List<Guid>>> dmgMembers = new List<List<List<Guid>>>();
        foreach (
            (
                List<WorkspaceMember> members,
                Workspace workspace
            ) in testWorkspaceMembers.Zip(testWorkspaces)
        )
        {
            List<DirectMessageGroup> addedGroups =
                new List<DirectMessageGroup>();
            List<List<Guid>> addedGroupMembers = new List<List<Guid>>();
            HashSet<(int, int)> pairs = new HashSet<(int, int)>();
            for (int i = 0; i < groupsPerWorkspace; i++)
            {
                DirectMessageGroup currGroup = CreateTestDirectMessageGroup(
                    workspace
                );
                addedGroups.Add(currGroup);
                (int, int) idxPair = RandomDistinctOrderedIntPair(
                    0,
                    members.Count - 1
                );
                while (pairs.Contains(idxPair))
                {
                    idxPair = RandomDistinctOrderedIntPair(
                        0,
                        members.Count - 1
                    );
                }
                (int j, int k) = idxPair;
                addedGroupMembers.Add(
                    new List<Guid> { members[j].UserId, members[k].UserId }
                );
            }
            await _directMessageGroupStore.InsertDirectMessageGroups(
                addedGroups,
                addedGroupMembers,
                workspace.Id
            );
            groups.Add(addedGroups);
            dmgMembers.Add(addedGroupMembers);
        }

        return (groups, dmgMembers);
    }

    private static (int, int) RandomDistinctOrderedIntPair(int min, int max)
    {
        if (min >= max)
        {
            throw new ArgumentException("Invalid args");
        }
        int first = Store.random.Next(min, max + 1);
        int second = first;
        while (second == first)
        {
            second = Store.random.Next(min, max + 1);
        }

        return first < second ? (first, second) : (second, first);
    }

    private async Task<List<User>> InsertTestUsers(int numUsers)
    {
        List<User> testUsers = new List<User>();
        for (int i = 0; i < numUsers; i++)
        {
            var user = CreateTestUser();
            if (i == 0)
            {
                user.UserName = "dev";
                user.NormalizedUserName = "dev".ToUpper();
                user.Email = "dev@test.com";
                user.NormalizedEmail = "dev@test.com".ToUpper();
            }
            testUsers.Add(user);
        }

        context.AddRange(testUsers);
        await context.SaveChangesAsync();
        return testUsers;
    }

    private async Task<List<Workspace>> InsertTestWorkspaces(int numWorkspaces)
    {
        List<Workspace> testWorkspaces = new List<Workspace>();
        for (int _ = 0; _ < numWorkspaces; _++)
        {
            testWorkspaces.Add(CreateTestWorkspace());
        }

        return await _workspaceStore.InsertWorkspaces(testWorkspaces);
    }

    private async Task<List<List<WorkspaceMember>>> EnrollIntoWorkspaces(
        User devUser,
        List<User> testUsers,
        List<Workspace> testWorkspaces
    )
    {
        int usersPerWorkspace = testUsers.Count / testWorkspaces.Count;
        int i = 0;
        List<List<WorkspaceMember>> members = new List<List<WorkspaceMember>>();
        foreach (Workspace workspace in testWorkspaces)
        {
            int usersCurrentWorkspace = 0;
            List<Guid> userIds = new List<Guid>();
            List<string> titles = new List<string>();
            while (
                i < testUsers.Count && usersCurrentWorkspace < usersPerWorkspace
            )
            {
                userIds.Add(testUsers[i++].Id);
                titles.Add("Member");
                if (usersCurrentWorkspace++ < 2)
                {
                    titles[usersCurrentWorkspace - 1] =
                        usersCurrentWorkspace == 1 ? "Owner" : "Admin";
                }
            }
            if (i > usersPerWorkspace)
            {
                userIds.Add(devUser.Id);
                titles.Add("Member");
            }
            List<WorkspaceMember> addedMembers =
                await _workspaceStore.InsertWorkspaceMembers(
                    workspace.Id,
                    userIds,
                    titles
                );
            await _workspaceStore.InsertWorkspaceAdmin(
                addedMembers[0].UserId,
                workspace.Id,
                2047 // full permissions
            );
            await _workspaceStore.InsertWorkspaceAdmin(
                addedMembers[1].UserId,
                workspace.Id
            );
            addedMembers[0].Owner = true;
            await context.SaveChangesAsync();
            members.Add(addedMembers);
        }

        return members;
    }

    private async Task<List<List<Channel>>> InsertTestChannels(
        List<List<WorkspaceMember>> testMembers,
        List<Workspace> testWorkspaces,
        int minChannelsPerWorkspace,
        int maxChannelsPerWorkspace
    )
    {
        List<List<Channel>> channels = new List<List<Channel>>();
        foreach (
            (
                List<WorkspaceMember> members,
                Workspace workspace
            ) in testMembers.Zip(testWorkspaces)
        )
        {
            List<Channel> addedChannels = new List<Channel>();
            for (
                int i = 0;
                i
                    < Math.Max(
                        Store.random.Next(maxChannelsPerWorkspace + 1),
                        minChannelsPerWorkspace
                    );
                i++
            )
            {
                addedChannels.Add(
                    CreateTestChannel(
                        members[i % members.Count].UserId,
                        workspace
                    )
                );
            }
            channels.Add(addedChannels);
        }

        await _channelStore.InsertChannels(
            channels.SelectMany(channels => channels).ToList()
        );

        return channels;
    }

    private async Task MakeSomeChannelsPrivate(List<List<Channel>> testChannels)
    {
        foreach (List<Channel> workspaceChannels in testChannels)
        {
            foreach (Channel channel in workspaceChannels)
            {
                channel.Private = Store.random.Next(10) == 1;
            }
            workspaceChannels[1].Private = true;
        }

        await context.SaveChangesAsync();
    }

    private async Task<List<List<List<ChannelMember>>>> EnrollChannelMembers(
        User devUser,
        List<List<Channel>> testChannels,
        List<List<WorkspaceMember>> testWorkspaceMembers,
        int minMembersPerChannel
    )
    {
        List<List<List<ChannelMember>>> members =
            new List<List<List<ChannelMember>>>();
        foreach (
            (
                List<Channel> testWorkspaceChannels,
                List<WorkspaceMember> testMembers
            ) in testChannels.Zip(testWorkspaceMembers)
        )
        {
            List<List<ChannelMember>> workspaceChannelMembers =
                new List<List<ChannelMember>>();
            int i = 0;
            foreach (Channel testChannel in testWorkspaceChannels)
            {
                List<ChannelMember> channelMembers;
                if (testChannel == testWorkspaceChannels[0])
                {
                    channelMembers = await _channelStore.InsertChannelMembers(
                        testChannel.Id,
                        testChannel.WorkspaceId,
                        testMembers.Select(m => m.UserId).ToList()
                    );
                }
                else
                {
                    List<int> randomIndices = Enumerable
                        .Range(0, testWorkspaceMembers.Count)
                        .OrderBy(_ => Store.random.Next())
                        .ToList();
                    List<Guid> memberIds = testMembers
                        .Where((m, i) => randomIndices.Contains(i))
                        .Select(m => m.UserId)
                        .Take(
                            Math.Max(
                                Store.random.Next(
                                    testWorkspaceMembers.Count + 1
                                ),
                                minMembersPerChannel
                            )
                        )
                        .ToList();
                    if (
                        testChannel.CreatedById != devUser.Id
                        && !memberIds.Contains(devUser.Id)
                    )
                    {
                        memberIds.Add(devUser.Id);
                    }
                    channelMembers = await _channelStore.InsertChannelMembers(
                        testChannel.Id,
                        testChannel.WorkspaceId,
                        memberIds
                    );
                }
                if (testChannel.Private)
                {
                    channelMembers[0].Admin = true;
                    await context.SaveChangesAsync();
                }
                if ((i++ & 1) == 1)
                {
                    var testStar = CreateTestStar(
                        devUser,
                        testChannel.Workspace,
                        testChannel
                    );
                    context.Add(testStar);
                    await context.SaveChangesAsync();
                }
                workspaceChannelMembers.Add(channelMembers);
            }
            members.Add(workspaceChannelMembers);
        }

        return members;
    }

    public static Star CreateTestStar(
        User testUser,
        Workspace testWorkspace,
        Channel? testChannel = null,
        DirectMessageGroup? testDirectMessageGroup = null
    )
    {
        if (testChannel is null && testDirectMessageGroup is null)
        {
            throw new InvalidOperationException();
        }
        return new Star
        {
            User = testUser,
            Workspace = testWorkspace,
            Channel = testChannel,
            DirectMessageGroup = testDirectMessageGroup,
        };
    }

    public static Channel CreateTestChannel(
        Guid testCreatorId,
        Workspace testWorkspace
    )
    {
        return new Channel
        {
            CreatedById = testCreatorId,
            Description = "test-description",
            Name = "test-channel-name-" + Store.GenerateRandomString(5),
            Workspace = testWorkspace
        };
    }

    public static ChannelMessage CreateTestChannelMessage(
        Guid testChannelId,
        Guid testAuthorId
    )
    {
        return new ChannelMessage
        {
            ChannelId = testChannelId,
            Content = "test content",
            UserId = testAuthorId
        };
    }

    public static Workspace CreateTestWorkspace()
    {
        return new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + Store.GenerateRandomString(10)
        };
    }

    public static DirectMessageGroup CreateTestDirectMessageGroup(
        Workspace testWorkspace
    )
    {
        return new DirectMessageGroup { Workspace = testWorkspace };
    }

    public static DirectMessageGroupMember CreateTestDirectMessageGroupMember(
        User testUser,
        DirectMessageGroup testGroup
    )
    {
        return new DirectMessageGroupMember
        {
            DirectMessageGroup = testGroup,
            User = testUser
        };
    }

    public User CreateTestUser()
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
            PasswordHash = HashedTestPassword,
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = username.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
    }
}
