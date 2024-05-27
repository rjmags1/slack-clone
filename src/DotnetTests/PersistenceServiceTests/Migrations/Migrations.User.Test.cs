using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DotnetTests.PersistenceService.Migrations;

[Trait("Category", "Order 1")]
[Collection("Empty Database Test Collection")]
public class UserMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IEntityType _entityType;

    public UserMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.Context;
        _entityType = _dbContext.Model.FindEntityType(typeof(User))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(User.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal("gen_random_uuid()", defaultValueSql);
        string idColumnType = idProperty.GetColumnType();
        var idColumnNullable = idProperty.IsColumnNullable();
        Assert.Equal("uuid", idColumnType);
        Assert.False(idColumnNullable);
        Assert.True(idProperty.IsPrimaryKey());
    }

    [Fact]
    public void AccessFailedCountColumn()
    {
        var accessFailedCountProperty = _entityType.FindProperty(
            nameof(User.AccessFailedCount)
        )!;
        Assert.Equal("integer", accessFailedCountProperty.GetColumnType());
        Assert.False(accessFailedCountProperty.IsColumnNullable());
    }

    [Fact]
    public void AvatarIdColumn()
    {
        var avatarIdProperty = _entityType.FindProperty(nameof(User.AvatarId))!;
        var foreignKey = avatarIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", avatarIdProperty.GetColumnType());
        Assert.True(avatarIdProperty.IsColumnNullable());
    }

    [Fact]
    public void ConcurrencyStampColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(User.ConcurrencyStamp)
        )!;
        Assert.Equal(
            "character varying(36)",
            concurrencyStampProperty.GetColumnType()
        );
        Assert.False(concurrencyStampProperty.IsColumnNullable());
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(User.CreatedAt)
        )!;
        Assert.Equal("timestamp", concurrencyStampProperty.GetColumnType());
        Assert.Equal("now()", concurrencyStampProperty.GetDefaultValueSql());
    }

    [Fact]
    public void DeletedColumn()
    {
        var deletedProperty = _entityType.FindProperty(nameof(User.Deleted))!;
        Assert.Equal("false", deletedProperty.GetDefaultValueSql());
    }

    [Fact]
    public void EmailColumn()
    {
        var emailProperty = _entityType.FindProperty(nameof(User.Email))!;
        Assert.Equal("character varying(320)", emailProperty.GetColumnType());
        Assert.False(emailProperty.IsColumnNullable());
    }

    [Fact]
    public void EmailConfirmedColumn()
    {
        var emailConfirmedProperty = _entityType.FindProperty(
            nameof(User.EmailConfirmed)
        )!;
        Assert.False(emailConfirmedProperty.IsColumnNullable());
        Assert.Equal(emailConfirmedProperty.GetDefaultValue(), false);
    }

    [Fact]
    public void FirstNameColumn()
    {
        var firstNameProperty = _entityType.FindProperty(
            nameof(User.FirstName)
        )!;
        Assert.Equal(
            "character varying(20)",
            firstNameProperty.GetColumnType()
        );
        Assert.False(firstNameProperty.IsColumnNullable());
    }

    [Fact]
    public void LastNameColumn()
    {
        var lastNameProperty = _entityType.FindProperty(nameof(User.LastName))!;
        Assert.Equal("character varying(50)", lastNameProperty.GetColumnType());
        Assert.False(lastNameProperty.IsColumnNullable());
    }

    [Fact]
    public void LockoutEnabledColumn()
    {
        var lockoutEnabledProperty = _entityType.FindProperty(
            nameof(User.LockoutEnabled)
        )!;
        Assert.False(lockoutEnabledProperty.IsColumnNullable());
        Assert.Equal(lockoutEnabledProperty.GetDefaultValue(), false);
    }

    [Fact]
    public void LockoutEndColumn()
    {
        var lockoutEndProperty = _entityType.FindProperty(
            nameof(User.LockoutEnd)
        )!;
        Assert.True(lockoutEndProperty.IsColumnNullable());
        Assert.Equal("timestamp", lockoutEndProperty.GetColumnType());
    }

    [Fact]
    public void UserNotificationsPreferencesMaskColumn()
    {
        var userNotificationsPreferencesMaskProperty = _entityType.FindProperty(
            nameof(User.UserNotificationsPreferencesMask)
        )!;
        Assert.Equal(
            "0",
            userNotificationsPreferencesMaskProperty.GetDefaultValueSql()
        );
    }

    [Fact]
    public void NotificationsAllowTimeStartColumn()
    {
        var notificationsAllowStartTimeProperty = _entityType.FindProperty(
            nameof(User.NotificationsAllowStartTime)
        )!;
        Assert.True(notificationsAllowStartTimeProperty.IsColumnNullable());
        Assert.Equal(
            "time without time zone",
            notificationsAllowStartTimeProperty.GetColumnType()
        );
    }

    [Fact]
    public void NotificationsAllowTimeEndColumn()
    {
        var notificationsAllowEndTimeProperty = _entityType.FindProperty(
            nameof(User.NotificationsAllowEndTime)
        )!;
        Assert.True(notificationsAllowEndTimeProperty.IsColumnNullable());
        Assert.Equal(
            "time without time zone",
            notificationsAllowEndTimeProperty.GetColumnType()
        );
    }

    [Fact]
    public void NotificationsPauseUntilColumn()
    {
        var notificationsPauseUntilProperty = _entityType.FindProperty(
            nameof(User.NotificationsPauseUntil)
        )!;
        Assert.True(notificationsPauseUntilProperty.IsColumnNullable());
        Assert.Equal(
            "time without time zone",
            notificationsPauseUntilProperty.GetColumnType()
        );
    }

    [Fact]
    public void NotificationSoundColumn()
    {
        var notificationSoundProperty = _entityType.FindProperty(
            nameof(User.NotificationSound)
        )!;
        Assert.Equal("0", notificationSoundProperty.GetDefaultValueSql());
    }

    [Fact]
    public void NormalizedEmailColumn()
    {
        var normalizedEmailProperty = _entityType.FindProperty(
            nameof(User.NormalizedEmail)
        )!;
        Assert.Equal(
            "character varying(320)",
            normalizedEmailProperty.GetColumnType()
        );
        Assert.False(normalizedEmailProperty.IsColumnNullable());
    }

    [Fact]
    public void NormalizedUserNameColumn()
    {
        var normalizedUsernameProperty = _entityType.FindProperty(
            nameof(User.NormalizedUserName)
        )!;
        Assert.Equal(
            "character varying(30)",
            normalizedUsernameProperty.GetColumnType()
        );
        Assert.False(normalizedUsernameProperty.IsColumnNullable());
    }

    [Fact]
    public void OnlineStatusColumn()
    {
        var onlineStatusProperty = _entityType.FindProperty(
            nameof(User.OnlineStatus)
        )!;
        Assert.Equal(
            "character varying(20)",
            onlineStatusProperty.GetColumnType()
        );
        Assert.True(onlineStatusProperty.IsColumnNullable());
    }

    [Fact]
    public void OnlineStatusUntilColumn()
    {
        var onlineStatusUntilProperty = _entityType.FindProperty(
            nameof(User.OnlineStatusUntil)
        )!;
        Assert.Equal("timestamp", onlineStatusUntilProperty.GetColumnType());
        Assert.True(onlineStatusUntilProperty.IsColumnNullable());
    }

    [Fact]
    public void PasswordHashColumn()
    {
        var passwordHashProperty = _entityType.FindProperty(
            nameof(User.PasswordHash)
        )!;
        Assert.Equal(
            "character varying(128)",
            passwordHashProperty.GetColumnType()
        );
        Assert.False(passwordHashProperty.IsColumnNullable());
    }

    [Fact]
    public void PhoneNumberColumn()
    {
        var phoneNumberProperty = _entityType.FindProperty(
            nameof(User.PhoneNumber)
        )!;
        Assert.Equal(
            "character varying(20)",
            phoneNumberProperty.GetColumnType()
        );
        Assert.True(phoneNumberProperty.IsColumnNullable());
    }

    [Fact]
    public void PhoneNumberConfirmedColumn()
    {
        var phoneNumberConfirmedProperty = _entityType.FindProperty(
            nameof(User.PhoneNumberConfirmed)
        )!;
        Assert.Equal("boolean", phoneNumberConfirmedProperty.GetColumnType());
        Assert.False(phoneNumberConfirmedProperty.IsColumnNullable());
        Assert.Equal(phoneNumberConfirmedProperty.GetDefaultValue(), false);
    }

    [Fact]
    public void SecurityStampColumn()
    {
        var securityStampProperty = _entityType.FindProperty(
            nameof(User.SecurityStamp)
        )!;
        Assert.Equal(
            "character varying(36)",
            securityStampProperty.GetColumnType()
        );
    }

    [Fact]
    public void ThemeColumn()
    {
        var themeIdProperty = _entityType.FindProperty(nameof(User.ThemeId))!;
        var foreignKey = themeIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal("uuid", themeIdProperty.GetColumnType());
        Assert.True(themeIdProperty.IsColumnNullable());
    }

    [Fact]
    public void TimezoneColumn()
    {
        var timezoneProperty = _entityType.FindProperty(nameof(User.Timezone))!;
        Assert.Equal("character varying(40)", timezoneProperty.GetColumnType());
        Assert.False(timezoneProperty.IsColumnNullable());
    }

    [Fact]
    public void TwoFactorEnabledColumn()
    {
        var twoFactorEnabledProperty = _entityType.FindProperty(
            nameof(User.TwoFactorEnabled)
        )!;
        Assert.False(twoFactorEnabledProperty.IsColumnNullable());
        Assert.Equal(twoFactorEnabledProperty.GetDefaultValue(), false);
    }

    [Fact]
    public void UserNameColumn()
    {
        var usernameProperty = _entityType.FindProperty(nameof(User.UserName))!;
        Assert.Equal("character varying(40)", usernameProperty.GetColumnType());
        Assert.False(usernameProperty.IsColumnNullable());
    }

    [Fact]
    public async void UserDDLMigration_ShouldHaveHappened()
    {
        int numUserRows = await _dbContext.Users.CountAsync();
        Assert.True(numUserRows >= 0);
    }

    [Fact]
    public void Indexes()
    {
        var deletedProperty = _entityType.FindProperty(nameof(User.Deleted))!;
        Assert.NotNull(_entityType.FindIndex(deletedProperty));
        var normalizedEmailProperty = _entityType.FindProperty(
            nameof(User.NormalizedEmail)
        )!;
        Assert.NotNull(_entityType.FindIndex(normalizedEmailProperty));
        var normalizedUsernameProperty = _entityType.FindProperty(
            nameof(User.NormalizedUserName)
        )!;
        Assert.NotNull(_entityType.FindIndex(normalizedUsernameProperty));
    }
}
