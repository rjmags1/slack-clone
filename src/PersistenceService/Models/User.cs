using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PersistenceService.Models;

[Index(nameof(Deleted))]
[Index(nameof(NormalizedEmail))]
[Index(nameof(NormalizedUserName))]
public class User : IdentityUser<Guid>
{
#pragma warning disable CS8618, CS0114
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public File? Avatar { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Avatar))]
    public Guid? AvatarId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(false)]
    public bool Deleted { get; set; }

#pragma warning disable CS8618
    [MaxLength(20)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; }
#pragma warning restore CS8618

    [DefaultValue(0)]
    public int UserNotificationsPreferencesMask { get; set; }

    public TimeOnly? NotificationsAllowStartTime { get; set; }

    public TimeOnly? NotificationsAllowEndTime { get; set; }

    public TimeOnly? NotificationsPauseUntil { get; set; }

    [DefaultValue(0)]
    public int NotificationSound { get; set; }

#pragma warning disable CS8618
    [MaxLength(20)]
    public string? OnlineStatus { get; set; }
#pragma warning restore CS8618

    [Column(TypeName = "timestamp")]
    public DateTime? OnlineStatusUntil { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Theme? Theme { get; set; }

    [ForeignKey(nameof(Theme))]
    public Guid? ThemeId { get; set; }

#pragma warning disable CS8618
    [MaxLength(40)]
    public string Timezone { get; set; }
#pragma warning restore CS8618
}
