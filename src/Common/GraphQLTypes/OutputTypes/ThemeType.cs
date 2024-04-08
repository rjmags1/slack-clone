using GraphQL.Types;
using Common.SlackCloneGraphQL.Types.Connections;

namespace Common.SlackCloneGraphQL.Types;

public class ThemeType : ObjectGraphType<Theme>, INodeGraphType<Theme>
{
    public ThemeType()
    {
        Name = "Theme";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the theme")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<StringGraphType>>("name")
            .Description("The name of the theme")
            .Resolve(context => context.Source.Name);
    }
}

public class Theme : INode
{
    public Guid Id { get; set; }
#pragma warning disable CS8618
    public string Name { get; set; }
#pragma warning restore CS8618
}
