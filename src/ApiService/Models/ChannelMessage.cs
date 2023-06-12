using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Models;

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

    [DefaultValue(false)]
    public bool Admin { get; set; }

#pragma warning disable CS8618
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    public Guid ChannelId { get; set; }

    [MaxLength(2000)]
#pragma warning disable CS8618
    public string Content { get; set; }
#pragma warning restore CS8618

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DefaultValue(false)]
    public bool Deleted { get; set; }

    [DefaultValue(true)]
    public bool Draft { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime LastEdit { get; set; }

    public ChannelMessageLaterFlag? LaterFlag { get; set; }

    public Guid? ChannelMessageLaterFlagId { get; set; }

    public DateTime? SentAt { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
