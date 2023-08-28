using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class UserInputType : InputObjectGraphType<UserInput>
{
    public UserInputType()
    {
        Name = "UserInput";
        Field<IdGraphType>("id");
        Field<IdGraphType>("avatarId");
        Field<StringGraphType>("onlineStatus");
        Field<TimeOnlyGraphType>("onlineStatusUntil");
        Field<UserInfoInputType>("userInfo");
        Field<StringGraphType>("username");
    }
}

public class UserInput
{
    public Guid? Id { get; set; }
    public Guid? AvatarId { get; set; }
    public string? OnlineStatus { get; set; }
    public TimeOnly? OnlineStatusUntil { get; set; }
    public UserInfoInput? UserInfo { get; set; }
    public string? Username { get; set; }
}
