using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(CreatedAt))]
[Index(nameof(ChannelInviteStatus))]
[Index(nameof(UserId), nameof(WorkspaceId))]
public class ChannelInvite
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public User Admin { get; set; }
#pragma warning restore CS8618

    public Guid AdminId { get; set; }

#pragma warning disable CS8618
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    public Guid ChannelId { get; set; }

    [DefaultValue(1)]
    public int ChannelInviteStatus { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }

#pragma warning disable CS8618
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    public Guid WorkspaceId { get; set; }
}
