using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class WorkspacesFilterInputType : InputObjectGraphType<WorkspacesFilter>
{
    public WorkspacesFilterInputType()
    {
        Name = "WorkspacesFilter";
        Field<StringGraphType>("nameQuery");
        Field<NonNullGraphType<IdGraphType>>("userId");
    }
}

public class WorkspacesFilter
{
    public string? NameQuery { get; set; }
    public Guid UserId { get; set; }
}
