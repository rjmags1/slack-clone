namespace SlackCloneGraphQL.Types.Connections;

public class DirectMessageGroupsConnectionEdgeType
    : ConnectionEdgeType<DirectMessageGroupType, DirectMessageGroup>
{
    public DirectMessageGroupsConnectionEdgeType()
    {
        Name = "DirectMessageGroupsConnectionEdge";
    }
}
