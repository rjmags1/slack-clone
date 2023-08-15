using GraphQL;
using GraphQL.Types;
using PersistenceService.Utils.GraphQL;
using SlackCloneGraphQL.Types;

namespace SlackCloneGraphQL;

public class SlackCloneQuery : ObjectGraphType<object>
{
    public SlackCloneQuery(SlackCloneData data)
    {
        Name = "query";
        Field<StringGraphType>("test")
            .Resolve(context =>
            {
                return "Hello";
            });
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
            .Arguments(
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                    {
                        Name = "userId"
                    },
                    new QueryArgument<
                        NonNullGraphType<WorkspacesFilterInputType>
                    >
                    {
                        Name = "workspacesFilter"
                    }
                )
            )
            .ResolveAsync(async context =>
            {
                var userId = context.GetArgument<Guid>("userId");
                var fragments = FieldAnalyzer.GetFragments(context);
                FieldInfo userFieldsInfo = FieldAnalyzer.User(
                    context,
                    fragments
                );
                var workspacesFilter = context.GetArgument<WorkspacesFilter>(
                    "workspacesFilter"
                );
                FieldInfo workspacesFieldsInfo = FieldAnalyzer.Workspaces(
                    context,
                    fragments
                );

                return new WorkspacesPageData
                {
                    User = await data.GetUserById(
                        userId,
                        userFieldsInfo.SubfieldNames
                    ),
                    Workspaces = await data.GetWorkspaces(
                        workspacesFilter,
                        workspacesFieldsInfo
                    ),
                };
            });
    }
}
