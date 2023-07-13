using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersistenceService.Models;
using CustomBaseStore = PersistenceService.Stores.Store;

namespace PersistenceService.Stores;

public class UserStore : UserManager<User>
{
    public static List<TimeZoneInfo> timezones { get; set; } =
        TimeZoneInfo.GetSystemTimeZones().ToList();

    public static string testPhoneNumber = "9-999-999-9999";

    public UserStore(
        IUserStore<User> userStore,
        IOptions<IdentityOptions> options,
        IPasswordHasher<User> passwordHasher,
        IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<User>> logger
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
        ) { }

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
