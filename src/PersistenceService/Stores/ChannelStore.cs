using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Constants;
using PersistenceService.Utils;

namespace PersistenceService.Stores;

public class ChannelStore : Store
{
    public ChannelStore(ApplicationDbContext context)
        : base(context) { }

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
            Draft = draft,
            SentAt = draft ? null : DateTime.Now,
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
                new ChannelMember { ChannelId = channelId, UserId = userId }
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
}
