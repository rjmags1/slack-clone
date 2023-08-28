namespace SlackCloneGraphQL.Types.Connections;

public class ReactionsConnectionType
    : ConnectionType<ReactionType, Reaction, ReactionsConnectionEdgeType>
{
    public ReactionsConnectionType()
    {
        Name = "ReactionsConnection";
    }
}
