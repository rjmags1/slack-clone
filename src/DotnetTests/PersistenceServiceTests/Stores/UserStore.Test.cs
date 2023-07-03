using DotnetTests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Stores;
using PersistenceService.Utils;

namespace DotnetTests.PersistenceService.Stores;

[Collection("Database collection")]
public class UserStoreTests
{
    private UserStore _userStore = GetUserStore();

    private ApplicationDbContext _dbContext;

    public UserStoreTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
    }

    [Fact]
    public async void InsertUsers_ShouldInsertUsersAndHashedPasswords()
    {
        int expectedInserted = 5;
        List<User> users = new List<User>();
        List<string> passwords = new List<string>();
        for (int i = 0; i < expectedInserted; i++)
        {
            User u = new User
            {
                FirstName = "test-fname-" + i.ToString(),
                LastName = "test-lname-" + i.ToString(),
                Timezone = UserStore.timezones[
                    i % UserStore.timezones.Count
                ].Id,
                UserName = "test-username-" + i.ToString(),
                Email = UserStore.GenerateTestEmail(10),
                PhoneNumber = "1-234-456-789" + i.ToString()
            };
            users.Add(u);
            passwords.Add(UserStore.testPassword);
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
                .Zip(users)
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
        User badUser1 = new User
        {
            FirstName = "test-fname",
            LastName = "test-lname",
            Timezone = UserStore.timezones[0].Id,
            UserName = "",
            Email = UserStore.GenerateTestEmail(10),
            PhoneNumber = "1-234-456-7890"
        };
        User badUser2 = new User
        {
            FirstName = "test-fname",
            LastName = "test-lname",
            Timezone = UserStore.timezones[0].Id,
            UserName = "!~[]?><",
            Email = UserStore.GenerateTestEmail(10),
            PhoneNumber = "1-234-456-7890"
        };
        User goodUser = new User
        {
            FirstName = "test-fname",
            LastName = "test-lname",
            Timezone = UserStore.timezones[0].Id,
            UserName = "test-username",
            Email = UserStore.GenerateTestEmail(10),
            PhoneNumber = "1-234-456-7890"
        };
        await Assert.ThrowsAsync<ArgumentException>(
            () =>
                _userStore.InsertUsers(
                    new List<User> { badUser1 },
                    new List<string> { UserStore.testPassword }
                )
        );
        await Assert.ThrowsAsync<ArgumentException>(
            () =>
                _userStore.InsertUsers(
                    new List<User> { badUser2 },
                    new List<string> { UserStore.testPassword }
                )
        );
        Assert.Contains(
            goodUser,
            await _userStore.InsertUsers(
                new List<User> { goodUser },
                new List<string> { UserStore.testPassword }
            )
        );
    }

    [Fact]
    public async void InsertUsers_ShouldThrowExceptionOnInvalidPassword()
    {
        User goodUser = new User
        {
            FirstName = "test-fname",
            LastName = "test-lname",
            Timezone = UserStore.timezones[0].Id,
            UserName = "valid_username",
            Email = UserStore.GenerateTestEmail(10),
            PhoneNumber = "1-234-456-7890"
        };
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
        User user1 = new User
        {
            FirstName = "test-fname",
            LastName = "test-lname",
            Timezone = UserStore.timezones[0].Id,
            UserName = "valid_username_1",
            Email = "test-email@test.com",
            PhoneNumber = "1-234-456-7890"
        };
        User duplicateEmailUser = new User
        {
            FirstName = "test-fname",
            LastName = "test-lname",
            Timezone = UserStore.timezones[0].Id,
            UserName = "valid_username_2",
            Email = "test-email@test.com",
            PhoneNumber = "1-234-456-7890"
        };
        await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await _userStore.InsertUsers(
                    new List<User> { user1, duplicateEmailUser },
                    new List<string>
                    {
                        UserStore.testPassword,
                        UserStore.testPassword
                    }
                )
        );
    }

    [Fact]
    public async void InsertTestUsers_ShouldInsertTestUsers()
    {
        int initialUsers = _dbContext.Users.Count();
        await _userStore.InsertTestUsers(100);
        int currentUsers = _dbContext.Users.Count();
        Assert.Equal(initialUsers + 100, currentUsers);
    }

    private static UserStore GetUserStore()
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
        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseNpgsql(
                    Environment.GetEnvironmentVariable(
                        "TEST_DB_CONNECTION_STRING"
                    )
                )
        );
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<UserStore>();
    }
}
