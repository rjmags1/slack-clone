using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Models;

[Index(nameof(FirstMessageId))]
public class Thread
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

#pragma warning disable CS8618
    public Channel Channel { get; set; }
#pragma warning restore CS8618

    public Guid ChannelId { get; set; }

    public ChannelMessage? FirstMessage { get; set; }

    public Guid? FirstMessageId { get; set; }

#pragma warning disable CS8618
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    public Guid WorkspaceId { get; set; }
}
