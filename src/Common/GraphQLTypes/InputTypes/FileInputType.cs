using GraphQL.Types;

namespace Common.SlackCloneGraphQL.Types;

public class FileInputType : InputObjectGraphType<FileInput>
{
    public FileInputType()
    {
        Name = "FileInput";
        Field<NonNullGraphType<StringGraphType>>("name");
        Field<NonNullGraphType<StringGraphType>>("storeKey");
        Field<NonNullGraphType<IdGraphType>>("uploaderId");
    }
}

public class FileInput
{
#pragma warning disable CS8618
    public string Name { get; set; }
    public string StoreKey { get; set; }
#pragma warning restore CS8618
    public Guid UploaderId { get; set; }
}
