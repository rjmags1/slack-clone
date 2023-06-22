using DotnetTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DotnetTests.PersistenceService.Migrations;

[Collection("Database collection")]
public class UserMigrationsTests
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IEntityType _entityType;

    public UserMigrationsTests(
        ApplicationDbContextFixture applicationDbContextFixture
    )
    {
        _dbContext = applicationDbContextFixture.context;
        _entityType = _dbContext.Model.FindEntityType(typeof(User))!;
    }

    [Fact]
    public void IdColumn()
    {
        var idProperty = _entityType.FindProperty(nameof(User.Id))!;
        string defaultValueSql = idProperty.GetDefaultValueSql()!;
        Assert.Equal(defaultValueSql, "gen_random_uuid()");
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
        Assert.Equal(accessFailedCountProperty.GetColumnType(), "integer");
        Assert.Equal(accessFailedCountProperty.IsNullable, false);
    }

    [Fact]
    public void AvatarIdColumn()
    {
        var avatarIdProperty = _entityType.FindProperty(nameof(User.AvatarId))!;
        string avatarIdColumnType = avatarIdProperty.GetColumnType();
        var foreignKey = avatarIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal(avatarIdColumnType, "uuid");
    }

    [Fact]
    public void ConcurrencyStampColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(User.ConcurrencyStamp)
        )!;
        Assert.Equal(
            concurrencyStampProperty.GetColumnType(),
            "character varying(36)"
        );
    }

    [Fact]
    public void CreatedAtColumn()
    {
        var concurrencyStampProperty = _entityType.FindProperty(
            nameof(User.CreatedAt)
        )!;
        Assert.Equal(concurrencyStampProperty.GetColumnType(), "timestamp");
        Assert.Equal(concurrencyStampProperty.GetDefaultValueSql(), "now()");
    }

    [Fact]
    public void DeletedColumn()
    {
        var deletedProperty = _entityType.FindProperty(nameof(User.Deleted))!;
        Assert.Equal(deletedProperty.GetColumnType(), "boolean");
        Assert.False(deletedProperty.IsNullable);
        Assert.Equal(deletedProperty.GetDefaultValue(), false);
    }

    [Fact]
    public void EmailColumn()
    {
        var emailProperty = _entityType.FindProperty(nameof(User.Email))!;
        Assert.Equal(emailProperty.GetColumnType(), "character varying(320)");
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
            firstNameProperty.GetColumnType(),
            "character varying(20)"
        );
        Assert.False(firstNameProperty.IsColumnNullable());
    }

    [Fact]
    public void LastNameColumn()
    {
        var lastNameProperty = _entityType.FindProperty(nameof(User.LastName))!;
        Assert.Equal(lastNameProperty.GetColumnType(), "character varying(50)");
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
        Assert.Equal(lockoutEndProperty.GetColumnType(), "timestamp");
    }

    [Fact]
    public void UserNotificationsPreferencesMaskColumn()
    {
        var userNotificationsPreferencesMaskProperty = _entityType.FindProperty(
            nameof(User.UserNotificationsPreferencesMask)
        )!;
        Assert.False(
            userNotificationsPreferencesMaskProperty.IsColumnNullable()
        );
        Assert.Equal(
            userNotificationsPreferencesMaskProperty.GetColumnType(),
            "integer"
        );
        Assert.Equal(
            userNotificationsPreferencesMaskProperty.GetDefaultValue(),
            0
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
            notificationsAllowStartTimeProperty.GetColumnType(),
            "time without time zone"
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
            notificationsAllowEndTimeProperty.GetColumnType(),
            "time without time zone"
        );
    }

    [Fact]
    public void NotificationsPauseUntilColumn()
    {
        var notificationsPauseUntilProperty = _entityType.FindProperty(
            nameof(User.NotificationsAllowEndTime)
        )!;
        Assert.True(notificationsPauseUntilProperty.IsColumnNullable());
        Assert.Equal(
            notificationsPauseUntilProperty.GetColumnType(),
            "time without time zone"
        );
    }

    [Fact]
    public void NotificationSoundColumn()
    {
        var notificationSoundProperty = _entityType.FindProperty(
            nameof(User.NotificationSound)
        )!;
        Assert.False(notificationSoundProperty.IsColumnNullable());
        Assert.Equal(notificationSoundProperty.GetColumnType(), "integer");
        Assert.Equal(notificationSoundProperty.GetDefaultValue(), 0);
    }

    [Fact]
    public void NormalizedEmailColumn()
    {
        var normalizedEmailProperty = _entityType.FindProperty(
            nameof(User.NormalizedEmail)
        )!;
        Assert.Equal(
            normalizedEmailProperty.GetColumnType(),
            "character varying(320)"
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
            normalizedUsernameProperty.GetColumnType(),
            "character varying(30)"
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
            onlineStatusProperty.GetColumnType(),
            "character varying(20)"
        );
        Assert.True(onlineStatusProperty.IsColumnNullable());
    }

    [Fact]
    public void OnlineStatusUntilColumn()
    {
        var onlineStatusUntilProperty = _entityType.FindProperty(
            nameof(User.OnlineStatusUntil)
        )!;
        Assert.Equal(onlineStatusUntilProperty.GetColumnType(), "timestamp");
        Assert.True(onlineStatusUntilProperty.IsColumnNullable());
    }

    [Fact]
    public void PasswordHashColumn()
    {
        var passwordHashProperty = _entityType.FindProperty(
            nameof(User.PasswordHash)
        )!;
        Assert.Equal(
            passwordHashProperty.GetColumnType(),
            "character varying(128)"
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
            phoneNumberProperty.GetColumnType(),
            "character varying(20)"
        );
        Assert.True(phoneNumberProperty.IsColumnNullable());
    }

    [Fact]
    public void PhoneNumberConfirmedColumn()
    {
        var phoneNumberConfirmedProperty = _entityType.FindProperty(
            nameof(User.PhoneNumberConfirmed)
        )!;
        Assert.Equal(phoneNumberConfirmedProperty.GetColumnType(), "boolean");
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
            securityStampProperty.GetColumnType(),
            "character varying(36)"
        );
    }

    [Fact]
    public void ThemeColumn()
    {
        var themeIdProperty = _entityType.FindProperty(nameof(User.ThemeId))!;
        string avatarIdColumnType = themeIdProperty.GetColumnType();
        var foreignKey = themeIdProperty
            .GetContainingForeignKeys()
            .SingleOrDefault();

        Assert.NotNull(foreignKey);
        Assert.Equal(DeleteBehavior.SetNull, foreignKey.DeleteBehavior);
        Assert.Equal(avatarIdColumnType, "uuid");
    }

    [Fact]
    public void TimezoneColumn()
    {
        var timezoneProperty = _entityType.FindProperty(nameof(User.Timezone))!;
        Assert.Equal(timezoneProperty.GetColumnType(), "character varying(40)");
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
        Assert.Equal(usernameProperty.GetColumnType(), "character varying(30)");
        Assert.False(usernameProperty.IsColumnNullable());
    }

    public async void UserDDLMigration_ShouldHaveHappened()
    {
        int numUserRows = await _dbContext.Users.CountAsync();
        Assert.True(numUserRows >= 0);
    }

    public void Indexes()
    {
        var deletedProperty = _entityType.FindProperty(nameof(User.Deleted))!;
        Assert.NotNull(deletedProperty.GetIndex());
        var normalizedEmailProperty = _entityType.FindProperty(
            nameof(User.NormalizedEmail)
        )!;
        Assert.NotNull(normalizedEmailProperty.GetIndex());
        var normalizedUsernameProperty = _entityType.FindProperty(
            nameof(User.NormalizedUserName)
        )!;
        Assert.NotNull(normalizedUsernameProperty.GetIndex());
    }
}
