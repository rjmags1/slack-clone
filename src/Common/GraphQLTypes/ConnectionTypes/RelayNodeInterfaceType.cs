using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types.Connections;

/// <summary>
///  This class implements the SDL Node interface used by Relay.
///  This is purposely distinct from the INodeGraphType C# interface.
///  Both indicate the presence of an id field, but the SDL interface
///  is used only on types that are capable of being fetched by fragment
///  queries in Relay.
/// </summary>
public class RelayNodeInterfaceType : InterfaceGraphType<INode>
{
    public RelayNodeInterfaceType()
    {
        Field<NonNullGraphType<IdGraphType>>("id");
    }
}
