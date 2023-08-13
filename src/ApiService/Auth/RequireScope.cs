using Microsoft.AspNetCore.Authorization;

namespace ApiService.Auth;

public class ScopeRequirement : IAuthorizationRequirement
{
    public ScopeRequirement(string scope) => RequiredScope = scope;

    public string RequiredScope { get; }
}

public class RequiredScopeHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScopeRequirement requirement
    )
    {
        var hasScope = context.User.Claims.Any(
            c => c.Type == "scope" && c.Value == requirement.RequiredScope
        );
        if (!hasScope)
        {
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
