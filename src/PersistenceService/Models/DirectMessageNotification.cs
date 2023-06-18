using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(UserId), nameof(DirectMessageId), IsUnique = true)]
[Index(nameof(CreatedAt))]
public class DirectMessageNotification
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(DirectMessage))]
    public Guid DirectMessageId { get; set; }

    public int DirectMessageNotificationType { get; set; }

    [DefaultValue(false)]
    public bool Seen { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
}