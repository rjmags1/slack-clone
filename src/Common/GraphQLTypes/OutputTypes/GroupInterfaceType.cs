using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

namespace Common.SlackCloneGraphQL.Types;

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

public class Group : IGroup
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Workspace Workspace { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime StarredAt { get; set; }
}
