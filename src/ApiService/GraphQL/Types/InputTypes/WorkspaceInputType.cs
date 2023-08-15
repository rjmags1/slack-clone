using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class WorkspaceInputType : InputObjectGraphType<WorkspaceInput>
{
    public WorkspaceInputType()
    {
        Name = "WorkspaceInput";
        Field<NonNullGraphType<StringGraphType>>("name");
        Field<StringGraphType>("description");
        Field<IdGraphType>("avatarId");
        Field<ListGraphType<NonNullGraphType<StringGraphType>>>(
            "invitedUserEmails"
        );
    }
}

public class WorkspaceInput
{
#pragma warning disable CS8618
    public string Name { get; set; }
#pragma warning restore CS8618
    public string? Description { get; set; }
    public Guid? AvatarId { get; set; }
    public List<string>? InvitedUserEmails { get; set; }
}
