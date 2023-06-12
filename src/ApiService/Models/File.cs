using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Models;

[Index(nameof(UploadedAt))]
[Index(nameof(DirectMessageId))]
[Index(nameof(ChannelMessageId))]
public class File
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(80)]
#pragma warning disable CS8618
    public string Name { get; set; }

    [MaxLength(256)]
    public string StoreKey { get; set; }
#pragma warning restore CS8618

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UploadedAt { get; set; }

    public DirectMessage? DirectMessage { get; set; }

    public Guid? DirectMessageId { get; set; }

    public ChannelMessage? ChannelMessage { get; set; }

    public Guid? ChannelMessageId { get; set; }
}
