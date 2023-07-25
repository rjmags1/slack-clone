using GraphQL.Types;
using Models = PersistenceService.Models;

namespace SlackCloneGraphQL.Types;

public class UserInfoType : ObjectGraphType<UserInfo>
{
    public UserInfoType()
    {
        Name = "UserInfo";
        Field<NonNullGraphType<StringGraphType>>("email")
            .Description("The user's email")
            .Resolve(context => context.Source.Email);
        Field<NonNullGraphType<BooleanGraphType>>("emailConfirmed")
            .Description("If the user has confirmed their email")
            .Resolve(context => context.Source.EmailConfirmed);
        Field<NonNullGraphType<StringGraphType>>("firstName")
            .Description("The user's first name")
            .Resolve(context => context.Source.FirstName);
        Field<NonNullGraphType<StringGraphType>>("lastName")
            .Description("The user's last name")
            .Resolve(context => context.Source.LastName);
        Field<NonNullGraphType<UserNotificationsPreferencesType>>(
                "userNotificationsPreferences"
            )
            .Description("The user's notification preferences")
            .Resolve(context => context.Source.UserNotificationsPreferences);
        Field<NonNullGraphType<ThemeType>>("theme")
            .Description("The user's default workspace theme")
            .Resolve(context => context.Source.Theme);
        Field<NonNullGraphType<StringGraphType>>("timezone")
            .Description("The user's timezone")
            .Resolve(context => context.Source.Timezone);
    }
}

public class UserInfo
{
#pragma warning disable CS8618
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserNotificationsPreferences UserNotificationsPreferences { get; set; }
    public Theme Theme { get; set; }
    public string Timezone { get; set; }
#pragma warning restore CS8618
}
