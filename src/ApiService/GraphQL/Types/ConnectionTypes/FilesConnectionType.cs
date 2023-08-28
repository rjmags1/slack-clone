namespace SlackCloneGraphQL.Types.Connections;

public class FilesConnectionType
    : ConnectionType<FileType, File, FilesConnectionEdgeType>
{
    public FilesConnectionType()
    {
        Name = "FilesConnection";
    }
}
