namespace SlackCloneGraphQL.Types.Connections;

public class WorkspacesConnectionType
    : ConnectionType<WorkspaceType, Workspace, WorkspaceConnectionEdgeType>
{
    public WorkspacesConnectionType()
    {
        Name = "WorkspacesConnection";
    }
}

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
