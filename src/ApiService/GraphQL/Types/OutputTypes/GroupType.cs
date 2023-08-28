using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class GroupType : UnionGraphType
{
    public GroupType()
    {
        Type<ChannelType>();
        Type<DirectMessageGroupType>();
    }
}

public interface IGroup { }
