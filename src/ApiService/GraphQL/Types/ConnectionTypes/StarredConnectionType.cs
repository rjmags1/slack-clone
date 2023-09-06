namespace SlackCloneGraphQL.Types.Connections;

public class StarredConnectionType
    : ConnectionType<GroupInterfaceType, IGroup, StarredConnectionEdgeType>
{
    public StarredConnectionType()
    {
        Name = "StarredConnection";
    }
}
