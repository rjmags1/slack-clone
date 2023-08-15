using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public class ValidationResultType : ObjectGraphType<ValidationResult>
{
    public ValidationResultType()
    {
        Name = "ValidationResult";
        Field<NonNullGraphType<BooleanGraphType>>("valid")
            .Description("The result of the validation")
            .Resolve(context => context.Source.Valid);
    }
}

public class ValidationResult
{
    public bool Valid { get; set; }
}
