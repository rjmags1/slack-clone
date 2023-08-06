using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class WorkspacesFilterInputType : InputObjectGraphType<WorkspacesFilter>
{
    public WorkspacesFilterInputType()
    {
        Name = "WorkspacesFilter";
        Field<NonNullGraphType<CursorInputType>>("cursor");
        Field<StringGraphType>("nameQuery");
        Field<NonNullGraphType<IdGraphType>>("userId");
    }
}

public class WorkspacesFilter : IInputFilter<Workspace>
{
#pragma warning disable CS8618
    public Cursor Cursor { get; set; }
    public string? NameQuery { get; set; }
#pragma warning restore CS8618
    public Guid UserId { get; set; }
}
