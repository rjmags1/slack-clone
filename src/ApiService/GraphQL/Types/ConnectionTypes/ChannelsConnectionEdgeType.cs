namespace SlackCloneGraphQL.Types.Connections;

public class ChannelsConnectionEdgeType
    : ConnectionEdgeType<ChannelType, Channel>
{
    public ChannelsConnectionEdgeType()
    {
        Name = "ChannelsConnectionEdge";
    }
}
