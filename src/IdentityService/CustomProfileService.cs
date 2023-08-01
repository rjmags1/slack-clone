using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using PersistenceService.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityModel;

namespace IdentityService
{
    public class CustomProfileService : ProfileService<User>
    {
        public CustomProfileService(
            UserManager<User> userManager,
            IUserClaimsPrincipalFactory<User> claimsFactory
        )
            : base(userManager, claimsFactory) { }

        protected override async Task GetProfileDataAsync(
            ProfileDataRequestContext context,
            User user
        )
        {
            var principal = await GetUserClaimsAsync(user);
        }
    }
}
