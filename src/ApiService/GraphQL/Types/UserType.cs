using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class UserType : ObjectGraphType<User>
{
    public UserType(SlackCloneData data)
    {
        Name = "User";
        //
    }
}

public class User { }
