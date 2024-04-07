using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types;

public class UserNotificationsPreferencesType
    : MessageNotificationsType<UserNotificationsPreferences>
{
    public UserNotificationsPreferencesType()
        : base()
    {
        Name = "UserNotificationsPreferences";
        Field<NonNullGraphType<IntGraphType>>("notifSound")
            .Description(
                "The sound that plays when a user gets a notification alert"
            )
            .Resolve(context => context.Source.NotifSound);
        Field<TimeOnlyGraphType>("allowAlertsStartTimeUTC")
            .Description(
                "When during the day a user can receive notification alerts"
            )
            .Resolve(context => context.Source.AllowAlertsStartTimeUTC);
        Field<TimeOnlyGraphType>("allowAlertsEndTimeUTC")
            .Description(
                "When during the day a user stops receiving notification alerts"
            )
            .Resolve(context => context.Source.AllowAlertsEndTimeUTC);
        Field<DateTimeGraphType>("pauseAlertsUntil")
            .Description(
                "Pause all notification alerts until this date and time"
            )
            .Resolve(context => context.Source.PauseAlertsUntil);
    }
}

public class UserNotificationsPreferences : MessageNotifications
{
    public int NotifSound { get; set; }
    public TimeOnly? AllowAlertsStartTimeUTC { get; set; }
    public TimeOnly? AllowAlertsEndTimeUTC { get; set; }
    public TimeOnly? PauseAlertsUntil { get; set; }
}
