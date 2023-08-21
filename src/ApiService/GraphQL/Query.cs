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
            .Resolve(context =>
            {
                var fragments = FieldAnalyzer.GetFragments(context);
                context.UserContext.Add("fragments", fragments);

                return new WorkspacesPageData { };
            });
        Field<RelayNodeInterfaceType>("node")
            .Argument<NonNullGraphType<IdGraphType>>("id")
            .Resolve(context =>
            {
                var queryName =
                    AuthUtils.GetQueryName(
                        (context.UserContext as GraphQLUserContext)!
                    )
                    ?? throw new InvalidOperationException(
                        "Queries with node field must be named"
                    );
                var id = context.GetArgument<Guid>("id");
                if (queryName == "WorkspacesListPaginationQuery")
                {
                    var fragments = FieldAnalyzer.GetFragments(context);
                    context.UserContext.Add("fragments", fragments);

                    return new WorkspacesPageData { Id = id };
                }

                return null;
            });
    }
}
