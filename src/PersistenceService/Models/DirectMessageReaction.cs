using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(DirectMessageId), nameof(UserId))]
[Index(nameof(CreatedAt))]
[Index(nameof(UserId))]
public class DirectMessageReaction
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageId { get; set; }

#pragma warning disable CS8618
    [MaxLength(4)]
    public string Emoji { get; set; }

    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
