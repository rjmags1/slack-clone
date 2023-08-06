namespace SlackCloneGraphQL.Types.Connections;

public class WorkspaceConnectionEdgeType
    : ConnectionEdgeType<WorkspaceType, Workspace>
{
    public WorkspaceConnectionEdgeType()
    {
        Name = "WorkspacesConnectionEdge";
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
