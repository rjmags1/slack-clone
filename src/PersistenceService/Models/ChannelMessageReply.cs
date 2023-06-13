using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(MessageRepliedToId))]
[Index(nameof(ThreadId))]
public class ChannelMessageReply
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public ChannelMessage ChannelMessage { get; set; }
#pragma warning restore CS8618

    public Guid ChannelMessageId { get; set; }

    public User? RepliedTo { get; set; }

    public Guid? RepliedToId { get; set; }

#pragma warning disable CS8618
    public User Replier { get; set; }
#pragma warning restore CS8618

    public Guid ReplierId { get; set; }

    public ChannelMessage? MessageRepliedTo { get; set; }

    public Guid? MessageRepliedToId { get; set; }

#pragma warning disable CS8618
    public Thread Thread { get; set; }
#pragma warning restore CS8618

    public Guid ThreadId { get; set; }
}
