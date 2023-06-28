using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
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
