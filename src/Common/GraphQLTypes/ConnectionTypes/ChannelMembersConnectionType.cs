namespace Common.SlackCloneGraphQL.Types.Connections;

public class ChannelMembersConnectionType
    : ConnectionType<
        ChannelMemberType,
        ChannelMember,
        ChannelMembersConnectionEdgeType
    >
{
    public ChannelMembersConnectionType()
    {
        Name = "ChannelMembersConnection";
    }
}
