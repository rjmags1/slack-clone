namespace Common.SlackCloneGraphQL.Types.Connections;

public class FilesConnectionEdgeType : ConnectionEdgeType<FileType, File>
{
    public FilesConnectionEdgeType()
    {
        Name = "FilesConnectionEdge";
    }
}
