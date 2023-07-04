namespace PersistenceService.Constants;

public static class MaskEnumDefs
{
    public const string REPLY = "reply";
    public const string MENTION = "mention";
    public const string REACTION = "reaction";
    public const string THREAD_WATCH = "thread_watch";

    public static readonly Dictionary<string, int> NotificationTypes =
        new Dictionary<string, int>
        {
            { REPLY, 1 },
            { MENTION, 2 },
            { REACTION, 4 },
            { THREAD_WATCH, 8 }
        };
}
