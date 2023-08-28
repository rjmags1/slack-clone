namespace SlackCloneGraphQL.Types.Connections;

public class DirectMessageGroupsConnectionType
    : ConnectionType<
        DirectMessageGroupType,
        DirectMessageGroup,
        DirectMessageGroupsConnectionEdgeType
    >
{
    public DirectMessageGroupsConnectionType()
    {
        Name = "DirectMessageGroupsConnection";
    }
}
