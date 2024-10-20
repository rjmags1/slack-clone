using Common.SlackCloneGraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;
using File = Common.SlackCloneGraphQL.Types.File;

namespace Common.SlackCloneGraphQL;

public interface ISlackCloneData
{
    public Task<User> GetUserById(Guid userId, IEnumerable<string> cols);

    public Task<Channel> GetChannel(Guid channelId);

    public Task<DirectMessageGroup> GetDirectMessageGroup(Guid groupId);

    public Task<Connection<Message>> GetChannelMessages(
        Guid channelId,
        MessagesFilter? filter,
        int first,
        Guid? after,
        List<string> cols
    );

    public Task<Connection<ChannelMember>> GetChannelMembers(
        Guid channelId,
        UsersFilter? filter,
        int first,
        Guid? after,
        List<string> cols
    );

    public Task<Connection<WorkspaceMember>> GetWorkspaceMembers(
        Guid workspaceId,
        UsersFilter? filter,
        int first,
        Guid? after,
        List<string> cols
    );

    public Task<Connection<Workspace>> GetWorkspaces(
        int first,
        Guid? after,
        WorkspacesFilter filter,
        IEnumerable<string> cols
    );

    public Task<Workspace> GetWorkspace(Guid workspaceId, List<string> dbCols);

    public Task<Workspace> CreateWorkspace(
        WorkspaceInput workspaceInfo,
        Guid creatorId
    );

    public Task<File> CreateAvatar(FileInput fileInfo);

    public Task<bool> ValidUserEmail(string email);

    public Task<Connection<Channel>> GetChannels(
        int first,
        Guid? after,
        ChannelsFilter filer,
        List<string> cols
    );

    public Task<Connection<DirectMessageGroup>> GetDirectMessageGroups(
        int first,
        Guid? after,
        DirectMessageGroupsFilter filer,
        IEnumerable<string> cols
    );

    public Task<Connection<IGroup>> GetStarred(
        int first,
        Guid? after,
        StarredFilter filter,
        List<string> cols
    );

    public Task<Connection<Message>> GetDirectMessages(
        Guid directMessageGroupId,
        MessagesFilter? filter,
        int first,
        Guid? after,
        List<string> cols
    );
}
