using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class CursorInputType : InputObjectGraphType<Cursor>
{
    public CursorInputType()
    {
        Name = "Cursor";
        Field<NonNullGraphType<IntGraphType>>("first");
        Field<IdGraphType>("after");
    }
}

public class Cursor
{
    public int First { get; set; }
    public object? After { get; set; }
}
