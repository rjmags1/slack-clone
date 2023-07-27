using PersistenceService.Stores;
using Models = PersistenceService.Models;
using SlackCloneGraphQL.Types;
using SlackCloneGraphQL.Types.Connections;
using System.Collections;

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
        (string, ArrayList?) requestedFields,
        UsersFilter filter
    )
    {
        using var scope = _provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (IEnumerable<Models.WorkspaceMember> dbMembers, bool lastPage) =
            await workspaceStore.LoadWorkspaceMembersEagerNavPropLoading(
                filter.UserId,
                filter.Cursor.First,
                requestedFields,
                filter.WorkspaceId,
                (Guid?)filter.Cursor.After
            );

        List<WorkspaceMember> members = dbMembers
            .Select(dbm => ModelToObjectConverters.ConvertWorkspaceMember(dbm))
            .ToList();

        return ModelToObjectConverters.ToConnection<
            WorkspaceMember,
            UsersFilter
        >(
            members,
            requestedFields,
            filter,
            filter.Cursor.After is null,
            lastPage
        );
    }

    public async Task<Connection<Workspace>> GetWorkspaces(
        WorkspacesFilter filter,
        (string, ArrayList) connectionTree,
        List<string> requestedFields
    )
    {
        if (!(filter.NameQuery is null))
        {
            throw new NotImplementedException();
        }
        if (requestedFields.Contains(nameof(Workspace.Members)))
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
                connectionTree,
                (DateTime?)filter.Cursor.After
            );

        List<Workspace> workspaces = new List<Workspace>();
        foreach (var dbw in dbWorkspaces)
        {
            Workspace workspace = (Workspace)
                ModelToObjectConverters.ConvertWorkspace(dbw);
            workspaces.Add(workspace);
        }

        return ModelToObjectConverters.ToConnection<Workspace>(
            workspaces,
            requestedFields,
            filter.Cursor.After is null,
            lastPage
        );
    }
}
