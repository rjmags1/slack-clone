using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(UserId), nameof(WorkspaceId), IsUnique = true)]
[Index(nameof(JoinedAt))]
[Index(nameof(WorkspaceId), nameof(UserId))]
public class WorkspaceMember
{
    public Guid Id { get; set; }

    public bool Admin { get; set; } = false;

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public WorkspaceAdminPermissions? WorkspaceAdminPermissions { get; set; }

    [ForeignKey(nameof(WorkspaceAdminPermissions))]
    public Guid? WorkspaceAdminPermissionsId { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public File? Avatar { get; set; }

    [ForeignKey(nameof(Avatar))]
    public Guid? AvatarId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime JoinedAt { get; set; }

    public TimeOnly? NotificationsAllowTimeStart { get; set; }

    public TimeOnly? NotificationsAllowTimeEnd { get; set; }

    public int NotificationSound { get; set; } = 0;

#pragma warning disable CS8618
    [MaxLength(20)]
    public string? OnlineStatus { get; set; }
#pragma warning restore CS8618

    [Column(TypeName = "timestamp")]
    public DateTime? OnlineStatusUntil { get; set; }

    public bool Owner { get; set; } = false;

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Theme? Theme { get; set; }

    [ForeignKey(nameof(Theme))]
    public Guid? ThemeId { get; set; }

#pragma warning disable CS8618
    [MaxLength(80)]
    public string Title { get; set; }
#pragma warning restore CS8618

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
}
