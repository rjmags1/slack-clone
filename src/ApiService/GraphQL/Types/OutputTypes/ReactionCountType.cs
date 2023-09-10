using GraphQL.Types;
using SlackCloneGraphQL.Types.Connections;
using SlackCloneGraphQL.Types;

public class ReactionCountType : ObjectGraphType<ReactionCount>
{
    public ReactionCountType()
    {
        Name = "ReactionCount";
        Field<NonNullGraphType<IdGraphType>>("id")
            .Description(
                "Id of the reaction count type, included for Relay compatibility."
            )
            .Resolve(context => context.Source.Id ?? Guid.NewGuid());
        Field<NonNullGraphType<IntGraphType>>("count")
            .Description(
                "The number of this type of reaction to the associated message."
            )
            .Resolve(context => context.Source.Count);
        Field<NonNullGraphType<StringGraphType>>("emoji")
            .Description("The emoji associated with the reaction.")
            .Resolve(context => context.Source.Emoji);
        Field<IdGraphType>("userReactionId")
            .Description(
                "The reaction of this type by the user to the associated message."
            )
            .Resolve(context => context.Source.UserReactionId);
    }
}

public class ReactionCount
{
    public Guid? Id { get; set; }
    public int Count { get; set; }
#pragma warning disable CS8618
    public string Emoji { get; set; }
    public Guid? UserReactionId { get; set; }
#pragma warning restore CS8618
}
