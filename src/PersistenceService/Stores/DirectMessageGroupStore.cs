using PersistenceService.Constants;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;
using PersistenceService.Utils;
using PersistenceService.Utils.GraphQL;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Stores;

public class DirectMessageReactionCount
{
#pragma warning disable CS8618
    public Guid DirectMessageId { get; set; }
    public int Count { get; set; }
    public string Emoji { get; set; }
#pragma warning restore CS8618
    public DirectMessageReaction? UserReaction { get; set; }
}

public class DirectMessageGroupStore : Store
{
    public DirectMessageGroupStore(ApplicationDbContext context)
        : base(context) { }

    public async Task<DirectMessageGroup> LoadDirectMessageGroup(Guid groupId)
    {
        var query =
            from g in _context
                .Set<DirectMessageGroup>()
                .Where(g => g.Id == groupId)
            from m in _context
                .Set<DirectMessageGroupMember>()
                .Where(m => m.DirectMessageGroupId == g.Id)
            from u in _context.Set<User>().Where(u => u.Id == m.UserId)
            select new
            {
                g,
                m,
                u
            };
        var queryResult = await query.ToListAsync();
        var group = queryResult.First().g;
        foreach (var row in queryResult)
        {
            row.m.User = row.u;
        }

        return group;
    }

    public async Task<
        List<DirectMessageGroupMember>
    > InsertDirectMessageGroupMembers(
        Guid directMessageGroupId,
        Guid workspaceId,
        List<Guid> userIds
    )
    {
        DirectMessageGroup? group = _context.DirectMessageGroups.FirstOrDefault(
            dmg => dmg.Id == directMessageGroupId
        );
        bool allUsersAreWorkspaceMembers =
            group is not null
            && _context.WorkspaceMembers
                .Where(member => member.WorkspaceId == group.WorkspaceId)
                .Where(member => userIds.Contains(member.UserId))
                .Count() == userIds.Count;
        bool allUsersNotGroupMembers =
            _context.DirectMessageGroupMembers
                .Where(member => userIds.Contains(member.UserId))
                .Count() == 0;
        if (!allUsersAreWorkspaceMembers || !allUsersNotGroupMembers)
        {
            throw new InvalidOperationException(
                "Users must be workspace members and not already group members"
            );
        }

        List<DirectMessageGroupMember> groupMembers =
            new List<DirectMessageGroupMember>();
        foreach (Guid userId in userIds)
        {
            groupMembers.Add(
                new DirectMessageGroupMember
                {
                    DirectMessageGroupId = directMessageGroupId,
                    WorkspaceId = workspaceId,
                    UserId = userId
                }
            );
        }

        _context.AddRange(groupMembers);
        group!.Size += groupMembers.Count;
        await _context.SaveChangesAsync();

        return groupMembers;
    }

    public async Task<DirectMessageReaction> InsertMessageReaction(
        Guid directMessageId,
        Guid userId,
        string emoji
    )
    {
        bool invalidEmoji =
            emoji.Any(c => char.IsWhiteSpace(c)) || !EmojiUtils.IsEmoji(emoji);
        if (invalidEmoji)
        {
            throw new ArgumentException("Invalid arguments");
        }
        DirectMessage? directMessage = _context.DirectMessages
            .Where(dm => dm.Id == directMessageId)
            .FirstOrDefault();
        if (directMessage is null)
        {
            throw new ArgumentException("Invalid arguments");
        }
        bool isMember =
            _context.DirectMessageGroupMembers
                .Where(
                    dm =>
                        dm.UserId == userId
                        && dm.DirectMessageGroupId
                            == directMessage.DirectMessageGroupId
                )
                .Count() == 1;
        if (!isMember)
        {
            throw new ArgumentException("Invalid arguments");
        }

        DirectMessageReaction reaction = new DirectMessageReaction
        {
            DirectMessageId = directMessageId,
            Emoji = emoji,
            UserId = userId
        };
        _context.Add(reaction);

        await _context.SaveChangesAsync();

        return reaction;
    }

    public async Task<DirectMessageNotification> InsertReplyNotification(
        DirectMessageReply replyRecord
    )
    {
        bool recordExists =
            _context.DirectMessageReplies
                .Where(r => r.Id == replyRecord.Id)
                .Count() == 1;
        if (!recordExists)
        {
            throw new ArgumentException("Invalid arguments");
        }

        DirectMessageNotification notification = new DirectMessageNotification
        {
            DirectMessageId = replyRecord.DirectMessageId,
            DirectMessageNotificationType = MaskEnumDefs.NotificationTypes[
                MaskEnumDefs.REPLY
            ],
            UserId = replyRecord.RepliedToId
        };
        _context.Add(notification);

        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<
        List<DirectMessageNotification>
    > InsertMentionNotifications(List<DirectMessageMention> mentionRecords)
    {
        List<Guid> mentionRecordIds = mentionRecords.Select(m => m.Id).ToList();
        bool validArgs =
            _context.DirectMessageMentions
                .Where(dm => mentionRecordIds.Contains(dm.Id))
                .Count() == mentionRecordIds.Count;
        if (!validArgs)
        {
            throw new ArgumentException("Invalid arguments");
        }

        List<DirectMessageNotification> notifications =
            new List<DirectMessageNotification>();
        foreach (DirectMessageMention mentionRecord in mentionRecords)
        {
            notifications.Add(
                new DirectMessageNotification
                {
                    DirectMessageId = mentionRecord.DirectMessageId,
                    DirectMessageNotificationType =
                        MaskEnumDefs.NotificationTypes[MaskEnumDefs.MENTION],
                    UserId = mentionRecord.MentionedId
                }
            );
        }
        _context.AddRange(notifications);

        await _context.SaveChangesAsync();

        return notifications;
    }

    public async Task<DirectMessageNotification> InsertReactionNotification(
        DirectMessageReaction reactionRecord
    )
    {
        bool validArgs =
            _context.DirectMessageReactions
                .Where(dmr => dmr.Id == reactionRecord.Id)
                .Count() == 1;
        if (!validArgs)
        {
            throw new ArgumentException("Invalid arguments");
        }

        DirectMessageNotification notification = new DirectMessageNotification
        {
            DirectMessageId = reactionRecord.DirectMessageId,
            DirectMessageNotificationType = MaskEnumDefs.NotificationTypes[
                MaskEnumDefs.REACTION
            ],
            UserId = reactionRecord.DirectMessage.UserId
        };
        _context.Add(notification);

        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<DirectMessageLaterFlag> InsertDirectMessageLaterFlag(
        Guid directMessageId,
        Guid userId
    )
    {
        DirectMessage? directMessage = _context.DirectMessages
            .Where(dm => dm.Id == directMessageId)
            .FirstOrDefault();
        if (directMessage is null)
        {
            throw new ArgumentException("Invalid arguments");
        }

        Guid groupId = directMessage.DirectMessageGroupId;
        bool isGroupMember =
            _context.DirectMessageGroupMembers
                .Where(
                    dm =>
                        dm.UserId == userId
                        && dm.DirectMessageGroupId == groupId
                )
                .Count() == 1;
        if (!isGroupMember)
        {
            throw new ArgumentException("Invalid arguments");
        }

        Guid workspaceId = directMessage.DirectMessageGroup.WorkspaceId;
        DirectMessageLaterFlag laterFlag = new DirectMessageLaterFlag
        {
            DirectMessageGroupId = groupId,
            DirectMessageId = directMessageId,
            UserId = userId,
            WorkspaceId = workspaceId
        };
        _context.Add(laterFlag);
        directMessage.LaterFlag = laterFlag;

        await _context.SaveChangesAsync();

        return laterFlag;
    }

    public async Task<DirectMessage> InsertDirectMessage(
        Guid directMessageGroupId,
        string content,
        Guid userId,
        List<Guid>? mentionedPeople = null,
        Guid? messageRepliedToId = null,
        Guid? personRepliedToId = null,
        bool draft = false
    )
    {
        bool validMessage =
            content.Length > 0
            && _context.DirectMessageGroupMembers
                .Where(
                    dmgm =>
                        dmgm.UserId == userId
                        && dmgm.DirectMessageGroupId == directMessageGroupId
                )
                .Count() == 1;
        if (!validMessage)
        {
            throw new InvalidOperationException("Could not add new message");
        }

        bool reply =
            messageRepliedToId is not null && personRepliedToId is not null;
        bool regularMessage =
            messageRepliedToId is null && personRepliedToId is null;
        if (!reply && !regularMessage)
        {
            throw new ArgumentException("Invalid arguments");
        }

        bool needRecordMentions =
            !draft && mentionedPeople is not null && mentionedPeople.Count > 0;
        if (needRecordMentions)
        {
            bool mentionedAllMembers =
                _context.DirectMessageGroupMembers
                    .Where(
                        dmgm =>
                            mentionedPeople!.Contains(dmgm.UserId)
                            && dmgm.DirectMessageGroupId == directMessageGroupId
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
                _context.DirectMessages
                    .Where(
                        dm =>
                            dm.Id == messageRepliedToId
                            && dm.UserId == personRepliedToId
                    )
                    .Count() == 1;
            if (!validReply)
            {
                throw new InvalidOperationException("Could not add reply");
            }
        }

        DirectMessage message = new DirectMessage
        {
            Content = content,
            DirectMessageGroupId = directMessageGroupId,
            SentAt = draft ? null : DateTime.Now,
            UserId = userId,
            IsReply = reply,
            ReplyToId = messageRepliedToId
        };
        _context.Add(message);

        if (reply)
        {
            DirectMessageReply replyRecord = new DirectMessageReply
            {
                DirectMessage = message,
                MessageRepliedToId = (Guid)messageRepliedToId!,
                RepliedToId = (Guid)personRepliedToId!,
                ReplierId = userId
            };
            _context.Add(replyRecord);
        }

        if (needRecordMentions)
        {
            foreach (Guid mentionedId in mentionedPeople!)
            {
                _context.Add(
                    new DirectMessageMention
                    {
                        DirectMessage = message,
                        MentionedId = mentionedId,
                        MentionerId = userId
                    }
                );
            }
        }

        await _context.SaveChangesAsync();

        return message;
    }

    public async Task<List<DirectMessageGroup>> InsertDirectMessageGroups(
        List<DirectMessageGroup> directMessageGroups,
        List<List<Guid>> members,
        Guid workspaceId
    )
    {
        using var transaction = _context.Database.BeginTransaction();
        _context.AddRange(directMessageGroups);
        await _context.SaveChangesAsync();
        foreach (
            (
                DirectMessageGroup dmg,
                List<Guid> groupMembers
            ) in directMessageGroups.Zip(members)
        )
        {
            foreach (Guid memberId in groupMembers)
            {
                _context.Add(
                    new DirectMessageGroupMember
                    {
                        DirectMessageGroup = dmg,
                        UserId = memberId,
                        WorkspaceId = workspaceId
                    }
                );
            }
        }
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return directMessageGroups;
    }

    public async Task<(
        List<dynamic> dbDirectMessageGroups,
        bool lastPage
    )> LoadDirectMessageGroups(
        Guid workspaceId,
        Guid userId,
        int first,
        FieldTree connectionTree,
        Guid? after = null
    )
    {
        IQueryable<DirectMessageGroupMember> memberships =
            _context.DirectMessageGroupMembers
                .Where(
                    dmg =>
                        dmg.WorkspaceId == workspaceId && dmg.UserId == userId
                )
                .OrderByDescending(dmg => dmg.LastViewedAt.HasValue)
                .ThenByDescending(dmg => dmg.LastViewedAt)
                .ThenByDescending(dmg => dmg.JoinedAt);
        if (!(after is null))
        {
            DirectMessageGroupMember afterMembership = memberships
                .Where(
                    dmg =>
                        dmg.WorkspaceId == workspaceId
                        && dmg.UserId == userId
                        && dmg.DirectMessageGroupId == after
                )
                .First();
            if (afterMembership.LastViewedAt is null)
            {
                memberships = memberships.Where(
                    dmg =>
                        dmg.LastViewedAt != null
                        || dmg.JoinedAt < afterMembership.JoinedAt
                );
            }
            else
            {
                memberships = memberships.Where(
                    dmg =>
                        dmg.LastViewedAt != null
                        || dmg.LastViewedAt < afterMembership.LastViewedAt
                );
            }
        }
        var directMessageGroups = memberships
            .Take(first + 1)
            .Select(dmg => dmg.DirectMessageGroup);

        var dynamicDirectMessageGroups = await directMessageGroups
            .Select(
                DynamicLinqUtils.NodeFieldToDynamicSelectString(
                    connectionTree,
                    forceInclude: new List<string> { "members" },
                    skip: new List<string> { "name" },
                    map: new Dictionary<string, string>
                    {
                        { "members", "directMessageGroupMembers" }
                    }
                )
            )
            .ToDynamicListAsync();

        bool lastPage = dynamicDirectMessageGroups.Count <= first;
        if (!lastPage)
        {
            dynamicDirectMessageGroups.RemoveAt(
                dynamicDirectMessageGroups.Count - 1
            );
        }
        return (dynamicDirectMessageGroups, lastPage);
    }

    public async Task<(
        List<dynamic> dbMessages,
        List<DirectMessageReactionCount> reactionCounts,
        bool lastPage
    )> LoadDirectMessages(
        Guid userId,
        Guid groupId,
        FieldInfo fieldInfo,
        int first,
        Guid? after
    )
    {
        IQueryable<DirectMessage> messages = _context.DirectMessages
            .Where(
                dm => dm.DirectMessageGroupId == groupId && dm.SentAt != null
            )
            .OrderByDescending(dm => dm.SentAt);
        if (after is not null)
        {
            DateTime prevLastSentAt = (DateTime)
                (
                    _context.DirectMessages
                        .Where(dm => dm.Id == after)
                        .Select(dm => dm.SentAt)
                        .First()
                )!;
            messages = messages.Where(
                dm => dm.SentAt <= prevLastSentAt && dm.Id != after
            );
        }
        messages = messages.Take(first + 1);

        List<dynamic> dynamicDirectMessages = await messages
            .Select(
                DynamicLinqUtils.NodeFieldToDynamicSelectString(
                    fieldInfo.FieldTree,
                    forceInclude: new List<string> { "id", "deleted" },
                    skip: new List<string> { "reactions", "group", "type" }
                )
            )
            .ToDynamicListAsync();

        List<DirectMessageReactionCount> reactionCounts = new();
        if (fieldInfo.SubfieldNames.Contains("reactions"))
        {
            List<Guid> messageIds = new();
            foreach (dynamic message in messages)
            {
                messageIds.Add((Guid)message.Id);
            }
            var reactions = _context.DirectMessageReactions
                .Where(dmr => messageIds.Contains(dmr.DirectMessageId))
                .GroupBy(dmr => new { dmr.DirectMessageId, dmr.Emoji })
                .Select(
                    group =>
                        new
                        {
                            DirectMessageId = group.Key.DirectMessageId,
                            Emoji = group.Key.Emoji,
                            Count_ = group.Count(),
                            UserReaction = group
                                .Where(dmr => dmr.UserId == userId)
                                .FirstOrDefault()
                        }
                )
                .ToList();

            foreach (var reaction in reactions)
            {
                reactionCounts.Add(
                    new DirectMessageReactionCount
                    {
                        DirectMessageId = reaction.DirectMessageId,
                        Count = reaction.Count_,
                        Emoji = reaction.Emoji,
                        UserReaction = reaction.UserReaction
                    }
                );
            }
        }

        bool lastPage = dynamicDirectMessages.Count <= first;
        if (!lastPage)
        {
            dynamicDirectMessages.RemoveAt(dynamicDirectMessages.Count - 1);
        }
        return (dynamicDirectMessages, reactionCounts, lastPage);
    }
}
