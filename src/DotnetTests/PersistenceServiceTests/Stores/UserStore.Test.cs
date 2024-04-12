using DotnetTests.Fixtures;
using DotnetTests.PersistenceService.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;
using PersistenceService.Utils;

namespace DotnetTests.PersistenceService.Stores;

[Trait("Category", "Order 1")]
[Collection("Database collection 1")]
public class UserStoreTests1
{
    private readonly UserStore _userStore = GetUserStore();

    private readonly ApplicationDbContext _dbContext;

    public UserStoreTests1(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
    }

    private Guid DevId = Guid.Parse("aea5a17c-b73e-43a5-be7b-8352a2adce5d");
    private Guid DevAvatarId = Guid.Parse(
        "a2ba2801-94c1-40d5-86be-e10f434bcc51"
    );
    private Guid DevThemeId = Guid.Parse(
        "93c1414b-29d5-40e8-9bcc-5a45a273843a"
    );

    [Fact]
    public async void FindById_ShouldWork()
    {
        var cols = new List<string>
        {
            "Id",
            "AvatarId",
            "CreatedAt",
            "OnlineStatus",
            "UserName",
            "Email",
            "EmailConfirmed",
            "FirstName",
            "LastName",
            "NotificationSound",
            "NotificationsAllowStartTime",
            "NotificationsAllowEndTime",
            "NotificationsPauseUntil",
            "ThemeId",
            "Timezone"
        };
        var fetched = await _userStore.FindById(DevId, cols);
        Assert.Equal(DevId, fetched.Id);
        Assert.Equal(DevAvatarId, fetched.Avatar.Id);
        Assert.Equal(DevThemeId, fetched.PersonalInfo?.Theme.Id);
        var defaultDateTime = new DateTime();
        Assert.NotEqual(fetched.CreatedAt, defaultDateTime);
        Assert.NotNull(fetched.OnlineStatus);
        Assert.NotNull(fetched.Username);
        Assert.NotNull(fetched.PersonalInfo?.Email);
        Assert.False(fetched.PersonalInfo?.EmailConfirmed);
        Assert.NotNull(fetched.PersonalInfo?.FirstName);
        Assert.NotNull(fetched.PersonalInfo?.LastName);
        Assert.Equal(
            0,
            fetched.PersonalInfo?.UserNotificationsPreferences?.NotifSound
        );
        Assert.Null(
            fetched
                .PersonalInfo
                ?.UserNotificationsPreferences
                ?.AllowAlertsStartTimeUTC
        );
        Assert.Null(
            fetched
                .PersonalInfo
                ?.UserNotificationsPreferences
                ?.AllowAlertsEndTimeUTC
        );
        Assert.Null(
            fetched.PersonalInfo?.UserNotificationsPreferences?.PauseAlertsUntil
        );
        Assert.NotNull(fetched.PersonalInfo?.Timezone);
    }

    /**
        [Fact]
        public async void InsertUsers_ShouldInsertUsersAndHashedPasswords()
        {
            int expectedInserted = 5;
            List<User> users = new List<User>();
            List<string> passwords = new List<string>();
            for (int i = 0; i < expectedInserted; i++)
            {
                User u = StoreTestUtils.CreateTestUnregisteredUser();
                users.Add(u);
                passwords.Add(StoreTestUtils.testPassword);
            }
    
            List<User> loaded = (await _userStore.InsertUsers(users, passwords))
                .OrderBy(u => u.UserName)
                .ToList();
    
            var insertedIds = loaded.Select(u => u.Id);
            int numInserted = _dbContext.Users
                .Where(u => insertedIds.Contains(u.Id))
                .Count();
            Assert.Equal(expectedInserted, numInserted);
    
            foreach (
                ((User loadedUser, User user), string password) in loaded
                    .Zip(users.OrderBy(u => u.UserName))
                    .Zip(passwords)
            )
            {
                Assert.NotEqual(loadedUser.Id, Guid.Empty);
                Assert.Null(loadedUser.Avatar);
                Assert.Null(loadedUser.AvatarId);
                Assert.NotEqual(loadedUser.CreatedAt, default(DateTime));
                Assert.False(loadedUser.Deleted);
                Assert.Equal(loadedUser.FirstName, user.FirstName);
                Assert.Equal(loadedUser.LastName, user.LastName);
                Assert.Equal(0, loadedUser.UserNotificationsPreferencesMask);
                Assert.Null(loadedUser.NotificationsAllowStartTime);
                Assert.Null(loadedUser.NotificationsAllowEndTime);
                Assert.Null(loadedUser.NotificationsPauseUntil);
                Assert.Equal(0, loadedUser.NotificationSound);
                Assert.Null(loadedUser.OnlineStatus);
                Assert.Null(loadedUser.OnlineStatusUntil);
                Assert.Null(loadedUser.Theme);
                Assert.Null(loadedUser.ThemeId);
                Assert.Equal(loadedUser.Timezone, user.Timezone);
                Assert.Equal(loadedUser.UserName, user.UserName);
                Assert.Equal(
                    loadedUser.NormalizedUserName,
                    user.NormalizedUserName
                );
                Assert.Equal(loadedUser.Email, user.Email);
                Assert.Equal(loadedUser.NormalizedEmail, user.NormalizedEmail);
                Assert.Equal(loadedUser.PhoneNumber, user.PhoneNumber);
                Assert.NotEqual(loadedUser.ConcurrencyStamp, Guid.Empty.ToString());
                Assert.NotEqual(loadedUser.SecurityStamp, Guid.Empty.ToString());
                Assert.True(await _userStore.CheckPasswordAsync(user, password));
            }
        }
    
        [Fact]
        public async void InsertUsers_RejectsInvalidUserName()
        {
            User badUser1 = StoreTestUtils.CreateTestUnregisteredUser();
            badUser1.UserName = "";
            User badUser2 = StoreTestUtils.CreateTestUnregisteredUser();
            badUser2.UserName = "!~[]?><";
            User goodUser = StoreTestUtils.CreateTestUnregisteredUser();
            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    _userStore.InsertUsers(
                        new List<User> { badUser1 },
                        new List<string> { StoreTestUtils.testPassword }
                    )
            );
            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    _userStore.InsertUsers(
                        new List<User> { badUser2 },
                        new List<string> { StoreTestUtils.testPassword }
                    )
            );
            Assert.Contains(
                goodUser,
                await _userStore.InsertUsers(
                    new List<User> { goodUser },
                    new List<string> { StoreTestUtils.testPassword }
                )
            );
        }
    
        [Fact]
        public async void InsertUsers_ShouldThrowExceptionOnInvalidPassword()
        {
            User goodUser = StoreTestUtils.CreateTestUnregisteredUser();
            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    _userStore.InsertUsers(
                        new List<User> { goodUser },
                        new List<string> { "No_digit_password" }
                    )
            );
            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    _userStore.InsertUsers(
                        new List<User> { goodUser },
                        new List<string> { "Short1!" }
                    )
            );
            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    _userStore.InsertUsers(
                        new List<User> { goodUser },
                        new List<string> { "AAAaaa1!" }
                    )
            );
            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    _userStore.InsertUsers(
                        new List<User> { goodUser },
                        new List<string> { "no_upper1!" }
                    )
            );
            await Assert.ThrowsAsync<ArgumentException>(
                () =>
                    _userStore.InsertUsers(
                        new List<User> { goodUser },
                        new List<string> { "NO_LOWER1!" }
                    )
            );
        }
    
        [Fact]
        public async void InsertUsers_ShouldThrowExceptionOnNonUniqueEmail()
        {
            User user1 = StoreTestUtils.CreateTestUnregisteredUser();
            User duplicateEmailUser = StoreTestUtils.CreateTestUnregisteredUser();
            duplicateEmailUser.Email = user1.Email;
            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _userStore.InsertUsers(
                        new List<User> { user1, duplicateEmailUser },
                        new List<string>
                        {
                            StoreTestUtils.testPassword,
                            StoreTestUtils.testPassword
                        }
                    )
            );
        }
        **/

    public static UserStore GetUserStore()
    {
        var services = new ServiceCollection();
        services.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher>();

        services
            .AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddUserManager<UserStore>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        string connectionString =
            Environment.GetEnvironmentVariable("ENV") == "dev"
                ? "LOCAL_DB_CONNECTION_STRING"
                : "TEST_DB_CONNECTION_STRING";
        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseNpgsql(
                    Environment.GetEnvironmentVariable(connectionString)
                )
        );
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<UserStore>();
    }
}

[Trait("Category", "Order 2")]
[Collection("Database collection 2")]
public class UserStoreTests2
{
    private readonly UserStore _userStore = UserStoreTests1.GetUserStore();

    private readonly ApplicationDbContext _dbContext;

    public UserStoreTests2(
        FilledApplicationDbContextFixture filledApplicationDbContextFixture
    )
    {
        _dbContext = filledApplicationDbContextFixture.Context;
    }

    [Fact]
    public void SeedHappened()
    {
        Assert.True(_dbContext.Users.Count() > 0);
    }
}
