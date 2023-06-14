using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(ChannelId))]
[Index(nameof(Deleted))]
[Index(nameof(Draft))]
[Index(nameof(ChannelMessageLaterFlagId))]
[Index(nameof(SentAt))]
[Index(nameof(UserId))]
public class ChannelMessage
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    public Guid ChannelId { get; set; }

    public ChannelMessageLaterFlag? ChannelMessageLaterFlag { get; set; }

    public Guid? ChannelMessageLaterFlagId { get; set; }

    [MaxLength(2500)]
#pragma warning disable CS8618
    public string Content { get; set; }
#pragma warning restore CS8618

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(false)]
    public bool Deleted { get; set; }

    [DefaultValue(true)]
    public bool Draft { get; set; }

    public ICollection<File> Files { get; } = new List<File>();

    public DateTime? LastEdit { get; set; }

    public ICollection<ChannelMessageMention> Mentions { get; } =
        new List<ChannelMessageMention>();

    public ICollection<ChannelMessageReaction> Reactions { get; } =
        new List<ChannelMessageReaction>();

    public ICollection<ChannelMessageReply> Replies { get; } =
        new List<ChannelMessageReply>();

    public DateTime? SentAt { get; set; }

    public User? User { get; set; }

    public Guid? UserId { get; set; }
}
