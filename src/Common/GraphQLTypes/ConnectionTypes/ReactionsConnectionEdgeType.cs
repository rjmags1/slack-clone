namespace Common.SlackCloneGraphQL.Types.Connections;

public class ReactionsConnectionEdgeType
    : ConnectionEdgeType<ReactionType, Reaction>
{
    public ReactionsConnectionEdgeType()
    {
        Name = "ReactionsConnectionEdge";
    }
}
