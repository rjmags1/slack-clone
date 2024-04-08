namespace Common.SlackCloneGraphQL.Types.Connections;

public class WorkspaceMembersConnectionType
    : ConnectionType<
        WorkspaceMemberType,
        WorkspaceMember,
        WorkspaceMembersConnectionEdgeType
    >
{
    public WorkspaceMembersConnectionType()
    {
        Name = "WorkspaceMembersConnection";
    }
}
