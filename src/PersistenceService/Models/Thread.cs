using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(FirstMessageId), IsUnique = true)]
[Index(nameof(ChannelId))]
public class Thread
{
    public Guid Id { get; set; }

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Channel))]
    public Guid ChannelId { get; set; }

#pragma warning disable CS8618
    [ConcurrencyCheck]
    public Guid ConcurrencyStamp { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public ChannelMessage FirstMessage { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(FirstMessage))]
    public Guid FirstMessageId { get; set; }

    public ICollection<ChannelMessage> Messages { get; } =
        new List<ChannelMessage>();

    [DefaultValue(2)]
    public int NumMessages { get; set; } = 2;

#pragma warning disable CS8618
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    [ForeignKey(nameof(Workspace))]
    public Guid WorkspaceId { get; set; }
}
