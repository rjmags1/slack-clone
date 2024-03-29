using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class GroupInterfaceType : InterfaceGraphType<IGroup>
{
    public GroupInterfaceType()
    {
        Name = "Group";
        Field<NonNullGraphType<IdGraphType>>("id");
        Field<NonNullGraphType<DateTimeGraphType>>("createdAtUTC");
        Field<NonNullGraphType<WorkspaceType>>("workspace");
        Field<NonNullGraphType<StringGraphType>>("name");
    }
}

public interface IGroup : INode
{
    public DateTime CreatedAt { get; set; }
    public Workspace Workspace { get; set; }
    public string Name { get; set; }
}
