namespace SlackCloneGraphQL.Types.Connections;

public class WorkspacesConnectionType
    : ConnectionType<WorkspaceType, Workspace, WorkspacesConnectionEdgeType>
{
    public WorkspacesConnectionType()
    {
        Name = "WorkspacesConnection";
    }
}
