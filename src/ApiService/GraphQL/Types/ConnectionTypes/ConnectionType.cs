using GraphQL.Types;
using ApiService.Utils;

namespace SlackCloneGraphQL.Types.Connections;

public abstract class ConnectionType<T, U, E> : ObjectGraphType<Connection<U>>
    where T : INodeGraphType<U>
    where U : INode
    where E : ConnectionEdgeType<T, U>
{
    public ConnectionType()
    {
        Name = $"{nameof(U)}sConnection";
        Field<NonNullGraphType<IntGraphType>>("totalEdges")
            .Description("Total number of edges in the connection");
        Field<NonNullGraphType<ListGraphType<NonNullGraphType<E>>>>("edges")
            .Description("The requested page of connection edges");
        Field<NonNullGraphType<PageInfoType>>("pageInfo")
            .Description("Metadata about the requested connection page");
    }
}

public class Connection<T>
    where T : INode
{
    public int TotalEdges { get; set; }
#pragma warning disable CS8618
    public IEnumerable<ConnectionEdge<T>> Edges { get; set; }
    public PageInfo PageInfo { get; set; }
}
