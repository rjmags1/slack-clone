using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace DotnetTest.PersistenceService.Stores;

[Collection("Database collection")]
public class WorkspaceStoreTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly WorkspaceStore _workspaceStore;

    public WorkspaceStoreTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _workspaceStore = new WorkspaceStore(_dbContext);
    }

    [Fact]
    public async void InsertWorkspaceSearch_ShouldInsertWorkspaceSearch()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string username = UserStore.GenerateTestUserName(10);
        string email = UserStore.GenerateTestEmail(10);
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

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            Workspace = testWorkspace,
            User = testUser
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        WorkspaceSearch inserted = await _workspaceStore.InsertWorkspaceSearch(
            testWorkspace.Id,
            testUser.Id,
            "test query"
        );

        Assert.NotEqual(inserted.Id, Guid.Empty);
        Assert.NotEqual(inserted.CreatedAt, default(DateTime));
        Assert.Equal("test query", inserted.Query);
        Assert.Equal(inserted.UserId, testUser.Id);
        Assert.Equal(inserted.WorkspaceId, testWorkspace.Id);
    }

    [Fact]
    public async void InsertWorkspaceSearch_ShouldThrowOnNonexistentIdsEmptySearch()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string username = UserStore.GenerateTestUserName(10);
        string email = UserStore.GenerateTestEmail(10);
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

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            Workspace = testWorkspace,
            User = testUser
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        Assert.NotNull(
            _workspaceStore.InsertWorkspaceSearch(
                testWorkspace.Id,
                testUser.Id,
                "test query"
            )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceSearch(
                    Guid.Empty,
                    testUser.Id,
                    "test query"
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceSearch(
                    testWorkspace.Id,
                    Guid.Empty,
                    "test query"
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceSearch(
                    testWorkspace.Id,
                    testUser.Id,
                    ""
                )
        );
    }

    [Fact]
    public async void InsertWorkspaceSearch_ShouldThrowOnNotWorkspaceMember()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string username = UserStore.GenerateTestUserName(10);
        string email = UserStore.GenerateTestEmail(10);
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

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceSearch(
                    testWorkspace.Id,
                    testUser.Id,
                    "test query"
                )
        );
    }

    [Fact]
    public async void InsertWorkspaceMembers_ShouldInsertWorkspaceMembers()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        List<User> testMembers = new List<User>();
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
            testMembers.Add(user);
        }
        _dbContext.AddRange(testMembers);

        await _dbContext.SaveChangesAsync();

        List<WorkspaceMember> inserted =
            await _workspaceStore.InsertWorkspaceMembers(
                testWorkspace.Id,
                testMembers.Select(u => u.Id).ToList(),
                Enumerable.Repeat("Member", testMembers.Count).ToList()
            );

        foreach ((WorkspaceMember im, User m) in inserted.Zip(testMembers))
        {
            Assert.NotEqual(im.Id, Guid.Empty);
            Assert.False(im.Admin);
            Assert.Null(im.AvatarId);
            Assert.NotEqual(im.JoinedAt, default(DateTime));
            Assert.Null(im.NotificationsAllowTimeStart);
            Assert.Null(im.NotificationsAllTimeEnd);
            Assert.Equal(0, im.NotificationSound);
            Assert.Null(im.OnlineStatus);
            Assert.Null(im.OnlineStatusUntil);
            Assert.Null(im.ThemeId);
            Assert.Equal("Member", im.Title);
            Assert.Equal(im.UserId, m.Id);
            Assert.Equal(im.WorkspaceId, testWorkspace.Id);
        }
    }

    [Fact]
    public async void InsertWorkspaceMembers_ShouldThrowOnNonExistentIds()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string username = UserStore.GenerateTestUserName(10);
        string email = UserStore.GenerateTestEmail(10);
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
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceMembers(
                    Guid.Empty,
                    new List<Guid> { testMember.Id },
                    new List<string> { "Member" }
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceMembers(
                    testWorkspace.Id,
                    new List<Guid> { Guid.Empty },
                    new List<string> { "Member" }
                )
        );

        Assert.NotNull(
            await _workspaceStore.InsertWorkspaceMembers(
                testWorkspace.Id,
                new List<Guid> { testMember.Id },
                new List<string> { "Member" }
            )
        );
    }

    [Fact]
    public async void InsertWorkspaceMembers_ShouldThrowOnUsersAlreadyMembers()
    {
        Workspace testWorkspace = new Workspace
        {
            Description = "test description",
            Name = "test-workspace-name" + ChannelStore.GenerateRandomString(10)
        };
        _dbContext.Add(testWorkspace);

        string username = UserStore.GenerateTestUserName(10);
        string email = UserStore.GenerateTestEmail(10);
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

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Title = "Member",
            Workspace = testWorkspace,
            User = testMember
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceMembers(
                    testWorkspace.Id,
                    new List<Guid> { testMember.Id },
                    new List<string> { "Member " }
                )
        );
    }

    [Fact]
    public async void InsertWorkspaceInvite_ShouldInsertWorkspaceInvite()
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

        WorkspaceMember testWorkspaceMembership1 = new WorkspaceMember
        {
            Title = "Member",
            User = testUser1,
            Workspace = testWorkspace,
            Admin = true
        };

        _dbContext.Add(testWorkspaceMembership1);
        await _dbContext.SaveChangesAsync();

        WorkspaceInvite invite = await _workspaceStore.InsertWorkspaceInvite(
            testWorkspace.Id,
            testUser1.Id,
            testUser2.Id
        );

        Assert.NotEqual(invite.Id, Guid.Empty);
        Assert.Equal(invite.AdminId, testUser1.Id);
        Assert.NotEqual(invite.CreatedAt, default(DateTime));
        Assert.Equal(invite.UserId, testUser2.Id);
        Assert.Equal(invite.WorkspaceId, testWorkspace.Id);
        Assert.Equal(1, invite.WorkspaceInviteStatus);
    }

    [Fact]
    public async void InsertWorkspaceInvite_ShouldThrowOnNonexistentIds()
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

        WorkspaceMember testWorkspaceMembership1 = new WorkspaceMember
        {
            Title = "Member",
            User = testUser1,
            Workspace = testWorkspace,
            Admin = true
        };

        _dbContext.Add(testWorkspaceMembership1);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceInvite(
                    Guid.Empty,
                    testUser1.Id,
                    testUser2.Id
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceInvite(
                    testWorkspace.Id,
                    Guid.Empty,
                    testUser2.Id
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceInvite(
                    testWorkspace.Id,
                    testUser1.Id,
                    Guid.Empty
                )
        );

        Assert.NotNull(
            await _workspaceStore.InsertWorkspaceInvite(
                testWorkspace.Id,
                testUser1.Id,
                testUser2.Id
            )
        );
    }

    [Fact]
    public async void InsertWorkspaceInvite_ShouldThrowOnNonAdminInvite()
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

        WorkspaceMember testWorkspaceMembership1 = new WorkspaceMember
        {
            Title = "Member",
            User = testUser1,
            Workspace = testWorkspace,
            Admin = false
        };

        _dbContext.Add(testWorkspaceMembership1);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceInvite(
                    testWorkspace.Id,
                    testUser1.Id,
                    testUser2.Id
                )
        );
    }

    [Fact]
    public async void InsertWorkspaceInvite_ShouldThrowOnUserAlreadyMember()
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

        WorkspaceMember testWorkspaceMembership1 = new WorkspaceMember
        {
            Title = "Member",
            User = testUser1,
            Workspace = testWorkspace,
            Admin = true
        };

        WorkspaceMember testWorkspaceMembership2 = new WorkspaceMember
        {
            Title = "Member",
            User = testUser2,
            Workspace = testWorkspace,
        };

        _dbContext.Add(testWorkspaceMembership1);
        _dbContext.Add(testWorkspaceMembership2);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceInvite(
                    testWorkspace.Id,
                    testUser1.Id,
                    testUser2.Id
                )
        );
    }

    [Fact]
    public async void InsertWorkspaces_ShouldInsertWorkspaces()
    {
        List<Workspace> workspaces = new List<Workspace>();
        for (int i = 0; i < 10; i++)
        {
            workspaces.Add(
                new Workspace
                {
                    Description = "test description" + i.ToString(),
                    Name = "test-workspace-name" + i.ToString()
                }
            );
        }

        List<Workspace> inserted = await _workspaceStore.InsertWorkspaces(
            workspaces
        );
        foreach ((Workspace iw, Workspace w) in inserted.Zip(workspaces))
        {
            Assert.NotEqual(iw.Id, Guid.Empty);
            Assert.Null(iw.Avatar);
            Assert.Null(iw.AvatarId);
            Assert.NotEqual(iw.ConcurrencyStamp, Guid.Empty);
            Assert.NotEqual(iw.CreatedAt, default(DateTime));
            Assert.Equal(iw.Description, w.Description);
            Assert.Equal(iw.Name, w.Name);
            Assert.Equal(1, iw.NumMembers);
        }
    }

    [Fact]
    public async void InsertTestWorkspaces_ShouldInsertTestWorkspaces()
    {
        int initialWorkspaces = _dbContext.Workspaces.Count();
        await _workspaceStore.InsertTestWorkspaces(100);
        int currentWorkspaces = _dbContext.Workspaces.Count();
        Assert.Equal(initialWorkspaces + 100, currentWorkspaces);
    }
}
