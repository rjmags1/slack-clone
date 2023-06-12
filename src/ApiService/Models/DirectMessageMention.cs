using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Models;

[Index(
    nameof(DirectMessageId),
    nameof(MentionedId),
    nameof(MentionerId),
    IsUnique = true
)]
[Index(nameof(CreatedAt))]
[Index(nameof(MentionedId))]
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

    public User? Mentioned { get; set; }

    public Guid? MentionedId { get; set; }

#pragma warning disable CS8618
    public User Mentioner { get; set; }
#pragma warning restore CS8618

    public Guid MentionerId { get; set; }
}
