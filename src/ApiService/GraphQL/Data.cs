using PersistenceService.Stores;
using Models = PersistenceService.Models;
using Common.SlackCloneGraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;
using File = Common.SlackCloneGraphQL.Types.File;
using WorkspaceStore = PersistenceService.Stores.WorkspaceStore;
using Common.SlackCloneGraphQL;

namespace SlackCloneGraphQL;

public class SlackCloneData : ISlackCloneData
{
    private IServiceProvider Provider { get; set; }

    public SlackCloneData(IServiceProvider provider)
    {
        Provider = provider;
    }

    public async Task<User> GetUserById(Guid userId, IEnumerable<string> cols)
    {
        using var scope = Provider.CreateScope();
        UserStore userStore =
            scope.ServiceProvider.GetRequiredService<UserStore>();
        return await userStore.FindById(userId, cols);
    }

    public async Task<Channel> GetChannel(Guid channelId)
    {
        using var scope = Provider.CreateScope();
        ChannelStore channelStore =
            scope.ServiceProvider.GetRequiredService<ChannelStore>();
        return await channelStore.LoadChannel(channelId);
    }

    public async Task<DirectMessageGroup> GetDirectMessageGroup(Guid groupId)
    {
        using var scope = Provider.CreateScope();
        DirectMessageGroupStore directMessageGroupStore =
            scope.ServiceProvider.GetRequiredService<DirectMessageGroupStore>();
        return await directMessageGroupStore.LoadDirectMessageGroup(groupId);
    }

    public async Task<Connection<Message>> GetChannelMessages(
        Guid channelId,
        MessagesFilter? filter,
        int first,
        Guid? after,
        List<string> cols
    )
    {
        //// TODO: add filtering capabilities
        /*
        if (filter is not null)
        {
            throw new NotImplementedException();
        }
        */

        using var scope = Provider.CreateScope();
        ChannelStore channelStore =
            scope.ServiceProvider.GetRequiredService<ChannelStore>();
        (List<Message> messages, bool lastPage) =
            await channelStore.LoadChannelMessages(
                channelId,
                cols,
                first,
                after
            );

        return ToConnection(messages, after is null, lastPage);
    }

    public async Task<Connection<ChannelMember>> GetChannelMembers(
        Guid channelId,
        UsersFilter? filter,
        int first,
        Guid? after,
        List<string> cols
    )
    {
        if (
            filter is not null
            && (
                filter.JoinedAfter is not null
                || filter.JoinedBefore is not null
                || filter.Query is not null
            )
        )
        {
            throw new NotImplementedException();
        }

        using var scope = Provider.CreateScope();
        ChannelStore channelStore =
            scope.ServiceProvider.GetRequiredService<ChannelStore>();
        (List<ChannelMember> members, bool lastPage) =
            await channelStore.LoadChannelMembers(
                first,
                cols,
                channelId,
                after
            );

        return ToConnection(members, after is null, lastPage);
    }

    public async Task<Connection<WorkspaceMember>> GetWorkspaceMembers(
        Guid workspaceId,
        UsersFilter? filter,
        int first,
        Guid? after,
        List<string> cols
    )
    {
        if (filter is not null)
        {
            throw new NotImplementedException();
        }

        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (List<WorkspaceMember> members, bool lastPage) =
            await workspaceStore.LoadWorkspaceMembers(
                first,
                cols,
                workspaceId,
                after
            );

        return ToConnection(members, after is null, lastPage);
    }

    public async Task<Connection<Workspace>> GetWorkspaces(
        int first,
        Guid? after,
        WorkspacesFilter filter,
        IEnumerable<string> cols
    )
    {
        if (filter.NameQuery is not null)
        {
            throw new NotImplementedException();
        }
        if (cols.Contains("members"))
        {
            throw new InvalidOperationException(
                "Forbidden attempt to load a connection within a connection"
            );
        }

        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (List<Workspace> workspaces, bool lastPage) =
            await workspaceStore.LoadWorkspaces(
                filter.UserId,
                first,
                cols,
                after
            );

        return ToConnection(workspaces, after is null, lastPage);
    }

    private static Connection<T> ToConnection<T>(
        List<T> nodes,
        bool firstPage,
        bool lastPage
    )
        where T : INode
    {
        var connection = new Connection<T>
        {
            TotalEdges = nodes.Count,
            Edges = nodes.Select(
                n => new ConnectionEdge<T> { Node = n, Cursor = n.Id }
            ),
            PageInfo = new PageInfo
            {
                StartCursor = nodes.FirstOrDefault()?.Id,
                EndCursor = nodes.LastOrDefault()?.Id,
                HasNextPage = !lastPage,
                HasPreviousPage = !firstPage
            }
        };

        return connection;
    }

    public async Task<Workspace> GetWorkspace(
        Guid workspaceId,
        List<string> dbCols
    )
    {
        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        return await workspaceStore.GetWorkspace(workspaceId, dbCols);
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

        Models.Workspace dbWorkspace = await workspaceStore.CreateWorkspace(
            dbWorkspaceSkeleton,
            creatorId,
            workspaceInfo.InvitedUserEmails
        );

        var workspace = new Workspace
        {
            Id = dbWorkspace.Id,
            CreatedAt = dbWorkspace.CreatedAt,
            Description = dbWorkspace.Description,
            Name = dbWorkspace.Name,
            NumMembers = dbWorkspace.NumMembers,
        };

        if (workspaceInfo.AvatarId is not null)
        {
            FileStore fileStore =
                scope.ServiceProvider.GetRequiredService<FileStore>();
            workspace.Avatar = (
                await fileStore.GetFileById(workspaceInfo.AvatarId)
            )!;
        }

        return workspace;
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

        return new File
        {
            Id = dbAvatar.Id,
            Name = dbAvatar.Name,
            StoreKey = dbAvatar.StoreKey,
            UploadedAt = dbAvatar.UploadedAt
        };
    }

    public async Task<bool> ValidUserEmail(string email)
    {
        using var scope = Provider.CreateScope();
        UserStore userStore =
            scope.ServiceProvider.GetRequiredService<UserStore>();
        return await userStore.RegisteredEmail(email);
    }

    public async Task<Connection<Channel>> GetChannels(
        int first,
        Guid? after,
        ChannelsFilter filter,
        List<string> cols
    )
    {
        if (filter.WorkspaceId is null || filter.UserId is null)
        {
            throw new InvalidOperationException();
        }
        if (
            filter.SortOrder is not null
            || filter.Query is not null
            || filter.With is not null
            || filter.LastActivityAfter is not null
            || filter.LastActivityBefore is not null
            || filter.CreatedAfter is not null
            || filter.CreatedBefore is not null
        )
        {
            throw new NotImplementedException();
        }
        if (cols.Contains("members") || cols.Contains("messages"))
        {
            throw new InvalidOperationException(
                "Requested a connection within a connection"
            );
        }

        using var scope = Provider.CreateScope();
        ChannelStore channelStore =
            scope.ServiceProvider.GetRequiredService<ChannelStore>();
        (var channels, var lastPage) = await channelStore.LoadChannels(
            (Guid)filter.WorkspaceId,
            (Guid)filter.UserId,
            first,
            cols
        );
        return ToConnection(channels, after is null, lastPage);
    }

    public async Task<Connection<DirectMessageGroup>> GetDirectMessageGroups(
        int first,
        Guid? after,
        DirectMessageGroupsFilter filter,
        IEnumerable<string> cols
    )
    {
        if (filter.SortOrder is not null)
        {
            throw new NotImplementedException();
        }
        if (cols.Contains("messages") || cols.Contains("members"))
        {
            throw new InvalidOperationException(
                "Requested a connection within a connection"
            );
        }

        using var scope = Provider.CreateScope();
        DirectMessageGroupStore directMessageGroupStore =
            scope.ServiceProvider.GetRequiredService<DirectMessageGroupStore>();
        (List<DirectMessageGroup> dmgs, bool lastPage) =
            await directMessageGroupStore.LoadDirectMessageGroups(
                filter.WorkspaceId,
                filter.UserId,
                first,
                cols.ToList(),
                after
            );

        return ToConnection(dmgs, after is null, lastPage);
    }

    public async Task<Connection<IGroup>> GetStarred(
        int first,
        Guid? after,
        StarredFilter filter,
        List<string> cols
    )
    {
        if (cols.Contains("Members") || cols.Contains("Messages"))
        {
            throw new InvalidOperationException(
                "Requested a connection within a connection"
            );
        }

        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (List<IGroup> starred, bool lastPage) =
            await workspaceStore.LoadStarred(
                filter.WorkspaceId,
                filter.UserId,
                first,
                cols,
                after
            );

        return ToConnection(new List<IGroup>(), after is null, true);
    }

    public async Task<Connection<Message>> GetDirectMessages(
        Guid directMessageGroupId,
        MessagesFilter? filter,
        int first,
        Guid? after,
        List<string> cols
    )
    {
        if (filter is not null)
        {
            throw new NotImplementedException();
        }

        using var scope = Provider.CreateScope();
        DirectMessageGroupStore directMessageGroupStore =
            scope.ServiceProvider.GetRequiredService<DirectMessageGroupStore>();
        (List<Message> messages, bool lastPage) =
            await directMessageGroupStore.LoadDirectMessages(
                directMessageGroupId,
                cols,
                first,
                after
            );

        return ToConnection(messages, after is null, lastPage);
    }
}
