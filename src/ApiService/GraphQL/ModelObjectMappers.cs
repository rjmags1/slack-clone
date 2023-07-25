using ApiService.Utils;
using SlackCloneGraphQL.Types;
using File = SlackCloneGraphQL.Types.File;
using Models = PersistenceService.Models;

namespace SlackCloneGraphQL;

public class ModelToObjectConverters
{
    private const string DEFAULT_ONLINE_STATUS = "offline";

    private static readonly File DefaultAvatar = new File
    {
        Id = Guid.Empty,
        Name = "DEFAULT_AVATAR",
        StoreKey = "DEFAULT_AVATAR",
        UploadedAt = default(DateTime)
    };

    private static readonly Theme DefaultTheme = new Theme
    {
        Id = Guid.Empty,
        Name = "DEFAULT_THEME"
    };

    public static User ConvertUser(
        Models.User modelUser,
        IEnumerable<string> requestedFields
    )
    {
        User user = new User
        {
            Id = modelUser.Id,
            Avatar = ConvertAvatar(modelUser.Avatar),
            OnlineStatus = modelUser.OnlineStatus ?? DEFAULT_ONLINE_STATUS,
            OnlineStatusUntil = modelUser.OnlineStatusUntil,
            Username = modelUser.UserName,
            CreatedAt = modelUser.CreatedAt,
        };
        if (
            requestedFields.Contains(
                StringUtils.ToLowerFirstLetter(nameof(User.PersonalInfo))
            )
        )
        {
            UserInfo userInfo = new UserInfo
            {
                Email = modelUser.Email,
                EmailConfirmed = modelUser.EmailConfirmed,
                FirstName = modelUser.FirstName,
                LastName = modelUser.LastName,
                Theme = ConvertTheme(modelUser.Theme),
                Timezone = modelUser.Timezone,
                UserNotificationsPreferences =
                    ConvertUserNotificationsPreferences(modelUser)
            };
            user.PersonalInfo = userInfo;
        }

        return user;
    }

    public static File ConvertAvatar(Models.File? avatar)
    {
        return avatar is null
            ? DefaultAvatar
            : new File
            {
                Id = avatar.Id,
                Name = avatar.Name,
                StoreKey = avatar.StoreKey,
                UploadedAt = avatar.UploadedAt
            };
    }

    public static Theme ConvertTheme(Models.Theme? theme)
    {
        return theme is null
            ? DefaultTheme
            : new Theme { Id = theme.Id, Name = theme.Name };
    }

    public static UserNotificationsPreferences ConvertUserNotificationsPreferences(
        Models.User modelUser
    )
    {
        int mask = modelUser.UserNotificationsPreferencesMask;
        return new UserNotificationsPreferences
        {
            AllMessages = (mask & 1) > 0,
            NoMessages = (mask & 2) > 0,
            Mentions = (mask & 4) > 0,
            DMs = (mask & 8) > 0,
            Replies = (mask & 16) > 0,
            ThreadWatch = (mask & 32) > 0,
            NotifSound = modelUser.NotificationSound,
            AllowAlertsStartTimeUTC = modelUser.NotificationsAllowStartTime,
            AllowAlertsEndTimeUTC = modelUser.NotificationsAllowEndTime,
            PauseAlertsUntil = modelUser.NotificationsPauseUntil
        };
    }
}
