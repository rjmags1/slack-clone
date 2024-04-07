namespace Common.SlackCloneGraphQL.Types.Connections;

public class StarredConnectionEdgeType
    : ConnectionEdgeType<GroupInterfaceType, IGroup>
{
    public StarredConnectionEdgeType()
    {
        Name = "StarredConnectionEdge";
    }
}
