using System.Text.Json;
using ApiService.Utils;
using PersistenceService.Stores;
using SlackCloneGraphQL.Types;
using SlackCloneGraphQL.Types.Connections;
using File = SlackCloneGraphQL.Types.File;
using Models = PersistenceService.Models;
using Thread = SlackCloneGraphQL.Types.Thread;

namespace SlackCloneGraphQL;

/// <summary>
/// This class performs data transfer utility functions that enable the
/// translation of both EF Core Model, and anonymous objects resulting
/// from optimized dynamic LINQ
/// queries, into objects that can be understood by GraphQL.NET and translated
/// into GraphQL responses.
///
/// The methods of the class that handle anonymous object LINQ query results convert
/// them into duck-typed System.Dynamic.Expando objects that implement
/// IDictionary. The conversion is done via json serialization
/// and is useful because it provides a way to check for the presence of individual
/// members of EF Core model objects (which the expando objects originate from).
/// </summary>
public static class ModelToObjectConverters
{
    private const string DEFAULT_ONLINE_STATUS = "offline";

    private static readonly File DefaultAvatar =
        new()
        {
            Id = Guid.Empty,
            Name = "DEFAULT_AVATAR",
            StoreKey = "DEFAULT_AVATAR",
        };

    private static readonly Theme DefaultTheme =
        new() { Id = Guid.Empty, Name = "DEFAULT_THEME" };

    public static Message ConvertDynamicChannelMessage(
        dynamic modelChannelMessage,
        List<ChannelMessageReactionCount>? reactionCounts,
        List<string> userFields
    )
    {
        var expando = DynamicUtils.ToExpando(modelChannelMessage);
        if (
            !DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMessage.Deleted)
            )
        )
        {
            throw new InvalidOperationException("Must query deleted column");
        }
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMessage.Channel)
            )
        )
        {
            throw new InvalidOperationException("Channel column redundant");
        }

        Message message = new();
        if (DynamicUtils.HasProperty(expando, nameof(Message.Id)))
        {
            message.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.User)))
        {
            if (expando.Deleted)
            {
                message.User = null;
            }
            else
            {
                Models.User dbUser = JsonSerializer.Deserialize<Models.User>(
                    expando.User
                );
                message.User = ConvertUser(dbUser, userFields);
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.Content)))
        {
            message.Content = expando.Deleted
                ? "deleted"
                : JsonSerializer.Deserialize<string>(expando.Content);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.CreatedAt)))
        {
            message.CreatedAt = JsonSerializer.Deserialize<DateTime>(
                expando.CreatedAt
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.Files)))
        {
            List<Models.File> dbFiles = JsonSerializer.Deserialize<
                List<Models.File>
            >(expando.Files);
            if (expando.Deleted)
            {
                message.Files = null;
            }
            else
            {
                List<File> files = new();
                foreach (Models.File dbFile in dbFiles)
                {
                    files.Add(ConvertFile(dbFile));
                }
                message.Files = files;
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.IsReply)))
        {
            message.IsReply = JsonSerializer.Deserialize<bool>(expando.IsReply);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.LastEdit)))
        {
            message.LastEdit = expando.LastEdit is null
                ? null
                : JsonSerializer.Deserialize<DateTime>(expando.LastEdit);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.LaterFlag)))
        {
            message.LaterFlag = null;
            if (!expando.Deleted && expando.LaterFlag is not null)
            {
                Models.ChannelMessageLaterFlag dbLaterFlag =
                    JsonSerializer.Deserialize<Models.ChannelMessageLaterFlag>(
                        expando.LaterFlag
                    );
                LaterFlag? laterFlag = ConvertChannelMessageLaterFlag(
                    dbLaterFlag
                );
                message.LaterFlag = laterFlag;
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.Mentions)))
        {
            List<Mention> mentions = new();
            List<Models.ChannelMessageMention> dbMentions =
                JsonSerializer.Deserialize<List<Models.ChannelMessageMention>>(
                    expando.Mentions
                );
            message.Mentions = null;
            if (!expando.Deleted && dbMentions.Count > 0)
            {
                foreach (Models.ChannelMessageMention dbMention in dbMentions)
                {
                    mentions.Add(ConvertChannelMessageMention(dbMention));
                }
                message.Mentions = mentions;
            }
        }
        if (reactionCounts is not null && reactionCounts.Count > 0)
        {
            message.Reactions = expando.Deleted
                ? null
                : ConvertChannelMessageReactionCounts(reactionCounts);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.ReplyToId)))
        {
            message.ReplyToId = null;
            if (expando.ReplyToId is not null)
            {
                message.ReplyToId = JsonSerializer.Deserialize<Guid>(
                    expando.ReplyToId
                );
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.SentAt)))
        {
            message.SentAt = expando.SentAt is null
                ? null
                : JsonSerializer.Deserialize<DateTime>(expando.SentAt);
            message.Draft = expando.SentAt is null;
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.ThreadId)))
        {
            message.ThreadId = null;
            if (expando.ThreadId is not null)
            {
                message.ThreadId = JsonSerializer.Deserialize<Guid>(
                    expando.ThreadId
                );
            }
        }
        message.Type = 1;

        return message;
    }

    public static Thread ConvertThread(Models.Thread dbThread)
    {
        return new Thread
        {
            Id = dbThread.Id,
            Channel = ConvertChannel(dbThread.Channel),
            FirstMessage = ConvertChannelMessage(dbThread.FirstMessage),
            NumMessages = dbThread.NumMessages,
            Workspace = ConvertWorkspace(dbThread.Workspace)
        };
    }

    public static LaterFlag? ConvertDirectMessageLaterFlag(
        Models.DirectMessageLaterFlag? dbLaterFlag
    )
    {
        return dbLaterFlag is null
            ? null
            : new LaterFlag
            {
                Id = dbLaterFlag.Id,
                Message = ConvertDirectMessage(dbLaterFlag.DirectMessage)!,
                Status = dbLaterFlag.DirectMessageLaterFlagStatus
            };
    }

    public static Message? ConvertDirectMessage(
        Models.DirectMessage? dbMessage,
        List<DirectMessageReactionCount>? reactionsCounts = null
    )
    {
        return dbMessage is null
            ? null
            : new Message
            {
                Id = dbMessage.Id,
                User = ConvertUser(dbMessage.User),
                Content = dbMessage.Content,
                CreatedAt = dbMessage.CreatedAt,
                LastEdit = dbMessage.LastEdit,
                Files = dbMessage.Files.Select(f => ConvertFile(f)).ToList(),
                Group = ConvertDirectMessageGroup(dbMessage.DirectMessageGroup),
                IsReply = dbMessage.IsReply,
                LaterFlag = ConvertDirectMessageLaterFlag(dbMessage.LaterFlag),
                Mentions = dbMessage.Mentions
                    .Select(m => ConvertDirectMessageMention(m))
                    .ToList(),
                Reactions = reactionsCounts is null
                    ? null
                    : ConvertDirectMessageReactionCounts(reactionsCounts),
                ReplyToId = dbMessage.ReplyToId,
                SentAt = dbMessage.SentAt,
                Type = 1
            };
    }

    public static Mention ConvertDirectMessageMention(
        Models.DirectMessageMention dbMention
    )
    {
        return new Mention
        {
            Id = dbMention.Id,
            CreatedAt = dbMention.CreatedAt
        };
    }

    public static DirectMessageGroup ConvertDirectMessageGroup(
        Models.DirectMessageGroup dbGroup
    )
    {
        return new DirectMessageGroup
        {
            Id = dbGroup.Id,
            CreatedAt = dbGroup.CreatedAt,
            Members = dbGroup.DirectMessageGroupMembers
                .Select(dbMember => ConvertDirectMessageGroupMember(dbMember))
                .ToList(),
            Workspace = ConvertWorkspace(dbGroup.Workspace)
        };
    }

    public static LaterFlag? ConvertChannelMessageLaterFlag(
        Models.ChannelMessageLaterFlag? dbLaterFlag
    )
    {
        return dbLaterFlag is null
            ? null
            : new LaterFlag
            {
                Id = dbLaterFlag.Id,
                Message = ConvertChannelMessage(dbLaterFlag.ChannelMessage)!,
                Status = dbLaterFlag.ChannelLaterFlagStatus
            };
    }

    public static List<ReactionCount> ConvertChannelMessageReactionCounts(
        List<ChannelMessageReactionCount> dbReactionCounts
    )
    {
        return dbReactionCounts
            .Select(
                dbrc =>
                    new ReactionCount
                    {
                        Count = dbrc.Count,
                        Emoji = dbrc.Emoji,
                        UserReactionId = dbrc.UserReaction?.Id
                    }
            )
            .ToList();
    }

    public static List<ReactionCount> ConvertDirectMessageReactionCounts(
        List<DirectMessageReactionCount> dbReactionCounts
    )
    {
        return dbReactionCounts
            .Select(
                dbrc =>
                    new ReactionCount
                    {
                        Count = dbrc.Count,
                        Emoji = dbrc.Emoji,
                        UserReactionId = dbrc.UserReaction?.Id
                    }
            )
            .ToList();
    }

    public static Mention ConvertChannelMessageMention(
        Models.ChannelMessageMention dbMention
    )
    {
        return new Mention
        {
            Id = dbMention.Id,
            CreatedAt = dbMention.CreatedAt
        };
    }

    public static Message? ConvertChannelMessage(
        Models.ChannelMessage? dbMessage,
        List<ChannelMessageReactionCount>? reactionsCounts = null
    )
    {
        return dbMessage is null
            ? null
            : new Message
            {
                Id = dbMessage.Id,
                User = ConvertUser(dbMessage.User),
                Content = dbMessage.Content,
                CreatedAt = dbMessage.CreatedAt,
                LastEdit = dbMessage.LastEdit,
                Files = dbMessage.Files.Select(f => ConvertFile(f)).ToList(),
                Group = ConvertChannel(dbMessage.Channel),
                IsReply = dbMessage.IsReply,
                LaterFlag = ConvertChannelMessageLaterFlag(dbMessage.LaterFlag),
                Mentions = dbMessage.Mentions
                    .Select(m => ConvertChannelMessageMention(m))
                    .ToList(),
                Reactions = reactionsCounts is null
                    ? null
                    : ConvertChannelMessageReactionCounts(reactionsCounts),
                ReplyToId = dbMessage.ReplyToId,
                SentAt = dbMessage.SentAt,
                Type = 1
            };
    }

    public static DirectMessageGroup ConvertDynamicDirectMessageGroup(
        dynamic modelDirectMessageGroup
    )
    {
        var expando = DynamicUtils.ToExpando(modelDirectMessageGroup);
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.DirectMessageGroup.DirectMessages)
            )
        )
        {
            throw new InvalidOperationException(
                "Cannot load a paginated collection within a collection"
            );
        }

        DirectMessageGroup group = new();
        if (DynamicUtils.HasProperty(expando, nameof(DirectMessageGroup.Id)))
        {
            group.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(DirectMessageGroup.CreatedAt)
            )
        )
        {
            group.CreatedAt = JsonSerializer.Deserialize<DateTime>(
                expando.CreatedAt
            );
        }
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.DirectMessageGroup.DirectMessageGroupMembers)
            )
        )
        {
            List<Models.DirectMessageGroupMember> dbMembers =
                JsonSerializer.Deserialize<
                    List<Models.DirectMessageGroupMember>
                >(expando.DirectMessageGroupMembers);
            group.Members = dbMembers
                .Select(dbMember => ConvertDirectMessageGroupMember(dbMember))
                .ToList();
        }
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(DirectMessageGroup.Workspace)
            )
        )
        {
            var dbWorkspace = JsonSerializer.Deserialize<Models.Workspace>(
                expando.Workspace
            );
            group.Workspace = ConvertWorkspace(dbWorkspace);
        }

        return group;
    }

    public static DirectMessageGroupMember ConvertDirectMessageGroupMember(
        Models.DirectMessageGroupMember dbMember
    )
    {
        return new DirectMessageGroupMember
        {
            Id = dbMember.Id,
            DirectMessageGroupId = dbMember.DirectMessageGroupId,
            JoinedAt = dbMember.JoinedAt,
            LastViewedAt = dbMember.LastViewedAt,
            Starred = dbMember.Starred,
            User = ConvertUser(dbMember.User)
        };
    }

    public static Message ConvertDynamicDirectMessage(
        dynamic modelDirectMessage,
        List<DirectMessageReactionCount> reactionCounts,
        List<string> userFields
    )
    {
        var expando = DynamicUtils.ToExpando(modelDirectMessage);
        if (
            !DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMessage.Deleted)
            )
        )
        {
            throw new InvalidOperationException("Must query deleted column");
        }
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.DirectMessage.DirectMessageGroup)
            )
        )
        {
            throw new InvalidOperationException(
                "DirectMessage column redundant"
            );
        }

        Message message = new();
        if (DynamicUtils.HasProperty(expando, nameof(Message.Id)))
        {
            message.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.User)))
        {
            if (expando.Deleted)
            {
                message.User = null;
            }
            else
            {
                Models.User dbUser = JsonSerializer.Deserialize<Models.User>(
                    expando.User
                );
                message.User = ConvertUser(dbUser, userFields);
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.Content)))
        {
            message.Content = expando.Deleted
                ? "deleted"
                : JsonSerializer.Deserialize<string>(expando.Content);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.CreatedAt)))
        {
            message.CreatedAt = JsonSerializer.Deserialize<DateTime>(
                expando.CreatedAt
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.Files)))
        {
            List<Models.File> dbFiles = JsonSerializer.Deserialize<
                List<Models.File>
            >(expando.Files);
            if (expando.Deleted)
            {
                message.Files = null;
            }
            else
            {
                List<File> files = new();
                foreach (Models.File dbFile in dbFiles)
                {
                    files.Add(ConvertFile(dbFile));
                }
                message.Files = files;
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.IsReply)))
        {
            message.IsReply = JsonSerializer.Deserialize<bool>(expando.IsReply);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.LastEdit)))
        {
            message.LastEdit = expando.LastEdit is null
                ? null
                : JsonSerializer.Deserialize<DateTime>(expando.LastEdit);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.LaterFlag)))
        {
            message.LaterFlag = null;
            if (!expando.Deleted && expando.LaterFlag is not null)
            {
                Models.ChannelMessageLaterFlag dbLaterFlag =
                    JsonSerializer.Deserialize<Models.DirectMessageLaterFlag>(
                        expando.LaterFlag
                    );
                LaterFlag? laterFlag = ConvertChannelMessageLaterFlag(
                    dbLaterFlag
                );
                message.LaterFlag = laterFlag;
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.Mentions)))
        {
            List<Mention> mentions = new();
            List<Models.DirectMessageMention> dbMentions =
                JsonSerializer.Deserialize<List<Models.DirectMessageMention>>(
                    expando.Mentions
                );
            message.Mentions = null;
            if (!expando.Deleted && dbMentions.Count > 0)
            {
                foreach (Models.DirectMessageMention dbMention in dbMentions)
                {
                    mentions.Add(ConvertDirectMessageMention(dbMention));
                }
                message.Mentions = mentions;
            }
        }
        if (reactionCounts is not null && reactionCounts.Count > 0)
        {
            message.Reactions = expando.Deleted
                ? null
                : ConvertDirectMessageReactionCounts(reactionCounts);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.ReplyToId)))
        {
            message.ReplyToId = null;
            if (expando.ReplyToId is not null)
            {
                message.ReplyToId = JsonSerializer.Deserialize<Guid>(
                    expando.ReplyToId
                );
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Message.SentAt)))
        {
            message.SentAt = expando.SentAt is null
                ? null
                : JsonSerializer.Deserialize<DateTime>(expando.SentAt);
            message.Draft = expando.SentAt is null;
        }
        message.Type = 2;

        return message;
    }

    public static Channel ConvertDynamicChannel(dynamic modelChannel)
    {
        var expando = DynamicUtils.ToExpando(modelChannel);
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.Channel.ChannelMembers)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.Channel.ChannelMessages)
            )
        )
        {
            throw new InvalidOperationException(
                "Cannot load a paginated collection within a collection"
            );
        }

        Channel channel = new();
        if (DynamicUtils.HasProperty(expando, nameof(Channel.Id)))
        {
            channel.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.AllowThreads)))
        {
            channel.AllowThreads = JsonSerializer.Deserialize<bool>(
                expando.AllowThreads
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.Avatar)))
        {
            if (expando.Avatar is null)
            {
                channel.Avatar = ConvertFile(null);
            }
            else
            {
                var dbAvatar = JsonSerializer.Deserialize<Models.File>(
                    (Stream)expando.Avatar
                );
                channel.Avatar = ConvertFile(dbAvatar);
            }
        }
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Channel.AllowedPostersMask)
            )
        )
        {
            channel.AllowedPostersMask = JsonSerializer.Deserialize<int>(
                expando.AllowedPostersMask
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.CreatedAt)))
        {
            channel.CreatedAt = JsonSerializer.Deserialize<DateTime>(
                expando.CreatedAt
            );
        }
        if (
            DynamicUtils.HasProperty(expando, nameof(Channel.CreatedBy))
            && expando.CreatedBy is not null
        )
        {
            channel.CreatedBy = null;
            if (expando.CreatedBy is not null)
            {
                Models.User dbUser = JsonSerializer.Deserialize<Models.User>(
                    expando.CreatedBy
                );
                channel.CreatedBy = ConvertUser(dbUser);
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.Description)))
        {
            channel.Description = null;
            if (expando.Description is not null)
            {
                channel.Description = JsonSerializer.Deserialize<string>(
                    expando.Description
                );
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.Name)))
        {
            channel.Name = JsonSerializer.Deserialize<string>(expando.Name);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.NumMembers)))
        {
            channel.NumMembers = JsonSerializer.Deserialize<int>(
                expando.NumMembers
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.Private)))
        {
            channel.Private = JsonSerializer.Deserialize<bool>(expando.Private);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.Topic)))
        {
            channel.Topic = null;
            if (expando.Topic is not null)
            {
                channel.Topic = JsonSerializer.Deserialize<string>(
                    expando.Topic
                );
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Channel.Workspace)))
        {
            var dbWorkspace = JsonSerializer.Deserialize<Models.Workspace>(
                expando.Workspace
            );
            channel.Workspace = ConvertWorkspace(dbWorkspace);
        }

        return channel;
    }

    public static Workspace ConvertDynamicWorkspace(dynamic modelWorkspace)
    {
        var expando = DynamicUtils.ToExpando(modelWorkspace);
        Workspace workspace = new();
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.Id)))
        {
            workspace.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.Avatar)))
        {
            if (expando.Avatar is null)
            {
                workspace.Avatar = DefaultAvatar;
            }
            else
            {
                Models.File dbAvatar = JsonSerializer.Deserialize<Models.File>(
                    expando.Avatar
                );
                workspace.Avatar = ConvertFile(dbAvatar);
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.CreatedAt)))
        {
            workspace.CreatedAt = JsonSerializer.Deserialize<DateTime>(
                expando.CreatedAt
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.Description)))
        {
            workspace.Description = JsonSerializer.Deserialize<string>(
                expando.Description
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.Name)))
        {
            workspace.Name = JsonSerializer.Deserialize<string>(expando.Name);
        }
        if (DynamicUtils.HasProperty(expando, nameof(Workspace.NumMembers)))
        {
            workspace.NumMembers = JsonSerializer.Deserialize<int>(
                expando.NumMembers
            );
        }

        return workspace;
    }

    public static User ConvertUser(Models.User modelUser)
    {
        User user =
            new()
            {
                Id = modelUser.Id,
                Avatar = ConvertFile(modelUser.Avatar),
                OnlineStatus = modelUser.OnlineStatus ?? DEFAULT_ONLINE_STATUS,
                OnlineStatusUntil = modelUser.OnlineStatusUntil,
                Username = modelUser.UserName,
                CreatedAt = modelUser.CreatedAt,
            };
        IncludePersonalInfo(user, modelUser);
        return user;
    }

    public static void IncludePersonalInfo(User user, Models.User modelUser)
    {
        user.PersonalInfo = new UserInfo
        {
            Email = modelUser.Email,
            EmailConfirmed = modelUser.EmailConfirmed,
            FirstName = modelUser.FirstName,
            LastName = modelUser.LastName,
            UserNotificationsPreferences = new UserNotificationsPreferences
            {
                NotifSound = modelUser.NotificationSound,
                AllowAlertsStartTimeUTC = modelUser.NotificationsAllowStartTime,
                AllowAlertsEndTimeUTC = modelUser.NotificationsAllowEndTime,
                PauseAlertsUntil = modelUser.NotificationsPauseUntil
            },
            Theme = ConvertTheme(modelUser.Theme),
            Timezone = modelUser.Timezone
        };
    }

    public static User ConvertUser(
        Models.User modelUser,
        IEnumerable<string> requestedFields
    )
    {
        User user =
            new()
            {
                Id = modelUser.Id,
                Avatar = ConvertFile(modelUser.Avatar),
                OnlineStatus = modelUser.OnlineStatus ?? DEFAULT_ONLINE_STATUS,
                OnlineStatusUntil = modelUser.OnlineStatusUntil,
                Username = modelUser.UserName,
                CreatedAt = modelUser.CreatedAt,
            };
        if (
            requestedFields.Contains(
                StringUtils.ToLowerFirstLetter(nameof(User.PersonalInfo))
            )
        )
        {
            UserInfo userInfo =
                new()
                {
                    Email = modelUser.Email,
                    EmailConfirmed = modelUser.EmailConfirmed,
                    FirstName = modelUser.FirstName,
                    LastName = modelUser.LastName,
                    Theme = ConvertTheme(modelUser.Theme),
                    Timezone = modelUser.Timezone,
                    UserNotificationsPreferences =
                        ConvertUserNotificationsPreferences(modelUser)
                };
            user.PersonalInfo = userInfo;
        }

        return user;
    }

    public static File ConvertFile(Models.File? dbFile)
    {
        return dbFile is null
            ? DefaultAvatar
            : new File
            {
                Id = dbFile.Id,
                Name = dbFile.Name,
                StoreKey = dbFile.StoreKey,
                UploadedAt = dbFile.UploadedAt
            };
    }

    public static Theme ConvertTheme(Models.Theme? theme)
    {
        return theme is null
            ? DefaultTheme
            : new Theme { Id = theme.Id, Name = theme.Name };
    }

    public static UserNotificationsPreferences ConvertUserNotificationsPreferences(
        Models.User modelUser
    )
    {
        int mask = modelUser.UserNotificationsPreferencesMask;
        return new UserNotificationsPreferences
        {
            AllMessages = (mask & 1) > 0,
            NoMessages = (mask & 2) > 0,
            Mentions = (mask & 4) > 0,
            DMs = (mask & 8) > 0,
            Replies = (mask & 16) > 0,
            ThreadWatch = (mask & 32) > 0,
            NotifSound = modelUser.NotificationSound,
            AllowAlertsStartTimeUTC = modelUser.NotificationsAllowStartTime,
            AllowAlertsEndTimeUTC = modelUser.NotificationsAllowEndTime,
            PauseAlertsUntil = modelUser.NotificationsPauseUntil
        };
    }

    public static Connection<T> ToConnection<T>(
        List<T> nodes,
        bool firstPage,
        bool lastPage
    )
        where T : INode
    {
        return new Connection<T>
        {
            TotalEdges = nodes.Count,
            Edges = nodes
                .Select(n => new ConnectionEdge<T> { Node = n, Cursor = n.Id })
                .ToList(),
            PageInfo = new PageInfo
            {
                StartCursor = nodes.Count > 0 ? nodes.First().Id : null,
                EndCursor = nodes.Count > 0 ? nodes.Last().Id : null,
                HasPreviousPage = !firstPage,
                HasNextPage = !lastPage
            }
        };
    }

    public static ChannelMember ConvertDynamicChannelMember(
        dynamic modelChannelMember,
        List<string> userFields
    )
    {
        var expando = DynamicUtils.ToExpando(modelChannelMember);
        ChannelMember member = new();
        if (DynamicUtils.HasProperty(expando, nameof(ChannelMember.Id)))
        {
            member.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (DynamicUtils.HasProperty(expando, nameof(ChannelMember.Admin)))
        {
            member.Admin = JsonSerializer.Deserialize<bool>(expando.Admin);
        }
        if (DynamicUtils.HasProperty(expando, nameof(ChannelMember.User)))
        {
            var modelUser = JsonSerializer.Deserialize<Models.User>(
                expando.User
            );
            member.User = ConvertUser(modelUser, userFields);
        }
        if (IncludeChannelMemberInfo(expando))
        {
            member.MemberInfo = ConvertChannelMemberInfo(expando);
        }
        return member;
    }

    public static ChannelMemberInfo ConvertChannelMemberInfo(dynamic expando)
    {
        ChannelMemberInfo memberInfo = new();
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMember.EnableNotifications)
            )
        )
        {
            memberInfo.EnableNotifications = JsonSerializer.Deserialize<bool>(
                expando.EnableNotifications
            );
        }

        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMember.LastViewedAt)
            )
        )
        {
            memberInfo.LastViewedAt = expando.LastViewedAt is null
                ? null
                : JsonSerializer.Deserialize<DateTime>(
                    (Stream)expando.LastViewedAt
                );
        }
        if (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMember.Starred)
            )
        )
        {
            memberInfo.Starred = JsonSerializer.Deserialize<bool>(
                expando.Starred
            );
        }
        return memberInfo;
    }

    public static WorkspaceMember ConvertDynamicWorkspaceMember(
        dynamic modelWorkspaceMember,
        List<string> userFields,
        List<string> adminFields
    )
    {
        var expando = DynamicUtils.ToExpando(modelWorkspaceMember);
        WorkspaceMember member = new();
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.Id)))
        {
            member.Id = JsonSerializer.Deserialize<Guid>(expando.Id);
        }
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.Avatar)))
        {
            if (expando.Avatar is null)
            {
                member.Avatar = DefaultAvatar;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.JoinedAt)))
        {
            member.JoinedAt = JsonSerializer.Deserialize<DateTime>(
                expando.JoinedAt
            );
        }
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.Title)))
        {
            member.Title = JsonSerializer.Deserialize<string>(expando.Title);
        }
        if (DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.User)))
        {
            Models.User modelUser = JsonSerializer.Deserialize<Models.User>(
                expando.User
            );
            member.User = ConvertUser(modelUser, userFields);
        }
        if (
            DynamicUtils.HasProperty(expando, nameof(WorkspaceMember.Workspace))
        )
        {
            Models.Workspace modelWorkspace =
                JsonSerializer.Deserialize<Models.Workspace>(expando.Workspace);
            member.Workspace = ConvertWorkspace(modelWorkspace);
        }
        if (IncludeWorkspaceMemberInfo(expando))
        {
            member.WorkspaceMemberInfo = ConvertWorkspaceMemberInfo(
                expando,
                adminFields
            );
        }

        return member;
    }

    public static WorkspaceMemberInfo ConvertWorkspaceMemberInfo(
        dynamic expandoModelWorkspaceMember,
        List<string> adminFields
    )
    {
        WorkspaceMemberInfo memberInfo = new();
        if (
            DynamicUtils.HasProperty(
                expandoModelWorkspaceMember,
                nameof(WorkspaceMemberInfo.Admin)
            )
        )
        {
            memberInfo.Admin = JsonSerializer.Deserialize<bool>(
                expandoModelWorkspaceMember.Admin
            );
        }
        if (
            DynamicUtils.HasProperty(
                expandoModelWorkspaceMember,
                nameof(WorkspaceMemberInfo.Owner)
            )
        )
        {
            memberInfo.Owner = JsonSerializer.Deserialize<bool>(
                expandoModelWorkspaceMember.Owner
            );
        }
        if (
            DynamicUtils.HasProperty(
                expandoModelWorkspaceMember,
                nameof(WorkspaceMemberInfo.WorkspaceAdminPermissions)
            )
            && expandoModelWorkspaceMember.WorkspaceAdminPermissions is not null
        )
        {
            Models.WorkspaceAdminPermissions modelPermissions =
                JsonSerializer.Deserialize<Models.WorkspaceAdminPermissions>(
                    expandoModelWorkspaceMember.WorkspaceAdminPermissions
                );
            memberInfo.WorkspaceAdminPermissions =
                ConvertWorkspaceAdminPermissions(modelPermissions, adminFields);
        }

        return memberInfo;
    }

    public static WorkspaceAdminPermissions ConvertWorkspaceAdminPermissions(
        Models.WorkspaceAdminPermissions modelPermissions,
        List<string> adminFields
    )
    {
        int mask = modelPermissions.WorkspaceAdminPermissionsMask;
        return new WorkspaceAdminPermissions
        {
            Admin = ConvertUser(modelPermissions.Admin, adminFields),
            All = (mask & 1) > 0,
            Invite = (mask & 2) > 0,
            Kick = (mask & 4) > 0,
            AdminGrant = (mask & 8) > 0,
            AdminRevoke = (mask & 16) > 0,
            GrantAdminPermissions = (mask & 32) > 0,
            RevokeAdminPermissions = (mask & 64) > 0,
            EditMessages = (mask & 128) > 0,
            DeleteMessages = (mask & 256) > 0
        };
    }

    public static Workspace ConvertWorkspace(Models.Workspace modelWorkspace)
    {
        return new Workspace
        {
            Id = modelWorkspace.Id,
            Avatar = ConvertFile(modelWorkspace.Avatar),
            CreatedAt = modelWorkspace.CreatedAt,
            Description = modelWorkspace.Description,
            Name = modelWorkspace.Name,
            NumMembers = modelWorkspace.NumMembers
        };
    }

    public static Channel ConvertChannel(
        Models.Channel modelChannel,
        bool skipWorkspace = false
    )
    {
        var channel = new Channel
        {
            Id = modelChannel.Id,
            AllowThreads = modelChannel.AllowThreads,
            AllowedPostersMask = modelChannel.AllowedPostersMask,
            Avatar = ConvertFile(modelChannel.Avatar),
            CreatedAt = modelChannel.CreatedAt,
            Description = modelChannel.Description ?? "",
            Name = modelChannel.Name,
            NumMembers = modelChannel.NumMembers,
            Private = modelChannel.Private,
            Topic = modelChannel.Topic ?? "",
        };
        if (!skipWorkspace)
        {
            channel.Workspace = ConvertWorkspace(modelChannel.Workspace);
        }
        return channel;
    }

    private static bool IncludeWorkspaceMemberInfo(dynamic expando)
    {
        return (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.Admin)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.WorkspaceAdminPermissions)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.Owner)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.NotificationsAllowTimeStart)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.NotificationsAllTimeEnd)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.NotificationSound)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.WorkspaceMember.Theme)
            )
        );
    }

    private static bool IncludeChannelMemberInfo(dynamic expando)
    {
        return (
            DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMember.EnableNotifications)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMember.LastViewedAt)
            )
            || DynamicUtils.HasProperty(
                expando,
                nameof(Models.ChannelMember.Starred)
            )
        );
    }
}
