using GraphQL;
using GraphQL.Types;
using SlackCloneGraphQL.Types;

namespace SlackCloneGraphQL;

public class SlackCloneMutation : ObjectGraphType
{
    public SlackCloneMutation()
    {
        Name = "Mutation";
    }
}