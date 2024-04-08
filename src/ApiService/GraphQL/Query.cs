using ApiService.Utils;
using GraphQL;
using GraphQL.Types;
using Common.SlackCloneGraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

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
        Field<ChannelType>("viewChannel")
            .Directive(
                "requiresClaimMapping",
                "claimName",
                "sub",
                "constraint",
                "equivalent-userId"
            )
            .Argument<NonNullGraphType<IdGraphType>>("userId")
            .Argument<NonNullGraphType<IdGraphType>>("channelId")
            .ResolveAsync(async context =>
            {
                string query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var fragments = FieldAnalyzer.GetFragments(query);
                context.UserContext.Add("fragments", fragments);
                var channelId = context.GetArgument<Guid>("channelId");

                var channel = await data.GetChannel(channelId);
                context.UserContext.Add("source", channel);
                return new Channel { Id = channelId };
            });
        Field<DirectMessageGroupType>("viewDirectMessageGroup")
            .Directive(
                "requiresClaimMapping",
                "claimName",
                "sub",
                "constraint",
                "equivalent-userId"
            )
            .Argument<NonNullGraphType<IdGraphType>>("userId")
            .Argument<NonNullGraphType<IdGraphType>>("directMessageGroupId")
            .ResolveAsync(async context =>
            {
                string query = GraphQLUtils.GetQuery(
                    (context.UserContext as GraphQLUserContext)!
                )!;
                var fragments = FieldAnalyzer.GetFragments(query);
                context.UserContext.Add("fragments", fragments);
                var groupId = context.GetArgument<Guid>("directMessageGroupId");

                var group = await data.GetDirectMessageGroup(groupId);
                context.UserContext.Add("source", group);
                return new DirectMessageGroup { Id = groupId };
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
                else if (queryName == "ChannelsListPaginationQuery")
                {
                    var fragments = FieldAnalyzer.GetFragments(query);
                    context.UserContext.Add("fragments", fragments);
                    return new WorkspacePageData { Id = id };
                }
                else if (queryName == "DirectMessageGroupsListPaginationQuery")
                {
                    var fragments = FieldAnalyzer.GetFragments(query);
                    context.UserContext.Add("fragments", fragments);
                    return new WorkspacePageData { Id = id };
                }
                else if (queryName == "StarredsListPaginationQuery")
                {
                    var fragments = FieldAnalyzer.GetFragments(query);
                    context.UserContext.Add("fragments", fragments);
                    return new WorkspacePageData { Id = id };
                }
                else if (queryName == "ChannelMessagesListPaginationQuery")
                {
                    var fragments = FieldAnalyzer.GetFragments(query);
                    context.UserContext.Add("fragments", fragments);
                    return new Channel { Id = id };
                }
                else if (queryName == "DirectMessagesListPaginationQuery")
                {
                    var fragments = FieldAnalyzer.GetFragments(query);
                    context.UserContext.Add("fragments", fragments);
                    return new DirectMessageGroup { Id = id };
                }
                return null;
            });
    }
}
