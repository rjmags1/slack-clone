namespace SlackCloneGraphQL.Types.Connections;

public class MentionsConnectionEdgeType
    : ConnectionEdgeType<MentionType, Mention>
{
    public MentionsConnectionEdgeType()
    {
        Name = "MentionsConnectionEdge";
    }
}
