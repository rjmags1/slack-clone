using PersistenceService.Stores;
using Models = PersistenceService.Models;
using SlackCloneGraphQL.Types;
using SlackCloneGraphQL.Types.Connections;
using PersistenceService.Utils.GraphQL;
using File = SlackCloneGraphQL.Types.File;

namespace SlackCloneGraphQL;

public class SlackCloneData
{
    private IServiceProvider Provider { get; set; }

    public SlackCloneData(IServiceProvider provider)
    {
        Provider = provider;
    }

    public async Task<User> GetUserById(
        Guid userId,
        IEnumerable<string> requestedFields
    )
    {
        using var scope = Provider.CreateScope();
        UserStore userStore =
            scope.ServiceProvider.GetRequiredService<UserStore>();
        Models.User dbUser =
            await userStore.FindByIdAsyncWithEagerNavPropLoading(
                userId,
                requestedFields
            );

        return ModelToObjectConverters.ConvertUser(dbUser, requestedFields);
    }

    public async Task<Connection<WorkspaceMember>> GetWorkspaceMembers(
        FieldInfo fieldInfo,
        UsersFilter filter
    )
    {
        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (List<dynamic> dbMembers, bool lastPage) =
            await workspaceStore.LoadWorkspaceMembers(
                filter.UserId,
                filter.Cursor.First,
                fieldInfo.FieldTree,
                filter.WorkspaceId,
                (Guid?)filter.Cursor.After
            );

        List<WorkspaceMember> members = new();
        foreach (dynamic dbm in dbMembers)
        {
            WorkspaceMember member =
                ModelToObjectConverters.ConvertDynamicWorkspaceMember(
                    dbm,
                    FieldAnalyzer.ExtractUserFields(
                        "user",
                        fieldInfo.FieldTree
                    ),
                    FieldAnalyzer.ExtractUserFields(
                        "admin",
                        fieldInfo.FieldTree
                    )
                );
            members.Add(member);
        }

        return ModelToObjectConverters.ToConnection<WorkspaceMember>(
            members,
            filter.Cursor.After is null,
            lastPage
        );
    }

    public async Task<Connection<Workspace>> GetWorkspaces(
        int first,
        Guid? after,
        WorkspacesFilter filter,
        FieldInfo fieldInfo
    )
    {
        if (filter.NameQuery is not null)
        {
            throw new NotImplementedException();
        }
        if (fieldInfo.SubfieldNames.Contains(nameof(Workspace.Members)))
        {
            throw new InvalidOperationException(
                "Forbidden attempt to load a connection within a connection"
            );
        }

        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (List<dynamic> dbWorkspaces, bool lastPage) =
            await workspaceStore.LoadWorkspaces(
                filter.UserId,
                first,
                fieldInfo.FieldTree,
                after
            );

        List<Workspace> workspaces = new();
        foreach (dynamic dbw in dbWorkspaces)
        {
            Workspace workspace = (Workspace)
                ModelToObjectConverters.ConvertDynamicWorkspace(dbw);
            workspaces.Add(workspace);
        }

        return ModelToObjectConverters.ToConnection<Workspace>(
            workspaces,
            after is null,
            lastPage
        );
    }

    public async Task<Workspace> GetWorkspace(Guid workspaceId)
    {
        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        Models.Workspace dbWorkspace = await workspaceStore.GetWorkspace(
            workspaceId
        );

        return ModelToObjectConverters.ConvertWorkspace(dbWorkspace);
    }

    public async Task<Workspace> CreateWorkspace(
        WorkspaceInput workspaceInfo,
        Guid creatorId
    )
    {
        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();

        Models.Workspace dbWorkspaceSkeleton =
            new()
            {
                Description = workspaceInfo.Description ?? "",
                Name = workspaceInfo.Name,
            };

        if (workspaceInfo.AvatarId is not null)
        {
            FileStore fileStore =
                scope.ServiceProvider.GetRequiredService<FileStore>();
            dbWorkspaceSkeleton.Avatar = await fileStore.GetFileById(
                workspaceInfo.AvatarId
            );
        }

        Models.Workspace dbWorkspace = (
            await workspaceStore.InsertWorkspaces(
                new List<Models.Workspace> { dbWorkspaceSkeleton }
            )
        ).First();

        await workspaceStore.InsertWorkspaceAdmin(creatorId, dbWorkspace.Id, 1);

        if (workspaceInfo.InvitedUserEmails is not null)
        {
            await workspaceStore.InviteUsersByEmail(
                dbWorkspace.Id,
                creatorId,
                workspaceInfo.InvitedUserEmails
            );
        }

        return ModelToObjectConverters.ConvertWorkspace(dbWorkspace);
    }

    public async Task<File> CreateAvatar(FileInput fileinfo)
    {
        using var scope = Provider.CreateScope();
        FileStore fileStore =
            scope.ServiceProvider.GetRequiredService<FileStore>();

        Models.File dbAvatarSkeleton =
            new() { Name = fileinfo.Name, StoreKey = fileinfo.StoreKey };

        Models.File dbAvatar = (
            await fileStore.InsertFiles(
                new List<Models.File> { dbAvatarSkeleton }
            )
        ).First();

        return ModelToObjectConverters.ConvertAvatar(dbAvatar);
    }

    public async Task<bool> ValidUserEmail(string email)
    {
        using var scope = Provider.CreateScope();
        UserStore userStore =
            scope.ServiceProvider.GetRequiredService<UserStore>();
        return await userStore.RegisteredEmail(email);
    }
}
