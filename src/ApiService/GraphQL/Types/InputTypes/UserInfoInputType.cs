using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class UserInfoInputType : InputObjectGraphType<UserInfoInput>
{
    public UserInfoInputType()
    {
        Name = "UserInfoInput";
        Field<StringGraphType>("email");
        Field<StringGraphType>("firstName");
        Field<StringGraphType>("lastName");
        Field<IdGraphType>("themeId");
        Field<StringGraphType>("timezone");
        Field<UserNotificationsPreferencesInputType>(
            "notificationsPreferences"
        );
    }
}

public class UserInfoInput
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid? ThemeId { get; set; }
    public string? Timezone { get; set; }
    public UserNotificationsPreferencesInput? NotificationsPreferences { get; set; }
}
