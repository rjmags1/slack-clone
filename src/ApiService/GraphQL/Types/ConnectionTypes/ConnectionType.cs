using GraphQL.Types;
using ApiService.Utils;

namespace SlackCloneGraphQL.Types.Connections;

public class ConnectionType<T, U> : ObjectGraphType<Connection<T, U>>
    where T : INodeGraphType<U>
    where U : INode
{
    public ConnectionType()
    {
        Name = StringUtils.ToLowerFirstLetter($"{nameof(T)}Connection");
        Field<NonNullGraphType<IntGraphType>>("totalEdges")
            .Description("Total number of edges in the connection");
        Field<
            NonNullGraphType<
                ListGraphType<NonNullGraphType<ConnectionEdgeType<T, U>>>
            >
        >("edges")
            .Description("The requested page of connection edges");
        Field<NonNullGraphType<PageInfoType>>("pageInfo")
            .Description("Metadata about the requested connection page");
    }
}

public class Connection<T, U>
    where T : INodeGraphType<U>
    where U : INode
{
    public int TotalEdges { get; set; }
#pragma warning disable CS8618
    public IEnumerable<ConnectionEdge<T, U>> Edges { get; set; }
    public PageInfo PageInfo { get; set; }
}
