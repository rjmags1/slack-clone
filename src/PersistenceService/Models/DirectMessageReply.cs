using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(MessageRepliedToId))]
[Index(nameof(DirectMessageId), IsUnique = true)]
public class DirectMessageReply
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(DirectMessage))]
    public Guid DirectMessageId { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public User? RepliedTo { get; set; }

    [ForeignKey(nameof(RepliedTo))]
    public Guid? RepliedToId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User Replier { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Replier))]
    public Guid ReplierId { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public DirectMessage? MessageRepliedTo { get; set; }

    [ForeignKey(nameof(MessageRepliedTo))]
    public Guid? MessageRepliedToId { get; set; }
}
