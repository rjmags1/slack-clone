using GraphQL.Types;

namespace SlackCloneGraphQL.Types.Connections;

public class ConnectionEdgeType<T, U> : ObjectGraphType<ConnectionEdge<T, U>>
    where T : INodeGraphType<U>
    where U : INode
{
    public ConnectionEdgeType()
    {
        Name = $"{nameof(T)}ConnectionEdge";
        Field<NonNullGraphType<T>>("node")
            .Description("The node pointed to by a connection edge.");
        Field<NonNullGraphType<IdGraphType>>("cursor")
            .Description(
                "The cursor (UUID) associated with the node for an edge"
            );
    }
}

public class ConnectionEdge<T, U>
    where T : INodeGraphType<U>
    where U : INode
{
#pragma warning disable CS8618
    public T Node { get; set; }
    public string Cursor { get; set; }
}
