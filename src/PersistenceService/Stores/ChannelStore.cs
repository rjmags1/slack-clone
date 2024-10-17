using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Constants;
using PersistenceService.Utils;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using GraphQLTypes = Common.SlackCloneGraphQL.Types;
using Dapper;

namespace PersistenceService.Stores;

public class ChannelStore : Store
{
    public ChannelStore(ApplicationDbContext context)
        : base(context) { }

    public async Task<GraphQLTypes.Channel> LoadChannel(Guid channelId)
    {
        var sqlBuilder = new List<string>
        {
            $"WITH channel AS (",
            $"SELECT * FROM {wdq("Channels")} WHERE {wdq("Id")} = @ChannelId",
            ")",
            $"SELECT * FROM",
            $"channel LEFT JOIN {wdq("Files")} ON {wdq("Files")}.{wdq("Id")} = channel.{wdq("AvatarId")}",
            $"LEFT JOIN {wdq("Workspaces")} ON {wdq("Workspaces")}.{wdq("Id")} = channels.{wdq("WorkspaceId")}",
            $"LEFT JOIN {wdq("AspNetUsers")} ON",
            $"{wdq("AspNetUsers")}.{wdq("Id")} = channel.{wdq("CreatedById")}"
        };

        var sql = string.Join("\n", sqlBuilder);
        var param = new { ChannelId = channelId };
        var conn = _context.GetConnection();

        var channel = (
            await conn.QueryAsync<
                Models.Channel,
                GraphQLTypes.File,
                GraphQLTypes.Workspace,
                GraphQLTypes.User,
                GraphQLTypes.Channel
            >(
                sql: sql,
                param: param,
                map: (channel, avatar, workspace, user) =>
                {
                    return new GraphQLTypes.Channel
                    {
                        Id = channel.Id,
                        AllowThreads = channel.AllowThreads,
                        AllowedPostersMask = channel.AllowedPostersMask,
                        Avatar = avatar,
                        CreatedAt = channel.CreatedAt,
                        CreatedBy = user,
                        Description = channel.Description,
                        Name = channel.Name,
                        NumMembers = channel.NumMembers,
                        Private = channel.Private,
                        Topic = channel.Topic,
                        Workspace = workspace
                    };
                }
            )
        ).First();

        return channel;
    }

    public async Task<(
        List<GraphQLTypes.Message> messages,
        bool lastPage
    )> LoadChannelMessages(
        Guid channelId,
        List<string> dbCols,
        int first,
        Guid? after = null
    )
    {
        var joinCols = new string[] { "Mentions", "Reactions", "Files" };
        dbCols.RemoveAll(c => joinCols.Contains(c));
        if (!dbCols.Contains("SentAt"))
        {
            dbCols.Add("SentAt");
        }

        List<string> sqlBuilder = new();
        if (after is not null)
        {
            sqlBuilder.Add("WITH after AS (");
            sqlBuilder.Add(
                $"SELECT {wdq("SentAt")} FROM {wdq("ChannelMessages")}"
            );
            sqlBuilder.Add($"WHERE {wdq("Id")} = @AfterId");
            sqlBuilder.Add("),\n");
        }
        else
        {
            sqlBuilder.Add("WITH");
        }

        sqlBuilder.Add("messages AS (");
        sqlBuilder.Add("SELECT");
        sqlBuilder.AddRange(
            dbCols.Select(
                (c, i) =>
                    i == dbCols.Count - 1
                        ? $"{wdq("ChannelMessages")}.{wdq(c)}"
                        : $"{wdq("ChannelMessages")}.{wdq(c)},"
            )
        );
        sqlBuilder.Add($"FROM {wdq("ChannelMessages")}");
        sqlBuilder.Add(
            $"WHERE {wdq("ChannelId")} = @ChannelId AND NOT {wdq("Deleted")} AND {wdq("SentAt")} IS NOT NULL"
        );
        if (after is not null)
        {
            sqlBuilder.Add(
                $"AND {wdq("SentAt")} < (SELECT {wdq("SentAt")} FROM after)"
            );
        }
        sqlBuilder.Add($"ORDER BY {wdq("SentAt")} DESC");
        sqlBuilder.Add("LIMIT @First");
        sqlBuilder.Add(")\n");

        sqlBuilder.Add($"SELECT messages.*,");
        sqlBuilder.Add(
            @$"{wdq("ChannelMessageMentions")}.{wdq("Id")}, 
                {wdq("ChannelMessageMentions")}.{wdq("MentionedId")},"
        );
        sqlBuilder.Add(
            @$"{wdq("ChannelMessageReactions")}.{wdq("Id")}, 
                {wdq("ChannelMessageReactions")}.{wdq("Emoji")}, 
                {wdq("ChannelMessageReactions")}.{wdq("UserId")},"
        );
        sqlBuilder.Add(
            @$"{wdq("Files")}.{wdq("Id")},
                {wdq("Files")}.{wdq("Name")},
                {wdq("Files")}.{wdq("StoreKey")}"
        );
        sqlBuilder.Add("FROM messages");
        sqlBuilder.Add(
            @$"LEFT JOIN {wdq("ChannelMessageMentions")} ON 
                {wdq("ChannelMessageMentions")}.{wdq("ChannelMessageId")} = messages.{wdq("Id")}"
        );
        sqlBuilder.Add(
            @$"LEFT JOIN {wdq("ChannelMessageReactions")} ON 
                {wdq("ChannelMessageReactions")}.{wdq("ChannelMessageId")} = messages.{wdq("Id")}"
        );
        sqlBuilder.Add(
            @$"LEFT JOIN {wdq("Files")} ON 
                {wdq("Files")}.{wdq("ChannelMessageId")} = messages.{wdq("Id")}"
        );
        sqlBuilder.Add($"ORDER BY messages.{wdq("SentAt")} DESC");

        var sql = string.Join("\n", sqlBuilder);
        var param = new
        {
            ChannelId = channelId,
            First = first + 1,
            AfterId = after
        };
        var conn = _context.GetConnection();
        List<GraphQLTypes.Message> messages = (
            await conn.QueryAsync<
                Models.ChannelMessage,
                GraphQLTypes.Mention,
                GraphQLTypes.Reaction,
                GraphQLTypes.File,
                GraphQLTypes.Message
            >(
                sql: sql,
                param: param,
                map: (messageModel, mention, reaction, file) =>
                {
                    var message = new GraphQLTypes.Message
                    {
                        Id = messageModel.Id,
                        User = new GraphQLTypes.User
                        {
                            Id = messageModel.UserId,
                        },
                        Content = messageModel.Content,
                        CreatedAt = messageModel.CreatedAt,
                        Draft = messageModel.SentAt is null,
                        LastEdit = messageModel.LastEdit,
                        Files = new() { file },
                        Group = new GraphQLTypes.Group
                        {
                            Id = messageModel.ChannelId
                        },
                        IsReply = messageModel.IsReply,
                        LaterFlag = messageModel.LaterFlagId is null
                            ? null
                            : new GraphQLTypes.LaterFlag
                            {
                                Id = (Guid)messageModel.LaterFlagId
                            },
                        Mentions = new() { mention },
                        Reactions = new() { reaction },
                        ReplyToId = messageModel.ReplyToId,
                        SentAt = messageModel.SentAt,
                        ThreadId = messageModel.ThreadId,
                        Type = "ChannelMessage"
                    };

                    return message;
                }
            )
        ).ToList();

        messages = messages
            .GroupBy(m => m.Id)
            .Select(g =>
            {
                var message = g.First();
                var files = g.Select(g => g.Files?.First())
                    .Where(f => f is not null)
                    .ToList();
                var mentions = g.Select(g => g.Mentions?.First())
                    .Where(m => m is not null)
                    .ToList();
                var reactions = g.Select(g => g.Reactions?.First())
                    .Where(r => r is not null)
                    .ToList();
                message.Files = files as List<GraphQLTypes.File>;
                message.Mentions = mentions as List<GraphQLTypes.Mention>;
                message.Reactions = reactions as List<GraphQLTypes.Reaction>;
                return message;
            })
            .ToList();

        var lastPage = messages.Count <= first;
        if (!lastPage)
        {
            messages.RemoveAt(messages.Count - 1);
        }

        return (messages, lastPage);
    }

    public async Task<(
        List<GraphQLTypes.ChannelMember> members,
        bool lastPage
    )> LoadChannelMembers(
        int first,
        List<string> dbCols,
        Guid channelId,
        Guid? after = null
    )
    {
        List<string> sqlBuilder = new();
        if (after is not null)
        {
            sqlBuilder.Add("WITH afterId AS (");
            sqlBuilder.Add(
                $"SELECT {wdq("UserId")} FROM {wdq("ChannelMembers")}"
            );
            sqlBuilder.Add($"WHERE {wdq("Id")} = @AfterId");
            sqlBuilder.Add("),\n");

            sqlBuilder.Add("afterName AS (");
            sqlBuilder.Add($"SELECT {wdq("AspNetUsers")}.{wdq("UserName")}");
            sqlBuilder.Add($"FROM {wdq("AspNetUsers")}");
            sqlBuilder.Add(
                $"WHERE {wdq("Id")} = (SELECT {wdq("UserId")} FROM afterId)"
            );
            sqlBuilder.Add("),\n");
        }
        else
        {
            sqlBuilder.Add("WITH");
        }
        sqlBuilder.Add($"members AS (");
        sqlBuilder.Add(
            $"SELECT * FROM {wdq("ChannelMembers")} WHERE {wdq("ChannelId")} = @ChannelId"
        );
        sqlBuilder.Add(")\n");

        sqlBuilder.Add("SELECT");
        sqlBuilder.AddRange(dbCols.Select((c, i) => $"members.{wdq(c)},"));
        sqlBuilder.Add(
            $"{wdq("AspNetUsers")}.{wdq("Id")}, {wdq("AspNetUsers")}.{wdq("UserName")} AS {wdq("Username")}"
        );
        sqlBuilder.Add($"FROM members INNER JOIN {wdq("AspNetUsers")}");
        sqlBuilder.Add(
            $"ON members.{wdq("UserId")} = {wdq("AspNetUsers")}.{wdq("Id")}"
        );
        if (after is not null)
        {
            sqlBuilder.Add(
                $"WHERE {wdq("AspNetUsers")}.{wdq("UserName")} > (SELECT {wdq("UserName")} FROM afterName)"
            );
        }
        sqlBuilder.Add($"ORDER BY {wdq("Username")}");
        sqlBuilder.Add("LIMIT @First;");

        var sql = string.Join("\n", sqlBuilder);
        var param = new
        {
            AfterId = after,
            ChannelId = channelId,
            First = first + 1
        };
        var conn = _context.GetConnection();

        var members = (
            await conn.QueryAsync<
                GraphQLTypes.ChannelMember,
                GraphQLTypes.User,
                GraphQLTypes.ChannelMember
            >(
                sql: sql,
                param: param,
                map: (member, user) =>
                {
                    member.User = user;
                    return member;
                }
            )
        ).ToList();

        var lastPage = members.Count <= first;
        if (!lastPage)
        {
            members.RemoveAt(members.Count - 1);
        }

        return (members, lastPage);
    }

    public async Task<ChannelMessageReaction> InsertMessageReaction(
        Guid channelMessageId,
        Guid userId,
        string emoji
    )
    {
        if (!EmojiUtils.IsEmoji(emoji))
        {
            throw new ArgumentException("Invalid arguments");
        }
        ChannelMessage? channelMessage = _context.ChannelMessages
            .Where(cm => cm.Id == channelMessageId)
            .FirstOrDefault();
        if (channelMessage is null)
        {
            throw new ArgumentException("Invalid arguments");
        }
        bool isMember =
            _context.ChannelMembers
                .Where(
                    cm =>
                        cm.ChannelId == channelMessage.ChannelId
                        && cm.UserId == userId
                )
                .Count() == 1;
        if (!isMember)
        {
            throw new ArgumentException("Invalid arguments");
        }

        ChannelMessageReaction reaction = new ChannelMessageReaction
        {
            ChannelMessageId = channelMessageId,
            Emoji = emoji,
            UserId = userId
        };
        _context.Add(reaction);

        await _context.SaveChangesAsync();

        return reaction;
    }

    public async Task<ChannelMessageNotification> InsertReplyNotification(
        ChannelMessageReply replyRecord
    )
    {
        bool recordExists =
            _context.ChannelMessageReplies
                .Where(r => r.Id == replyRecord.Id)
                .Count() == 1;
        if (!recordExists)
        {
            throw new ArgumentException("Invalid arguments");
        }

        ChannelMessageNotification notification = new ChannelMessageNotification
        {
            ChannelMessageId = replyRecord.ChannelMessageId,
            ChannelMessageNotificationType = MaskEnumDefs.NotificationTypes[
                MaskEnumDefs.REPLY
            ],
            UserId = replyRecord.RepliedToId
        };
        _context.Add(notification);

        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<
        List<ChannelMessageNotification>
    > InsertMentionNotifications(List<ChannelMessageMention> mentionRecords)
    {
        List<Guid> mentionRecordIds = mentionRecords.Select(m => m.Id).ToList();
        bool validArgs =
            _context.ChannelMessageMentions
                .Where(cm => mentionRecordIds.Contains(cm.Id))
                .Count() == mentionRecordIds.Count;
        if (!validArgs)
        {
            throw new ArgumentException("Invalid arguments");
        }

        List<ChannelMessageNotification> notifications =
            new List<ChannelMessageNotification>();
        foreach (ChannelMessageMention mentionRecord in mentionRecords)
        {
            notifications.Add(
                new ChannelMessageNotification
                {
                    ChannelMessageId = mentionRecord.ChannelMessageId,
                    ChannelMessageNotificationType =
                        MaskEnumDefs.NotificationTypes[MaskEnumDefs.MENTION],
                    UserId = mentionRecord.MentionedId
                }
            );
        }
        _context.AddRange(notifications);

        await _context.SaveChangesAsync();

        return notifications;
    }

    public async Task<ChannelMessageNotification> InsertReactionNotification(
        ChannelMessageReaction reactionRecord
    )
    {
        bool validArgs =
            _context.ChannelMessageReactions
                .Where(cmr => cmr.Id == reactionRecord.Id)
                .Count() == 1;
        if (!validArgs)
        {
            throw new ArgumentException("Invalid arguments");
        }

        ChannelMessageNotification notification = new ChannelMessageNotification
        {
            ChannelMessageId = reactionRecord.ChannelMessageId,
            ChannelMessageNotificationType = MaskEnumDefs.NotificationTypes[
                MaskEnumDefs.REACTION
            ],
            UserId = reactionRecord.ChannelMessage.UserId
        };
        _context.Add(notification);

        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<
        List<ChannelMessageNotification>
    > InsertThreadWatchNotifications(
        List<ThreadWatch> threadWatches,
        Guid channelMessageId
    )
    {
        List<Guid> threadWatchIds = threadWatches.Select(tw => tw.Id).ToList();
        List<Guid> threadIds = threadWatches
            .Select(tw => tw.ThreadId)
            .Distinct()
            .ToList();
        bool validThreadWatches =
            threadIds.Count() == 1
            && _context.ThreadWatches
                .Where(tw => threadWatchIds.Contains(tw.Id))
                .Count() == threadWatches.Count();
        if (!validThreadWatches)
        {
            throw new ArgumentException("Invalid arguments");
        }
        bool messageInThread =
            threadIds[0]
            == _context.ChannelMessages
                .Where(cm => cm.Id == channelMessageId)
                .Select(cm => cm.ThreadId)
                .FirstOrDefault();
        if (!messageInThread)
        {
            throw new ArgumentException("Invalid arguments");
        }

        List<ChannelMessageNotification> notifications =
            new List<ChannelMessageNotification>();
        foreach (ThreadWatch threadWatch in threadWatches)
        {
            notifications.Add(
                new ChannelMessageNotification
                {
                    ChannelMessageId = channelMessageId,
                    ChannelMessageNotificationType =
                        MaskEnumDefs.NotificationTypes[
                            MaskEnumDefs.THREAD_WATCH
                        ],
                    UserId = threadWatch.UserId
                }
            );
        }
        _context.AddRange(notifications);

        await _context.SaveChangesAsync();

        return notifications;
    }

    public async Task<ChannelMessageLaterFlag> InsertChannelMessageLaterFlag(
        Guid channelMessageId,
        Guid userId
    )
    {
        ChannelMessage? channelMessage = _context.ChannelMessages
            .Where(cm => cm.Id == channelMessageId)
            .FirstOrDefault();
        if (channelMessage is null)
        {
            throw new ArgumentException("Invalid arguments");
        }

        Guid channelId = channelMessage.ChannelId;
        bool isChannelMember =
            _context.ChannelMembers
                .Where(cm => cm.ChannelId == channelId && cm.UserId == userId)
                .Count() == 1;
        if (!isChannelMember)
        {
            throw new ArgumentException("Invalid arguments");
        }

        Guid workspaceId = channelMessage.Channel.WorkspaceId;
        ChannelMessageLaterFlag laterFlag = new ChannelMessageLaterFlag
        {
            ChannelId = channelId,
            ChannelMessageId = channelMessageId,
            UserId = userId,
            WorkspaceId = workspaceId
        };
        _context.Add(laterFlag);
        channelMessage.LaterFlag = laterFlag;

        await _context.SaveChangesAsync();

        return laterFlag;
    }

    public async Task<ChannelMessage> InsertChannelMessage(
        Guid channelId,
        string content,
        Guid userId,
        List<Guid>? mentionedPeople = null,
        Guid? threadId = null,
        Guid? messageRepliedToId = null,
        Guid? personRepliedToId = null,
        bool draft = false
    )
    {
        bool validMessage =
            content.Length > 0
            && _context.ChannelMembers
                .Where(cm => cm.ChannelId == channelId && cm.UserId == userId)
                .Count() == 1
            && (
                threadId is null
                || _context.Threads
                    .Where(t => t.Id == threadId && t.ChannelId == channelId)
                    .Count() == 1
            );
        if (!validMessage)
        {
            throw new InvalidOperationException("Could not add new message");
        }

        int specifiedReplyArgs = (
            new List<Guid?> { threadId, messageRepliedToId, personRepliedToId }
        ).Count(id => id is not null);

        bool reply = specifiedReplyArgs == 3;
        bool topLevelMessage = specifiedReplyArgs == 0;
        if (!reply && !topLevelMessage)
        {
            throw new ArgumentException("Invalid arguments");
        }

        bool needRecordMentions =
            !draft && mentionedPeople is not null && mentionedPeople.Count > 0;
        if (needRecordMentions)
        {
            bool mentionedAllMembers =
                _context.ChannelMembers
                    .Where(
                        cm =>
                            cm.ChannelId == channelId
                            && mentionedPeople!.Contains(cm.UserId)
                    )
                    .Count() == mentionedPeople!.Count;
            if (!mentionedAllMembers)
            {
                throw new ArgumentException("Invalid arguments");
            }
        }

        if (reply)
        {
            bool validReply =
                _context.ChannelMessages
                    .Where(
                        cm =>
                            cm.Id == messageRepliedToId
                            && cm.UserId == personRepliedToId
                            && cm.ThreadId == threadId
                    )
                    .Count() == 1;
            if (!validReply)
            {
                throw new InvalidOperationException("Could not add reply");
            }
        }

        ChannelMessage message = new ChannelMessage
        {
            ChannelId = channelId,
            Content = content,
            ThreadId = threadId,
            UserId = userId,
            SentAt = draft ? null : DateTime.Now,
            IsReply = reply,
            ReplyToId = messageRepliedToId
        };
        _context.Add(message);

        if (reply)
        {
            ChannelMessageReply replyRecord = new ChannelMessageReply
            {
                ChannelMessage = message,
                MessageRepliedToId = (Guid)messageRepliedToId!,
                RepliedToId = (Guid)personRepliedToId!,
                ThreadId = (Guid)threadId!,
                ReplierId = userId
            };
            _context.Add(replyRecord);
            _context.Threads.Where(t => t.Id == threadId).First().NumMessages +=
                1;
        }

        if (needRecordMentions)
        {
            foreach (Guid mentionedId in mentionedPeople!)
            {
                _context.Add(
                    new ChannelMessageMention
                    {
                        ChannelMessage = message,
                        MentionedId = mentionedId,
                        MentionerId = userId
                    }
                );
            }
        }

        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<ThreadWatch> InsertThreadWatch(Guid userId, Guid threadId)
    {
        try
        {
            Guid threadChannelId = _context.Threads
                .Where(t => t.Id == threadId)
                .First()
                .ChannelId;
            ChannelMember userInThreadChannel = _context.ChannelMembers
                .Where(cm => cm.ChannelId == threadChannelId)
                .Where(cm => cm.UserId == userId)
                .First();
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid arguments");
        }

        ThreadWatch threadWatch = new ThreadWatch
        {
            ThreadId = threadId,
            UserId = userId
        };
        _context.Add(threadWatch);

        await _context.SaveChangesAsync();

        return threadWatch;
    }

    public async Task<Models.Thread> InsertThread(
        Guid channelId,
        Guid repliedToId,
        ChannelMessage reply
    )
    {
        ChannelMessage repliedTo;
        Guid workspaceId;
        try
        {
            repliedTo = _context.ChannelMessages
                .Where(cm => cm.Id == repliedToId)
                .Where(cm => cm.ChannelId == channelId)
                .First();
            workspaceId = _context.Channels
                .Where(c => c.Id == channelId)
                .First()
                .WorkspaceId;
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid arguments");
        }

        bool replyInChannel = reply.ChannelId == channelId;
        bool replyUserSpecified = reply.UserId != default(Guid);
        bool replyContentSpecified = reply.Content?.Length > 0;
        if (!replyInChannel || !replyUserSpecified || !replyContentSpecified)
        {
            throw new ArgumentException("Invalid arguments");
        }

        Models.Thread thread = new Models.Thread
        {
            ChannelId = channelId,
            FirstMessageId = repliedToId,
            WorkspaceId = workspaceId
        };
        _context.Add(thread);

        repliedTo.Thread = thread;

        _context.Attach(reply);
        reply.Thread = thread;

        ChannelMessageReply replyEntry = new ChannelMessageReply
        {
            ChannelMessage = reply,
            MessageRepliedToId = repliedToId,
            RepliedToId = repliedTo.UserId,
            ReplierId = reply.UserId,
            Thread = thread
        };
        _context.Add(replyEntry);

        await _context.SaveChangesAsync();

        return thread;
    }

    public async Task<List<ChannelMember>> InsertChannelMembers(
        Guid channelId,
        Guid workspaceId,
        List<Guid> userIds
    )
    {
        Channel? channel = _context.Channels.FirstOrDefault(
            c => c.Id == channelId
        );
        bool allUsersAreWorkspaceMembers =
            channel is not null
            && _context.WorkspaceMembers
                .Where(member => member.WorkspaceId == channel.WorkspaceId)
                .Where(member => userIds.Contains(member.UserId))
                .Count() == userIds.Count;
        bool allUsersNotChannelMembers =
            _context.ChannelMembers
                .Where(
                    member =>
                        member.ChannelId == channelId
                        && userIds.Contains(member.UserId)
                )
                .Count() == 0;
        if (!allUsersAreWorkspaceMembers || !allUsersNotChannelMembers)
        {
            throw new InvalidOperationException(
                "Users must be workspace members and not already channel members"
            );
        }

        List<ChannelMember> channelMembers = new List<ChannelMember>();
        foreach (Guid userId in userIds)
        {
            channelMembers.Add(
                new ChannelMember
                {
                    ChannelId = channelId,
                    UserId = userId,
                    WorkspaceId = workspaceId
                }
            );
        }

        _context.AddRange(channelMembers);
        channel!.NumMembers += channelMembers.Count;
        await _context.SaveChangesAsync();

        return channelMembers;
    }

    public async Task<ChannelInvite> InsertChannelInvite(
        Guid channelId,
        Guid adminId,
        Guid userId
    )
    {
        ChannelMember adminMembership = _context.ChannelMembers.First(
            cm => cm.ChannelId == channelId && cm.UserId == adminId
        );
        if (!adminMembership.Admin)
        {
            throw new InvalidOperationException(
                "Only channel admins may send invites"
            );
        }

        bool invitedExists =
            _context.Users.Where(u => u.Id == userId).Count() == 1;
        if (!invitedExists)
        {
            throw new InvalidOperationException("Could not invite user");
        }

        Guid workspaceId = _context.Channels
            .First(c => c.Id == channelId)
            .WorkspaceId;
        bool invitedIsWorkspaceMember =
            _context.WorkspaceMembers
                .Where(wm => wm.UserId == userId)
                .Where(wm => wm.WorkspaceId == workspaceId)
                .Count() == 1;
        bool invitedAlreadyChannelMember =
            _context.ChannelMembers
                .Where(cm => cm.ChannelId == channelId)
                .Where(cm => cm.UserId == userId)
                .Count() == 1;
        if (!invitedIsWorkspaceMember || invitedAlreadyChannelMember)
        {
            throw new InvalidOperationException("Could not invite user");
        }

        ChannelInvite invite = new ChannelInvite
        {
            AdminId = adminId,
            ChannelId = channelId,
            UserId = userId,
            WorkspaceId = workspaceId
        };
        _context.Add(invite);
        await _context.SaveChangesAsync();

        return invite;
    }

    public async Task<List<Channel>> InsertChannels(List<Channel> channels)
    {
        _context.AddRange(channels);
        await _context.SaveChangesAsync();
        return channels;
    }

    public async Task<(
        List<GraphQLTypes.Channel> channels,
        bool lastPage
    )> LoadChannels(
        Guid workspaceId,
        Guid userId,
        int first,
        List<string> cols,
        Guid? after = null
    )
    {
        string[] joinCols = { "AvatarId", "CreatedById", "WorkspaceId" };
        if (cols.Any(c => joinCols.Contains(c)))
        {
            foreach (var c in joinCols)
            {
                cols.Remove(c);
                cols.Add(c);
            }
        }

        List<string> sqlBuilder = new();
        if (after is not null)
        {
            sqlBuilder.Add("WITH _after AS (");
            sqlBuilder.Add($"SELECT {wdq("Name")} FROM {wdq("Channels")}");
            sqlBuilder.Add($"WHERE {wdq("Id")} = @AfterId");
            sqlBuilder.Add("),\n");
        }
        else
        {
            sqlBuilder.Add("WITH");
        }
        sqlBuilder.Add("_memberships AS (");
        sqlBuilder.Add(
            $"SELECT {wdq("ChannelId")}, {wdq("LastViewedAt")}, {wdq("JoinedAt")}"
        );
        sqlBuilder.Add($"FROM {wdq("ChannelMembers")} WHERE");
        sqlBuilder.Add($"{wdq("WorkspaceId")} = @WorkspaceId AND");
        sqlBuilder.Add($"{wdq("UserId")} = @UserId");
        sqlBuilder.Add("),\n");

        sqlBuilder.Add("_channels AS (");
        sqlBuilder.Add($"SELECT {wdq("Channels")}.*, ");
        sqlBuilder.Add(
            $"_memberships.{wdq("LastViewedAt")}, _memberships.{wdq("JoinedAt")}"
        );
        sqlBuilder.Add("FROM");
        sqlBuilder.Add($"_memberships INNER JOIN {wdq("Channels")} ON");
        sqlBuilder.Add(
            $"{wdq("Channels")}.{wdq("Id")} = _memberships.{wdq("ChannelId")}"
        );
        if (after is not null)
        {
            sqlBuilder.Add(
                $"WHERE {wdq("Channels")}.{wdq("Name")} > (SELECT {wdq("Name")} FROM _after)"
            );
        }
        sqlBuilder.Add("LIMIT @First");
        sqlBuilder.Add(")\n");

        sqlBuilder.Add(
            $"SELECT _channels.*, {wdq("Files")}.*, {wdq("AspNetUsers")}.*, {wdq("Workspaces")}.* FROM _channels"
        );
        sqlBuilder.Add(
            $"LEFT JOIN {wdq("Files")} ON {wdq("Files")}.{wdq("Id")} = _channels.{wdq("AvatarId")}"
        );
        sqlBuilder.Add(
            $"LEFT JOIN {wdq("AspNetUsers")} ON {wdq("AspNetUsers")}.{wdq("Id")} = _channels.{wdq("CreatedById")}"
        );
        sqlBuilder.Add(
            $"LEFT JOIN {wdq("Workspaces")} ON {wdq("Workspaces")}.{wdq("Id")} = _channels.{wdq("WorkspaceId")}"
        );
        sqlBuilder.Add($"ORDER BY _channels.{wdq("Name")};");

        var sql = string.Join("\n", sqlBuilder);

        var conn = _context.GetConnection();
        var parameters = new
        {
            UserId = userId,
            AfterId = after,
            WorkspaceId = workspaceId,
            First = first + 1
        };
        var channels = (
            await conn.QueryAsync<
                Models.Channel,
                GraphQLTypes.File,
                GraphQLTypes.User,
                GraphQLTypes.Workspace,
                GraphQLTypes.Channel
            >(
                sql: sql,
                param: parameters,
                map: (channel, avatar, user, workspace) =>
                {
                    return new GraphQLTypes.Channel
                    {
                        Id = channel.Id,
                        AllowThreads = channel.AllowThreads,
                        AllowedPostersMask = channel.AllowedPostersMask,
                        Avatar = avatar,
                        CreatedAt = channel.CreatedAt,
                        CreatedBy = user,
                        Description = channel.Description,
                        Name = channel.Name,
                        NumMembers = channel.NumMembers,
                        Private = channel.Private,
                        Topic = channel.Topic,
                        Workspace = workspace
                    };
                }
            )
        ).ToList();
        var lastPage = channels.Count <= first;
        if (!lastPage)
        {
            channels.RemoveAt(channels.Count - 1);
        }

        return (channels, lastPage);
    }
}
