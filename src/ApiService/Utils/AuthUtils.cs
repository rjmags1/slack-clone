using SlackCloneGraphQL;
using System.Security.Claims;

namespace ApiService.Utils;

public static class AuthUtils
{
    public static ClaimsPrincipal GetClaims(GraphQLUserContext context)
    {
        if (!context.ContainsKey("claims"))
        {
            throw new NullReferenceException();
        }

        return (ClaimsPrincipal)context["claims"]!;
    }

    public static Claim? GetClaim(string claimName, ClaimsPrincipal claims)
    {
        var requiredClaim = claims.Claims
            .Where(c => c.Type == claimName)
            .FirstOrDefault();
        if (requiredClaim is null && claimName == "sub")
        {
            requiredClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
        }

        return requiredClaim;
    }
}
