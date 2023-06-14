using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(
    nameof(MentionedId),
    nameof(ChannelMessageId),
    nameof(MentionerId),
    IsUnique = true
)]
[Index(nameof(CreatedAt))]
public class ChannelMessageMention
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public ChannelMessage ChannelMessage { get; set; }
#pragma warning restore CS8618

    public Guid ChannelMessageId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    public User? Mentioned { get; set; }

    public Guid? MentionedId { get; set; }

#pragma warning disable CS8618
    public User? Mentioner { get; set; }
#pragma warning restore CS8618

    public Guid? MentionerId { get; set; }
}
