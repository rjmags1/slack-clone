using PersistenceService.Constants;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Models;

namespace PersistenceService.Stores;

public class DirectMessageGroupStore : Store
{
    public DirectMessageGroupStore(ApplicationDbContext context)
        : base(context) { }

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

        await _context.SaveChangesAsync();

        return laterFlag;
    }

    public async Task<DirectMessage> InsertDirectMessage(
        Guid directMessageGroupId,
        string content,
        Guid userId,
        List<Guid>? mentionedPeople,
        Guid? messageRepliedToId,
        Guid? personRepliedToId,
        bool draft = false
    )
    {
        bool validMessage =
            content.Length > 0
            && _context.DirectMessageGroupMembers
                .Where(
                    dmgm =>
                        dmgm.DirectMessageGroupId == directMessageGroupId
                        && dmgm.UserId == userId
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
                            dmgm.DirectMessageGroupId == directMessageGroupId
                            && mentionedPeople!.Contains(dmgm.UserId)
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
            Draft = draft,
            SentAt = draft ? null : DateTime.Now,
            UserId = userId
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
        List<List<Guid>> members
    )
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
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
                            UserId = memberId
                        }
                    );
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }

        return directMessageGroups;
    }

    public async Task<List<DirectMessageGroup>> InsertTestDirectMessageGroups(
        int numTestDirectMessageGroups
    )
    {
        string emailPrefix = UserStore.GenerateTestEmail(10);
        string usernamePrefix = "dgcreator-uname";
        List<List<User>> testMembers = new List<List<User>>();
        List<DirectMessageGroup> testGroups = new List<DirectMessageGroup>();
        Workspace directMessageGroupWorkspace = new Workspace
        {
            Description = "test-description",
            Name = "test-workspace-direct-message-group-name"
        };
        for (int i = 0; i < numTestDirectMessageGroups; i++)
        {
            string email1 =
                emailPrefix + DirectMessageGroupStore.GenerateRandomString(15);
            string email2 =
                emailPrefix + DirectMessageGroupStore.GenerateRandomString(15);
            string username1 =
                usernamePrefix
                + DirectMessageGroupStore.GenerateRandomString(10);
            string username2 =
                usernamePrefix
                + DirectMessageGroupStore.GenerateRandomString(10);
            User dmgUser1 = new User
            {
                FirstName = "test-ccreator-fname",
                LastName = "test-dmg-creator-lname",
                Timezone = UserStore.timezones[0].Id,
                UserName = username1,
                Email = email1,
                NormalizedEmail = email1.ToUpper(),
                NormalizedUserName = username1.ToUpper(),
                PhoneNumber = "1-234-456-7890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    UserStore.testPassword,
                    4
                ),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            User dmgUser2 = new User
            {
                FirstName = "test-ccreator-fname",
                LastName = "test-dmg-creator-lname",
                Timezone = UserStore.timezones[0].Id,
                UserName = username2,
                Email = email2,
                NormalizedEmail = email2.ToUpper(),
                NormalizedUserName = username2.ToUpper(),
                PhoneNumber = "1-234-456-7890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    UserStore.testPassword,
                    4
                ),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            testGroups.Add(
                new DirectMessageGroup
                {
                    Workspace = directMessageGroupWorkspace,
                }
            );
            testMembers.Add(new List<User> { dmgUser1, dmgUser2 });
        }
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            _context.AddRange(testGroups);
            _context.AddRange(testMembers.SelectMany(pair => pair).ToList());
            await _context.SaveChangesAsync();
            foreach (
                (
                    DirectMessageGroup dmg,
                    List<User> groupMembers
                ) in testGroups.Zip(testMembers)
            )
            {
                foreach (User member in groupMembers)
                {
                    _context.Add(
                        new DirectMessageGroupMember
                        {
                            DirectMessageGroup = dmg,
                            User = member
                        }
                    );
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }

        return testGroups;
    }
}
