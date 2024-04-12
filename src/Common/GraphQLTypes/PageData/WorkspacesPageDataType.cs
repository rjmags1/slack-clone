using System.Security.Claims;
using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;
using Common.SlackCloneGraphQL.Types.Connections;

namespace Common.SlackCloneGraphQL.Types;

public class WorkspacesPageDataType : ObjectGraphType<WorkspacesPageData>
{
    public WorkspacesPageDataType(ISlackCloneData data)
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
            .ResolveAsync(async context =>
            {
                var userId = context.GetArgument<Guid>("id");
                var dbCols = FieldAnalyzer.UserDbColumns(
                    context.FieldAst,
                    context.Document
                );
                return await data.GetUserById(userId, dbCols);
            });
        Field<NonNullGraphType<WorkspacesConnectionType>>("workspaces")
            .Description("The current page of this workspaces connection")
            .Argument<WorkspacesFilterInputType>("filter")
            .Argument<NonNullGraphType<IntGraphType>>("first")
            .Argument<IdGraphType>("after")
            .ResolveAsync(async context =>
            {
                throw new NotImplementedException();
                //var first = context.GetArgument<int>("first");
                //var after = context.GetArgument<Guid?>("after");
                //var workspacesFilter = context.GetArgument<WorkspacesFilter?>(
                //"filter"
                //);
                //if (workspacesFilter is null)
                //{
                //var claims = AuthUtils.GetClaims(
                //(context.UserContext as GraphQLUserContext)!
                //)!;
                //workspacesFilter = new WorkspacesFilter
                //{
                //UserId = Guid.Parse(
                //AuthUtils
                //.GetClaim(ClaimTypes.NameIdentifier, claims)!
                //.Value
                //)
                //};
                //}
                //var fragments = (
                //context.UserContext["fragments"]
                //as Dictionary<string, string>
                //)!;
                //var query = GraphQLUtils.GetQuery(
                //(context.UserContext as GraphQLUserContext)!
                //)!;
                //var workspacesFieldsInfo = FieldAnalyzer.Workspaces(
                //query,
                //fragments
                //);

                //return await data.GetWorkspaces(
                //first,
                //after,
                //workspacesFilter,
                //workspacesFieldsInfo
                //);
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
