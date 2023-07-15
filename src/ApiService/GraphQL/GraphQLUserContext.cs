using System.Security.Claims;

namespace SlackCloneGraphQL;

public class GraphQLUserContext : Dictionary<string, object>
{
#pragma warning disable CS8618
    public ClaimsPrincipal User { get; set; }
}
