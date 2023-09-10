using System.Dynamic;
using System.Security.Claims;
using System.Text.Json;
using SlackCloneGraphQL;

namespace ApiService.Utils;

public class StringUtils
{
    public static string ToLowerFirstLetter(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return char.ToLower(s[0]) + s[1..];
    }
}

public static class DynamicUtils
{
    public static ExpandoObject ToExpando(dynamic dynamicObj)
    {
        string json = JsonSerializer.Serialize(dynamicObj);
        return JsonSerializer.Deserialize<ExpandoObject>(json)!;
    }

    public static bool HasProperty(dynamic obj, string propertyName)
    {
        return obj is IDictionary<string, object> dict
            && dict.ContainsKey(propertyName);
    }

    public static object GetProperty(dynamic obj, string propertyName)
    {
        if (
            obj is IDictionary<string, object> dict
            && dict.ContainsKey(propertyName)
        )
        {
            return obj[propertyName];
        }

        throw new InvalidOperationException();
    }
}

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

public static class GraphQLUtils
{
    public static string? GetQueryName(GraphQLUserContext context)
    {
        return (string?)context["queryName"];
    }

    public static string? GetQuery(GraphQLUserContext context)
    {
        return (string?)context["query"];
    }

    public static Guid GetSubClaim(GraphQLUserContext context)
    {
        return (Guid)context["sub"]!;
    }
}
