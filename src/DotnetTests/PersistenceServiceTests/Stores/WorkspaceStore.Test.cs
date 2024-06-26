using DotnetTests.Fixtures;
using DotnetTests.PersistenceService.Stores;
using DotnetTests.PersistenceService.Utils;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;
using GraphQLTypes = Common.SlackCloneGraphQL.Types;

namespace DotnetTest.PersistenceService.Stores;

[Trait("Category", "Order 0")]
[Collection("Empty Database Test Collection")]
public class WorkspaceStoreTests1
{
    private readonly ApplicationDbContext _dbContext;
    private readonly WorkspaceStore _workspaceStore;

    public WorkspaceStoreTests1(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _workspaceStore = new WorkspaceStore(_dbContext);
    }

    [Fact]
    public async void InsertWorkspaceAdmin_ShouldInsertWorkspaceAdminAlreadyMember()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember updatedMember =
            await _workspaceStore.InsertWorkspaceAdmin(
                testUser.Id,
                testWorkspace.Id
            );

        WorkspaceAdminPermissions insertedPermissions =
            updatedMember.WorkspaceAdminPermissions!;
        Assert.True(updatedMember.Admin);
        Assert.NotEqual(updatedMember.WorkspaceAdminPermissionsId, Guid.Empty);
        Assert.NotNull(insertedPermissions);
        Assert.NotEqual(insertedPermissions.AdminId, Guid.Empty);
        Assert.Equal(testUser.Id, insertedPermissions.AdminId);
        Assert.NotEqual(insertedPermissions.ConcurrencyStamp, Guid.Empty);
        Assert.Equal(1, insertedPermissions.WorkspaceAdminPermissionsMask);
        Assert.Equal(testWorkspace.Id, insertedPermissions.WorkspaceId);
    }

    [Fact]
    public async void InsertWorkspaceAdmin_ShouldInsertWorkspaceAdminNotAlreadyMember()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        await _dbContext.SaveChangesAsync();

        WorkspaceMember insertedMember =
            await _workspaceStore.InsertWorkspaceAdmin(
                testUser.Id,
                testWorkspace.Id,
                2
            );

        Assert.NotEqual(insertedMember.Id, Guid.Empty);
        Assert.True(insertedMember.Admin);
        Assert.NotEqual(insertedMember.WorkspaceAdminPermissionsId, Guid.Empty);
        Assert.Null(insertedMember.AvatarId);
        Assert.NotEqual(insertedMember.JoinedAt, default(DateTime));
        Assert.Null(insertedMember.NotificationsAllowTimeStart);
        Assert.Null(insertedMember.NotificationsAllTimeEnd);
        Assert.Equal(0, insertedMember.NotificationSound);
        Assert.Null(insertedMember.OnlineStatus);
        Assert.False(insertedMember.Owner);
        Assert.Null(insertedMember.ThemeId);
        Assert.Equal("Admin", insertedMember.Title);
        Assert.Equal(insertedMember.UserId, testUser.Id);
        Assert.Equal(insertedMember.WorkspaceId, testWorkspace.Id);

        WorkspaceAdminPermissions insertedPermissions =
            insertedMember.WorkspaceAdminPermissions!;
        Assert.NotNull(insertedPermissions);
        Assert.NotEqual(insertedPermissions.AdminId, Guid.Empty);
        Assert.Equal(testUser.Id, insertedPermissions.AdminId);
        Assert.NotEqual(insertedPermissions.ConcurrencyStamp, Guid.Empty);
        Assert.Equal(2, insertedPermissions.WorkspaceAdminPermissionsMask);
        Assert.Equal(testWorkspace.Id, insertedPermissions.WorkspaceId);
    }

    [Fact]
    public async void InsertWorkspaceAdmin_ShouldThrowOnNonexistentIdsInvalidMask()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceAdmin(
                    Guid.Empty,
                    testWorkspace.Id
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceAdmin(
                    testUser.Id,
                    Guid.Empty
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceAdmin(
                    testUser.Id,
                    testWorkspace.Id,
                    -100
                )
        );

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceAdmin(
                    testUser.Id,
                    testWorkspace.Id,
                    2048
                )
        );

        Assert.NotNull(
            await _workspaceStore.InsertWorkspaceAdmin(
                testUser.Id,
                testWorkspace.Id
            )
        );
    }

    [Fact]
    public async void InsertWorkspaceAdmin_ShouldThrowOnAlreadyAdmin()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership = new WorkspaceMember
        {
            Admin = true,
            Title = "Member",
            Workspace = testWorkspace,
            User = testUser
        };
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceAdmin(
                    testUser.Id,
                    testWorkspace.Id
                )
        );
    }

    [Fact]
    public async void InsertWorkspaceSearch_ShouldInsertWorkspaceSearch()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        List<User> testMembers = new List<User>();
        for (int i = 0; i < 10; i++)
        {
            testMembers.Add(StoreTestUtils.CreateTestUser());
        }
        _dbContext.AddRange(testMembers);

        await _dbContext.SaveChangesAsync();

        List<WorkspaceMember> inserted =
            await _workspaceStore.InsertWorkspaceMembers(
                testWorkspace.Id,
                testMembers.Select(u => u.Id).ToList(),
                Enumerable.Repeat("Member", testMembers.Count).ToList()
            );

        Assert.Equal(testMembers.Count, testWorkspace.NumMembers);
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
            Assert.False(im.Owner);
            Assert.Null(im.ThemeId);
            Assert.Equal("Member", im.Title);
            Assert.Equal(im.UserId, m.Id);
            Assert.Equal(im.WorkspaceId, testWorkspace.Id);
        }
    }

    [Fact]
    public async void InsertWorkspaceMembers_ShouldThrowOnNonExistentIds()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testMember = StoreTestUtils.CreateTestUser();
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser);

        WorkspaceMember testWorkspaceMembership =
            StoreTestUtils.CreateTestWorkspaceMember(testUser, testWorkspace);
        _dbContext.Add(testWorkspaceMembership);

        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await _workspaceStore.InsertWorkspaceMembers(
                    testWorkspace.Id,
                    new List<Guid> { testUser.Id },
                    new List<string> { "Member " }
                )
        );
    }

    [Fact]
    public async void InsertWorkspaceInvite_ShouldInsertWorkspaceInvite()
    {
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser1 = StoreTestUtils.CreateTestUser();
        User testUser2 = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser1);
        _dbContext.Add(testUser2);

        WorkspaceMember testWorkspaceMembership1 =
            StoreTestUtils.CreateTestWorkspaceMember(testUser1, testWorkspace);
        testWorkspaceMembership1.Admin = true;

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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser1 = StoreTestUtils.CreateTestUser();
        User testUser2 = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser1);
        _dbContext.Add(testUser2);

        WorkspaceMember testWorkspaceMembership1 =
            StoreTestUtils.CreateTestWorkspaceMember(testUser1, testWorkspace);
        testWorkspaceMembership1.Admin = true;

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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

        User testUser1 = StoreTestUtils.CreateTestUser();
        User testUser2 = StoreTestUtils.CreateTestUser();
        _dbContext.Add(testUser1);
        _dbContext.Add(testUser2);

        WorkspaceMember testWorkspaceMembership1 =
            StoreTestUtils.CreateTestWorkspaceMember(testUser1, testWorkspace);
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
        Workspace testWorkspace = StoreTestUtils.CreateTestWorkspace();
        _dbContext.Add(testWorkspace);

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
            workspaces.Add(StoreTestUtils.CreateTestWorkspace());
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
            Assert.Equal(0, iw.NumMembers);
        }
    }
}

[Trait("Category", "Order 2")]
[Collection("Filled Database Test Collection")]
public class WorkspaceStoreTests2 : StoreTest
{
    private readonly WorkspaceStore _workspaceStore;

    private readonly Guid _userId;

    public WorkspaceStoreTests2(
        FilledApplicationDbContextFixture filledApplicationDbContextFixture
    )
    {
        DbContext = filledApplicationDbContextFixture.Context;
        _workspaceStore = new WorkspaceStore(DbContext);
        _userId = GetUserId();
    }

    [Fact]
    public void SeedHappened()
    {
        Assert.True(DbContext.Workspaces.Count() > 0);
        Assert.True(DbContext.WorkspaceMembers.Count() > 0);
    }

    [Fact]
    public async void LoadWorkspaceMembers_ShouldWork()
    {
        List<string> dbCols =
            new()
            {
                "Id",
                "AvatarId",
                "JoinedAt",
                "Title",
                "UserId",
                "WorkspaceId",
                "Admin",
                "Owner",
                "OnlineStatus",
                "OnlineStatusUntil",
                "WorkspaceAdminPermissionsId",
                "ThemeId",
                "NotificationSound",
                "NotificationsAllowTimeStart",
                "NotificationsAllowTimeEnd"
            };
        List<string> throw1 = dbCols
            .Where(c => c != "WorkspaceAdminPermissionsId")
            .ToList();
        List<string> throw2 = dbCols.Where(c => c != "ThemeId").ToList();
        List<string> throw3 = dbCols
            .Where(c => c != "NotificationsAllowTimeStart")
            .ToList();
        List<string> throw4 = dbCols
            .Where(c => c != "NotificationsAllowTimeEnd")
            .ToList();
        List<string> throw5 = dbCols
            .Where(c => c != "NotificationSound")
            .ToList();
        List<List<string>> throws =
            new() { throw1, throw2, throw3, throw4, throw5 };

        List<string> noThrow = dbCols.GetRange(0, dbCols.Count - 5);

        Guid workspaceId = GetWorkspaceIdContainingUser(_userId);
        (var afterId, var totalMembers) =
            GetFirstAlphaWorkspaceMemberTotalWorkspaceMembers(workspaceId);

        int first = totalMembers - 1;
        throws.ForEach(
            async t =>
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async () =>
                        await _workspaceStore.LoadWorkspaceMembers(
                            first,
                            throw1,
                            workspaceId
                        )
                )
        );

        (var members1, var lastPage1) =
            await _workspaceStore.LoadWorkspaceMembers(
                first,
                noThrow,
                workspaceId
            );
        (var members2, var lastPage2) =
            await _workspaceStore.LoadWorkspaceMembers(
                first,
                noThrow,
                workspaceId,
                afterId
            );

        Assert.False(lastPage1);
        Assert.Equal(
            members1.Select(m => m.User.Username).OrderBy(un => un),
            members1.Select(m => m.User.Username)
        );
        Assert.True(lastPage2);
    }

    [Fact]
    public async void LoadStarred_ShouldWork()
    {
        List<string> dbCols =
            new() { "Id", "CreatedAt", "WorkspaceId", "Name" };

        Guid workspaceId = GetWorkspaceIdContainingUser(_userId);
        (var afterId, var totalStars) = GetMostRecentStarredTotalStarred(
            workspaceId,
            _userId
        );

        int first = totalStars - 1;
        (var groups1, var lastPage1) = await _workspaceStore.LoadStarred(
            workspaceId,
            _userId,
            first,
            dbCols
        );

        (var groups2, var lastPage2) = await _workspaceStore.LoadStarred(
            workspaceId,
            _userId,
            first,
            dbCols,
            afterId
        );

        Assert.False(lastPage1);
        Assert.Equal(first, groups1.Count);
        Assert.True(lastPage2);
        if (first > 0)
        {
            Assert.NotEqual(afterId, groups2.First().Id);
        }
    }

    [Fact]
    public async void GetWorkspace_ShouldWork()
    {
        Guid workspaceId = GetWorkspaceIdContainingUser(_userId);
        List<string> dbCols1 =
            new() { "Id", "CreatedAt", "Description", "Name", "NumMembers" };
        List<string> dbCols2 =
            new()
            {
                "Id",
                "AvatarId",
                "CreatedAt",
                "Description",
                "Name",
                "NumMembers"
            };
        var workspace1 = await _workspaceStore.GetWorkspace(
            workspaceId,
            dbCols1
        );
        var workspace2 = await _workspaceStore.GetWorkspace(
            workspaceId,
            dbCols2
        );
        Assert.Equal(workspaceId, workspace1.Id);
        Assert.Equal(workspaceId, workspace2.Id);
    }

    [Fact]
    public async void LoadWorkspaces_ShouldWork()
    {
        string[] cols =
        {
            "Id",
            "AvatarId",
            "CreatedAt",
            "Description",
            "Name",
            "NumMembers"
        };

        (var afterId, var totalWorkspaces) =
            GetFirstAlphaWorkspaceTotalWorkspaces(_userId);

        int first = totalWorkspaces - 1;
        (List<GraphQLTypes.Workspace> workspaces1, bool lastPage1) =
            await _workspaceStore.LoadWorkspaces(_userId, first, cols);
        (List<GraphQLTypes.Workspace> workspaces2, bool lastPage2) =
            await _workspaceStore.LoadWorkspaces(_userId, first, cols, afterId);
        Assert.False(lastPage1);
        Assert.True(lastPage2);
    }
}
