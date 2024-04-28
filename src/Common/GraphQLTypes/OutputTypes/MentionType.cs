using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

namespace Common.SlackCloneGraphQL.Types;

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
        Field<NonNullGraphType<UserType>>("mentionedId")
            .Description("The person mentioned.")
            .Resolve(context => context.Source.MentionedId);
        Field<NonNullGraphType<UserType>>("mentionerId")
            .Description("The person who created the mention.")
            .Resolve(context => context.Source.MentionerId);
    }
}

public class Mention : INode
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
#pragma warning disable CS8618
    public Message Message { get; set; }
    public Guid MentionedId { get; set; }
    public Guid MentionerId { get; set; }
#pragma warning restore CS8618
}
