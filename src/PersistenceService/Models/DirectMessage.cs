using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(DirectMessageGroupId))]
[Index(nameof(Deleted))]
[Index(nameof(Draft))]
[Index(nameof(SentAt))]
[Index(nameof(UserId))]
public class DirectMessage
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [ConcurrencyCheck]
    public Guid ConcurrencyStamp { get; set; }

    [MaxLength(2500)]
    public string Content { get; set; }
#pragma warning restore CS8618

    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

    public bool Deleted { get; set; } = false;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessageGroup DirectMessageGroup { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(DirectMessageGroup))]
    public Guid DirectMessageGroupId { get; set; }

    public bool? Draft { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? LastEdit { get; set; }

    public ICollection<File> Files { get; } = new List<File>();

    public ICollection<DirectMessageMention> Mentions { get; } =
        new List<DirectMessageMention>();

    public ICollection<DirectMessageReaction> Reactions { get; } =
        new List<DirectMessageReaction>();

    public ICollection<DirectMessageReply> Replies { get; } =
        new List<DirectMessageReply>();

    [Column(TypeName = "timestamp")]
    public DateTime? SentAt { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
}
