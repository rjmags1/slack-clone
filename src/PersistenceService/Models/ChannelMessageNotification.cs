using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(UserId), nameof(ChannelMessageId), IsUnique = true)]
[Index(nameof(CreatedAt))]
public class ChannelMessageNotification
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ChannelMessage ChannelMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(ChannelMessage))]
    public Guid ChannelMessageId { get; set; }

    public int ChannelMessageNotificationType { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    public bool Seen { get; set; } = false;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
}
