using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace PersistenceService.Models;

[Index(nameof(ChannelId))]
[Index(nameof(Deleted))]
[Index(nameof(SentAt))]
[Index(nameof(UserId))]
public class ChannelMessage
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Channel))]
    public Guid ChannelId { get; set; }

    [MaxLength(2500)]
#pragma warning disable CS8618
    public string Content { get; set; }

    [ConcurrencyCheck]
    public Guid ConcurrencyStamp { get; set; }
#pragma warning restore CS8618

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    public bool Deleted { get; set; }

    public ICollection<File> Files { get; } = new List<File>();

    public bool IsReply { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? LastEdit { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    [JsonIgnore]
    public ChannelMessageLaterFlag? LaterFlag { get; set; }

    [ForeignKey(nameof(LaterFlag))]
    public Guid? LaterFlagId { get; set; }

    public ICollection<ChannelMessageMention> Mentions { get; } =
        new List<ChannelMessageMention>();

    public ICollection<ChannelMessageReaction> Reactions { get; } =
        new List<ChannelMessageReaction>();

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public ChannelMessage? ReplyTo { get; set; }

    [ForeignKey(nameof(ReplyTo))]
    public Guid? ReplyToId { get; set; }

    public ICollection<ChannelMessageReply> Replies { get; } =
        new List<ChannelMessageReply>();

    [Column(TypeName = "timestamp")]
    public DateTime? SentAt { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Thread? Thread { get; set; }

    [ForeignKey(nameof(Thread))]
    public Guid? ThreadId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
}
