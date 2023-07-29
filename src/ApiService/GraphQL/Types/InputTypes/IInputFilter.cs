using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public interface IInputFilter<T>
    where T : INode
{
    public Cursor Cursor { get; set; }
}
