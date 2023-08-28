using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;
using SlackCloneGraphQL.Types;

public class ReactionType : ObjectGraphType<Reaction>, INodeGraphType<Reaction>
{
    public ReactionType()
    {
        Name = "Reaction";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the reaction.")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAt")
            .Description("When the reaction was created")
            .Resolve(context => context.Source.CreatedAt);
        Field<NonNullGraphType<StringGraphType>>("emoji")
            .Description("The emoji associated with the reaction.")
            .Resolve(context => context.Source.Emoji);
        Field<NonNullGraphType<MessageType>>("message")
            .Description("The message reacted to.")
            .Resolve(context => context.Source.Message);
        Field<NonNullGraphType<UserType>>("user")
            .Description("The user who reacted")
            .Resolve(context => context.Source.User);
    }
}

public class Reaction : INode
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
#pragma warning disable CS8618
    public string Emoji { get; set; }
    public Message Message { get; set; }
    public User User { get; set; }
#pragma warning restore CS8618
}
