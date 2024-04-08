using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

namespace Common.SlackCloneGraphQL.Types;

public class LaterFlagType : ObjectGraphType<LaterFlag>
{
    public LaterFlagType()
    {
        Name = "LaterFlag";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the later flag.")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<MessageType>>("message")
            .Description("The flagged message.")
            .Resolve(context => context.Source.Message);
        Field<NonNullGraphType<IntGraphType>>("status")
            .Description("Bitmask representing the status of the later flag")
            .Resolve(context => context.Source.Status);
    }
}

public class LaterFlag : INode
{
    public Guid Id { get; set; }
#pragma warning disable CS8618
    public Message Message { get; set; }
#pragma warning restore CS8618
    public int Status { get; set; }
}
