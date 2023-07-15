using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using SlackCloneGraphQL.Types;

namespace SlackCloneGraphQL;

public class SlackCloneQuery : ObjectGraphType<object>
{
    public SlackCloneQuery(SlackCloneData data)
    {
        Name = "Query";
    }
}
