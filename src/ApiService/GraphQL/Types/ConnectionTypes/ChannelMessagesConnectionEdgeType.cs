namespace SlackCloneGraphQL.Types.Connections;

public class ChannelMessagesConnectionEdgeType
    : ConnectionEdgeType<MessageType, Message>
{
    public ChannelMessagesConnectionEdgeType()
    {
        Name = "ChannelMessagesConnectionEdge";
    }
}
