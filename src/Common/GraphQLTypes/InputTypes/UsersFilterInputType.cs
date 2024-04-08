using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types;

public class UsersFilterInputType : InputObjectGraphType<UsersFilter>
{
    public UsersFilterInputType()
    {
        Name = "UsersFilter";
        Field<IdGraphType>("userId");
        Field<NonNullGraphType<IdGraphType>>("workspaceId");
        Field<ListGraphType<NonNullGraphType<IdGraphType>>>("users");
        Field<ListGraphType<NonNullGraphType<IdGraphType>>>("channels");
        Field<DateTimeGraphType>("joinedAfter");
        Field<DateTimeGraphType>("joinedBefore");
        Field<StringGraphType>("query");
        Field<IntGraphType>("queryTypeMask");
    }
}

public class UsersFilter
{
    public Guid UserId { get; set; }
    public Guid WorkspaceId { get; set; }
    public List<Guid>? Users { get; set; }
    public List<Guid>? Channels { get; set; }
    public DateTime? JoinedAfter { get; set; }
    public DateTime? JoinedBefore { get; set; }
    public string? Query { get; set; }
    public int? QueryTypeMask { get; set; }
}
