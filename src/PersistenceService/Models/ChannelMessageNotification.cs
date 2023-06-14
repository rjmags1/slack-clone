using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(UserId), nameof(ChannelMessageId), IsUnique = true)]
[Index(nameof(CreatedAt))]
public class ChannelMessageNotification
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public ChannelMessage ChannelMessage { get; set; }
#pragma warning restore CS8618

    public Guid ChannelMessageId { get; set; }

    public int ChannelMessageNotificationType { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(false)]
    public bool Seen { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
