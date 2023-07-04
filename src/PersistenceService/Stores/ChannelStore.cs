using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Constants;

namespace PersistenceService.Stores;

public class ChannelStore : Store
{
    public ChannelStore(ApplicationDbContext context)
        : base(context) { }

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
                .Where(cm => cm.UserId == userId && cm.ChannelId == channelId)
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
        List<Guid>? mentionedPeople,
        Guid? threadId,
        Guid? messageRepliedToId,
        Guid? personRepliedToId,
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
                            && cm.ThreadId == threadId
                            && cm.UserId == personRepliedToId
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
                .Where(cm => cm.UserId == userId)
                .Where(cm => cm.ChannelId == threadChannelId)
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
        Guid workspaceId = _context.Channels
            .First(c => c.Id == channelId)
            .WorkspaceId;
        bool allUsersAreWorkspaceMembers =
            _context.WorkspaceMembers
                .Where(member => member.WorkspaceId == workspaceId)
                .Where(member => userIds.Contains(member.UserId))
                .Count() == userIds.Count;
        bool allUsersNotChannelMembers =
            _context.ChannelMembers
                .Where(member => userIds.Contains(member.UserId))
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
            cm => cm.UserId == adminId && cm.ChannelId == channelId
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

    public async Task<List<Channel>> InsertTestChannels(int numTestChannels)
    {
        string email = UserStore.GenerateTestEmail(10);
        string username =
            "tccreator-uname" + ChannelStore.GenerateRandomString(15);
        User channelCreator = new User
        {
            FirstName = "test-ccreator-fname",
            LastName = "test-channel-creator-lname",
            Timezone = UserStore.timezones[0].Id,
            UserName = username,
            Email = email,
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = username.ToUpper(),
            PhoneNumber = "1-234-456-7890",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                UserStore.testPassword,
                4
            ),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        Workspace channelWorkspace = new Workspace
        {
            Description = "test-description",
            Name = "test-workspace-channels-name"
        };
        List<Channel> channels = new List<Channel>();
        for (int i = 0; i < numTestChannels; i++)
        {
            channels.Add(
                new Channel
                {
                    CreatedBy = channelCreator,
                    Description = "test-description",
                    Name = "test-channel-name-" + i.ToString(),
                    Workspace = channelWorkspace
                }
            );
        }

        _context.AddRange(channels);
        await _context.SaveChangesAsync();

        return channels;
    }
}
