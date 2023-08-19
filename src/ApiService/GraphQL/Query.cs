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
            .Resolve(context =>
            {
                var fragments = FieldAnalyzer.GetFragments(context);
                context.UserContext.Add("fragments", fragments);

                return new WorkspacesPageData { };
            });
    }
}
