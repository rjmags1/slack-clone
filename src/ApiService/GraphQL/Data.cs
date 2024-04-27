using PersistenceService.Stores;
using Models = PersistenceService.Models;
using Common.SlackCloneGraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;
using PersistenceService.Utils.GraphQL;
using File = Common.SlackCloneGraphQL.Types.File;
using WorkspaceStore = PersistenceService.Stores.WorkspaceStore;
using Common.SlackCloneGraphQL;
using GraphQLParser.AST;

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
        //using var scope = Provider.CreateScope();
        //ChannelStore channelStore =
        //scope.ServiceProvider.GetRequiredService<ChannelStore>();
        //Models.Channel dbChannel = await channelStore.LoadChannel(channelId);

        //return ModelToObjectConverters.ConvertChannel(
        //dbChannel,
        //skipWorkspace: true
        //);
        throw new NotImplementedException();
    }

    public async Task<DirectMessageGroup> GetDirectMessageGroup(Guid groupId)
    {
        //using var scope = Provider.CreateScope();
        //DirectMessageGroupStore directMessageGroupStore =
        //scope.ServiceProvider.GetRequiredService<DirectMessageGroupStore>();
        //Models.DirectMessageGroup dbGroup =
        //await directMessageGroupStore.LoadDirectMessageGroup(groupId);

        //return ModelToObjectConverters.ConvertDirectMessageGroup(
        //dbGroup,
        //skipWorkspace: true
        //);
        throw new NotImplementedException();
    }

    public async Task<Connection<Message>> GetChannelMessages(
        Guid userId,
        Guid channelId,
        MessagesFilter? filter,
        int first,
        Guid? after,
        IEnumerable<string> cols
    )
    {
        //// TODO: add filtering capabilities

        //using var scope = Provider.CreateScope();
        //ChannelStore channelStore =
        //scope.ServiceProvider.GetRequiredService<ChannelStore>();
        //(
        //List<dynamic> dbMessages,
        //List<ChannelMessageReactionCount> reactionCounts,
        //bool lastPage
        //) = await channelStore.LoadChannelMessages(
        //userId,
        //channelId,
        //fieldInfo,
        //first,
        //after
        //);

        //Dictionary<Guid, List<ChannelMessageReactionCount>?> countsDict = new();
        //foreach (ChannelMessageReactionCount reactionCount in reactionCounts)
        //{
        //if (!countsDict.ContainsKey(reactionCount.ChannelMessageId))
        //{
        //countsDict[reactionCount.ChannelMessageId] =
        //new List<ChannelMessageReactionCount>();
        //}
        //countsDict[reactionCount.ChannelMessageId]!.Add(reactionCount);
        //}
        //List<Message> messages = new();
        //foreach (dynamic dbm in dbMessages)
        //{
        //Message message =
        //ModelToObjectConverters.ConvertDynamicChannelMessage(
        //dbm,
        //countsDict.GetValueOrDefault((Guid)dbm.Id, null),
        //FieldAnalyzer.ExtractUserFields("user", fieldInfo.FieldTree)
        //);
        //messages.Add(message);
        //}

        //return ModelToObjectConverters.ToConnection<Message>(
        //messages,
        //after is null,
        //lastPage
        //);
        throw new NotImplementedException();
    }

    public async Task<Connection<ChannelMember>> GetChannelMembers(
        Guid channelId,
        UsersFilter filter,
        int first,
        Guid? after,
        List<string> cols
    )
    {
        if (
            filter.JoinedAfter is not null
            || filter.JoinedBefore is not null
            || filter.Query is not null
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
        UsersFilter filter,
        int first,
        Guid? after,
        IEnumerable<string> cols
    )
    {
        //using var scope = Provider.CreateScope();
        //WorkspaceStore workspaceStore =
        //scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        //(List<dynamic> dbMembers, bool lastPage) =
        //await workspaceStore.LoadWorkspaceMembers(
        //filter.UserId,
        //first,
        //fieldInfo.FieldTree,
        //filter.WorkspaceId,
        //after
        //);

        //List<WorkspaceMember> members = new();
        //foreach (dynamic dbm in dbMembers)
        //{
        //WorkspaceMember member =
        //ModelToObjectConverters.ConvertDynamicWorkspaceMember(
        //dbm,
        //FieldAnalyzer.ExtractUserFields(
        //"user",
        //fieldInfo.FieldTree
        //),
        //FieldAnalyzer.ExtractUserFields(
        //"admin",
        //fieldInfo.FieldTree
        //)
        //);
        //members.Add(member);
        //}

        //return ModelToObjectConverters.ToConnection<WorkspaceMember>(
        //members,
        //after is null,
        //lastPage
        //);
        throw new NotImplementedException();
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
        //using var scope = Provider.CreateScope();
        //WorkspaceStore workspaceStore =
        //scope.ServiceProvider.GetRequiredService<WorkspaceStore>();

        //Models.Workspace dbWorkspaceSkeleton =
        //new()
        //{
        //Description = workspaceInfo.Description ?? "",
        //Name = workspaceInfo.Name,
        //};

        //if (workspaceInfo.AvatarId is not null)
        //{
        //FileStore fileStore =
        //scope.ServiceProvider.GetRequiredService<FileStore>();
        //dbWorkspaceSkeleton.Avatar = fileStore.GetFileById(
        //workspaceInfo.AvatarId
        //);
        //}

        //Models.Workspace dbWorkspace = await workspaceStore.CreateWorkspace(
        //dbWorkspaceSkeleton,
        //creatorId,
        //workspaceInfo.InvitedUserEmails
        //);

        //return ModelToObjectConverters.ConvertWorkspace(dbWorkspace);
        throw new NotImplementedException();
    }

    public async Task<File> CreateAvatar(FileInput fileinfo)
    {
        //using var scope = Provider.CreateScope();
        //FileStore fileStore =
        //scope.ServiceProvider.GetRequiredService<FileStore>();

        //Models.File dbAvatarSkeleton =
        //new() { Name = fileinfo.Name, StoreKey = fileinfo.StoreKey };

        //Models.File dbAvatar = (
        //await fileStore.InsertFiles(
        //new List<Models.File> { dbAvatarSkeleton }
        //)
        //).First();

        //return ModelToObjectConverters.ConvertFile(dbAvatar);
        throw new NotImplementedException();
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

    public async Task<Connection<Group>> GetStarred(
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
        (List<Group> starred, bool lastPage) = await workspaceStore.LoadStarred(
            filter.WorkspaceId,
            filter.UserId,
            first,
            cols,
            after
        );

        return ToConnection(starred, after is null, lastPage);
    }

    public async Task<Connection<Message>> GetDirectMessages(
        Guid userId,
        Guid directMessageGroupId,
        MessagesFilter? filter,
        int first,
        Guid? after,
        IEnumerable<string> cols
    )
    {
        //if (filter is not null)
        //{
        //throw new NotImplementedException();
        //}

        //using var scope = Provider.CreateScope();
        //DirectMessageGroupStore directMessageGroupStore =
        //scope.ServiceProvider.GetRequiredService<DirectMessageGroupStore>();
        //(
        //List<dynamic> dbMessages,
        //List<DirectMessageReactionCount> reactionCounts,
        //bool lastPage
        //) = await directMessageGroupStore.LoadDirectMessages(
        //userId,
        //directMessageGroupId,
        //fieldInfo,
        //first,
        //after
        //);

        //Dictionary<Guid, List<DirectMessageReactionCount>?> countsDict = new();
        //foreach (DirectMessageReactionCount reactionCount in reactionCounts)
        //{
        //if (!countsDict.ContainsKey(reactionCount.DirectMessageId))
        //{
        //countsDict[reactionCount.DirectMessageId] =
        //new List<DirectMessageReactionCount>();
        //}
        //countsDict[reactionCount.DirectMessageId]!.Add(reactionCount);
        //}
        //List<Message> messages = new();
        //foreach (dynamic dbm in dbMessages)
        //{
        //Message message =
        //ModelToObjectConverters.ConvertDynamicDirectMessage(
        //dbm,
        //countsDict.GetValueOrDefault((Guid)dbm.Id, null),
        //FieldAnalyzer.ExtractUserFields("user", fieldInfo.FieldTree)
        //);
        //messages.Add(message);
        //}

        //return ModelToObjectConverters.ToConnection<Message>(
        //messages,
        //after is null,
        //lastPage
        //);
        throw new NotImplementedException();
    }
}
