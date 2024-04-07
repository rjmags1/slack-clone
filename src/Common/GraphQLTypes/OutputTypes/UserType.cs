using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

namespace Common.SlackCloneGraphQL.Types;

public class UserType : ObjectGraphType<User>, INodeGraphType<User>
{
    public UserType()
    {
        Name = "User";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the user")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<FileType>>("avatar")
            .Description("The user's avatar")
            .Resolve(context => context.Source.Avatar);
        Field<NonNullGraphType<DateTimeGraphType>>("createdAt")
            .Description("When the user's account was created")
            .Resolve(context => context.Source.CreatedAt);
        Field<NonNullGraphType<StringGraphType>>("onlineStatus")
            .Description("The user's current online status")
            .Resolve(context => context.Source.OnlineStatus);
        Field<UserInfoType>("personalInfo")
            .Description("The user's personal information")
            .Resolve(context => context.Source.PersonalInfo);
        Field<NonNullGraphType<StringGraphType>>("username")
            .Description("The user's username")
            .Resolve(context => context.Source.Username);
    }
}

public class User : INode
{
    public Guid Id { get; set; }
#pragma warning disable CS8618
    public File Avatar { get; set; }
    public string OnlineStatus { get; set; }
    public DateTime? OnlineStatusUntil { get; set; }
    public string Username { get; set; }
#pragma warning restore CS8618
    public DateTime CreatedAt { get; set; }
    public UserInfo? PersonalInfo { get; set; }
}
