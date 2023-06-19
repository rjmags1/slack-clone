using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(ChannelMessageId), nameof(UserId))]
[Index(nameof(CreatedAt))]
[Index(nameof(UserId))]
public class ChannelMessageReaction
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ChannelMessage ChannelMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(ChannelMessage))]
    public Guid ChannelMessageId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    [MaxLength(4)]
    public string Emoji { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
}
