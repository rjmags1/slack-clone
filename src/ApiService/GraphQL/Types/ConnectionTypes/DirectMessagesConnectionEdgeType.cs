using SlackCloneGraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

public class DirectMessagesConnectionEdgeType
    : ConnectionEdgeType<MessageType, Message>
{
    public DirectMessagesConnectionEdgeType()
    {
        Name = "DirectMessagesConnectionEdge";
    }
}
