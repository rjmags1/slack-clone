using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Utils;
using CustomBaseStore = PersistenceService.Stores.Store;

namespace PersistenceService.Stores;

public class UserStore : UserManager<User>
{
    public static List<TimeZoneInfo> timezones { get; set; } =
        TimeZoneInfo.GetSystemTimeZones().ToList();

    public static string testPhoneNumber = "9-999-999-9999";

    private const string AVATAR_NAV_PROP = "Avatar";

    private const string THEME_NAV_PROP = "Theme";

    private ApplicationDbContext context { get; set; }

    public UserStore(
        IUserStore<User> userStore,
        IOptions<IdentityOptions> options,
        IPasswordHasher<User> passwordHasher,
        IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<User>> logger,
        ApplicationDbContext dbContext
    )
        : base(
            userStore,
            options,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger
        )
    {
        context = dbContext;
    }

    public async Task<User> FindByIdAsyncWithEagerNavPropLoading(
        Guid userId,
        IEnumerable<string> fields
    )
    {
        IEnumerable<string> uppercaseFields = fields.Select(
            f => StringUtils.ToUpperFirstLetter(f)
        );
        IQueryable<User> query = context.Users.Where(u => u.Id == userId);

        bool avatarRequested = false;
        bool themeRequested = false;
        foreach (string f in fields)
        {
            if (f == AVATAR_NAV_PROP)
            {
                avatarRequested = true;
            }
            if (f == THEME_NAV_PROP)
            {
                themeRequested = true;
            }
        }

        if (avatarRequested)
        {
            query = query.Include(u => u.Avatar).Where(u => u.Id == userId);
        }
        if (themeRequested)
        {
            query = query.Include(u => u.Theme).Where(u => u.Id == userId);
        }

        return await query.FirstAsync();
    }

    public async Task<List<User>> InsertUsers(
        List<User> users,
        List<string> passwords
    )
    {
        foreach (
            (User user, string password) in users.Zip(
                passwords,
                (u, p) => (u, p)
            )
        )
        {
            IdentityResult addResult = await CreateAsync(user, password);
            if (!addResult.Succeeded)
            {
                throw new ArgumentException(
                    addResult.Errors.First().Description
                );
            }
        }

        return users;
    }

    public static string GenerateTestUserName(int randsize) =>
        "test_user_name" + CustomBaseStore.random.Next(100000000).ToString();

    public static string GenerateTestFirstName(int randsize) =>
        "test_fname" + CustomBaseStore.GenerateRandomString(randsize);

    public static string GenerateTestLastName(int randsize) =>
        "test_lname" + CustomBaseStore.GenerateRandomString(randsize);

    public static string GenerateTestEmail(int randsize) =>
        "test-user"
        + CustomBaseStore.GenerateRandomString(randsize)
        + "@testemail.com";
}
