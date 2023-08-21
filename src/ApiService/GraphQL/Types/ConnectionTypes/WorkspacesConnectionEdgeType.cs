namespace SlackCloneGraphQL.Types.Connections;

public class WorkspacesConnectionEdgeType
    : ConnectionEdgeType<WorkspaceType, Workspace>
{
    public WorkspacesConnectionEdgeType()
    {
        Name = "WorkspacesConnectionEdge";
    }
}
