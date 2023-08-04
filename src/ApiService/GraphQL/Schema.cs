using GraphQL.Instrumentation;
using GraphQL.Types;
using SlackCloneGraphQL.Auth;

namespace SlackCloneGraphQL;

public class SlackCloneSchema : Schema
{
    public SlackCloneSchema(IServiceProvider provider)
        : base(provider)
    {
        Query =
            (SlackCloneQuery)provider.GetService(typeof(SlackCloneQuery))!
            ?? throw new InvalidOperationException();
        //Mutation =
        //(SlackCloneMutation)provider.GetService(typeof(SlackCloneMutation))!
        //?? throw new InvalidOperationException();

        FieldMiddleware.Use(new InstrumentFieldsMiddleware());
        FieldMiddleware.Use(new VariableClaimMappingCheckFieldMiddleware());

        Directives.Register(new RequiresVariableClaimMappingDirective());
    }
}
