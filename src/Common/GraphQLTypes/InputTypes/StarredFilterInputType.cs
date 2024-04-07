using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types;

public class StarredFilterInputType : InputObjectGraphType<StarredFilter>
{
    public StarredFilterInputType()
    {
        Name = "StarredFilter";
        Field<NonNullGraphType<IdGraphType>>("userId");
        Field<NonNullGraphType<IdGraphType>>("workspaceId");
    }
}

public class StarredFilter
{
    public Guid UserId { get; set; }
    public Guid WorkspaceId { get; set; }
}
