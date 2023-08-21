using GraphQL.Types;

namespace SlackCloneGraphQL.Types.Connections;

/// <summary>
/// See the note in RelayNodeInterfaceType.cs
/// </summary>
public interface INodeGraphType<T> : IObjectGraphType
    where T : INode { }

public interface INode
{
    public Guid Id { get; set; }
}
