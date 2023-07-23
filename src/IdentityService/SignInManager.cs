using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PersistenceService.Models;
using PersistenceService.Stores;

namespace IdentityService;

public class BCryptSigninManager : SignInManager<User>
{
    public BCryptSigninManager(
        UserManager<User> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<User> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<User>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<User> confirmation
    )
        : base(
            userManager,
            contextAccessor,
            claimsFactory,
            optionsAccessor,
            logger,
            schemes,
            confirmation
        ) { }

    public override async Task<SignInResult> PasswordSignInAsync(
        string email,
        string password,
        bool isPersistent,
        bool lockoutOnFailure
    )
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user is null)
        {
            return SignInResult.Failed;
        }

        return await PasswordSignInAsync(
            user,
            password,
            isPersistent,
            lockoutOnFailure
        );
    }
}
