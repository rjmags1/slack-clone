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

    public Channel? Channel { get; set; }

    public Guid? ChannelId { get; set; }

    public ChannelMessage? ChannelMessage { get; set; }

    public Guid? ChannelMessageId { get; set; }

    public DirectMessage? DirectMessage { get; set; }

    public Guid? DirectMessageId { get; set; }

    public DirectMessageGroup? DirectMessageGroup { get; set; }

    public Guid? DirectMessageGroupId { get; set; }

    [MaxLength(80)]
#pragma warning disable CS8618
    public string Name { get; set; }

    [MaxLength(256)]
    public string StoreKey { get; set; }
#pragma warning restore CS8618

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UploadedAt { get; set; }
}
