using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityService
{
    public class CustomProfileService : ProfileService<ApplicationUser>
    {
        public CustomProfileService(
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory
        )
            : base(userManager, claimsFactory) { }

        protected override async Task GetProfileDataAsync(
            ProfileDataRequestContext context,
            ApplicationUser user
        )
        {
            var principal = await GetUserClaimsAsync(user);
            var id = (ClaimsIdentity)principal.Identity;
            if (!string.IsNullOrEmpty(user.FavoriteColor))
            {
                id.AddClaim(new Claim("favorite_color", user.FavoriteColor));
            }

            context.AddRequestedClaims(principal.Claims);
        }
    }
}
