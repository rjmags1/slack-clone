using GraphQL.Types;

namespace SlackCloneGraphQL.Types;

public abstract class MessageNotificationsType<T> : ObjectGraphType<T>
    where T : MessageNotifications
{
    public MessageNotificationsType()
    {
        Field<NonNullGraphType<BooleanGraphType>>("allMessages")
            .Description("Toggle on all message notifications")
            .Resolve(context => context.Source.AllMessages);
        Field<NonNullGraphType<BooleanGraphType>>("noMessages")
            .Description("Toggle off all message notifications")
            .Resolve(context => context.Source.NoMessages);
        Field<NonNullGraphType<BooleanGraphType>>("mentions")
            .Description("Toggle user mention notifications")
            .Resolve(context => context.Source.Mentions);
        Field<NonNullGraphType<BooleanGraphType>>("dms")
            .Description("Toggle direct message notifications")
            .Resolve(context => context.Source.DMs);
        Field<NonNullGraphType<BooleanGraphType>>("replies")
            .Description("Toggle reply notifications")
            .Resolve(context => context.Source.Replies);
        Field<NonNullGraphType<BooleanGraphType>>("threadWatch")
            .Description("Toggle thread watch notifications")
            .Resolve(context => context.Source.ThreadWatch);
    }
}

public abstract class MessageNotifications
{
    public bool AllMessages { get; set; }
    public bool NoMessages { get; set; }
    public bool Mentions { get; set; }
    public bool DMs { get; set; }
    public bool Replies { get; set; }
    public bool ThreadWatch { get; set; }
}
