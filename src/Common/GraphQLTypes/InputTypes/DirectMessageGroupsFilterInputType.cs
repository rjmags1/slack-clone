using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types;

public class DirectMessageGroupsFilterInputType
    : InputObjectGraphType<DirectMessageGroupsFilter>
{
    public DirectMessageGroupsFilterInputType()
    {
        Name = "DirectMessageGroupsFilter";
        Field<NonNullGraphType<IdGraphType>>("userId");
        Field<NonNullGraphType<IdGraphType>>("workspaceId");
        Field<IntGraphType>("sortOrder");
    }
}

public class DirectMessageGroupsFilter
{
    public Guid UserId { get; set; }
    public Guid WorkspaceId { get; set; }
    public int? SortOrder { get; set; }
}
