using ApiService.Utils;
using GraphQL;
using GraphQL.Types;
using SlackCloneGraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL;

public class SlackCloneQuery : ObjectGraphType<object>
{
    public SlackCloneQuery(SlackCloneData data)
    {
        Name = "query";
        Field<ValidationResultType>("validUserEmail")
            .Argument<NonNullGraphType<StringGraphType>>("email")
            .ResolveAsync(async context =>
            {
                var email = context.GetArgument<string>("email");
                var valid = await data.ValidUserEmail(email);
                return new ValidationResult { Valid = valid };
            });

        Field<WorkspacesPageDataType>("workspacesPageData")
            .Directive(
                "requiresClaimMapping",
                "claimName",
                "sub",
                "constraint",
                "equivalent-userId"
            )
            .Argument<NonNullGraphType<IdGraphType>>("userId")
            .Resolve(context =>
            {
                string query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var fragments = FieldAnalyzer.GetFragments(query);
                context.UserContext.Add("fragments", fragments);

                return new WorkspacesPageData { };
            });
        Field<WorkspacePageDataType>("workspacePageData")
            .Directive(
                "requiresClaimMapping",
                "claimName",
                "sub",
                "constraint",
                "equivalent-userId"
            )
            .Argument<NonNullGraphType<IdGraphType>>("userId")
            .Resolve(context =>
            {
                string query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var fragments = FieldAnalyzer.GetFragments(query);
                context.UserContext.Add("fragments", fragments);

                return new WorkspacePageData { };
            });
        Field<RelayNodeInterfaceType>("node")
            .Argument<NonNullGraphType<IdGraphType>>("id")
            .Resolve(context =>
            {
                var queryName =
                    GraphQLUtils.GetQueryName(
                        (context.UserContext as GraphQLUserContext)!
                    )
                    ?? throw new InvalidOperationException(
                        "Queries with node field must be named"
                    );
                string query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var id = context.GetArgument<Guid>("id");
                if (queryName == "WorkspacesListPaginationQuery")
                {
                    var fragments = FieldAnalyzer.GetFragments(query);
                    context.UserContext.Add("fragments", fragments);

                    return new WorkspacesPageData { Id = id };
                }

                return null;
            });
    }
}
