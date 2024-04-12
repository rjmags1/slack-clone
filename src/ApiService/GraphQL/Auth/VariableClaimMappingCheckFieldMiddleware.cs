using System.Security.Claims;
using Common.Utils;
using GraphQL;
using GraphQL.Instrumentation;

namespace SlackCloneGraphQL.Auth;

public class VariableClaimMappingCheckFieldMiddleware : IFieldMiddleware
{
    private struct MappingConstraint
    {
        public string Name { get; set; }
        public string MappedVariable { get; set; }
    }

    public async ValueTask<object?> ResolveAsync(
        IResolveFieldContext context,
        FieldMiddlewareDelegate next
    )
    {
        if (!RequiresClaimDirective(context))
        {
            return await next(context);
        }

        var claimsDirective = context.FieldDefinition.FindAppliedDirective(
            "requiresClaimMapping"
        )!;
        var requiredClaimName = (string)
            claimsDirective.FindArgument("claimName")!.Value!;
        var constraintString = (string)
            claimsDirective.FindArgument("constraint")!.Value!;

        ClaimsPrincipal claims;
        claims = AuthUtils.GetClaims(
            (context.UserContext as GraphQLUserContext)!
        );

        var requiredClaim =
            AuthUtils.GetClaim(requiredClaimName, claims)
            ?? throw new InvalidOperationException();
        MappingConstraint constraint = ParseConstraintString(constraintString);
        bool validClaimVariableMapping = CheckConstraint(
            constraint,
            requiredClaim,
            context
        );

        return validClaimVariableMapping ? await next(context) : null;
    }

    private static bool CheckConstraint(
        MappingConstraint constraint,
        Claim requiredClaim,
        IResolveFieldContext context
    )
    {
        var mappedVariable = context.GetArgument<object>(
            constraint.MappedVariable
        );
        if (mappedVariable is null)
        {
            return false;
        }
        if (constraint.Name == "equivalent")
        {
            return mappedVariable.ToString() == requiredClaim.Value;
        }

        return false;
    }

    private static MappingConstraint ParseConstraintString(
        string constraintString
    )
    {
        int delimiterIdx = constraintString.IndexOf("-");
        return new MappingConstraint
        {
            Name = constraintString[..delimiterIdx],
            MappedVariable = constraintString[(delimiterIdx + 1)..]
        };
    }

    private static bool RequiresClaimDirective(IResolveFieldContext context)
    {
        return context.FieldDefinition.FindAppliedDirective(
            "requiresClaimMapping"
        )
            is not null;
    }
}
