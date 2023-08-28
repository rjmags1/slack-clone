namespace SlackCloneGraphQL.Types.Connections;

public class MentionsConnectionType
    : ConnectionType<MentionType, Mention, MentionsConnectionEdgeType>
{
    public MentionsConnectionType()
    {
        Name = "MentionsConnection";
    }
}
