using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(MessageRepliedToId))]
[Index(nameof(ThreadId))]
[Index(nameof(ChannelMessageId), IsUnique = true)]
public class ChannelMessageReply
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ChannelMessage ChannelMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(ChannelMessage))]
    public Guid ChannelMessageId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ChannelMessage MessageRepliedTo { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(MessageRepliedTo))]
    public Guid MessageRepliedToId { get; set; }

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
    public Thread Thread { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Thread))]
    public Guid ThreadId { get; set; }
}
