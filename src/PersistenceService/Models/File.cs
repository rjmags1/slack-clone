using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(UploadedAt))]
[Index(nameof(DirectMessageId))]
[Index(nameof(ChannelMessageId))]
[Index(nameof(DirectMessageGroupId))]
[Index(nameof(ChannelId))]
public class File
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Channel? Channel { get; set; }

    [ForeignKey(nameof(Channel))]
    public Guid? ChannelId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ChannelMessage? ChannelMessage { get; set; }

    [ForeignKey(nameof(ChannelMessage))]
    public Guid? ChannelMessageId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessage? DirectMessage { get; set; }

    [ForeignKey(nameof(DirectMessage))]
    public Guid? DirectMessageId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public DirectMessageGroup? DirectMessageGroup { get; set; }

    [ForeignKey(nameof(DirectMessageGroup))]
    public Guid? DirectMessageGroupId { get; set; }

    [MaxLength(80)]
#pragma warning disable CS8618
    public string Name { get; set; }

    [MaxLength(256)]
    public string StoreKey { get; set; }
#pragma warning restore CS8618

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = "timestamp")]
    public DateTime UploadedAt { get; set; }
}
