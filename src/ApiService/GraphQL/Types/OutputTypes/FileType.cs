using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;

namespace SlackCloneGraphQL.Types;

public class FileType : ObjectGraphType<File>, INodeGraphType<File>
{
    public FileType()
    {
        Name = "File";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description("The UUID of the file")
            .Resolve(context => context.Source.Id);
        Field<NonNullGraphType<StringGraphType>>("name")
            .Description("The name of the file")
            .Resolve(context => context.Source.Name);
        Field<NonNullGraphType<StringGraphType>>("storeKey")
            .Description(
                "The identifier used by the file store to fetch the file contents"
            )
            .Resolve(context => context.Source.StoreKey);
        Field<NonNullGraphType<DateTimeGraphType>>("uploadedAt")
            .Description("When the file was uploaded")
            .Resolve(context => context.Source.UploadedAt);
    }
}

public class File : INode
{
    public Guid Id { get; set; }
#pragma warning disable CS8618
    public string Name { get; set; }
    public string StoreKey { get; set; }
    public DateTime UploadedAt { get; set; }
#pragma warning restore CS8618
}
