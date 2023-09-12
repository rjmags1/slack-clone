using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class DirectMessageGroupMemberType
    : ObjectGraphType<DirectMessageGroupMember>,
        INodeGraphType<DirectMessageGroupMember>
{
    public DirectMessageGroupMemberType()
    {
        Name = "DirectMessageGroupMember";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description(
                "The UUID associated with the direct message group membership."
            )
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<IdGraphType>>("groupId")
            .Description(
                "The direct message group associated with the membership"
            )
            .Resolve(context => context.Source.DirectMessageGroupId);
        Field<NonNullGraphType<DateTimeGraphType>>("joinedAt")
            .Description("When the member joined the group")
            .Resolve(context => context.Source.JoinedAt);
        Field<DateTimeGraphType>("lastViewedAt")
            .Description("When the member last viewed the group")
            .Resolve(context => context.Source.LastViewedAt);
        Field<NonNullGraphType<BooleanGraphType>>("starred")
            .Description("If the user starred the group")
            .Resolve(context => context.Source.Starred);
        Field<NonNullGraphType<UserType>>("user")
            .Description("The user associated with the channel membership")
            .Resolve(context => context.Source.User);
    }
}

public class DirectMessageGroupMember : INode
{
    public Guid Id { get; set; }
    public Guid DirectMessageGroupId { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public bool Starred { get; set; }
#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618
}
