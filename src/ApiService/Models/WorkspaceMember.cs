using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Models;

[Index(nameof(UserId), nameof(WorkspaceId))]
[Index(nameof(JoinedAt))]
[Index(nameof(WorkspaceId), nameof(UserId))]
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

    public int NotificationSound { get; set; }

    [DefaultValue("offline")]
#pragma warning disable CS8618
    public string OnlineStatus { get; set; }
#pragma warning restore CS8618

    [DefaultValue(false)]
    public bool Owner { get; set; }

    public Theme? Theme { get; set; }

    public Guid? ThemeId { get; set; }

    [MaxLength(80)]
#pragma warning disable CS8618
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
