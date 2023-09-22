using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class UserNotificationsPreferencesInputType
    : InputObjectGraphType<UserNotificationsPreferencesInput>
{
    public UserNotificationsPreferencesInputType()
    {
        Name = "UserNotificationsPreferencesInput";
        Field<BooleanGraphType>("allMessages");
        Field<BooleanGraphType>("noMessages");
        Field<BooleanGraphType>("mentions");
        Field<BooleanGraphType>("dms");
        Field<BooleanGraphType>("replies");
        Field<BooleanGraphType>("threadWatch");
        Field<IdGraphType>("notifSound");
        Field<DateTimeGraphType>("allowAlertsStartTimeUTC");
        Field<DateTimeGraphType>("allowAlertsEndTimeUTC");
        Field<DateTimeGraphType>("pauseAlertsUntilUTC");
    }
}

public class UserNotificationsPreferencesInput
{
    public bool? AllMessages { get; set; }
    public bool? NoMessages { get; set; }
    public bool? Mentions { get; set; }
    public bool? Dms { get; set; }
    public bool? Replies { get; set; }
    public bool? ThreadWatch { get; set; }
    public Guid? NotifSound { get; set; }
    public DateTime? AllowAlertsStartTime { get; set; }
    public DateTime? AllowAlertsEndTime { get; set; }
    public DateTime? PauseAlertsUntil { get; set; }
}
