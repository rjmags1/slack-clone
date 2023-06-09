using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(ChannelMessageId), nameof(UserId), IsUnique = true)]
[Index(nameof(WorkspaceId), nameof(UserId))]
public class ChannelMessageLaterFlag
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Channel))]
    public Guid ChannelId { get; set; }

    public int ChannelLaterFlagStatus { get; set; } = 1;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ChannelMessage ChannelMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(ChannelMessage))]
    public Guid ChannelMessageId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

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
