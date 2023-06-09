using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[] { new IdentityResources.OpenId(), new IdentityResources.Profile() };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> { new ApiScope(name: "api1", displayName: "MyAPI") };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "bff",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                // where to redirect to after login
                RedirectUris = { "https://localhost:5003/signin-oidc" },
                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "api1"
                }
            }
        };
}
