using PersistenceService.Stores;
using Models = PersistenceService.Models;
using SlackCloneGraphQL.Types;
using SlackCloneGraphQL.Types.Connections;
using PersistenceService.Utils.GraphQL;

namespace SlackCloneGraphQL;

public class SlackCloneData
{
    private IServiceProvider _provider { get; set; }

    public SlackCloneData(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<User> GetUserById(
        Guid userId,
        IEnumerable<string> requestedFields
    )
    {
        using var scope = _provider.CreateScope();
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
        using var scope = _provider.CreateScope();
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

        List<WorkspaceMember> members = new List<WorkspaceMember>();
        foreach (dynamic dbm in dbMembers)
        {
            WorkspaceMember member = (WorkspaceMember)
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
        WorkspacesFilter filter,
        FieldInfo fieldInfo
    )
    {
        if (!(filter.NameQuery is null))
        {
            throw new NotImplementedException();
        }
        if (fieldInfo.SubfieldNames.Contains(nameof(Workspace.Members)))
        {
            throw new InvalidOperationException(
                "Cannot load connection within a connection here."
            );
        }

        using var scope = _provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (List<dynamic> dbWorkspaces, bool lastPage) =
            await workspaceStore.LoadWorkspaces(
                filter.UserId,
                filter.Cursor.First,
                fieldInfo.FieldTree,
                (DateTime?)filter.Cursor.After
            );

        List<Workspace> workspaces = new List<Workspace>();
        foreach (dynamic dbw in dbWorkspaces)
        {
            Workspace workspace = (Workspace)
                ModelToObjectConverters.ConvertDynamicWorkspace(dbw);
            workspaces.Add(workspace);
        }

        return ModelToObjectConverters.ToConnection<Workspace>(
            workspaces,
            filter.Cursor.After is null,
            lastPage
        );
    }

    public async Task<Workspace> GetWorkspace(Guid workspaceId)
    {
        using var scope = _provider.CreateScope();
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
        using var scope = _provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();

        Models.Workspace dbWorkspaceSkeleton =
            new()
            {
                AvatarId = workspaceInfo.AvatarId,
                Description = workspaceInfo.Description ?? "",
                Name = workspaceInfo.Name,
            };

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
}
