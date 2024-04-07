using PersistenceService.Stores;
using Models = PersistenceService.Models;
using Common.SlackCloneGraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;
using PersistenceService.Utils.GraphQL;
using File = Common.SlackCloneGraphQL.Types.File;
using WorkspaceStore = PersistenceService.Stores.WorkspaceStore;
using Common.SlackCloneGraphQL;

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

    public async Task<Channel> GetChannel(Guid channelId)
    {
        using var scope = Provider.CreateScope();
        ChannelStore channelStore =
            scope.ServiceProvider.GetRequiredService<ChannelStore>();
        Models.Channel dbChannel = await channelStore.LoadChannel(channelId);

        return ModelToObjectConverters.ConvertChannel(
            dbChannel,
            skipWorkspace: true
        );
    }

    public async Task<DirectMessageGroup> GetDirectMessageGroup(Guid groupId)
    {
        using var scope = Provider.CreateScope();
        DirectMessageGroupStore directMessageGroupStore =
            scope.ServiceProvider.GetRequiredService<DirectMessageGroupStore>();
        Models.DirectMessageGroup dbGroup =
            await directMessageGroupStore.LoadDirectMessageGroup(groupId);

        return ModelToObjectConverters.ConvertDirectMessageGroup(
            dbGroup,
            skipWorkspace: true
        );
    }

    public async Task<Connection<Message>> GetChannelMessages(
        Guid userId,
        Guid channelId,
        FieldInfo fieldInfo,
        MessagesFilter? filter,
        int first,
        Guid? after
    )
    {
        // TODO: add filtering capabilities

        using var scope = Provider.CreateScope();
        ChannelStore channelStore =
            scope.ServiceProvider.GetRequiredService<ChannelStore>();
        (
            List<dynamic> dbMessages,
            List<ChannelMessageReactionCount> reactionCounts,
            bool lastPage
        ) = await channelStore.LoadChannelMessages(
            userId,
            channelId,
            fieldInfo,
            first,
            after
        );

        Dictionary<Guid, List<ChannelMessageReactionCount>?> countsDict = new();
        foreach (ChannelMessageReactionCount reactionCount in reactionCounts)
        {
            if (!countsDict.ContainsKey(reactionCount.ChannelMessageId))
            {
                countsDict[reactionCount.ChannelMessageId] =
                    new List<ChannelMessageReactionCount>();
            }
            countsDict[reactionCount.ChannelMessageId]!.Add(reactionCount);
        }
        List<Message> messages = new();
        foreach (dynamic dbm in dbMessages)
        {
            Message message =
                ModelToObjectConverters.ConvertDynamicChannelMessage(
                    dbm,
                    countsDict.GetValueOrDefault((Guid)dbm.Id, null),
                    FieldAnalyzer.ExtractUserFields("user", fieldInfo.FieldTree)
                );
            messages.Add(message);
        }

        return ModelToObjectConverters.ToConnection<Message>(
            messages,
            after is null,
            lastPage
        );
    }

    public async Task<Connection<ChannelMember>> GetChannelMembers(
        FieldInfo fieldInfo,
        Guid channelId,
        UsersFilter filter,
        int first,
        Guid? after
    )
    {
        using var scope = Provider.CreateScope();
        ChannelStore channelStore =
            scope.ServiceProvider.GetRequiredService<ChannelStore>();
        (List<dynamic> dbMembers, bool lastPage) =
            await channelStore.LoadChannelMembers(
                filter.UserId,
                first,
                fieldInfo.FieldTree,
                channelId,
                after
            );

        List<ChannelMember> members = new();
        foreach (dynamic dbm in dbMembers)
        {
            ChannelMember member =
                ModelToObjectConverters.ConvertDynamicChannelMember(
                    dbm,
                    FieldAnalyzer.ExtractUserFields("user", fieldInfo.FieldTree)
                );
            members.Add(member);
        }

        return ModelToObjectConverters.ToConnection<ChannelMember>(
            members,
            after is null,
            lastPage
        );
    }

    public async Task<Connection<WorkspaceMember>> GetWorkspaceMembers(
        FieldInfo fieldInfo,
        UsersFilter filter,
        int first,
        Guid? after
    )
    {
        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (List<dynamic> dbMembers, bool lastPage) =
            await workspaceStore.LoadWorkspaceMembers(
                filter.UserId,
                first,
                fieldInfo.FieldTree,
                filter.WorkspaceId,
                after
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
            after is null,
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
            dbWorkspaceSkeleton.Avatar = fileStore.GetFileById(
                workspaceInfo.AvatarId
            );
        }

        Models.Workspace dbWorkspace = await workspaceStore.CreateWorkspace(
            dbWorkspaceSkeleton,
            creatorId,
            workspaceInfo.InvitedUserEmails
        );

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

        return ModelToObjectConverters.ConvertFile(dbAvatar);
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
        FieldInfo fieldInfo
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
        if (
            fieldInfo.SubfieldNames.Contains("members")
            || fieldInfo.SubfieldNames.Contains("messages")
        )
        {
            throw new InvalidOperationException(
                "Requested a connection within a connection"
            );
        }

        using var scope = Provider.CreateScope();
        ChannelStore channelStore =
            scope.ServiceProvider.GetRequiredService<ChannelStore>();
        (List<dynamic> dbChannels, bool lastPage) =
            await channelStore.LoadChannels(
                (Guid)filter.WorkspaceId,
                (Guid)filter.UserId,
                first,
                fieldInfo.FieldTree,
                after
            );

        List<Channel> channels = new();
        foreach (dynamic dbc in dbChannels)
        {
            Channel channel = (Channel)
                ModelToObjectConverters.ConvertDynamicChannel(dbc);
            channels.Add(channel);
        }

        return ModelToObjectConverters.ToConnection<Channel>(
            channels,
            after is null,
            lastPage
        );
    }

    public async Task<Connection<DirectMessageGroup>> GetDirectMessageGroups(
        int first,
        Guid? after,
        DirectMessageGroupsFilter filter,
        FieldInfo fieldInfo
    )
    {
        if (filter.SortOrder is not null)
        {
            throw new NotImplementedException();
        }
        if (fieldInfo.SubfieldNames.Contains("messages"))
        {
            throw new InvalidOperationException(
                "Requested a connection within a connection"
            );
        }

        using var scope = Provider.CreateScope();
        DirectMessageGroupStore directMessageGroupStore =
            scope.ServiceProvider.GetRequiredService<DirectMessageGroupStore>();
        (List<dynamic> dbDirectMessageGroups, bool lastPage) =
            await directMessageGroupStore.LoadDirectMessageGroups(
                filter.WorkspaceId,
                filter.UserId,
                first,
                fieldInfo.FieldTree,
                after
            );

        List<DirectMessageGroup> groups = new();
        foreach (dynamic dbg in dbDirectMessageGroups)
        {
            DirectMessageGroup directMessageGroup = (DirectMessageGroup)
                ModelToObjectConverters.ConvertDynamicDirectMessageGroup(dbg);
            groups.Add(directMessageGroup);
        }

        return ModelToObjectConverters.ToConnection<DirectMessageGroup>(
            groups,
            after is null,
            lastPage
        );
    }

    public async Task<Connection<IGroup>> GetStarred(
        int first,
        Guid? after,
        StarredFilter filter,
        FieldInfo fieldInfo
    )
    {
        if (
            fieldInfo.SubfieldNames.Contains("members")
            || fieldInfo.SubfieldNames.Contains("messages")
        )
        {
            throw new InvalidOperationException(
                "Requested a connection within a connection"
            );
        }

        using var scope = Provider.CreateScope();
        WorkspaceStore workspaceStore =
            scope.ServiceProvider.GetRequiredService<WorkspaceStore>();
        (List<WorkspaceStore.StarredInfo> dbStarred, bool lastPage) =
            await workspaceStore.LoadStarred(
                filter.WorkspaceId,
                filter.UserId,
                first,
                fieldInfo.FieldTree,
                after
            );

        List<IGroup> starred = new();
        foreach (WorkspaceStore.StarredInfo dbg in dbStarred)
        {
            if (dbg.Type == WorkspaceStore.CHANNEL)
            {
                Channel channel = (Channel)
                    ModelToObjectConverters.ConvertDynamicChannel(dbg.Starred);
                starred.Add(channel);
            }
            else if (dbg.Type == WorkspaceStore.DIRECT_MESSAGE_GROUP)
            {
                DirectMessageGroup directMessageGroup = (DirectMessageGroup)
                    ModelToObjectConverters.ConvertDynamicDirectMessageGroup(
                        dbg.Starred
                    );
                starred.Add(directMessageGroup);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        return ModelToObjectConverters.ToConnection<IGroup>(
            starred,
            after is null,
            lastPage
        );
    }

    public async Task<Connection<Message>> GetDirectMessages(
        Guid userId,
        Guid directMessageGroupId,
        FieldInfo fieldInfo,
        MessagesFilter? filter,
        int first,
        Guid? after
    )
    {
        if (filter is not null)
        {
            throw new NotImplementedException();
        }

        using var scope = Provider.CreateScope();
        DirectMessageGroupStore directMessageGroupStore =
            scope.ServiceProvider.GetRequiredService<DirectMessageGroupStore>();
        (
            List<dynamic> dbMessages,
            List<DirectMessageReactionCount> reactionCounts,
            bool lastPage
        ) = await directMessageGroupStore.LoadDirectMessages(
            userId,
            directMessageGroupId,
            fieldInfo,
            first,
            after
        );

        Dictionary<Guid, List<DirectMessageReactionCount>?> countsDict = new();
        foreach (DirectMessageReactionCount reactionCount in reactionCounts)
        {
            if (!countsDict.ContainsKey(reactionCount.DirectMessageId))
            {
                countsDict[reactionCount.DirectMessageId] =
                    new List<DirectMessageReactionCount>();
            }
            countsDict[reactionCount.DirectMessageId]!.Add(reactionCount);
        }
        List<Message> messages = new();
        foreach (dynamic dbm in dbMessages)
        {
            Message message =
                ModelToObjectConverters.ConvertDynamicDirectMessage(
                    dbm,
                    countsDict.GetValueOrDefault((Guid)dbm.Id, null),
                    FieldAnalyzer.ExtractUserFields("user", fieldInfo.FieldTree)
                );
            messages.Add(message);
        }

        return ModelToObjectConverters.ToConnection<Message>(
            messages,
            after is null,
            lastPage
        );
    }
}
