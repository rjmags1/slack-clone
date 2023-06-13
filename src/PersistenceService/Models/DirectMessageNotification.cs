using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(DirectMessageId), nameof(UserId), IsUnique = true)]
[Index(nameof(CreatedAt))]
[Index(nameof(UserId))]
public class DirectMessageNotification
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageId { get; set; }

    public int DirectMessageNotificationType { get; set; }

    [DefaultValue(false)]
    public bool Seen { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
