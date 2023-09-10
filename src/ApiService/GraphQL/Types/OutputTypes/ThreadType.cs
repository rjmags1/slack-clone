using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class ThreadType : ObjectGraphType<Thread>, INodeGraphType<Thread>
{
    public ThreadType()
    {
        Name = "Thread";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the thread.")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<ChannelType>>("channel")
            .Description("The channel associated with the thread.")
            .Resolve(context => context.Source.Channel);
        Field<MessageType>("firstMessage")
            .Description(
                "The top level message which started the thread. Null if was deleted."
            )
            .Resolve(context => context.Source.FirstMessage);
        Field<NonNullGraphType<ChannelMessagesConnectionType>>("messages")
            .Description("Relay connection representing messages of the thread")
            .Resolve(context => throw new NotImplementedException());
        Field<NonNullGraphType<IntGraphType>>("numMessages")
            .Description("The number of messages in the thread")
            .Resolve(context => context.Source.NumMessages);
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Description("The workspace containing the thread")
            .Resolve(context => context.Source.Workspace);
    }
}

public class Thread : INode
{
    public Guid Id { get; set; }
#pragma warning disable CS8618
    public Channel Channel { get; set; }
    public Message? FirstMessage { get; set; }
    public Connection<Message> Messages { get; set; }
    public int NumMessages { get; set; }
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618
}
