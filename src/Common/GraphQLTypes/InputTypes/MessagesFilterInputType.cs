using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types;

public class MessagesFilterInputType : InputObjectGraphType<MessagesFilter>
{
    public MessagesFilterInputType()
    {
        Name = "MessagesFilter";
        Field<IdGraphType>("workspaceId");
        Field<IdGraphType>("receiverId");
        Field<IdGraphType>("senderId");
        Field<BooleanGraphType>("unread");
        Field<BooleanGraphType>("directMessages");
        Field<BooleanGraphType>("channelMessages");
        Field<ListGraphType<IdGraphType>>("channelIds");
        Field<IntGraphType>("sortOrder");
        Field<ListGraphType<IdGraphType>>("from");
        Field<ListGraphType<IdGraphType>>("to");
        Field<ListGraphType<IdGraphType>>("mentioning");
        Field<DateTimeGraphType>("before");
        Field<DateTimeGraphType>("after");
        Field<StringGraphType>("query");
    }
}

public class MessagesFilter
{
    public Guid? WorkspaceId { get; set; }
    public Guid? ReceiverId { get; set; }
    public Guid? SenderId { get; set; }
    public bool? Unread { get; set; }
    public bool? DirectMessages { get; set; }
    public bool? ChannelMessages { get; set; }
    public List<Guid>? ChannelIds { get; set; }
    public int? SortOrder { get; set; }
    public List<Guid>? From { get; set; }
    public List<Guid>? To { get; set; }
    public List<Guid>? Mentioning { get; set; }
    public DateTime? Before { get; set; }
    public DateTime? After { get; set; }
    public string? Query { get; set; }
}
