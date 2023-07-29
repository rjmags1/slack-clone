using GraphQL.Types;
using ApiService.Utils;

namespace SlackCloneGraphQL.Types.Connections;

public abstract class ConnectionEdgeType<T, U>
    : ObjectGraphType<ConnectionEdge<U>>
    where T : INodeGraphType<U>
    where U : INode
{
    public ConnectionEdgeType()
    {
        Name = StringUtils.ToLowerFirstLetter($"{nameof(T)}ConnectionEdge");
        Field<NonNullGraphType<T>>("node")
            .Description("The node pointed to by a connection edge.");
        Field<NonNullGraphType<IdGraphType>>("cursor")
            .Description(
                "The cursor (UUID) associated with the node for an edge"
            );
    }
}

public class ConnectionEdge<T>
    where T : INode
{
#pragma warning disable CS8618
    public T Node { get; set; }
    public Guid Cursor { get; set; }
}
