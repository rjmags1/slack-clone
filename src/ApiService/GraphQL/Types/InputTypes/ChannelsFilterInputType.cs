using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class ChannelsFilterInputType : InputObjectGraphType<ChannelsFilter>
{
    public ChannelsFilterInputType()
    {
        Name = "ChannelsFilter";
        Field<NonNullGraphType<IdGraphType>>("workspaceId");
        Field<NonNullGraphType<IdGraphType>>("userId");
        Field<IntGraphType>("sortOrder");
        Field<StringGraphType>("query");
        Field<ListGraphType<NonNullGraphType<UserInputType>>>("with");
        Field<DateTimeGraphType>("lastActivityBefore");
        Field<DateTimeGraphType>("lastActivityAfter");
        Field<DateTimeGraphType>("createdBefore");
        Field<DateTimeGraphType>("createdAfter");
    }
}

public class ChannelsFilter
{
    public Guid? WorkspaceId { get; set; }
    public Guid? UserId { get; set; }
    public int? SortOrder { get; set; }
    public string? Query { get; set; }
    public List<UserInput>? With { get; set; }
    public DateTime? LastActivityBefore { get; set; }
    public DateTime? LastActivityAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? CreatedAfter { get; set; }
}
