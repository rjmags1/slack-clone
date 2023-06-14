using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(
    nameof(MentionedId),
    nameof(DirectMessageId),
    nameof(MentionerId),
    IsUnique = true
)]
[Index(nameof(CreatedAt))]
public class DirectMessageMention
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    public DirectMessage DirectMessage { get; set; }
#pragma warning restore CS8618

    public Guid DirectMessageId { get; set; }

#pragma warning disable CS8618
    public User Mentioned { get; set; }
#pragma warning restore CS8618

    public Guid MentionedId { get; set; }

#pragma warning disable CS8618
    public User Mentioner { get; set; }
#pragma warning restore CS8618

    public Guid MentionerId { get; set; }
}
