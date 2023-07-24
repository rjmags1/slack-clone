using GraphQL;
using GraphQL.Types;
using SlackCloneGraphQL.Types;

namespace SlackCloneGraphQL;

public class SlackCloneQuery : ObjectGraphType<object>
{
    public SlackCloneQuery()
    {
        Name = "query";
        Field<StringGraphType>("test").Resolve(context => "Hello");
        //AddField(
        //new FieldType
        //{
        //Name = "WorkspacesPageQuery",
        //Arguments = new QueryArguments(
        //new QueryArgument<NonNullGraphType<IdGraphType>>
        //{
        //Name = "userId"
        //},
        //new QueryArgument<
        //NonNullGraphType<WorkspacesFilterInputType>
        //>
        //{
        //Name = "workspacesFilter"
        //}
        //),
        //}
        //);
    }
}
