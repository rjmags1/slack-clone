﻿using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> { new ApiScope("api", "API") };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("bff", "bff graphql api") { Scopes = { "api" } }
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "bff",
                AllowOfflineAccess = true,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AccessTokenLifetime = 1800,
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://localhost:5003/signin-oidc" },
                PostLogoutRedirectUris =
                {
                    "https://localhost:5003/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "api"
                }
            }
        };
}
