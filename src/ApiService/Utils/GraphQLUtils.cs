using SlackCloneGraphQL;

namespace ApiService.Utils;

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
