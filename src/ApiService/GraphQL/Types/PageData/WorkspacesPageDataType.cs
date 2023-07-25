using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class WorkspacesPageDataType : ObjectGraphType<WorkspacesPageData>
{
    public WorkspacesPageDataType()
    {
        Name = "workspacesPageData";
        Field<NonNullGraphType<UserType>>("user")
            .Description("The authenticated user");
        Field<NonNullGraphType<ConnectionType<WorkspaceType, Workspace>>>(
                "workspaces"
            )
            .Description("The current workspaces connection page")
            .Argument<NonNullGraphType<WorkspacesFilterInputType>>(
                "filter",
                "The filter for managing the workspaces connection"
            );
    }
}

public class WorkspacesPageData
{
#pragma warning disable CS8618
    public User User { get; set; }
    public Connection<WorkspaceType, Workspace> Workspaces { get; set; }
#pragma warning restore CS8618
}
