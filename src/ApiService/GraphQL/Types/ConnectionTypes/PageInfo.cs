using GraphQL.Types;

namespace SlackCloneGraphQL.Types.Connections;

public class PageInfoType : ObjectGraphType<PageInfo>
{
    public PageInfoType()
    {
        Name = "pageInfo";
        Field<IdGraphType>("startCursor")
            .Description("Start cursor for the page");
        Field<IdGraphType>("endCursor").Description("End cursor for the page");
        Field<NonNullGraphType<BooleanGraphType>>("hasNextPage")
            .Description("If there is another page of edges in the connection");
        Field<NonNullGraphType<BooleanGraphType>>("hasPreviousPage")
            .Description(
                "If there was a previous page of edges in the connection"
            );
    }
}

public class PageInfo
{
#pragma warning disable CS8618
    Guid? StartCursor { get; set; }
    Guid? EndCursor { get; set; }
#pragma warning restore CS8618
    bool HasNextPage { get; set; }
    bool HasPreviousPage { get; set; }
}
