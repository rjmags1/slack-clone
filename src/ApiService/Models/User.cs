using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Models;

[Index(nameof(Deleted))]
[Index(nameof(NormalizedEmail))]
[Index(nameof(NormalizedUsername))]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Username), IsUnique = true)]
public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public File Avatar { get; set; }
#pragma warning restore CS8618

    public Guid AvatarId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(false)]
    public bool Deleted { get; set; }

#pragma warning disable CS8618
    [MaxLength(320)]
    public string Email { get; set; }

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

    [MaxLength(320)]
#pragma warning disable CS8618
    public string NormalizedEmail { get; set; }

    [MaxLength(80)]
    public string NormalizedUsername { get; set; }

    [MaxLength(20)]
    [DefaultValue("online")]
    public string OnlineStatus { get; set; }
#pragma warning restore CS8618

    public DateTime? OnlineStatusUntil { get; set; }

    public Theme? Theme { get; set; }

    public Guid? ThemeId { get; set; }

#pragma warning disable CS8618
    public TimeZoneInfo Timezone { get; set; }

    [MaxLength(48)]
    public string Username { get; set; }
#pragma warning restore CS8618
}
