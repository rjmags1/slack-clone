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
