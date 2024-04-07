namespace Common.SlackCloneGraphQL.Types.Connections;

public class DirectMessagesConnectionType
    : ConnectionType<MessageType, Message, DirectMessagesConnectionEdgeType>
{
    public DirectMessagesConnectionType()
    {
        Name = "DirectMessagesConnection";
    }
}
