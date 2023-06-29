using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(ChannelId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public class ChannelMember
{
    public Guid Id { get; set; }

    [DefaultValue(false)]
    public bool Admin { get; set; } = false;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Channel))]
    public Guid ChannelId { get; set; }

    public bool? EnableNotifications { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? LastViewedAt { get; set; }

    [DefaultValue(false)]
    public bool Starred { get; set; } = false;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
}
