using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(ChannelId))]
[Index(nameof(Deleted))]
[Index(nameof(Draft))]
[Index(nameof(SentAt))]
[Index(nameof(UserId))]
public class ChannelMessage
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    public byte[] ConcurrencyStamp { get; set; }
#pragma warning restore CS8618

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(false)]
    public bool Deleted { get; set; }

    [DefaultValue(true)]
    public bool Draft { get; set; }

    public ICollection<File> Files { get; } = new List<File>();

    [Column(TypeName = "timestamp")]
    public DateTime? LastEdit { get; set; }

    public ICollection<ChannelMessageMention> Mentions { get; } =
        new List<ChannelMessageMention>();

    public ICollection<ChannelMessageReaction> Reactions { get; } =
        new List<ChannelMessageReaction>();

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
