namespace SlackCloneGraphQL.Types.Connections;

public class WorkspaceConnectionEdgeType
    : ConnectionEdgeType<WorkspaceType, Workspace>
{
    public WorkspaceConnectionEdgeType()
    {
        Name = "WorkspaceConnectionEdge";
    }
}

public class WorkspaceMembersConnectionEdgeType
    : ConnectionEdgeType<WorkspaceMemberType, WorkspaceMember>
{
    public WorkspaceMembersConnectionEdgeType()
    {
        Name = "WorkspaceMembersConnectionEdge";
    }
}
