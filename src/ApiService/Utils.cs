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

        return char.ToLower(s[0]) + s.Substring(1);
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
}
