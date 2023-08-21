using System.Security.Claims;
using ApiService.Utils;
using GraphQL;
using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class WorkspacesPageDataType : ObjectGraphType<WorkspacesPageData>
{
    public WorkspacesPageDataType(SlackCloneData data)
    {
        Name = "WorkspacesPageData";
        Interface<RelayNodeInterfaceType>();
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description(
                "A UUID for page data - included for Relay compatibility"
            )
            .Resolve(context => context.Source.Id ?? Guid.NewGuid());
        Field<NonNullGraphType<UserType>>("user")
            .Description("The authenticated user")
            .Argument<NonNullGraphType<IdGraphType>>("id")
            .Directive(
                "requiresClaimMapping",
                "claimName",
                "sub",
                "constraint",
                "equivalent-id"
            )
            .ResolveAsync(async context =>
            {
                var userId = context.GetArgument<Guid>("id");
                var fragments = (
                    context.UserContext["fragments"]
                    as Dictionary<string, string>
                )!;
                var userFieldsInfo = FieldAnalyzer.User(context, fragments);
                return await data.GetUserById(
                    userId,
                    userFieldsInfo.SubfieldNames
                );
            });
        Field<NonNullGraphType<WorkspacesConnectionType>>("workspaces")
            .Description("The current page of this workspaces connection")
            .Argument<WorkspacesFilterInputType>("filter")
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .ResolveAsync(async context =>
            {
                var first = context.GetArgument<int>("first");
                var after = context.GetArgument<Guid?>("after");
                var workspacesFilter = context.GetArgument<WorkspacesFilter?>(
                    "filter"
                );
                if (workspacesFilter is null)
                {
                    var claims = AuthUtils.GetClaims(
                        (context.UserContext as GraphQLUserContext)!
                    )!;
                    workspacesFilter = new WorkspacesFilter
                    {
                        UserId = Guid.Parse(
                            AuthUtils
                                .GetClaim(ClaimTypes.NameIdentifier, claims)!
                                .Value
                        )
                    };
                }
                var fragments = (
                    context.UserContext["fragments"]
                    as Dictionary<string, string>
                )!;
                var workspacesFieldsInfo = FieldAnalyzer.Workspaces(
                    context,
                    fragments
                );

                return await data.GetWorkspaces(
                    first,
                    after,
                    workspacesFilter,
                    workspacesFieldsInfo
                );
            });
    }
}

public class WorkspacesPageData
{
    public Guid? Id { get; set; }
#pragma warning disable CS8618
    public User User { get; set; }
    public Connection<Workspace> Workspaces { get; set; }
#pragma warning restore CS8618
}
