using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PersistenceService.Models;

[Index(nameof(Deleted))]
public class User : IdentityUser
{
#pragma warning disable CS8618
    public File Avatar { get; set; }
#pragma warning restore CS8618

    public Guid AvatarId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    [DefaultValue("online")]
    public string OnlineStatus { get; set; }
#pragma warning restore CS8618

    public DateTime? OnlineStatusUntil { get; set; }

    public Theme? Theme { get; set; }

    public Guid? ThemeId { get; set; }

#pragma warning disable CS8618
    public string Timezone { get; set; }
#pragma warning restore CS8618
}
