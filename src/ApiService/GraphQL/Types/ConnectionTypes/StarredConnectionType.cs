namespace SlackCloneGraphQL.Types.Connections;

public class StarredConnectionType
    : ConnectionType<MessageType, Message, StarredConnectionEdgeType>
{
    public StarredConnectionType()
    {
        Name = "StarredConnection";
    }
}
