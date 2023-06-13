using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(MessageRepliedToId))]
public class DirectMessageReply
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageId { get; set; }

    public User? RepliedTo { get; set; }

    public Guid? RepliedToId { get; set; }

#pragma warning disable CS8618
    public User Replier { get; set; }
#pragma warning restore CS8618

    public Guid ReplierId { get; set; }

    public DirectMessage? MessageRepliedTo { get; set; }

    public Guid? MessageRepliedToId { get; set; }
}
