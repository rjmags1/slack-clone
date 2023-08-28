namespace SlackCloneGraphQL.Types.Connections;

public class StarredConnectionEdgeType
    : ConnectionEdgeType<MessageType, Message>
{
    public StarredConnectionEdgeType()
    {
        Name = "StarredConnectionEdge";
    }
}
