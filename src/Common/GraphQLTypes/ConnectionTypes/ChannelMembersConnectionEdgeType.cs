namespace Common.SlackCloneGraphQL.Types.Connections;

public class ChannelMembersConnectionEdgeType
    : ConnectionEdgeType<ChannelMemberType, ChannelMember>
{
    public ChannelMembersConnectionEdgeType()
    {
        Name = "ChannelMembersConnectionEdge";
    }
}
