using DotnetTests.Fixtures;
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
        Assert.NotNull(channelCreator.Id);
        Assert.NotNull(channelWorkspace.Id);

        foreach ((Channel ic, Channel c) in inserted.Zip(channels))
        {
            Assert.NotNull(ic.Id);
            Assert.True(ic.AllowThreads);
            Assert.Null(ic.Avatar);
            Assert.Null(ic.AvatarId);
            Assert.Equal(ic.AllowedChannelPostersMask, 1);
            Assert.NotNull(ic.CreatedAt);
            Assert.Equal(ic.CreatedBy, channelCreator);
            Assert.Equal(ic.CreatedById, channelCreator.Id);
            Assert.NotNull(ic.ConcurrencyStamp);
            Assert.Equal(ic.Description, c.Description);
            Assert.NotNull(ic.ChannelMembers);
            Assert.NotNull(ic.ChannelMessages);
            Assert.Equal(ic.Name, c.Name);
            Assert.False(ic.Private);
            Assert.Equal(ic.Topic, "");
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
