using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using PersistenceService.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
            //var id = (ClaimsIdentity)principal.Identity;
            //if (!string.IsNullOrEmpty(user.FavoriteColor))
            //{
            //id.AddClaim(new Claim("favorite_color", user.FavoriteColor));
            //}

            //context.AddRequestedClaims(principal.Claims);
        }
    }
}
