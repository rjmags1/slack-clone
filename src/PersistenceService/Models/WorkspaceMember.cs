using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(UserId), nameof(WorkspaceId), IsUnique = true)]
[Index(nameof(JoinedAt))]
[Index(nameof(WorkspaceId))]
public class WorkspaceMember
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DefaultValue(false)]
    public bool Admin { get; set; }

    public File? Avatar { get; set; }

    public Guid? AvatarId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime JoinedAt { get; set; }

    public TimeOnly NotificationsAllowTimeStart { get; set; }

    public TimeOnly NotificationsAllTimeEnd { get; set; }

    [DefaultValue(0)]
    public int NotificationSound { get; set; }

#pragma warning disable CS8618
    [DefaultValue("offline")]
    public string OnlineStatus { get; set; }
#pragma warning restore CS8618

    public DateTime? OnlineStatusUntil { get; set; }

    [DefaultValue(false)]
    public bool Owner { get; set; }

    public Theme? Theme { get; set; }

    public Guid? ThemeId { get; set; }

#pragma warning disable CS8618
    [MaxLength(80)]
    public string Title { get; set; }
#pragma warning restore CS8618

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }

#pragma warning disable CS8618
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    public Guid WorkspaceId { get; set; }
}
