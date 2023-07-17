using GraphQL.Types;

namespace SlackCloneGraphQL.Types.Connections;

public interface INodeGraphType<T> : IObjectGraphType
    where T : INode { }

public interface INode
{
    public Guid Id { get; set; }
}
