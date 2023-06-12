using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Models;

[Index(nameof(ChannelId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public class ChannelMember
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DefaultValue(false)]
    public bool Admin { get; set; }

#pragma warning disable CS8618
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    public Guid ChannelId { get; set; }

    [DefaultValue(true)]
    public bool EnableNotifications { get; set; }

    public DateTime? LastViewedAt { get; set; }

    [DefaultValue(false)]
    public bool Starred { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
