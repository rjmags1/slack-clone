using GraphQL.Types;
using Common.SlackCloneGraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

public class MentionType : ObjectGraphType<Mention>, INodeGraphType<Mention>
{
    public MentionType()
    {
        Name = "Mention";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the mention.")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAt")
            .Description("When the mention was created.")
            .Resolve(context => context.Source.CreatedAt);
        Field<NonNullGraphType<MessageType>>("message")
            .Description("The message containing the mention.")
            .Resolve(context => context.Source.Message);
        Field<NonNullGraphType<UserType>>("mentioned")
            .Description("The person mentioned.")
            .Resolve(context => context.Source.Mentioned);
        Field<NonNullGraphType<UserType>>("mentioner")
            .Description("The person who created the mention.")
            .Resolve(context => context.Source.Mentioner);
    }
}

public class Mention : INode
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
#pragma warning disable CS8618
    public Message Message { get; set; }
    public User Mentioned { get; set; }
    public User Mentioner { get; set; }
#pragma warning restore CS8618
}
