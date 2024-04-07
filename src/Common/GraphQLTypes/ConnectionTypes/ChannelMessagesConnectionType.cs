namespace Common.SlackCloneGraphQL.Types.Connections;

public class ChannelMessagesConnectionType
    : ConnectionType<MessageType, Message, ChannelMessagesConnectionEdgeType>
{
    public ChannelMessagesConnectionType()
    {
        Name = "ChannelMessagesConnection";
    }
}
