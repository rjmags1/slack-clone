using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Utils;
using CustomBaseStore = PersistenceService.Stores.Store;
using Dapper;
using GraphQLTypes = Common.SlackCloneGraphQL.Types;

namespace PersistenceService.Stores;

public class UserStore : UserManager<User>, IStore
{
    public static List<TimeZoneInfo> timezones { get; set; } =
        TimeZoneInfo.GetSystemTimeZones().ToList();

    public static string testPhoneNumber = "9-999-999-9999";

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

    public async Task<GraphQLTypes.User> FindById(
        Guid userId,
        IEnumerable<string> cols
    )
    {
        var wId = Stores.Store.wdq("Id");
        var wUser = Stores.Store.wdq("user_");
        var wTheme = Stores.Store.wdq("Themes");
        var wThemeId = Stores.Store.wdq("ThemeId");
        var wFiles = Stores.Store.wdq("Files");
        var wAvatarId = Stores.Store.wdq("AvatarId");

        var sqlBuilder = new List<string>();
        sqlBuilder.Add("WITH user_ AS (\n");
        sqlBuilder.Add("SELECT\n");
        sqlBuilder.AddRange(
            cols.Select(c => Stores.Store.wdq(c))
                .Select((c, i) => i == cols.Count() - 1 ? $"{c}\n" : $"{c},\n")
        );
        sqlBuilder.Add($"FROM {Stores.Store.wdq("AspNetUsers")}\n");
        sqlBuilder.Add($"WHERE {wId} = @UserId\n");
        sqlBuilder.Add(")\n\n");
        sqlBuilder.Add($"SELECT * FROM {wUser}\n");
        if (cols.Any(c => c == "ThemeId" || c == "AvatarId"))
        {
            sqlBuilder.Add(
                $"LEFT JOIN {wTheme} ON {wTheme}.{wId} = {wUser}.{wThemeId}\n"
            );
            sqlBuilder.Add(
                $"LEFT JOIN {wFiles} ON {wFiles}.{wId} = {wUser}.{wAvatarId}\n"
            );
        }
        sqlBuilder.Add(";");

        var sql = string.Join("", sqlBuilder);
        var conn = context.GetConnection();
        var parameters = new { UserId = userId };
        return (
            await conn.QueryAsync<
                Models.User,
                GraphQLTypes.Theme?,
                GraphQLTypes.File?,
                GraphQLTypes.User
            >(
                sql: sql,
                param: parameters,
                map: (user, theme, avatar) =>
                {
                    return new GraphQLTypes.User
                    {
                        Id = user.Id,
                        Avatar = avatar!,
                        OnlineStatus = user.OnlineStatus ?? "offline",
                        OnlineStatusUntil = user.OnlineStatusUntil,
                        Username = user.UserName,
                        CreatedAt = user.CreatedAt,
                        PersonalInfo = new GraphQLTypes.UserInfo
                        {
                            Email = user.Email,
                            EmailConfirmed = user.EmailConfirmed,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Theme = theme!,
                            Timezone = user.Timezone,
                            UserNotificationsPreferences =
                                new GraphQLTypes.UserNotificationsPreferences
                                {
                                    NotifSound = user.NotificationSound,
                                    AllowAlertsStartTimeUTC =
                                        user.NotificationsAllowStartTime,
                                    AllowAlertsEndTimeUTC =
                                        user.NotificationsAllowEndTime,
                                    PauseAlertsUntil =
                                        user.NotificationsPauseUntil
                                }
                        }
                    };
                }
            )
        ).First();
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

    public async Task<bool> RegisteredEmail(string email)
    {
        return (
                await context.Users
                    .Where(u => u.NormalizedEmail == email.ToUpper())
                    .CountAsync()
            ) == 1;
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
