namespace Common.SlackCloneGraphQL.Types.Connections;

public class WorkspaceMembersConnectionEdgeType
    : ConnectionEdgeType<WorkspaceMemberType, WorkspaceMember>
{
    public WorkspaceMembersConnectionEdgeType()
    {
        Name = "WorkspaceMembersConnectionEdge";
    }
}
