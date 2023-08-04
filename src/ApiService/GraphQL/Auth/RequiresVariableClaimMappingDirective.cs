using GraphQL.Types;
using GraphQLParser.AST;

namespace SlackCloneGraphQL.Auth;

public class RequiresVariableClaimMappingDirective : Directive
{
    public override bool? Introspectable => false;

    public RequiresVariableClaimMappingDirective()
        : base("requiresClaimMapping", DirectiveLocation.FieldDefinition)
    {
        Description =
            "Requires the user to have a specific claim in their JWT.";
        Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "claimName",
                Description = "The name of the required claim."
            },
            new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = "constraint",
                Description =
                    "Formatted string in the form of 'predicate-queryVariable'"
            }
        );
    }
}
