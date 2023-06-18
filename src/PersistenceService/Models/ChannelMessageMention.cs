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
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ChannelMessage ChannelMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Channel))]
    public Guid ChannelMessageId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User Mentioned { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Mentioned))]
    public Guid MentionedId { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User Mentioner { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Mentioner))]
    public Guid MentionerId { get; set; }
}
