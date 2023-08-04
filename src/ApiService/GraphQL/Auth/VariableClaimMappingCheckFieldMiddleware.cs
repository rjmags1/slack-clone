using System.Security.Claims;
using ApiService.Utils;
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
        try
        {
            claims = AuthUtils.GetClaims(
                (context.UserContext as GraphQLUserContext)!
            );
        }
        catch (Exception)
        {
            return null;
        }

        var requiredClaim = GetRequiredClaim(requiredClaimName, claims);
        if (requiredClaim is null)
        {
            return null;
        }

        MappingConstraint constraint = ParseConstraintString(constraintString);
        bool passed = CheckConstraint(constraint, requiredClaim, context);

        return passed ? await next(context) : null;
    }

    private static bool CheckConstraint(
        MappingConstraint constraint,
        Claim requiredClaim,
        IResolveFieldContext context
    )
    {
        var mappedVariable = context.Variables
            .Where(v => v.Name == constraint.MappedVariable)
            .FirstOrDefault();
        if (mappedVariable is null)
        {
            return false;
        }
        if (constraint.Name == "equivalent")
        {
            return (string)mappedVariable.Value! == requiredClaim.Value;
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

    private static Claim? GetRequiredClaim(
        string requiredClaimName,
        ClaimsPrincipal claims
    )
    {
        var requiredClaim = claims.Claims
            .Where(c => c.Type == requiredClaimName)
            .FirstOrDefault();
        if (requiredClaim is null && requiredClaimName == "sub")
        {
            requiredClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
        }

        return requiredClaim;
    }
}
