using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(ChannelId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public class ChannelMember
{
    public Guid Id { get; set; }

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

    public bool Starred { get; set; } = false;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
#pragma warning restore CS8618
}
