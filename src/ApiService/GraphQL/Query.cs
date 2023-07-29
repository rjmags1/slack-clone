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
        Field<StringGraphType>("test")
            .Resolve(context =>
            {
                return "Hello";
            });

        Field<WorkspacesPageDataType>("workspacesPageData")
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
                var userFields = FieldAnalyzer.User(context, userId);

                var workspacesFilter = context.GetArgument<WorkspacesFilter>(
                    "workspacesFilter"
                );
                var (fieldsTree, flattened) = FieldAnalyzer.Workspaces(
                    context,
                    workspacesFilter
                );

                return new WorkspacesPageData
                {
                    User = await data.GetUserById(userId, userFields),
                    Workspaces = await data.GetWorkspaces(
                        workspacesFilter,
                        fieldsTree,
                        flattened
                    ),
                };
            });

        Field<WorkspaceType>("testWorkspaceMembers")
            .Arguments(
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<UsersFilterInputType>>
                    {
                        Name = "usersFilter"
                    },
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                    {
                        Name = "workspaceId"
                    }
                )
            )
            .ResolveAsync(async context =>
            {
                var workspaceId = context.GetArgument<Guid>("workspaceId");
                if (workspaceId == Guid.Empty)
                {
                    throw new ArgumentNullException();
                }

                return await data.GetWorkspace(workspaceId);
            });
    }
}
