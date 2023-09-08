using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class MessageType : ObjectGraphType<Message>, INodeGraphType<Message>
{
    public MessageType()
    {
        Name = "Message";
        Field<NonNullGraphType<UserType>>("user")
            .Description("The author of the message.")
            .Resolve(context => context.Source.User);
        Field<NonNullGraphType<StringGraphType>>("content")
            .Description("The content of the message")
            .Resolve(context => context.Source.Content);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAt")
            .Description("When the message was created.")
            .Resolve(context => context.Source.CreatedAt);
        Field<NonNullGraphType<BooleanGraphType>>("draft")
            .Description("If the message is a draft or not")
            .Resolve(context => context.Source.Draft);
        Field<DateTimeGraphType>("lastEdit")
            .Description("When the message was last edited")
            .Resolve(context => context.Source.LastEdit);
        Field<ListGraphType<FileType>>("files")
            .Description(
                "Relay connection representing the collection of files associated with the message."
            )
            .Resolve(context => context.Source.Files);
        Field<NonNullGraphType<GroupInterfaceType>>("group")
            .Description(
                "The channel or direct message group associated with the message"
            )
            .Resolve(context => context.Source.Group);
        Field<NonNullGraphType<BooleanGraphType>>("isReply")
            .Description("If the message is a reply to another message or not")
            .Resolve(context => context.Source.IsReply);
        Field<ListGraphType<MentionType>>("mentions")
            .Description(
                "Relay connection representing mentions of other workspace members contained in the sent message"
            )
            .Resolve(context => context.Source.Mentions);
        Field<ListGraphType<ReactionCountType>>("reactions")
            .Description(
                "Relay connection representing reactions to the message by other workspace members"
            )
            .Resolve(context => context.Source.Reactions);
        Field<MessageType>("replyTo")
            .Description("The message this message was a reply to, if any.")
            .Resolve(context => context.Source.ReplyTo);
        Field<DateTimeGraphType>("sentAt")
            .Description("When the message was sent.")
            .Resolve(context => context.Source.SentAt);
        Field<ThreadType>("thread")
            .Description("The thread associated with the message, if any.")
            .Resolve(context => context.Source.Thread);
        Field<NonNullGraphType<IntGraphType>>("type")
            .Description("Bitmask representing the message type")
            .Resolve(context => context.Source.Type);
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Description("The workspace associated with the message")
            .Resolve(context => context.Source.Workspace);
    }
}

public class Message : INode
{
    public Guid Id { get; set; }
#pragma warning disable CS8618
    public User User { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Draft { get; set; }
    public DateTime? LastEdit { get; set; }
    public List<File> Files { get; set; }
    public IGroup Group { get; set; }
    public bool IsReply { get; set; }
    public LaterFlag? LaterFlag { get; set; }
    public List<Mention>? Mentions { get; set; }
    public List<ReactionCountType>? Reactions { get; set; }
    public Message? ReplyTo { get; set; }
    public DateTime? SentAt { get; set; }
    public Thread? Thread { get; set; }
    public int Type { get; set; }
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618
}
