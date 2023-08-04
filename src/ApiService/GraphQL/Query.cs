using System.Security.Claims;
using ApiService.Utils;
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
                FieldInfo userFieldsInfo = FieldAnalyzer.User(context, userId);

                var workspacesFilter = context.GetArgument<WorkspacesFilter>(
                    "workspacesFilter"
                );
                FieldInfo workspacesFieldsInfo = FieldAnalyzer.Workspaces(
                    context
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
