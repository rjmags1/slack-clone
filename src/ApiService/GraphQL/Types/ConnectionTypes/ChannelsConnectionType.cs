namespace SlackCloneGraphQL.Types.Connections;

public class ChannelsConnectionType
    : ConnectionType<ChannelType, Channel, ChannelsConnectionEdgeType>
{
    public ChannelsConnectionType()
    {
        Name = "ChannelsConnection";
    }
}
