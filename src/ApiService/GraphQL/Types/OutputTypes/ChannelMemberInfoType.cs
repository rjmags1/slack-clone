using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class ChannelMemberInfoType : ObjectGraphType<ChannelMemberInfo>
{
    public ChannelMemberInfoType()
    {
        Name = "ChannelMemberInfo";
        Field<NonNullGraphType<BooleanGraphType>>("enableNotifications")
            .Description(
                "Whether the member will receive notifications or not from the channel."
            )
            .Resolve(context => context.Source.EnableNotifications);
        Field<NonNullGraphType<DateTimeGraphType>>("lastViewedAt")
            .Description("When the channel member last viewed the channel.")
            .Resolve(context => context.Source.LastViewedAt);
        Field<NonNullGraphType<BooleanGraphType>>("starred")
            .Description("Whether the user has starred the channel or not.")
            .Resolve(context => context.Source.Starred);
    }
}

public class ChannelMemberInfo
{
    public bool EnableNotifications { get; set; }
    public DateTime LastViewedAt { get; set; }
    public bool Starred { get; set; }
}
