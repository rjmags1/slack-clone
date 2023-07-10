using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(MessageRepliedToId))]
[Index(nameof(DirectMessageId), IsUnique = true)]
public class DirectMessageReply
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(DirectMessage))]
    public Guid DirectMessageId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User RepliedTo { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(RepliedTo))]
    public Guid RepliedToId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User Replier { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Replier))]
    public Guid ReplierId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessage MessageRepliedTo { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(MessageRepliedTo))]
    public Guid MessageRepliedToId { get; set; }
}
