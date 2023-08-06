using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class WorkspacesPageDataType : ObjectGraphType<WorkspacesPageData>
{
    public WorkspacesPageDataType()
    {
        Name = "WorkspacesPageData";
        Field<NonNullGraphType<UserType>>("user")
            .Description("The authenticated user");
        Field<NonNullGraphType<WorkspacesConnectionType>>("workspaces")
            .Description("The current workspaces connection page");
    }
}

public class WorkspacesPageData
{
#pragma warning disable CS8618
    public User User { get; set; }
    public Connection<Workspace> Workspaces { get; set; }
#pragma warning restore CS8618
}
