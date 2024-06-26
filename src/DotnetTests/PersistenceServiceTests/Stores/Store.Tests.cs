using PersistenceService.Data.ApplicationDb;
using PersistenceService.Stores;
using File = PersistenceService.Models.File;

namespace DotnetTests.PersistenceService.Stores;

public abstract class StoreTest
{
    protected ApplicationDbContext DbContext { get; set; } = null!;

    protected Guid GetUserId()
    {
        return DbContext.Users.First(u => u.UserName == "dev").Id;
    }

    protected void SetUserAvatarIdThemeId(ref Guid avatarId, ref Guid themeId)
    {
        if (!DbContext.Files.Any())
        {
            var file = new File
            {
                Name = "test file",
                StoreKey = "test store key"
            };
            DbContext.Files.Add(file);
            DbContext.SaveChanges();
        }

        avatarId = DbContext.Files.First().Id;
        themeId = DbContext.Themes.First().Id;
        var user = DbContext.Users.First(u => u.UserName == "dev");
        user.AvatarId = avatarId;
        user.ThemeId = themeId;
        DbContext.SaveChanges();
    }

    protected Guid? GetUserAvatarId()
    {
        return DbContext.Users.First(u => u.UserName == "dev").AvatarId;
    }

    protected Guid? GetUserThemeId()
    {
        return DbContext.Users.First(u => u.UserName == "dev").ThemeId;
    }

    protected Guid GetWorkspaceIdWithUserDmgs(Guid userId)
    {
        return DbContext.DirectMessageGroupMembers
            .Where(dmgm => dmgm.UserId == userId)
            .GroupBy(dmgm => dmgm.WorkspaceId)
            .Select(g => new { WorkspaceId = g.Key, Count = g.Count() })
            .OrderByDescending(gc => gc.Count)
            .Select(gc => gc.WorkspaceId)
            .First();
    }

    protected (Guid, int) GetFirstAlphaWorkspaceTotalWorkspaces(Guid userId)
    {
        var wq = DbContext.WorkspaceMembers
            .Where(wm => wm.UserId == userId)
            .Select(wm => wm.Workspace)
            .OrderBy(w => w.Name);

        return (wq.First().Id, wq.Count());
    }

    protected Guid GetWorkspaceIdContainingUser(Guid userId)
    {
        return DbContext.WorkspaceMembers
            .Where(wm => wm.UserId == userId)
            .Select(wm => wm.Workspace)
            .OrderByDescending(w => w.NumMembers)
            .First()
            .Id;
    }

    protected (Guid, int) GetFirstAlphaChannelTotalChannels(Guid workspaceId)
    {
        var cq = DbContext.Channels
            .Where(c => c.WorkspaceId == workspaceId)
            .OrderBy(c => c.Name);

        return (cq.First().Id, cq.Count());
    }

    protected Guid GetChannelIdContainingUser(Guid userId)
    {
        return DbContext.ChannelMembers
            .Where(cm => cm.UserId == userId)
            .Select(cm => cm.Channel)
            .First(c => c.ChannelMessages.Count() > 1 && c.NumMembers > 1)
            .Id;
    }

    protected (Guid, int) GetMostRecentChannelMessageTotalChannelMessages(
        Guid channelId
    )
    {
        var mq = DbContext.ChannelMessages
            .Where(
                cm =>
                    cm.ChannelId == channelId
                    && cm.SentAt != null
                    && !cm.Deleted
            )
            .OrderByDescending(cm => cm.SentAt);

        return (mq.First().Id, mq.Count());
    }

    protected (Guid, int) GetMostRecentStarredTotalStarred(
        Guid workspaceId,
        Guid userId
    )
    {
        var sq = DbContext.Stars.Where(
            s => s.UserId == userId && s.WorkspaceId == workspaceId
        );

        return (sq.First().Id, sq.Count());
    }

    protected (Guid, int) GetFirstAlphaChannelMemberTotalChannelMembers(
        Guid channelId
    )
    {
        var mq = DbContext.ChannelMembers
            .Where(cm => cm.ChannelId == channelId)
            .OrderBy(cm => cm.User.UserName);

        return (mq.First().Id, mq.Count());
    }

    protected (Guid, int) GetFirstAlphaWorkspaceMemberTotalWorkspaceMembers(
        Guid workspaceId
    )
    {
        var mq = DbContext.WorkspaceMembers
            .Where(wm => wm.WorkspaceId == workspaceId)
            .OrderBy(wm => wm.User.UserName);

        return (mq.First().Id, mq.Count());
    }

    protected Guid GetDmgIdContainingUser(Guid userId)
    {
        return DbContext.DirectMessageGroupMembers
            .Where(dmgm => dmgm.UserId == userId)
            .Select(dmgm => dmgm.DirectMessageGroup)
            .OrderByDescending(dmg => dmg.DirectMessages.Count())
            .First()
            .Id;
    }

    protected (Guid, int) GetMostRecentDirectMessageTotalDirectMessages(
        Guid dmgId
    )
    {
        var mq = DbContext.DirectMessages
            .Where(
                dm =>
                    dm.DirectMessageGroupId == dmgId
                    && dm.SentAt != null
                    && !dm.Deleted
            )
            .OrderByDescending(dm => dm.SentAt);

        return (mq.First().Id, mq.Count());
    }

    protected (Guid, int) GetMostRecentDmgTotalDmgs(
        Guid workspaceId,
        Guid userId
    )
    {
        var dq = DbContext.DirectMessageGroupMembers
            .Where(
                dmgm => dmgm.WorkspaceId == workspaceId && dmgm.UserId == userId
            )
            .ToList()
            .OrderByDescending(
                dmgm =>
                    (
                        dmgm.LastViewedAt != null ? 1 : 0,
                        dmgm.LastViewedAt,
                        dmgm.JoinedAt
                    )
            );

        var dmg = dq.FirstOrDefault();

        return (
            dmg == null ? Guid.Empty : dmg.DirectMessageGroupId,
            dmg == null ? 0 : dq.Count()
        );
    }
}
