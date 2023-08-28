using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class ChannelType : ObjectGraphType<Channel>, INodeGraphType<Channel>
{
    public ChannelType()
    {
        Name = "Channel";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the channel.")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<BooleanGraphType>>("allowThreads")
            .Description("Whether threads are allowed in the channel.")
            .Resolve(context => context.Source.AllowThreads);
        Field<NonNullGraphType<IntGraphType>>("allowedPostersMask")
            .Description(
                "Bitmask representing who is allowed to post in the channel."
            )
            .Resolve(context => context.Source.AllowedPostersMask);
        Field<NonNullGraphType<FileType>>("avatar")
            .Description("The avatar of the channel.")
            .Resolve(context => context.Source.Avatar);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAt")
            .Description("When the channel was created.")
            .Resolve(context => context.Source.CreatedAt);
        Field<NonNullGraphType<ChannelMemberType>>("createdBy")
            .Description("Who created the channel.")
            .Resolve(context => context.Source.CreatedBy);
        Field<NonNullGraphType<StringGraphType>>("description")
            .Description("A brief description of the channel.")
            .Resolve(context => context.Source.Description);
        Field<NonNullGraphType<ChannelMembersConnectionType>>("members")
            .Description(
                "Relay connection representing collection of channel members."
            )
            .Resolve(context => throw new NotImplementedException());
        Field<NonNullGraphType<ChannelMessagesConnectionType>>("messages")
            .Description(
                "Relay connection representing collection of channel messages."
            )
            .Resolve(context => throw new NotImplementedException());
        Field<NonNullGraphType<StringGraphType>>("name")
            .Description("The name of the channel.")
            .Resolve(context => context.Source.Name);
        Field<NonNullGraphType<IntGraphType>>("numMembers")
            .Description("The number of members of the channel")
            .Resolve(context => context.Source.NumMembers);
        Field<NonNullGraphType<BooleanGraphType>>("private")
            .Description(
                "Whether viewing the channel is restricted to certain workspace members or not."
            )
            .Resolve(context => context.Source.Private);
        Field<NonNullGraphType<StringGraphType>>("topic")
            .Description("The topic of the channel")
            .Resolve(context => context.Source.Topic);
        Field<NonNullGraphType<WorkspaceType>>("workspace")
            .Description("The workspace containing the channel")
            .Resolve(context => context.Source.Workspace);
    }
}

public class Channel : INode, IGroup
{
    public Guid Id { get; set; }
    public bool AllowThreads { get; set; }
    public int AllowedPostersMask { get; set; }
#pragma warning disable CS8618
    public File Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public ChannelMember CreatedBy { get; set; }
    public string Description { get; set; }
    public Connection<ChannelMember> Members { get; set; }
    public Connection<Message> Messages { get; set; }
    public string Name { get; set; }
    public int NumMembers { get; set; }
    public bool Private { get; set; }
    public string Topic { get; set; }
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618
}