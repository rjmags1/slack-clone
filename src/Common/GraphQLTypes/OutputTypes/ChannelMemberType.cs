using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

namespace Common.SlackCloneGraphQL.Types;

public class ChannelMemberType
    : ObjectGraphType<ChannelMember>,
        INodeGraphType<ChannelMember>
{
    public ChannelMemberType()
    {
        Name = "ChannelMember";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID associated with the channel membership.")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<BooleanGraphType>>("admin")
            .Description(
                "Whether the member is an admin of the channel or not."
            )
            .Resolve(context => context.Source.Admin);
        Field<NonNullGraphType<ChannelMemberInfoType>>("memberInfo")
            .Description("Additional metadata about the channel membership")
            .Resolve(context => context.Source.MemberInfo);
        Field<NonNullGraphType<UserType>>("user")
            .Description("The user associated with the channel membership")
            .Resolve(context => context.Source.User);
    }
}

public class ChannelMember : INode
{
    public Guid Id { get; set; }
    public bool Admin { get; set; }
#pragma warning disable CS8618
    public ChannelMemberInfo MemberInfo { get; set; }
    public User User { get; set; }
#pragma warning restore CS8618
}
