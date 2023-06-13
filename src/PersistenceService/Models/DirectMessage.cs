using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(DirectMessageGroupId))]
[Index(nameof(Deleted))]
[Index(nameof(Draft))]
[Index(nameof(DirectMessageLaterFlagId))]
[Index(nameof(SentAt))]
[Index(nameof(UserId))]
public class DirectMessage
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(2000)]
#pragma warning disable CS8618
    public string Content { get; set; }
#pragma warning restore CS8618

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(false)]
    public bool Deleted { get; set; }

#pragma warning disable CS8618
    public DirectMessageGroup DirectMessageGroup { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageGroupId { get; set; }

    [DefaultValue(true)]
    public bool Draft { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime LastEdit { get; set; }

    public DirectMessageLaterFlag? DirectMessageLaterFlag { get; set; }

    public Guid? DirectMessageLaterFlagId { get; set; }

    public DateTime? SentAt { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
